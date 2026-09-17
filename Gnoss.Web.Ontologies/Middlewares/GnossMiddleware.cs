using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.AD.EntityModel;

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

        public async Task Invoke(HttpContext context, EntityContext entityContext)
        {
            entityContext.SetTrackingFalse();
            ConfigureServiceOntologies();
            await _next(context);
        }

        void ConfigureServiceOntologies()
        {
            // Código que se ejecuta al iniciarse la aplicación

            //Establezco la ruta del fichero de error por defecto
            LoggingService.RUTA_DIRECTORIO_ERROR = _configService.ErrorRoute;
            //_error.RUTA_DIRECTORIO_ERROR = this.Server.MapPath("~/logs");


            //Configuracion.ObtenerDesdeFicheroConexion = true;
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

