using System;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DashBoard.Business.Services;
using DashBoard.Data.Data;
using DashBoard.Web.Helpers;
using DashBoard.Business.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DashBoard.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env)  //IConfiguration configuration
        {
            _env = env;
            //_configuration = configuration;

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            
            //if (env.Iis)
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var connectionString = Environment.GetEnvironmentVariable("WATCHHOUNDAPI_CONNECTIONSTRING");
            var userSecret = Environment.GetEnvironmentVariable("USER_SECRET");
            var sendGridKey = Environment.GetEnvironmentVariable("SENDGRID_KEY");

            // Register the Swagger services


            services.AddSwaggerDocument();
            services.AddCors();
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            if (environment == "Production")
            {
                services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));
            }
            else
            {
                services.AddDbContext<DataContext>(options => options.UseSqlServer(_configuration["ConnectionStrings:WebApiDatabase"]));
            }

            services.Configure<AppSettings>(appSettings =>
            {
                appSettings.Secret = userSecret;
            }
            );

            services.Configure<AppMailSettings>(sendGrid =>
            {
                sendGrid.SendGridKey = sendGridKey;
            }
            );

            // configure strongly typed settings objects
            //var appSettingsSection = _configuration.GetSection("AppSettings");  
            //services.Configure<AppSettings>(appSettingsSection);

            //// configure jwt authentication
            //var appSettings = appSettingsSection.Get<AppSettings>();
            //var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var key = Encoding.ASCII.GetBytes(userSecret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = int.Parse(context.Principal.Identity.Name);                        
                        var user = userService.GetById(userId, userId.ToString()); //update
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            // configure DI for application services        
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDomainService, DomainService>();
            services.AddScoped<ILogsService, LogsService>();
            services.AddScoped<IRequestService, RequestsService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMailService, MailService>();
            services.AddHostedService<BackgroundMailService>();            

 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DataContext dataContext)
        {
            // Register the Swagger generator and the Swagger UI middlewares
            app.UseOpenApi();
            app.UseSwaggerUi3();
            //migrate any database changes on startup(includes initial db creation)
            dataContext.Database.Migrate();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
