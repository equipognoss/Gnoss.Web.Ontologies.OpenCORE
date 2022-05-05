using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Gnoss.Web.Ontologies.Middlewares;
using Gnoss.Web.Ontologies.Models.Services;
using System.IO;

namespace Gnoss.Web.Ontologies
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddScoped(typeof(UtilPeticion));
            services.AddScoped(typeof(UtilGeneral));
            services.AddScoped(typeof(Usuario));
            services.AddScoped(typeof(UtilTelemetry));
            services.AddScoped(typeof(Conexion));
            services.AddScoped(typeof(LoggingService));
            services.AddScoped<IServicioArchivoService, ServicioArchivoService>();
            services.AddSingleton(typeof(ConfigService));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GnossOntologies", Version = "v1" });
            });

            ConfigService config = new ConfigService();          
            config.ErrorRoute = Path.Combine(_env.ContentRootPath, "logs");
            string nodoRutaOnto = config.GetRutaOntologias();
            if (!string.IsNullOrEmpty(nodoRutaOnto))
            {
                config.RutaOnto = nodoRutaOnto;
            }
            else
            {
                config.RutaOnto = Path.Combine(_env.ContentRootPath, "archivos");
            }

            string nodoRutaMapping = config.RutaMapping;
            if (string.IsNullOrEmpty(nodoRutaMapping)) 
            { 
                config.RutaMapping = Path.Combine(_env.ContentRootPath, "Mapping");
            }


            if(config.RutaMapping.Length > 0 && config.RutaMapping[config.RutaMapping.Length - 1] != '/' && config.RutaMapping[config.RutaMapping.Length - 1] != '\\')
            {
                config.RutaMapping += "/";
            }
            services.AddSingleton(config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GnossServiciosInternos v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseGnossMiddleware();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
