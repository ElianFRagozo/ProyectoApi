using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using ProyectoApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Agrega servicios al contenedor.
builder.Services.AddControllers();
// Aprende más sobre cómo configurar Swagger/OpenAPI en https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agrega la configuración de MongoDB
builder.Services.AddSingleton<IMongoDatabaseSettings>(sp =>
    new ProyectoApi.Services.MongoDatabaseSettings
    {
        ConnectionString = "mongodb+srv://eenriquefragozo:PRaCUGb0aXeffjYF@cluster0.uawckuf.mongodb.net/?retryWrites=true&w=majority&appName=Cluster",
        DatabaseName = "Pacientes"
    });

builder.Services.AddSingleton<PatientService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<HorarioService>();

var app = builder.Build();

// Configura el pipeline de solicitudes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Agrega la autenticación
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
