using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using AutoMapper;
using Swashbuckle.AspNetCore.Swagger;
using WebApi.Infrastructure;
using WebApi.Entities;
using Hubs.BroadcastHub;
using MediatR;
using WebApi.Extensions;
using WebApi.Constants;
using System.Reflection;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Get key from Constants folder
            string secretKey = SecretKey.Value;
            var _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));
            
            // Setup Database connection
            if(!Environment.IsEnvironment("Test"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    // MS SQL database (default)
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))

                    // MySql database
                    // https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql
                    // options.UseMySql(Configuration.GetConnectionString("DefaultConnection"))

                    // MySql database
                    // https://www.nuget.org/packages/Npgsql.EntityFrameworkCore.PostgreSQL
                    // options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))

                    // See more database provider options:
                    // https://docs.microsoft.com/en-us/ef/core/providers/
                );
            }
            else
            {
                // Used for Integration Tests
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("Attendance Test Db")
                );
            }

            // DI: IoC
            services.AddMediatR(typeof(Startup));
            
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

            // Turn off the Validation for Issuer and Audience on Integration Tests
            var validateIssuerOrAudience = !Environment.IsEnvironment("Test");

            var tokEnvironmentalidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = validateIssuerOrAudience,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = validateIssuerOrAudience,
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
                configureOptions.TokenValidationParameters = tokEnvironmentalidationParameters;
                configureOptions.SaveToken = true;

                // We have to hook the OnMessageReceived event in order to
                // allow the JWT authentication handler to read the access
                // token from the query string when a WebSocket or 
                // Server-Sent Events request comes in.
                // https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-2.2
                configureOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/broadcast")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // Add Identity
            services.AddIdentityCore<User>(o =>
            {
                // Configure identity options
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddAutoMapper();
            services.AddMvc(o => 
            {
                // Catch cancelled exceptions
                o.Filters.Add<OperationCancelledExceptionFilter>();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            
            // X-CSRF-Token
            services.AddAntiforgery(options=> 
            {
                options.HeaderName = "X-XSRF-Token";
                options.SuppressXFrameOptionsHeader = false;
            });

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

                // Add XML comments in swagger
                // https://exceptionnotfound.net/adding-swagger-to-asp-net-core-web-api-using-xml-documentation/
                
                // Locate the XML file being generated by ASP.NET...
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                //... and tell Swagger to use those XML comments.
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment Environment, IServiceProvider services)
        {
            if (Environment.IsDevelopment())
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

            // Enable CORS
            app.UseCors(builder =>
                 builder
                    // from .netcore 2.1 => 2.2 
                    // .AllowAnyOrigin()
                    .SetIsOriginAllowed((host) => true)
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

            // Single Page Application set up
            app.UseDefaultFiles(); 
            app.UseStaticFiles();

            // Set up SignalR Hubs
            app.UseSignalR(routes =>
            {
                routes.MapHub<BroadcastHub>("/broadcast");
            });

            // Automatic database migration
            SeedData.InitializeDatabase(services);

            // Identity user seed
            SeedData.CreateDefaultAdminAndRoles(services).Wait();

            // Default Attendance Configuration
            SeedData.AttendanceConfiguration(services).Wait();

            if (!Environment.IsProduction())
            {
                SeedData.EnsureSeedEmployeesAndLogs(services).Wait();
            }
        }
    }
}
