using System.IO;
using Ghoul.Web.Configuration;
using Ghoul.Web.Extensions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ghoul.Web
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
            services.AddControllersWithViews();

            #region Infrastructure
                services.AddDatabaseSettings(Configuration);
            #endregion

            #region Application
                services.AddApplicationServices();

                // CQRS
                services.AddMediatR(typeof(Startup));
            #endregion

            #region Domain
                // services.AddDomainServices();
            #endregion

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "Frontend/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                // endpoints.MapControllerRoute(
                //     name: "api",
                //     pattern: "/api/{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "Frontend";

                if (env.IsDevelopment()) {
                    var frontendDirectory = Path.GetFullPath(Path.Combine(env.ContentRootPath, spa.Options.SourcePath));
                    spa.UseYarn(frontendDirectory);
                }

                // if (env.IsDevelopment())
                // {
                //     spa.UseReactDevelopmentServer(npmScript: "start");
                // }
            });
        }
    }
}
