using DSW_3P_PR01.Repositories;
using DSW_3P_PR01.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
// Una sola instancia coordina las operaciones sobre el archivo JSON.
builder.Services.AddSingleton<IMisionRepository, MisionJsonRepository>();
builder.Services.AddScoped<IMisionService, MisionService>();
var app = builder.Build();
app.UseExceptionHandler();
app.MapControllers();
app.Run();
