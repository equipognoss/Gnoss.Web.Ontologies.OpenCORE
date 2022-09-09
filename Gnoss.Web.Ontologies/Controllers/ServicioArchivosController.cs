using Es.Riam.Gnoss.FileManager;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Gnoss.Web.Ontologies.Models.Services;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Gnoss.Web.Ontologies.Controllers
{
    [ApiController]
    [Authorize]
    public class ServicioArchivosController : ControllerBase

    {

        #region Miembros
        private readonly ILogger<ServicioArchivosController> _logger;
        private readonly IHostingEnvironment _env;
        private readonly IServicioArchivoService _servicioArchivo;
        #endregion

        #region Constructor

        public ServicioArchivosController(IHostingEnvironment env, IServicioArchivoService servicioArchivo, ILogger<ServicioArchivosController> logger)
        {
            _logger = logger;
            _env = env;
            _servicioArchivo = servicioArchivo;
        }

        #endregion

        #region Metodos web

        [Microsoft.AspNetCore.Mvc.NonAction]
        public virtual void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {

        }

        [HttpGet]
        [Route("ObtenerMappingTesauro")]
        public IActionResult ObtenerMappingTesauro(string pNombreMapeo)
        {
            try
            {
                var result = _servicioArchivo.ObtenerMappingTesauro(pNombreMapeo);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest($"No se puede obtener un mapeo de {pNombreMapeo}");
                }
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Obtiene una ontología.
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <returns>Array del archivo</returns>
        [HttpGet]
        [Route("ObtenerOntologia")]
        public async Task<IActionResult> ObtenerOntologia(Guid pOntologiaID)
        {
            try
            {
                return Ok(await _servicioArchivo.ObtenerOntologia(pOntologiaID));
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Obtiene el XML de una ontología.
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <returns>Array del archivo</returns>
        [HttpGet]
        [Route("ObtenerXmlOntologia")]
        public async Task<IActionResult> ObtenerXmlOntologia(Guid pOntologiaID)
        {
            try
            {
                return Ok(await _servicioArchivo.ObtenerXmlOntologia(pOntologiaID));
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Obtiene una ontología.
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pNombreFraccion">Nombre del archivo fraccionado del owl</param>
        /// <returns>Array del archivo</returns>
        [HttpGet]
        [Route("ObtenerOntologiaFraccionada")]
        public async Task<IActionResult> ObtenerOntologiaFraccionada(Guid pOntologiaID, string pNombreFraccion)
        {
            try
            {
                return Ok(await _servicioArchivo.ObtenerOntologiaFraccionada(pOntologiaID, pNombreFraccion));
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Obtiene el XML de una ontología.
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pNombreFraccion">Nombre del archivo fraccionado del xml</param>
        /// <returns>Array del archivo</returns>
        [HttpGet]
        [Route("ObtenerXmlOntologiaFraccionado")]
        public async Task<IActionResult> ObtenerXmlOntologiaFraccionado(Guid pOntologiaID, string pNombreFraccion)
        {
            try
            {
                return Ok(await _servicioArchivo.ObtenerXmlOntologiaFraccionado(pOntologiaID, pNombreFraccion));
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Guarda una ontología.
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pFichero">Fichero de la ontología</param>
        /// <returns>Confirmación de la operación</returns>
        [HttpPost]
        [Route("GuardarOntologia")]
        public async Task<IActionResult> GuardarOntologia(IFormFile pFichero, Guid pOntologiaID)
        {
            try
            {
                Guid result = await _servicioArchivo.GuardarOntologia(pOntologiaID, ObtenerBytes(pFichero));
                return Ok(result);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Guarda una fracción del owl de una ontología.
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pNombreFraccion">Nombre del fichero que es una fracción del owl de la ontología</param>
        /// <param name="pFichero">Fichero de la ontología fraccionada</param>
        /// <returns>Confirmación de la operación</returns>
        [HttpPost]
        [Route("GuardarOntologiaFraccionada")]
        public async Task<IActionResult> GuardarOntologiaFraccionada(IFormFile pFichero, Guid pOntologiaID, string pNombreFraccion)
        {
            try 
            { 
                return Ok(await _servicioArchivo.GuardarOntologiaFraccionada(pOntologiaID, pNombreFraccion, ObtenerBytes(pFichero)));
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Guarda una fracción del Xml de una ontología.
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pNombreFraccion">Nombre del fichero que es una fracción del xml de la ontología</param>
        /// <param name="pFichero">Fichero del Xml de la ontología</param>
        /// <returns>Confirmación de la operación</returns>
        [HttpPost]
        [Route("GuardarXmlOntologiaFraccionado")]
        public async Task<IActionResult> GuardarXmlOntologiaFraccionado(IFormFile pFichero, Guid pOntologiaID, string pNombreFraccion)
        {
            try
            {
                return Ok(await _servicioArchivo.GuardarXmlOntologiaFraccionado(pOntologiaID, pNombreFraccion, ObtenerBytes(pFichero)));
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Guarda el Xml de una ontología.
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pFichero">Fichero del Xml de la ontología</param>
        /// <returns>Confirmación de la operación</returns>
        [HttpPost]
        [Route("GuardarXmlOntologia")]
        public async Task<IActionResult> GuardarXmlOntologia(IFormFile pFichero, Guid pOntologiaID)
        {
            try
            {
                return Ok(await _servicioArchivo.GuardarXmlOntologia(pOntologiaID, ObtenerBytes(pFichero)));
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Guarda el CSS de una ontología.
        /// </summary>
        /// <param name="pFichero">Fichero del CSS de la ontología, si es </param>
        /// <param name="pDocumentoID">Identificador del Documento</param>
        /// <param name="pDirectorio">Directorio donde se agrega el archivo</param>
        /// <param name="pExtensionArchivo">Extensión del archivo</param>
        [HttpPost]
        [Route("GuardarCSSOntologia")]
        public async Task<IActionResult> GuardarCSSOntologia(IFormFile pFichero, Guid pDocumentoID, string pDirectorio, string pExtensionArchivo)
        {
            try
            {

                return Ok(await _servicioArchivo.GuardarCSSOntologia(ObtenerBytes(pFichero), pDocumentoID, pDirectorio, pExtensionArchivo));
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("DescargarCSSOntologia")]
        public async Task<IActionResult> DescargarCSSOntologia(Guid pDocumentoID, string pExtensionArchivo)
        {
            return Ok(await _servicioArchivo.DescargarCSSOntologia(pDocumentoID, pExtensionArchivo));
        }

        /// <summary>
        /// Descarga una versión concreta del historial de una ontología
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pVersion">Versión del archivo a descargar (ej: 20160322_092000_B4A6887E-55B0-4F2C-B049-7F10A93E845B.xml)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("DescargarVersion")]
        public async Task<IActionResult> DescargarVersion(Guid pOntologiaID, string pVersion)
        {
            try
            {
                return Ok(await _servicioArchivo.DescargarVersion(pOntologiaID, pVersion));
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("ObtenerHistorialOntologia")]
        public async Task<IActionResult> ObtenerHistorialOntologia(Guid pOntologiaID)
        {
            return Ok(await _servicioArchivo.ObtenerHistorialOntologia(pOntologiaID));
        }

        [NonAction]
        private byte[] ObtenerBytes(IFormFile pFichero)
        {
            byte[] fileBytes = null;
            if (pFichero != null)
            {
                using (var ms = new MemoryStream())
                {
                    pFichero.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
            }
            return fileBytes;
        }

        #endregion

    }
}
