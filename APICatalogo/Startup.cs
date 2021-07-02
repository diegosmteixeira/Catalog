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
            services.AddScoped<ApiLoggingFilter>();

            string mySqlConnection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => 
            options.UseMySql(mySqlConnection, 
            ServerVersion.AutoDetect(mySqlConnection)));

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //This pipeline receives HTTP requests
            //IApplicationBuilder => Pipeline requisition for application
            //IWebHostEnvironment => what middleware is working
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); //Middleware of development
            }

            //Middleware - custom error
            app.ConfigureExceptionHandler();

            //UseMiddlewareName - this middleware redirect https
            app.UseHttpsRedirection();

            //Middleware for routing
            app.UseRouting();

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
