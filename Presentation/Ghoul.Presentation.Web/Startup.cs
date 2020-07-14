using System.IO;
using Ghoul.Application.Configuration.DI;
using Ghoul.Application.Model;
using Ghoul.Application.Model.Queries;
using Ghoul.Application.Service.Handlers.Queries;
using Ghoul.Domain.Configuration.DI;
using Ghoul.Persistence.Configuration.DI;
using Ghoul.Presentation.Web.Configuration.DI;
using Ghoul.Web.Configuration;
using Ghoul.Web.Middleware;
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
            #region Presentation
            services.AddControllersWithViews();
            // Le regole di mapping a livello presentazionale sono definite
            // tramite questo extensions method "AddPresentationMappings"
            services.AddPresentationMappings();
            #endregion

            #region Application
            services.AddApplicationMappings();

            // Configurazione di MediatR, MEDIATOR pattern
            // Consiste nel dargli in input un array di type di oggetti, contenenti COMMANDS e QUERIES.
            // Questi tipi gli servono per capire l'assembly dove cercare i comandi/query (eventi)
            // e gli handler (eventlisteners)
            // Quindi basta dirgli semplicemente UN comando/query, e un HANDLER, se sono entrambi nello stesso assembly
            // (assembly = progetto, in questo caso)
            //
            // "GetAllBuildsQuery" è una query (evento)
            // "GetAllBuildsQueryHandler" è un handler (event listener)
            //
            // Utilizzando le interfacce IRequest e IRequestHandler,
            // la libreria MediatR si fà la scansione di tutte le classi nell'assembly
            // e associa ad ogni richiesta di tipo IRequest, un handler di tipo IRequestHandler
            services.AddMediatR(typeof(GetAllBuildsQuery), typeof(GetAllBuildsQueryHandler));
            #endregion

            #region Persistence
            services.AddDatabaseSettings(Configuration);
            services.AddDBContext();
            services.AddRepositories();
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
