using ContactoApi.Models;
using System.Threading.Tasks;

namespace ContactoApi.Services
{
    public interface IServices_API
    {
        //en esta carpeta de servicios se va a implementar toda la logica de ejecución de nuestras APIS
        Task<List<Contacto>> GetContactos();
        Task<Contacto> GetContactoId(int IdContacto);
        Task<bool> InsertContacto(Contacto objeto);
        Task<bool> UpdateContacto(Contacto objeto);
        Task<bool> DeleteContacto(int IdContacto);
        //luego se van a declarar todos los metodos que se van a utilizar para nuestros servivios API      

    }
}
