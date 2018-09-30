using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Entities;
using WebApi.Features.Accounts;
using WebApi.Features.Logs;
using WebApi.Infrastructure;

namespace WebApi
{
    public static class SeedData
    {
        /// <summary>
        /// Create default admin user and application roles
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static async Task CreateUsersAndRoles(IServiceProvider services, IConfiguration configuration)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roles = new string[] { "Admin", "Employee" };

            // Add every role from roles if it doesn't exists
            foreach(var role in roles)
            {
                var isRoleExists = await roleManager.RoleExistsAsync(role);
                if(!isRoleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create default admin if it doesn't exists
            var isDefaultAdminExists = await userManager.FindByNameAsync(configuration["DefaultAdminCredentials:UserName"]);
            if(isDefaultAdminExists == null)
            {
                // Get the username/password from configuration file
                var defaultAdmin = new User { UserName = configuration["DefaultAdminCredentials:UserName"] };
                await userManager.CreateAsync(defaultAdmin, configuration["DefaultAdminCredentials:Password"]);

                // Add 'Admin' role
                await userManager.AddToRoleAsync(defaultAdmin, roles[0]);
            }
        }

        /// <summary>
        /// Set up default attendance configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static async Task AttendanceConfiguration(IServiceProvider services, IConfiguration configuration)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            // Check if configuration already exists
            if (!context.Configurations.Any())
            {
                // Add default configurations from config file
                await context.Configurations.AddAsync(new Configuration
                {
                    TimeIn = configuration["AttendanceConfig:TimeIn"],
                    TimeOut = configuration["AttendanceConfig:TimeOut"],
                    GracePeriod = configuration["AttendanceConfig:GracePeriod"]
                });

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Seed fake employees with logs
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static async Task EnsureSeedEmployeesAndLogs(IServiceProvider services, IConfiguration configuration)
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {  
                using(var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                {
                    // Check if Employee table is empty
                    if (!context.Employees.Any())
                    {
                        // Seed Data
                        Console.WriteLine("Seeding Employee & Log List...");

                        // Set fake data for employee
                        var fakeEmployee = new Faker<RegisterViewModel>()
                            .RuleFor(m => m.UserName, f => f.Internet.UserName())
                            .RuleFor(m => m.Password, () => "123456")
                            .RuleFor(m => m.FullName, f => f.Person.FullName)
                            .RuleFor(m => m.CardNo, f => f.Random.Number(10000, 99999).ToString())
                            .RuleFor(m => m.Position, f => f.Name.JobTitle());
                        
                        // Set up for generating fake logs
                        var gracePeriod = Convert.ToInt32(configuration["AttendanceConfig:GracePeriod"]) * 2;
                        var timeInString = $"{DateTime.Today.ToString("d")} {configuration["AttendanceConfig:TimeIn"]}";
                        var timeOutString = $"{DateTime.Today.ToString("d")} {configuration["AttendanceConfig:TimeOut"]}";
                        var timeIn = DateTime.ParseExact(timeInString, "MM/dd/yyyy H:mm", null).ToUniversalTime().AddDays(-1);
                        var timeOut = DateTime.ParseExact(timeOutString, "MM/dd/yyyy H:mm", null).ToUniversalTime().AddDays(-1);

                        var timeInRange = new DateTime[] { timeIn.AddMinutes(-gracePeriod), timeIn.AddMinutes(gracePeriod) };
                        var timeOutRange = new DateTime[] { timeOut.AddMinutes(-gracePeriod), timeOut.AddMinutes(gracePeriod) };
                        
                        // Set fake data for logs
                        var fakeLog = new Faker<Log>()
                            .RuleFor(m => m.TimeIn, f => f.Date.Between( timeInRange[0], timeInRange[1] ))
                            .RuleFor(m => m.TimeOut, f => f.Date.Between( timeOutRange[0], timeOutRange[1] ));
                        
                        // Generate List of fake employees
                        var fakeEmployeeList = fakeEmployee.Generate(12);

                        // Iterate each fake list
                        foreach(var employee in fakeEmployeeList)
                        {
                            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                            var userManager = services.GetRequiredService<UserManager<User>>();
                            var daysToMinus = 0;

                            // Generate List of fake logs
                            var fakeLogList = fakeLog.Generate(7);
                            fakeLogList = fakeLogList.Select(m => 
                            {
                                m.TimeIn = Convert.ToDateTime(m.TimeIn).AddDays(daysToMinus);
                                m.TimeOut = Convert.ToDateTime(m.TimeOut).AddDays(daysToMinus);
                                m.Created = Convert.ToDateTime(m.TimeIn);
                                daysToMinus--;
                                return m;
                            }).ToList();

                            // Create Employee & Logs, generated by fake data
                            var employeeInfo = new Employee
                            {
                                FullName = employee.FullName,
                                CardNo = employee.CardNo,
                                Position = employee.Position,
                                Status = Status.Active,
                                Logs = fakeLogList
                            };
                            var user = new User { UserName = employee.UserName, Employee = employeeInfo };
                            await userManager.CreateAsync(user, employee.Password);
                            await userManager.AddToRoleAsync(user, "Employee");   
                        }
                    }
                }
            }
        }
    }
}