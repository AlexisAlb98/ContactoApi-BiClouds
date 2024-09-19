using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ContactoApi.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace ContactoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly string secretkey;
        public AutenticacionController(IConfiguration config)
        {
            secretkey = config.GetSection("settings").GetSection("secretkey").ToString();// Declaramos una variable privada para almacenar la clave secreta que se utilizará para generar el token JWT.
        }

        [HttpPost]
        [Route("Validar")]
        public IActionResult Validar([FromBody] Usuario request)
        {
            if (request.user=="Admin123" && request.password == "123")//Vsalidamos que el usuario y la clave (que están hardcodeados) sean correctos
            {
                var keyBytes = Encoding.ASCII.GetBytes(secretkey);//Convertimos la llave secreta en un array de bytes usando codificacion ASCII
                var claims = new ClaimsIdentity(); //Creamos una instancia de ClaimsIdentity (esto representaria al usuario)

                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.user));// Añadimos la "claim" o reclamo con el identificador del usuario (nombre:user) para asociarlo al token JWT.

                var tokenDescriptor = new SecurityTokenDescriptor //configuramos los parametros del token
                {
                    Subject = claims, // Definimos la identidad asociada al token (el usuario).
                    Expires = DateTime.UtcNow.AddMinutes(5), //configuramos para que el token dure 5 minutos y que despues de eso expire
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)// "firma" o autoriza el token usando la secretkey, y luego con el algoritmo Hmacsha firma el token
                };

                var tokenHandler = new JwtSecurityTokenHandler(); //creamos la instancia del manejador de tokens JWT
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);//Creamos el token usando los parametros configurados en el "tokenDescriptor"

                string tokenCreado = tokenHandler.WriteToken(tokenConfig);//Convertimso el token en una cadena que sera enviada en la respuesta "en nuestro caso utilizando el swagger y postman"

                return StatusCode(StatusCodes.Status200OK, new { token = tokenCreado });//Devolvemos un status 200OK diciendo que salio todo bien y le mandamos el token

            }
            else
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });//en el caso de que no se hubiera validado el usuario y la clave, devolvemos un status 401Unauthorized con un toekn vacio
            }
        }


    }
}
