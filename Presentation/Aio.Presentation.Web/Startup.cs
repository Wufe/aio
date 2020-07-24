using System.IO;
using Aio.Application.Configuration.DI;
using Aio.Application.Model;
using Aio.Application.Model.Queries;
using Aio.Application.Service.Handlers.Queries;
using Aio.Domain.Configuration.DI;
using Aio.Persistence.Configuration.DI;
using Aio.Presentation.Web.Configuration.DI;
using Aio.Presentation.Web.HostedServices;
using Aio.Web.Configuration;
using Aio.Web.Middleware;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aio.Web
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
            #region Presentation
            services.AddControllersWithViews();
            services.AddPresentationMappings();
            services.AddBuildRunner();
            #endregion

            #region Application
            services.AddApplicationMappings();
            services.AddDomainRepositories();
            services.AddMediatR(typeof(GetAllBuildsQuery), typeof(GetAllBuildsQueryHandler));
            #endregion

            #region Persistence
            services.AddDatabaseSettings(Configuration);
            services.AddDBContext();
            services.AddGenericRepositories();
            #endregion

            #region Domain
            services.AddDomainServices();
            #endregion

            services.AddYarn();

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