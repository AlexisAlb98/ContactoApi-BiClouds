using ContactoApi.Models;
using Newtonsoft.Json; //libreria para convertir nuestras clases en objetos json
using System.Net.Http.Headers;
using System.Text;

namespace ContactoApi.Services
{
    public class Services_API
    {
        //En esta carpeta va a estar toda la logica (codigo) de cada método declarado en la interface Services_API

        private static string _usuario;
        private static string _clave;
        private static string _baseUrl;
        private static string token;

        public Services_API()
        {
            //define un builder donde va a estar guardado el directorio donde se encuentra la configuracion de nuestra API (appsettings.json)
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            //define las variables que vamos a usar para autenticar, en este caso
            _usuario = builder.GetSection("ApiSettings:usuario").Value;
            _clave = builder.GetSection("ApiSettings:clave").Value;
            _baseUrl = builder.GetSection("ApiSettings:baseUrl").Value;




        }

        public async Task Autenticacion()
        {
            //definimos un nuevo cliente
            var cliente = new HttpClient();

            //definimos la Uri donde esta nuestro proyecto, todo esto traido desde el appsettings
            cliente.BaseAddress = new Uri(_baseUrl);

            //definimos una nueva credencial que esta ligada con nuestro modeo Credencial. Aca definimos el usuario y la clave (tambien traidos del appsettings)
            var credenciales = new Credencial() { usuario = _usuario, clave = _clave };

            //creamos el contenido para pasarle a la API de autenticacion
            var content = new StringContent(JsonConvert.SerializeObject(credenciales),Encoding.UTF8, "application/json");

            //ejecutar la URL de autenticacion/validar
            var response = await cliente.PostAync("api/Autenticacion/Validar", content);//le pasamos el contenido donde estan nuestras credenciales 



        }




    }
}
