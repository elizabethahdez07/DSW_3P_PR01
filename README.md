# DSW_3P_PR01 — QuestLog

API REST de misiones, Etapa 1, con persistencia en JSON. Requiere SDK .NET 10.
Sin SQL Server ni Entity Framework. Swagger/OpenAPI se sirve con Swashbuckle.

## Ejecutar

Desde esta carpeta:

```powershell
dotnet build
dotnet run --launch-profile http
```

La API estará en http://localhost:5080/api/misiones. Abra el archivo
DSW_3P_PR01.http en Visual Studio o un cliente compatible para ejecutar las
peticiones en orden. También puede usar Postman. No se incluye Swagger.

## Organización

Se inspeccionó DSW_2P_EJ02(1).zip. Se conserva su proyecto Web API net10.0
y sus carpetas Controllers, Models, Services, Repositories, Data y Properties,
además de Program.cs, appsettings y el archivo HTTP. Se omiten bin/obj y
desarrolladoras.js, que no son necesarios para QuestLog. El paquete OpenAPI
del ejemplo no se conserva porque no se utilizaba.

El flujo es Controller → IMisionService/MisionService →
IMisionRepository/MisionJsonRepository → Data/misiones.json.
El controlador convierte resultados en HTTP; el servicio aplica reglas de
negocio; el repositorio lee y escribe el archivo. Las interfaces permiten
sustituir la persistencia sin cambiar controlador ni servicio.

## Contrato y reglas

- GET /api/misiones: 200 y lista (vacía si no hay registros).
- GET /api/misiones/{id}: 200 o 404.
- POST /api/misiones: 201, cuerpo creado y cabecera Location.
- PUT /api/misiones/{id}: 204 o 404; reemplaza los campos de la misión.
- DELETE /api/misiones/{id}: 204 o 404.
- Datos inválidos: 400 con ProblemDetails. Un JSON mal formado o tipos
  incompatibles también producen 400 mediante ApiController.
- Errores inesperados de almacenamiento: 500 con ProblemDetails; el detalle
  se registra en el servidor.

Nombre obligatorio (no solo espacios); dificultad exactamente Fácil, Media
o Difícil; XP mayor o igual que cero. POST asigna Id automáticamente e impone
Completada=false. PUT permite marcar Completada=true y usa el Id de la ruta,
ignorando el Id del cuerpo. PUT sobre una misión inexistente devuelve 404
antes de validar sus campos. No se crea una misión mediante PUT.

El cuerpo usa nombre, dificultad, recompensaXP y completada. ASP.NET Core
acepta también los nombres PascalCase. Los campos omitidos toman los valores
por defecto del modelo; nombre y dificultad vacíos se rechazan.

## Persistencia y pruebas manuales

Se incluyen tres misiones. Los cambios quedan en Data/misiones.json dentro
de la raíz del proyecto. Al publicar, el archivo se copia a la publicación.
La carpeta Data debe permitir escritura. Si falta el archivo se crea vacío;
si está corrupto se informa un error sin reemplazarlo por una lista vacía.
Se serializan las operaciones con un bloqueo y se escribe primero un temporal.
Ejecute una sola instancia de la API por archivo (no hay bloqueo entre procesos).
Los Id se calculan como máximo actual + 1; pueden reutilizarse si se elimina
el último registro. No edite el JSON mientras la API está ejecutándose.

1. GET de la lista: comprobar los tres ejemplos.
2. POST válido con completada=true: comprobar 201, Location y completada=false.
3. GET del Id creado: comprobar los datos.
4. PUT de ese Id con completada=true: 204; GET confirma el cambio.
5. Reiniciar la API: comprobar que el cambio permanece.
6. DELETE del Id creado: 204; GET/PUT/DELETE posteriores: 404.
7. Probar por separado nombre vacío, dificultad Extrema y XP=-1: 400.
8. Probar XP=0: aceptado. Enviar JSON roto: 400.

## Git

El ZIP puede incluir la carpeta .git con el commit local de entrega. Para ver:

```powershell
git log -1 --oneline
git status
```

Si su descompresor omite archivos ocultos o desea iniciar otro repositorio:

```powershell
git init
git add .
git commit -m "feat: implementar QuestLog etapa 1 con persistencia JSON"
```

No se proporcionó un texto obligatorio de commit en la conversación disponible;
se utiliza el mensaje anterior. No se publicó ningún repositorio remoto.

