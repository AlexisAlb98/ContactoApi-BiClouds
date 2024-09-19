using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json");
var secretkey = builder.Configuration.GetSection("settings").GetSection("secretkey").ToString();
var keyBytes = Encoding.UTF8.GetBytes(secretkey);

builder.Services.AddAuthentication(config =>  // A�adimos el servicio de autenticaci�n, configurandolo para usar JWT Bearer
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;// Establecimos el esquema de autenticaci�n por defecto como JWT Bearer
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(config => // Configuramos los parametros espec�ficos para el esquema JWT Bearer
{
    config.RequireHttpsMetadata = false;// aca deshabilitamos el requerimiento de HTTPS para tokens (mnas que nada porque estamos en un entorno de desarrollo)
    config.SaveToken = true;// Indicamos que el token validado debe ser guardado para, mas adelante, usarlo en nuestro codigo
    config.TokenValidationParameters = new TokenValidationParameters // Definimos los par�metros para la validaci�n del token
    {
        ValidateIssuerSigningKey = true,// Habilitamos la validaci�n de la firma del token
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes), // Utilizamos la llave secreta (convertida a bytes) para validar la firma del token
        ValidateIssuer = false,//no validamos el issuer 
        ValidateAudience = false//tampoco validamos la audiencia
    };
});


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// A�ade el servicio de configuraci�n de cadenas de conexi�n
builder.Services.AddControllers();

// Configura la cadena de conexi�n
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
