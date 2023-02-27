#Docker [Previamente Instalado]

Info: Se crea imagen de bases de datos mediante docket para ejecuión local

0. Descargar la imagen en el siguiente enlace "https://we.tl/t-Z81XKIpix9" [Expira en 7 días apartir de la fecha contactar en caso de ser necesario]
1. Abrir la consola de comando en el directorio donde se descargo la imagen "test-doublev-partners.tar"
2. Ejecutar el comando "docker load < test-doublev-partners.tar"
3. Ejecutar el comando "docker run --name test-doublev-partners -e POSTGRES_USER=DoubleV -e POSTGRES_PASSWORD=password -e POSTGRES_DB=PruebaDB -p 5432:5432 -d test-doublev-partners"


#VisualStudio 2022 [Previamente instlaado]

1. Clonar el repositorio "https://github.com/Leandro-GarciaG/APIDoubleV.git"
2. Abrir Visual Studio en la dirección del proyecto.
3. Abrir la consola de comandos dentro de la solución
4. Ejecutar el comando "dotnet build" (Descarga los paquetes necesarios para la solución)
5. Ejecutar el comando "dotnet ef database update" (Inicializa la base de datos)
6. Ejecutar el comando "dotnet test" (Ejecuta las pruebas unitarias)
7. Ejecutar la solución

#Información adicional

Se desarolla mediante patrones de diseño y principios SOLID

#Requerimientos
-Git
-Visual Studio
-Docket
