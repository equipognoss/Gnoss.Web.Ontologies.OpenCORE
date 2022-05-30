![](https://content.gnoss.ws/imagenes/proyectos/personalizacion/7e72bf14-28b9-4beb-82f8-e32a3b49d9d3/cms/logognossazulprincipal.png)

# Gnoss.Web.Ontologies.OpenCORE

Aplicación Web que se encarga de almacenar y servir las ontologías de la plataforma. Esta aplicación NO debe ser accesible desde el exterior de la plataforma GNOSS, sólo debe estar disponible para que el resto de aplicaciones puedan hacer peticiones Web a ella.

Configuración estandar de esta aplicación en el archivo docker-compose.yml: 

```yml
ontologies:
    image: gnoss/ontologies
    env_file: .env
    ports:
     - ${puerto_ontologies}:80
    environment:
     rutaOntologias: "/app/Ontologies/Files"
    volumes:
      - ./logs/ontologies:/app/logs
      - ./content/Ontologies:/app/Ontologies
```

Nota: Si se modifica el parámetro "rutaOntologias", se debe modificar también el volumen configurado hacia la ruta "app/Ontolgies". 

Se pueden consultar los posibles valores de configuración de cada parámetro aquí: https://github.com/equipognoss/Gnoss.Platform.Deploy
