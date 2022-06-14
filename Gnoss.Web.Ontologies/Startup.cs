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
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.AbstractsOpen;
using Es.Riam.OpenReplication;
using Es.Riam.InterfacesOpenArchivos;
using Es.Riam.OpenArchivos;
using System.Collections;
using System;
using Microsoft.EntityFrameworkCore;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

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
            services.AddScoped(typeof(UtilTelemetry));
            services.AddScoped(typeof(Usuario));
            services.AddScoped(typeof(UtilPeticion));
            services.AddScoped(typeof(Conexion));
            services.AddScoped(typeof(UtilGeneral));
            services.AddScoped(typeof(LoggingService));
            services.AddScoped(typeof(RedisCacheWrapper));
            services.AddScoped(typeof(Configuracion));
            services.AddScoped(typeof(GnossCache));
            services.AddScoped(typeof(VirtuosoAD));
            services.AddScoped(typeof(UtilServicios));
            services.AddScoped<IServicesUtilVirtuosoAndReplication, ServicesVirtuosoAndBidirectionalReplicationOpen>();
            services.AddScoped<IServicioArchivoService, ServicioArchivoService>();
            services.AddScoped<IUtilArchivos, UtilArchivosOpen>();

            string bdType = "";
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            if (environmentVariables.Contains("connectionType"))
            {
                bdType = environmentVariables["connectionType"] as string;
            }
            else
            {
                bdType = Configuration.GetConnectionString("connectionType");
            }
            if (bdType.Equals("2"))
            {
                services.AddScoped(typeof(DbContextOptions<EntityContext>));
                services.AddScoped(typeof(DbContextOptions<EntityContextBASE>));
            }
            services.AddSingleton(typeof(ConfigService));
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddMvc();
            string acid = "";
            if (environmentVariables.Contains("acid"))
            {
                acid = environmentVariables["acid"] as string;
            }
            else
            {
                acid = Configuration.GetConnectionString("acid");
            }
            string baseConnection = "";
            if (environmentVariables.Contains("base"))
            {
                baseConnection = environmentVariables["base"] as string;
            }
            else
            {
                baseConnection = Configuration.GetConnectionString("base");
            }
            if (bdType.Equals("0"))
            {
                services.AddDbContext<EntityContext>(options =>
                        options.UseSqlServer(acid)
                        );
                services.AddDbContext<EntityContextBASE>(options =>
                        options.UseSqlServer(baseConnection)

                        );
            }
            else if (bdType.Equals("1"))
            {
                services.AddDbContext<EntityContext, EntityContextOracle>(options =>
                options.UseOracle(acid)
                );
                services.AddDbContext<EntityContextBASE, EntityContextBASEOracle>(options =>
                options.UseOracle(baseConnection)


                );
                //services.AddDbContext<EntityContextOauth, EntityContextOauthOracle>(options =>
                // options.UseOracle(oauthConnection)
                // );
            }
            else if (bdType.Equals("2"))
            {
                services.AddDbContext<EntityContext, EntityContextPostgres>(opt =>
                {
                    var builder = new NpgsqlDbContextOptionsBuilder(opt);
                    builder.SetPostgresVersion(new Version(9, 6));
                    opt.UseNpgsql(acid);

                });
                services.AddDbContext<EntityContextBASE, EntityContextBASEPostgres>(opt =>
                {
                    var builder = new NpgsqlDbContextOptionsBuilder(opt);
                    builder.SetPostgresVersion(new Version(9, 6));
                    opt.UseNpgsql(baseConnection);

                });
            }

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
