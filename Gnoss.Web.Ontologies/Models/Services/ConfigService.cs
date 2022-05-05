using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.IO;

namespace Gnoss.Web.Ontologies.Models.Services
{
    public class ConfigService
    {
        public IConfigurationRoot Configuration { get; set; }
        private string rutaOntologias;
        private string rutaMapping;
        private string implementationKey;
        private string logLocation;
        private string logstashEndpoint;
        private string cadenaConexion;
        private string cadenaConexionVirtuoso;
        private string cadenaConexionAzureStorage;
        private string errorRoute;
        private string rutaOnto;
        public ConfigService()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }
        public string ErrorRoute
        {
            get
            {
                return errorRoute;
            }
            set
            {
                if (string.IsNullOrEmpty(errorRoute))
                {
                    errorRoute = value;
                }
            }
        }

        public string RutaOnto
        {
            get
            {
                return rutaOnto;
            }
            set
            {
                if (string.IsNullOrEmpty(rutaOnto))
                {
                    rutaOnto = value;
                }
            }
        }

        public string GetRutaOntologias()
        {
            if (string.IsNullOrEmpty(rutaOntologias))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("rutaOntologias"))
                {
                    rutaOntologias = environmentVariables["rutaOntologias"] as string;
                }
                else
                {
                    rutaOntologias = Configuration["rutaOntologias"];
                }

            }
            return rutaOntologias;
        }

        public string RutaMapping
        {
            get
            {
                if (string.IsNullOrEmpty(rutaMapping))
                {
                    IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                    if (environmentVariables.Contains("rutaMapping"))
                    {
                        rutaMapping = environmentVariables["rutaMapping"] as string;
                    }
                    else
                    {
                        rutaMapping = Configuration["rutaMapping"];
                    }
                }
                return rutaMapping;
            }
            set
            {
                rutaMapping = value;
            }
        }

        public string GetApplicationImplementationKey()
        {
            if (string.IsNullOrEmpty(implementationKey))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("applicationInsights:implementationKey"))
                {
                    implementationKey = environmentVariables["applicationInsights:implementationKey"] as string;
                }
                else
                {
                    implementationKey = Configuration["applicationInsights:implementationKey"];
                }
            }
            return implementationKey;
        }

        public string GetApplicationLogLocation()
        {
            if (string.IsNullOrEmpty(logLocation))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("applicationInsights:logLocation"))
                {
                    logLocation = environmentVariables["applicationInsights:logLocation"] as string;
                }
                else
                {
                    logLocation = Configuration["applicationInsights:logLocation"];
                }
            }
            return logLocation;
        }

        public string GetLogstashEndpoint()
        {
            if (string.IsNullOrEmpty(logstashEndpoint))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("logstashEndpoint"))
                {
                    logstashEndpoint = environmentVariables["logstashEndpoint"] as string;
                }
                else
                {
                    logstashEndpoint = Configuration["logstash:logstashEndpoint"];
                }
            }
            return logstashEndpoint;
        }

        public string GetCadenaConexion()
        {
            if (string.IsNullOrEmpty(cadenaConexion))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("cadenaConexion"))
                {
                    cadenaConexion = environmentVariables["cadenaConexion"] as string;
                }
                else
                {
                    cadenaConexion = Configuration.GetConnectionString("cadenaConexion");
                }
            }
            return cadenaConexion;
        }

        public string GetCadenaConexionVirtuoso()
        {
            if (string.IsNullOrEmpty(cadenaConexionVirtuoso))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("cadenaConexionVirtuoso"))
                {
                    cadenaConexionVirtuoso = environmentVariables["cadenaConexionVirtuoso"] as string;
                }
                else
                {
                    cadenaConexionVirtuoso = Configuration.GetConnectionString("cadenaConexionVirtuoso");
                }
            }
            return cadenaConexionVirtuoso;
        }

        public string GetCadenaConexionAzureStorage()
        {
            if (string.IsNullOrEmpty(cadenaConexionAzureStorage))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("cadenaConexionAzureStorage"))
                {
                    cadenaConexionAzureStorage = environmentVariables["cadenaConexionAzureStorage"] as string;
                }
                else
                {
                    cadenaConexionAzureStorage = Configuration.GetConnectionString("cadenaConexionAzureStorage");
                }
            }
            return cadenaConexionAzureStorage;
        }
    }
}
