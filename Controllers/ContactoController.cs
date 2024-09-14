using Microsoft.AspNetCore.Mvc;
using ContactoApi.Models;
using System.Data.SqlClient;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContactoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactoController : ControllerBase
    {
        string connectionString = "Server=BDTesting.mssql.somee.com; Initial Catalog=BDTesting; User ID=matiaschamarez_SQLLogin_1; Password=xqqx9qyuey; Persist Security Info=False; TrustServerCertificate=True;";


        [HttpGet]
        [Route("GetContactos")]
        public List<Contacto> GetContactos()
        {
            List<Contacto> lc = new List<Contacto>();

            string Query= "Select IdContacto,NombreCompleto,Telefono,Mail,Mensaje,FechaEnvioMensaje,Leido from Contacto\r\n";

            SqlConnection sqlConn = new SqlConnection(connectionString);
            sqlConn.Open();
            SqlCommand sqlCm = new SqlCommand(Query, sqlConn);
            SqlDataReader dr = sqlCm.ExecuteReader();

            while (dr.Read())
            {
                Contacto contacto = new Contacto();
                contacto.IdContacto = int.Parse(dr[0].ToString());
                contacto.NombreCompleto = dr[1].ToString();
                contacto.Telefono = dr[2].ToString();
                contacto.Mail = dr[3].ToString();
                contacto.Mensaje = dr[4].ToString();
                contacto.FechaEnvioMensaje = dr[5].ToString();
                var valorLeido = dr[6].ToString();
                contacto.Leido = bool.TryParse(valorLeido, out bool resultado) ? resultado : false;
                lc.Add(contacto);
            }

            sqlConn.Close();

            return lc; 
            
        }
   
    }
}
