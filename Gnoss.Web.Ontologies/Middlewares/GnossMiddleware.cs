using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Gnoss.Web.Ontologies.Models.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Gnoss.Web.Ontologies.Middlewares
{
    public class GnossMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ConfigService _configService;
        public GnossMiddleware(RequestDelegate next, ConfigService configService)
        {
            _next = next;
            _configService = configService;
        }

        public async Task Invoke(HttpContext context, UtilTelemetry utilTelemetry)
        {
            ConfigureServiceOntologies(utilTelemetry);
            await _next(context);
        }

        void ConfigureServiceOntologies(UtilTelemetry utilTelemetry)
        {
            // Código que se ejecuta al iniciarse la aplicación

            //Establezco la ruta del fichero de error por defecto
            LoggingService.RUTA_DIRECTORIO_ERROR = _configService.ErrorRoute;
            //_error.RUTA_DIRECTORIO_ERROR = this.Server.MapPath("~/logs");


            //Configuracion.ObtenerDesdeFicheroConexion = true;

            string nodoRutaLogstash = _configService.GetLogstashEndpoint();
            if (!string.IsNullOrEmpty(nodoRutaLogstash))
            {
                LoggingService.InicializarLogstash(nodoRutaLogstash);
            }

            LeerConfiguracionApplicationInsights(utilTelemetry);
        }

        /// <summary>
        /// Obtiene la configuración de application insights
        /// </summary>
        private void LeerConfiguracionApplicationInsights(UtilTelemetry utilTelemetry)
        {


            string implementationKeyNode = _configService.GetApplicationImplementationKey();
            if (!string.IsNullOrEmpty(implementationKeyNode))
            {
                string implementationKey = implementationKeyNode.ToLower();

                if (!string.IsNullOrEmpty(implementationKey))
                {
                    Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey = implementationKey;
                    utilTelemetry.Telemetry.InstrumentationKey = implementationKey;
                }
            }

            string logsLocationNode = _configService.GetApplicationLogLocation();
            if (!string.IsNullOrEmpty(logsLocationNode))
            {
                int valorInt = 0;
                if (int.TryParse(logsLocationNode, out valorInt))
                {
                    if (Enum.IsDefined(typeof(UtilTelemetry.UbicacionLogsYTrazas), valorInt))
                    {
                        LoggingService.UBICACIONLOGS = (UtilTelemetry.UbicacionLogsYTrazas)valorInt;
                    }
                }
            }

        }
    }

    public static class GlobalAsaxExtensions
    {
        public static IApplicationBuilder UseGnossMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GnossMiddleware>();
        }
    }
}

