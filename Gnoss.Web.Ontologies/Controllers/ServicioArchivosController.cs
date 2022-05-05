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

namespace Gnoss.Web.Ontologies.Controllers
{
    [ApiController]
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
        public IActionResult ObtenerOntologia(Guid pOntologiaID)
        {
            try
            {
                byte[] bytes = _servicioArchivo.ObtenerOntologia(pOntologiaID).Result;
                //return File(bytes, "application/xml",$"{pOntologiaID}.owl");
                return Ok(bytes);
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
        public IActionResult ObtenerXmlOntologia(Guid pOntologiaID)
        {
            try
            {
                return Ok(_servicioArchivo.ObtenerXmlOntologia(pOntologiaID).Result);
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
        public IActionResult ObtenerOntologiaFraccionada(Guid pOntologiaID, string pNombreFraccion)
        {
            try
            {
                return Ok(_servicioArchivo.ObtenerOntologiaFraccionada(pOntologiaID, pNombreFraccion).Result);
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
        public IActionResult ObtenerXmlOntologiaFraccionado(Guid pOntologiaID, string pNombreFraccion)
        {
            try
            {
                return Ok(_servicioArchivo.ObtenerXmlOntologiaFraccionado(pOntologiaID, pNombreFraccion).Result);
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
        public IActionResult GuardarOntologia(IFormFile pFichero, Guid pOntologiaID)
        {
            try
            {
                return Ok(_servicioArchivo.GuardarOntologia(pOntologiaID, ObtenerBytes(pFichero)).Result);
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
        public IActionResult GuardarOntologiaFraccionada(IFormFile pFichero, Guid pOntologiaID, string pNombreFraccion)
        {
            try 
            { 
                return Ok(_servicioArchivo.GuardarOntologiaFraccionada(pOntologiaID, pNombreFraccion, ObtenerBytes(pFichero)).Result);
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
        public IActionResult GuardarXmlOntologiaFraccionado(IFormFile pFichero, Guid pOntologiaID, string pNombreFraccion)
        {
            try
            {
                return Ok(_servicioArchivo.GuardarXmlOntologiaFraccionado(pOntologiaID, pNombreFraccion, ObtenerBytes(pFichero)).Result);
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
        public IActionResult GuardarXmlOntologia(IFormFile pFichero, Guid pOntologiaID)
        {
            try
            {
                return Ok(_servicioArchivo.GuardarXmlOntologia(pOntologiaID, ObtenerBytes(pFichero)).Result);
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
        public IActionResult GuardarCSSOntologia(IFormFile pFichero, Guid pDocumentoID, string pDirectorio, string pExtensionArchivo)
        {
            try
            {
                return Ok(_servicioArchivo.GuardarCSSOntologia(ObtenerBytes(pFichero), pDocumentoID, pDirectorio, pExtensionArchivo).Result);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("DescargarCSSOntologia")]
        public IActionResult DescargarCSSOntologia(Guid pDocumentoID, string pExtensionArchivo)
        {
            return Ok(_servicioArchivo.DescargarCSSOntologia(pDocumentoID, pExtensionArchivo).Result);
        }

        /// <summary>
        /// Descarga una versión concreta del historial de una ontología
        /// </summary>
        /// <param name="pOntologiaID">Identificador de la ontología</param>
        /// <param name="pVersion">Versión del archivo a descargar (ej: 20160322_092000_B4A6887E-55B0-4F2C-B049-7F10A93E845B.xml)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("DescargarVersion")]
        public IActionResult DescargarVersion(Guid pOntologiaID, string pVersion)
        {
            try
            {
                return Ok(_servicioArchivo.DescargarVersion(pOntologiaID, pVersion).Result);
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("ObtenerHistorialOntologia")]
        public IActionResult ObtenerHistorialOntologia(Guid pOntologiaID)
        {
            return Ok(_servicioArchivo.ObtenerHistorialOntologia(pOntologiaID).Result);
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
