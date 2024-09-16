namespace ContactoApi.Models
{
    public class ResultadoApi
    {
        public string Mensaje { get; set; }
        public List<Contacto> Lista { get; set; }
        public Contacto Objeto { get; set; }
    }
}
