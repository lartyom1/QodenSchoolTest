using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace WebApp
{
    public class Startup
    {
        // TODO 0: Something broken in ConfigureServices.
        // Don't focus attention on it right now, you will faced with problem in process of meeting the challenges TODOs
        // Pay attention to different possible solutions of the problem

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();// add all MVC services

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = true;
                }
                );
            services.AddSingleton<IAccountDatabase, AccountDatabaseStub>();
            services.AddSingleton<IAccountCache, AccountCache>();
            
            services.AddSingleton<IAccountService, AccountService>();//was missing TODO 0
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRouting();// MVC service, required to work with controllers

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseAuthorization();
          
            app.UseEndpoints(endpoints => 
            {
                endpoints.MapControllers();
            });

            app.Run(async (context) => 
            {           
                await context.Response.WriteAsync("Hello World!");
            });

        }
    }
}