using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gnoss.Web.Ontologies.Models.Services
{
    public interface IServicioArchivoService
    {
        public byte[] ObtenerMappingTesauro(string pNombreMapeo);
        public Task<byte[]> ObtenerOntologia(Guid pOntologiaID);
        public Task<byte[]> ObtenerXmlOntologia(Guid pOntologiaID);
        public Task<byte[]> ObtenerOntologiaFraccionada(Guid pOntologiaID, string pNombreFraccion);
        public Task<byte[]> ObtenerXmlOntologiaFraccionado(Guid pOntologiaID, string pNombreFraccion);
        public Task<Guid> GuardarOntologia(Guid pOntologiaID, byte[] pFichero);
        public Task<Guid> GuardarOntologiaFraccionada(Guid pOntologiaID, string pNombreFraccion, byte[] pFichero);
        public Task<Guid> GuardarXmlOntologiaFraccionado(Guid pOntologiaID, string pNombreFraccion, byte[] pFichero);
        public Task<Guid> GuardarXmlOntologia(Guid pOntologiaID, byte[] pFichero);
        public Task<Guid> GuardarCSSOntologia(byte[] pFichero, Guid pDocumentoID, string pDirectorio, string pExtensionArchivo);
        public Task<byte[]> DescargarCSSOntologia(Guid pDocumentoID, string pExtensionArchivo);
        public Task<string[]> ObtenerHistorialOntologia(Guid pOntologiaID);
        public Task<byte[]> DescargarVersion(Guid pOntologiaID, string pVersion);
    }
}
