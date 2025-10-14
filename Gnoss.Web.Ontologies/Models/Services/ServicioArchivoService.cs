using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.FileManager;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.InterfacesOpenArchivos;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Gnoss.Web.Ontologies.Models.Services
{
    public class ServicioArchivoService : IServicioArchivoService
    {
        private GestionArchivos mGestorArchivos;
        private readonly LoggingService _loggingService;
        private readonly IHostingEnvironment _env;
        private readonly ConfigService _configService;
        private readonly IUtilArchivos _utilArchivos;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ServicioArchivoService(ConfigService configService, LoggingService loggingService, IHostingEnvironment env, IUtilArchivos utilArchivos, ILogger<ServicioArchivoService> logger, ILoggerFactory loggerFactory)
        {
            _loggingService = loggingService;
            _configService = configService;
            _utilArchivos = utilArchivos;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mGestorArchivos = new GestionArchivos(loggingService, utilArchivos, mLoggerFactory.CreateLogger<GestionArchivos>(), mLoggerFactory, pRutaArchivos: configService.GetRutaOntologias(), pAzureStorageConnectionString: configService.GetCadenaConexionAzureStorage());
            _env = env;
        }


        public byte[] ObtenerMappingTesauro(string pNombreMapeo)
        {

            byte[] array = null;

            if (string.IsNullOrEmpty(_configService.GetCadenaConexionAzureStorage()))
            {
                //En azure no funcionará
                try
                {
                    string ruta = ObtenerRutaMapeo(pNombreMapeo);
                    FileInfo fileInfo = new FileInfo(ruta);
                    if (fileInfo.Exists)
                    {
                        FileStream file = new FileStream(ruta, FileMode.Open);
                        array = new byte[(int)file.Length];
                        file.Read(array, 0, (int)file.Length);
                        file.Close();
                        file.Dispose();
                        file = null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    _loggingService.GuardarLogError(ex, mlogger);
                    throw;
                }
            }
            return array;
        }

        public async Task<byte[]> ObtenerOntologia(Guid pOntologiaID)
        {
            try
            {
                return await mGestorArchivos.DescargarFichero(ObtenerRutaBase(pOntologiaID), ObtenerNombreArchivoOwlOntologia(pOntologiaID));
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, mlogger);
                throw new Exception(ex.Message);
            }
        }
        public async Task<byte[]> ObtenerXmlOntologia(Guid pOntologiaID)
        {
            try
            {
                return await mGestorArchivos.DescargarFichero(ObtenerRutaBase(pOntologiaID), ObtenerNombreArchivoXmlOntologia(pOntologiaID));
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, mlogger);
                throw new Exception(ex.Message);
            }
        }

        public async Task<byte[]> ObtenerOntologiaFraccionada(Guid pOntologiaID, string pNombreFraccion)
        {
            try
            {
                string ruta = Path.Combine(ObtenerRutaBase(pOntologiaID), pOntologiaID.ToString(), "owl");
                string archivo = pNombreFraccion + ".owl";

                return await mGestorArchivos.DescargarFichero(ruta, archivo);
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, mlogger);
                throw new Exception(ex.Message);
            }
        }

        public async Task<byte[]> ObtenerXmlOntologiaFraccionado(Guid pOntologiaID, string pNombreFraccion)
        {
            try
            {
                string ruta = Path.Combine(ObtenerRutaBase(pOntologiaID), pOntologiaID.ToString());
                string archivo = pNombreFraccion + ".xml";

                return await mGestorArchivos.DescargarFichero(ruta, archivo);
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, mlogger);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Guid> GuardarOntologia(Guid pOntologiaID, byte[] pFichero)
        {
            try
            {
                string ruta = ObtenerRutaBase(pOntologiaID);
                bool existe = await mGestorArchivos.ComprobarExisteDirectorio(ruta);
                if (!existe)
                {
                    mGestorArchivos.CrearDirectorioFisico(ruta);
                }

                string archivo = ObtenerNombreArchivoOwlOntologia(pOntologiaID);
                await HacerBackupArchivo(ruta, archivo);

                mGestorArchivos.CrearFicheroFisico(ruta, archivo, pFichero);
                return pOntologiaID;
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, mlogger);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Guid> GuardarOntologiaFraccionada(Guid pOntologiaID, string pNombreFraccion, byte[] pFichero)
        {
            try
            {
                string ruta = Path.Combine(ObtenerRutaBase(pOntologiaID), pOntologiaID.ToString(), "owl");
                string archivo = pNombreFraccion + ".owl";

                await CrearFichero(ruta, pFichero, archivo);

                return pOntologiaID;
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, mlogger);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Guid> GuardarXmlOntologiaFraccionado(Guid pOntologiaID, string pNombreFraccion, byte[] pFichero)
        {
            try
            {
                string ruta = Path.Combine(ObtenerRutaBase(pOntologiaID), pOntologiaID.ToString());
                string archivo = pNombreFraccion + ".xml";

                await CrearFichero(ruta, pFichero, archivo);

                return pOntologiaID;
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, mlogger);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Guid> GuardarXmlOntologia(Guid pOntologiaID, byte[] pFichero)
        {
            try
            {
                string ruta = ObtenerRutaBase(pOntologiaID);

                string archivo = ObtenerNombreArchivoXmlOntologia(pOntologiaID);
                await HacerBackupArchivo(ruta, archivo);

                await CrearFichero(ruta, pFichero, archivo);

                return pOntologiaID;
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, mlogger);
                throw new Exception(ex.Message);
            }
        }



        public async Task<Guid> GuardarCSSOntologia(byte[] pFichero, Guid pDocumentoID, string pDirectorio, string pExtensionArchivo)
        {
            try
            {
                string azureStorageConnectionStringCSS = null;

                if (!string.IsNullOrEmpty(_configService.GetCadenaConexionAzureStorage()))
                {
                    azureStorageConnectionStringCSS = _configService.GetCadenaConexionAzureStorage();

                    azureStorageConnectionStringCSS = azureStorageConnectionStringCSS.Trim('/').Trim('\\');

                    int indiceUltimoTramo = azureStorageConnectionStringCSS.LastIndexOf('/');

                    if (azureStorageConnectionStringCSS.LastIndexOf('\\') > indiceUltimoTramo)
                    {
                        indiceUltimoTramo = azureStorageConnectionStringCSS.LastIndexOf('\\');
                    }

                    azureStorageConnectionStringCSS = azureStorageConnectionStringCSS.Substring(0, indiceUltimoTramo);

                    if (!azureStorageConnectionStringCSS.EndsWith("/Ontologias"))
                    {
                        azureStorageConnectionStringCSS += "/Ontologias";
                    }
                }

                string rutaOntologias = _configService.GetRutaOntologias();
                //string rutaOntologias = Server.MapPath("~/" + UtilArchivos.ContentOntologias);


                GestionArchivos gestorArchivosCSS = new GestionArchivos(_loggingService, _utilArchivos, mLoggerFactory.CreateLogger<GestionArchivos>(), mLoggerFactory, rutaOntologias, azureStorageConnectionStringCSS);

                bool existeDirectorio = await gestorArchivosCSS.ComprobarExisteDirectorio(pDirectorio);
                if (!existeDirectorio)
                {
                    gestorArchivosCSS.CrearDirectorioFisico(pDirectorio);
                }

                if (pFichero != null)
                {
                    gestorArchivosCSS.CrearFicheroFisico(pDirectorio, pDocumentoID + pExtensionArchivo, pFichero);
                }

                return pDocumentoID;
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, mlogger);
                throw new Exception(ex.Message);
            }
        }

        public async Task<byte[]> DescargarCSSOntologia(Guid pDocumentoID, string pExtensionArchivo)
        {
            return await mGestorArchivos.DescargarFichero(ObtenerRutaBase(pDocumentoID), pDocumentoID + pExtensionArchivo);
        }

        public async Task<byte[]> DescargarVersion(Guid pOntologiaID, string pVersion)
        {
            try
            {
                string ruta = Path.Combine(ObtenerRutaBase(pOntologiaID), "backup");
                return await mGestorArchivos.DescargarFichero(ruta, pVersion);
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, mlogger);
                throw new Exception(ex.Message);
            }
        }

        public async Task<string[]> ObtenerHistorialOntologia(Guid pOntologiaID)
        {
            string ruta = Path.Combine(ObtenerRutaBase(pOntologiaID), "backup");

            return await mGestorArchivos.ObtenerFicherosDeDirectorio(ruta);
        }

        private string ObtenerRutaMapeo(string pNombreMapeo)
        {
            return $"{_configService.RutaMapping}/{pNombreMapeo}";
        }

        /// <summary>
        /// Obtiene la ruta base para los archivos.
        /// </summary>
        /// <param name="pOntologiaID">ID de ontología</param>
        /// <returns>Ruta de una ontología</returns>
        private string ObtenerRutaBase(Guid pOntologiaID)
        {
            string ruta = null;

            if (pOntologiaID == Guid.Empty)
            {
                ruta = "curriculum";
            }
            else
            {
                ruta = pOntologiaID.ToString().Substring(0, 3);
            }

            return ruta;
        }

        /// <summary>
        /// Obtiene la ruta de una ontología.
        /// </summary>
        /// <param name="pOntologiaID">ID de ontología</param>
        /// <returns>Ruta de una ontología</returns>
        private string ObtenerNombreArchivoOwlOntologia(Guid pOntologiaID)
        {
            if (pOntologiaID == Guid.Empty)
            {
                return "Curriculum.owl";
            }
            else
            {
                return pOntologiaID + ".owl";
            }
        }

        /// <summary>
        /// Obtiene la ruta del Xml de una ontología.
        /// </summary>
        /// <param name="pOntologiaID">ID de ontología</param>
        /// <returns>Ruta del Xml de una ontología</returns>
        private string ObtenerNombreArchivoXmlOntologia(Guid pOntologiaID)
        {
            if (pOntologiaID == Guid.Empty)
            {
                return "Curriculum.xml";
            }
            else
            {
                return pOntologiaID + ".xml";
            }
        }

        private async Task HacerBackupArchivo(string pRuta, string pArchivo)
        {
            bool exiteArchivo = await mGestorArchivos.ComprobarExisteArchivo(pRuta, pArchivo);
            if (exiteArchivo)
            {
                string rutaDestino = Path.Combine(pRuta, "backup");

                bool existeDirectorio = await mGestorArchivos.ComprobarExisteDirectorio(rutaDestino);
                if (!existeDirectorio)
                {
                    mGestorArchivos.CrearDirectorioFisico(rutaDestino);
                }

                mGestorArchivos.CopiarArchivo(pRuta, rutaDestino, pArchivo, true, pNombreArchivoDestino: DateTime.Now.ToString("yyyyMMdd_HHmmss_") + pArchivo);
            }
        }

        private async Task CrearFichero(string pRuta, byte[] pFichero, string pArchivo)
        {
            bool existeDirectorio = await mGestorArchivos.ComprobarExisteDirectorio(pRuta);
            if (!existeDirectorio)
            {
                mGestorArchivos.CrearDirectorioFisico(pRuta);
            }

            if (pFichero != null)
            {
                mGestorArchivos.CrearFicheroFisico(pRuta, pArchivo, pFichero);
            }
            else
            {
                bool existeArchivo = await mGestorArchivos.ComprobarExisteArchivo(pRuta, pArchivo);
                if (existeArchivo)
                {
                    mGestorArchivos.EliminarFicheroFisico(pRuta, pArchivo);
                }
            }
        }
        public async Task<Guid> GuardarDocumentacionOntologia(Guid communityID, byte[] pFichero, string lang)
        {
            try
            {
                string rutaBase = _configService.GetRutaOntologias();
                string rutaCompleta = Path.Combine(rutaBase, "documentacionOnto");

                string azureStorageConnectionStringCSS = null;
                if (!string.IsNullOrEmpty(_configService.GetCadenaConexionAzureStorage()))
                {
                    azureStorageConnectionStringCSS = _configService.GetCadenaConexionAzureStorage();

                    azureStorageConnectionStringCSS = azureStorageConnectionStringCSS.Trim('/').Trim('\\');

                    int indiceUltimoTramo = azureStorageConnectionStringCSS.LastIndexOf('/');

                    if (azureStorageConnectionStringCSS.LastIndexOf('\\') > indiceUltimoTramo)
                    {
                        indiceUltimoTramo = azureStorageConnectionStringCSS.LastIndexOf('\\');
                    }

                    azureStorageConnectionStringCSS = azureStorageConnectionStringCSS.Substring(0, indiceUltimoTramo);

                    if (!azureStorageConnectionStringCSS.EndsWith("/Ontologias"))
                    {
                        azureStorageConnectionStringCSS += "/Ontologias";
                    }
                }

                string rutaDireccionOntologias = _configService.GetRutaOntologias();
                GestionArchivos gestorArchivos = new GestionArchivos(_loggingService, _utilArchivos, mLoggerFactory.CreateLogger<GestionArchivos>(), mLoggerFactory, rutaDireccionOntologias, azureStorageConnectionStringCSS);

                string pDirectorio = "documentationOntology";
                string pExtensionArchivo = ".html";
                

                bool existeDirectorio = await gestorArchivos.ComprobarExisteDirectorio(pDirectorio);
                if (!existeDirectorio)
                {
                    gestorArchivos.CrearDirectorioFisico(pDirectorio);
                }
                if (pFichero != null)
                {
                    gestorArchivos.CrearFicheroFisico(pDirectorio, communityID + "-" + lang+ pExtensionArchivo, pFichero);
                }
                return communityID;
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex);
                throw new Exception(ex.Message);
            }

        }

        public async Task<Guid> GuardarOntologiaUnificada(Guid communityID, byte[] pFichero)
        {
            try
            {
                string azureStorageConnectionStringCSS = null;
                if (!string.IsNullOrEmpty(_configService.GetCadenaConexionAzureStorage()))
                {
                    azureStorageConnectionStringCSS = _configService.GetCadenaConexionAzureStorage();

                    azureStorageConnectionStringCSS = azureStorageConnectionStringCSS.Trim('/').Trim('\\');

                    int indiceUltimoTramo = azureStorageConnectionStringCSS.LastIndexOf('/');

                    if (azureStorageConnectionStringCSS.LastIndexOf('\\') > indiceUltimoTramo)
                    {
                        indiceUltimoTramo = azureStorageConnectionStringCSS.LastIndexOf('\\');
                    }

                    azureStorageConnectionStringCSS = azureStorageConnectionStringCSS.Substring(0, indiceUltimoTramo);

                    if (!azureStorageConnectionStringCSS.EndsWith("/Ontologias"))
                    {
                        azureStorageConnectionStringCSS += "/Ontologias";
                    }
                }

                string rutaDireccionOntologias = _configService.GetRutaOntologias();
                GestionArchivos gestorArchivos = new GestionArchivos(_loggingService, _utilArchivos, mLoggerFactory.CreateLogger<GestionArchivos>(), mLoggerFactory, rutaDireccionOntologias, azureStorageConnectionStringCSS);

                string pDirectorio = "unifiedOntology";
                string pExtensionArchivo = ".owl";


                bool existeDirectorio = await gestorArchivos.ComprobarExisteDirectorio(pDirectorio);
                if (!existeDirectorio)
                {
                    gestorArchivos.CrearDirectorioFisico(pDirectorio);
                }
                if (pFichero != null)
                {
                    //TODO: Comprobar si genera el nombre del archivo correctamente
                    gestorArchivos.CrearFicheroFisico(pDirectorio, communityID + pExtensionArchivo, pFichero);
                }
                return communityID;
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex);
                throw new Exception(ex.Message);
            }

        }

        public async Task<byte[]> ObtenerOntologiaUnificada(Guid pOntologiaID)
        {
            try
            {
                return await mGestorArchivos.DescargarFichero("unifiedOntology", $"{pOntologiaID.ToString()}.owl");
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex);
                throw new Exception(ex.Message);
            }
        }

        public async Task<byte[]> ObtenerDocumentacionOntologia(Guid communityID, string lang)
        {
            try
            {
                string fileName = $"{communityID.ToString()}-{lang}.html";
                return await mGestorArchivos.DescargarFichero("documentationOntology", fileName);
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex);
                throw new Exception(ex.Message);
            }
        }


    }
}
