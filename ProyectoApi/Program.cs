using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using ProyectoApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Agrega servicios al contenedor.
builder.Services.AddControllers();
// Aprende m�s sobre c�mo configurar Swagger/OpenAPI en https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agrega la configuraci�n de MongoDB
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

// Agrega la autenticaci�n
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
