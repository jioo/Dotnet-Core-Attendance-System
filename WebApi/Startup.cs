using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using AutoMapper;
using Swashbuckle.AspNetCore.Swagger;
using WebApi.Infrastructures;
using WebApi.Entities;
using WebApi.Services;
using WebApi.Repositories;
using WebApi.Helpers;
using Hubs.BroadcastHub;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string SecretKey = Configuration["AppSecret"];
            // string SecretKey = "141FE29A91D7FA1A13F3C713BB789";
            SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("WebApi"))
                );

            // DI: IoC
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IConfigService, ConfigService>();
            services.AddScoped(typeof(IContextRepository<>), typeof(ContextRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddSingleton<IJwtService, JwtService>();

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

            // JWT wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // Add Identity
            var builder = services.AddIdentityCore<User>(o =>
            {
                // configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            }).AddRoles<IdentityRole>();
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.AddAutoMapper();
            services.AddMvc(options => 
                {
                    // Add automatic model validation
                    options.Filters.Add(typeof(ValidateModelStateAttribute));
                    // options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            // services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            services.AddCors();
            services.AddSignalR();

            // Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Attendance Core Api", Version = "v1" });

                // Swagger 2.+ support
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });
            }
            else
            {
                // app.UseHsts();
            }

            app.UseExceptionHandler(
            builder =>
            {
                builder.Run(
                    async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                        }
                    });
            });

            app.UseCors(builder =>
                 builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                );
            
            app.UseAuthentication();
            app.UseMvc();
            app.Use(async (context, next) => 
            { 
                await next(); 
                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value)) 
                { 
                    context.Request.Path = "/index.html"; 
                    await next(); 
                } 
            }); 

            app.UseDefaultFiles(); 
            app.UseStaticFiles();

            app.UseSignalR(routes =>
            {
                routes.MapHub<BroadcastHub>("/broadcast");
            });

            CreateUsersAndRoles(services).Wait();
            AttendanceConfiguration(services).Wait();
        }

        private async Task CreateUsersAndRoles(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            IdentityResult result;

            // Check if Admin role exists
            var adminExist = await roleManager.RoleExistsAsync("Admin");
            if (!adminExist)
            {
                // Create Admin role
                result = await roleManager.CreateAsync(new IdentityRole("Admin"));

                // Create Default admin account
                var user = new User { UserName = Configuration["DefaultAdminCredentials:UserName"] };
                await userManager.CreateAsync(user, Configuration["DefaultAdminCredentials:Password"]);

                // Assign role
                await userManager.AddToRoleAsync(user, "Admin");
            }

            // Check if Employee role exist
            var employeeExist = await roleManager.RoleExistsAsync("Employee");

            // Create Employee role if does not exist
            if (!employeeExist) await roleManager.CreateAsync(new IdentityRole("Employee"));
        }

        private async Task AttendanceConfiguration(IServiceProvider services)
        {
            using (var context = services.GetRequiredService<ApplicationDbContext>())
            {
                // Check if configuration already exists
                var isConfigExist = await context.Configurations.OrderBy(m => m.Id).FirstOrDefaultAsync();

                if (isConfigExist == null)
                {
                    // Add default configurations from config file
                    await context.Configurations.AddAsync(new Configuration
                    {
                        TimeIn = Configuration["AttendanceConfig:TimeIn"],
                        TimeOut = Configuration["AttendanceConfig:TimeOut"],
                        GracePeriod = Configuration["AttendanceConfig:GracePeriod"]
                    });

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
