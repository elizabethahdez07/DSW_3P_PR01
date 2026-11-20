using Microsoft.EntityFrameworkCore;
using DSW_3P_PR01.Data;
using DSW_3P_PR01.Repositories;
using DSW_3P_PR01.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi


//Agregar Controladores
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT obtenido en POST /api/Auth/login."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

//Agregar dependencias
builder.Services.AddScoped<IMisionRepository, MisionSQLRepository>();
builder.Services.AddScoped<IMisionService, MisionService>();
//Configuracion del SQL SERVER
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServerConnection")
    )
);

//Configuración del JWT
var jwt = builder.Configuration.GetSection("Jwt");
var key = Convert.FromBase64String(
   jwt["Key"] ?? throw new InvalidOperationException(
       "Falta configurar Jwt:Key"));
builder.Services
   .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters =
           new TokenValidationParameters
           {
               ValidateIssuer = true,
               ValidIssuer = jwt["Issuer"],
               ValidateAudience = true,
               ValidAudience = jwt["Audience"],
               ValidateLifetime = true,
               ClockSkew = TimeSpan.Zero,
               ValidateIssuerSigningKey = true,
               IssuerSigningKey =
                   new SymmetricSecurityKey(key)
           };
   });

//Configurar Política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("QuestLogFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5500",
                "http://127.0.0.1:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });

    //Mapear Swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "API de Misiones v1"
        );
    });
}
app.UseCors("QuestLogFrontend");
app.UseHttpsRedirection();


//Mapear Controllers
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

//agrega caracteristicas nuevas a la api, (nugget) donde hay plugins para funcionalidades para la api
//ORM (conector de c# con la base de datos, mapeo de los objetos del codigo con el de la base de datos)
//dotnet add package Microsoft.EntityFrameworkCore.SqlServer
//dotnet add package Microsoft.EntityFrameworkCore.Design
//dotnet tool install --global dotnet-ef --version 10.0.*
//ef (entityframework)

//dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version "10.0.*"
// dotnet user-secrets init

/*
$keyBytes = New-Object byte[] 32
>> $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
>> try {
>>    $rng.GetBytes($keyBytes)
>> }
>> finally {
>>    $rng.Dispose()
>> }
>> $key = [Convert]::ToBase64String($keyBytes)
*/
// dotnet user-secrets set "Jwt:Key" $key
