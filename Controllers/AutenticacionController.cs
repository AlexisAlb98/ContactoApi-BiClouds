using System.Text;
using ContactoApi.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace ContactoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly string secretkey;

        public AutenticacionController(IConfiguration config)
        {
            // Intentamos obtener la clave secreta del archivo de configuración
            try
            {
                secretkey = config.GetSection("settings").GetSection("secretkey").Value;
            }
            catch (Exception ex)
            {
                // Si ocurre un error al obtener la clave, lanzamos una excepción informativa
                throw new ApplicationException("Error al obtener la clave secreta de configuración.", ex);
            }
        }

        [HttpPost]
        [Route("Validar")]
        public IActionResult Validar([FromBody] UsuarioAPI request)
        {
            try
            {
                // Validamos el usuario y la contraseña (hardcodeado)
                if (request.user == "Admin123" && request.password == "123")
                {
                    var keyBytes = Encoding.ASCII.GetBytes(secretkey);
                    var claims = new ClaimsIdentity();

                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.user));

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddMinutes(5),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
                    string tokenCreado = tokenHandler.WriteToken(tokenConfig);

                    return StatusCode(StatusCodes.Status200OK, new { token = tokenCreado });
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
                }
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Faltan parámetros requeridos: " + ex.Message);
            }
            catch (SecurityTokenException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al generar el token de seguridad: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado: " + ex.Message);
            }
        }
    }
}