using ContactoApi.Models;
using Newtonsoft.Json; //libreria para convertir nuestras clases en objetos json
using System.Net.Http.Headers;
using System.Text;

namespace ContactoApi.Services
{
    public class Services_API : IServices_API
    {
        //En esta carpeta va a estar toda la logica (codigo) de cada método declarado en la interface Services_API

        private static string _usuario;
        private static string _clave;
        private static string _baseUrl;
        private static string _token;

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
            var response = await cliente.PostAsync("api/Autenticacion/Validar", content);//le pasamos el contenido donde estan nuestras credenciales 
            //define el json de respuesta (que es lo que necesita nuestra api, ya que se va a consumir
            var json_respuesta = await response.Content.ReadAsStringAsync();

            //el resultado de nuestra respuesta lo convierte a un archivo Json
            var resultado = JsonConvert.DeserializeObject<ResultadoCredencial>(json_respuesta);

            //guardamos el token de la variable resultado en la variable _token
            _token = resultado.token;

        }

        //implementacion de los metodos creados en IServices (interface)

        public async Task<List<Contacto>> GetListaContactos()
        {
            List<Contacto> lista = new List<Contacto>();

            await Autenticacion();

            var cliente = new HttpClient();
            cliente.BaseAddress=new Uri(_baseUrl);
            cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Barer", _token);
            var response = await cliente.GetAsync("api/Contacto/GetListaContactos");

            if (response.IsSuccessStatusCode)
            {
                var json_respuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<ResultadoApi>(json_respuesta);
                lista = resultado.Lista;
            }

            return lista;
        }






        public Task<bool> DeleteContacto(int IdContacto)
        {
            throw new NotImplementedException();
        }

        public Task<Contacto> GetIdContacto(int IdContacto)
        {
            throw new NotImplementedException();
        }


        public Task<bool> InsertContacto(Contacto objeto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateContacto(Contacto objeto)
        {
            throw new NotImplementedException();
        }
    }
}
