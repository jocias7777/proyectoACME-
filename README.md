Instrucciones para correr el proyecto localmente

    Requisitos previos

     - Visual Studio 2019 o superior (Community, Professional o Enterprise)
     - .NET Framework 4.7.2 o superior
     - SQL Server 2014 o superior (puede ser SQL Server Express o LocalDB)
     - IIS Express (viene incluido con Visual Studio)

    1. Clonar el repositorio

     1 git clone https://github.com/jocias7777/proyectoACME-.git
     2 cd proyectoACME-

    2. Abrir el proyecto en Visual Studio

     - Abrir Visual Studio
     - Ir a Archivo > Abrir > Proyecto o solución
     - Seleccionar el archivo proyectoACME.sln

    3. Instalar dependencias (NuGet)

    El proyecto usa NuGet Package Manager. Al abrir la solución, Visual Studio debería restaurar automáticamente los
    paquetes. Si no, ejecutar desde la Package Manager Console:

     1 Update-Package -Reinstall

    Paquetes principales incluidos:
     - Microsoft.AspNet.Mvc 5.2.9
     - Microsoft.AspNet.Web.Optimization 1.1.3
     - BCrypt.Net-Next 4.1.0
     - bootstrap 3.4.1
     - jQuery 3.7.1
     - Microsoft.CodeDom.Providers.DotNetCompilerPlatform 2.0.1

    4. Configurar la base de datos

     1. Abrir SQL Server Management Studio (SSMS) o Azure Data Studio
     2. Conectarse a tu instancia de SQL Server
     3. Crear la base de datos:

     1 CREATE DATABASE AcmeSurveys;

     4. Ejecutar el script de creación de tablas


    5. Configurar la cadena de conexión

     1. Abrir el archivo Web.config en la raíz del proyecto
     2. Buscar la sección <connectionStrings>
     3. Modificar el valor de DefaultConnection con tu servidor:

     1 <connectionStrings>
     2     <add name="DefaultConnection"
     3          connectionString="Server=TU_SERVIDOR;Database=AcmeSurveys;Integrated Security=True;"
     4          providerName="System.Data.SqlClient" />
     5 </connectionStrings>

    Reemplazar TU_SERVIDOR por tu instancia de SQL Server (ejemplo: .\SQLEXPRESS o (localdb)\MSSQLLocalDB)

    6. Compilar el proyecto

     - En Visual Studio, ir a Compilar > Compilar solución (o presionar Ctrl+Shift+B)
     - Asegurarse de que la compilación sea exitosa sin errores

    7. Ejecutar el proyecto

     - Presionar F5 o ir a Depurar > Iniciar depuración
     - El proyecto se abrirá en el navegador con IIS Express
     - URL por defecto: http://localhost:xxxx/

    8. Usuario por defecto

    Si no hay usuarios creados, se puede registrar uno directamente en la base de datos:

     1 INSERT INTO dbo.Usuarios (Username, Password, FechaCreacion)
     2 VALUES ('admin', HASHBYTES('SHA2_256', 'tu_password'), GETDATE());

    O usar el BCrypt desde código para generar el hash de la contraseña
