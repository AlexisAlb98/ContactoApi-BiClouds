using System.ComponentModel.DataAnnotations;

namespace ContactoApi.Models
{
    public class Contacto
    {
        public int IdContacto { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        public string NombreCompleto { get; set; }

        [Phone(ErrorMessage = "El número de teléfono no es válido.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "El mensaje es obligatorio.")]
        public string Mensaje { get; set; }

        public string FechaEnvioMensaje { get; set; }

        public bool Leido {  get; set; }

    }
}
