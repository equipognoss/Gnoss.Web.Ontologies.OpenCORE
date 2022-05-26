# Gnoss.Web.Ontologies.OpenCORE

Aplicación Web que se encarga de almacenar y servir las ontologías de la plataforma. Esta aplicación NO debe ser accesible desde el exterior de la plataforma GNOSS, sólo debe estar disponible para que el resto de aplicaciones puedan hacer peticiones Web a ella.

Configuración estandar de esta aplicación en el archivo docker-compose.yml: 

```yml
archivo:
    image: ontologies
    env_file: .env
    ports:
     - ${puerto_archivo}:80
    environment:
     rutaOntologias: "/app/Ontologias/Archivos"
    volumes:
      - ./logs/ontologias:/app/logs
      - ./content/Ontologias:/app/Ontologias
```

Nota: Si se modifica el parámetro "rutaOntologias", se debe modificar también el volumen configurado hacia la ruta "app/Ontolgias". 

Se pueden consultar los posibles valores de configuración de cada parámetro aquí: https://github.com/equipognoss/Gnoss.Platform.Deploy
