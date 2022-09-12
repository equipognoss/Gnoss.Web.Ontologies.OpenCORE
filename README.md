![](https://content.gnoss.ws/imagenes/proyectos/personalizacion/7e72bf14-28b9-4beb-82f8-e32a3b49d9d3/cms/logognossazulprincipal.png)

# Gnoss.Web.Ontologies.OpenCORE

![](https://github.com/equipognoss/Gnoss.Web.Ontologies.OpenCORE/workflows/BuildOntologies/badge.svg)

Aplicación Web que se encarga de almacenar y servir las ontologías de la plataforma. Esta aplicación NO debe ser accesible desde el exterior de la plataforma GNOSS, sólo debe estar disponible para que el resto de aplicaciones puedan hacer peticiones Web a ella.

Configuración estandar de esta aplicación en el archivo docker-compose.yml: 

```yml
ontologies:
    image: gnoss/gnoss.web.ontologies.opencore
    env_file: .env
    ports:
     - ${puerto_ontologies}:80
    environment:
     rutaOntologias: "/app/Ontologies/Files"
     scopeIdentity: ${scopeIdentity}
     clientIDIdentity: ${clientIDIdentity}
     clientSecretIdentity: ${clientIDIdentity}
    volumes:
      - ./logs/ontologies:/app/logs
      - ./content/Ontologies:/app/Ontologies
```

En esta configuración, existe un volumen que apunta a la ruta /app/Ontologies del contenedor. Ese volumen almacenará todas las ontologías subidas a la plataforma. Se recomienda realizar copias de seguridad de la unidad en la que se mapee ese directorio. Si se modifica el parámetro "rutaOntologias", se debe modificar también el volumen configurado hacia la ruta "app/Ontolgies". 

Se pueden consultar los posibles valores de configuración de cada parámetro aquí: https://github.com/equipognoss/Gnoss.SemanticAIPlatform.OpenCORE

## Código de conducta
Este proyecto a adoptado el código de conducta definido por "Contributor Covenant" para definir el comportamiento esperado en las contribuciones a este proyecto. Para más información ver https://www.contributor-covenant.org/

## Licencia
Este producto es parte de la plataforma [Gnoss Semantic AI Platform Open Core](https://github.com/equipognoss/Gnoss.SemanticAIPlatform.OpenCORE), es un producto open source y está licenciado bajo GPLv3.
