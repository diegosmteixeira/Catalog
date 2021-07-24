using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Services;
using APICatalogo.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using APICatalogo.Logging;
using APICatalogo.Repository;
using AutoMapper;
using APICatalogo.DTO.Mappings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace APICatalogo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IUnityOfWork, UnityOfWork>();

            services.AddScoped<ApiLoggingFilter>();

            string mySqlConnection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => 
            options.UseMySql(mySqlConnection, 
            ServerVersion.AutoDetect(mySqlConnection)));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme).
                AddJwtBearer(options =>
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = Configuration["TokenConfiguration:Audience"],
                    ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["Jwt:key"]))
                });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling
                    = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            //Transient = created every time is called
            services.AddTransient<IMyService, MyService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //This pipeline receives HTTP requests
            //IApplicationBuilder => Pipeline requisition for application
            //IWebHostEnvironment => what middleware is working
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); //Middleware of development
            }

            loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            {
                LogLevel = LogLevel.Information
            }));

            //Middleware - custom error
            app.ConfigureExceptionHandler();

            //UseMiddlewareName - this middleware redirect https
            app.UseHttpsRedirection();

            //Middleware for routing
            app.UseRouting();

            //Midleware for authentication
            app.UseAuthentication();

            //Middleware for authorization
            app.UseAuthorization(); 

            //This Middleware run endpoint for current request
            app.UseEndpoints(endpoints =>
            {
                //Add endpoints to Controller Actions without especify routes
                endpoints.MapControllers();
            });
        }
    }
}
