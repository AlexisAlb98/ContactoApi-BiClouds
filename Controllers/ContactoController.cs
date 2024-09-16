using Microsoft.AspNetCore.Mvc;
using ContactoApi.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContactoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactoController : ControllerBase
    {
        private readonly string _connectionString;

        public ContactoController(IConfiguration configuration)
        {
            // Aquí obtienes la cadena de conexión del archivo appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        [Route("GetListaContactos")]
        public List<Contacto> GetListaContacto()
        {
            List<Contacto> lc = new List<Contacto>();

            string Query= "Select IdContacto,NombreCompleto,Telefono,Mail,Mensaje,FechaEnvioMensaje,Leido from Contacto\r\n";

            SqlConnection sqlConn = new SqlConnection(_connectionString);
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

        [HttpGet]
        [Route("GetContacto/{IdContacto?}")] // Filtramos por IdContacto
        public List<Contacto> GetIdContacto(int? IdContacto = null)
        {
            List<Contacto> lc = new List<Contacto>();

            // Consulta SQL modificada para filtrar el IdContacto
            string Query = "Select IdContacto, NombreCompleto, Telefono, Mail, Mensaje, FechaEnvioMensaje, Leido from Contacto";

            if (IdContacto.HasValue)
            {
                Query += " WHERE IdContacto = @IdContacto"; // Filtra por IdContacto
            }

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCm = new SqlCommand(Query, sqlConn))
                {
                    if (IdContacto.HasValue)
                    {
                        // Agrega el parámetro a la consulta SQL
                        sqlCm.Parameters.AddWithValue("@IdContacto", IdContacto.Value);
                    }

                    using (SqlDataReader dr = sqlCm.ExecuteReader())
                    {
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
                    }
                }
            }

            return lc;
        }

        [HttpPost]
        [Route("InsertContacto")]
        public IActionResult InsertContacto([FromBody] Contacto nuevoContacto)
        {
            // Definimos la consulta de inserción sin FechaEnvioMensaje y Leido dado que el primero se autocompleta por medio de un Trigger y el segundo es 0 por default.
            string Query = @"
                            INSERT INTO Contacto (NombreCompleto, Telefono, Mail, Mensaje)
                            VALUES (@NombreCompleto, @Telefono, @Mail, @Mensaje)";

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCm = new SqlCommand(Query, sqlConn))
                {
                    // Añadimos los parámetros sin los dos últimos campos, por lo antes mensionado.
                    sqlCm.Parameters.AddWithValue("@NombreCompleto", nuevoContacto.NombreCompleto);
                    sqlCm.Parameters.AddWithValue("@Telefono", nuevoContacto.Telefono);
                    sqlCm.Parameters.AddWithValue("@Mail", nuevoContacto.Mail);
                    sqlCm.Parameters.AddWithValue("@Mensaje", nuevoContacto.Mensaje);

                    // Ejecutamos la consulta.
                    int rowsAffected = sqlCm.ExecuteNonQuery();

                    // Verificamos si la inserción fue genero de manera exitosa.
                    if (rowsAffected > 0)
                    {
                        return Ok("Contacto insertado correctamente");
                    }
                    else
                    {
                        return BadRequest("Error al insertar el contacto");
                    }
                }
            }
        }

        [HttpDelete]
        [Route("DeleteContacto/{IdContacto}")]
        public IActionResult DeleteContacto(int IdContacto)
        {
            // Definimos la consulta de eliminación por IdContacto
            string Query = "DELETE FROM Contacto WHERE IdContacto = @IdContacto";

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCm = new SqlCommand(Query, sqlConn))
                {
                    // Agregamos el parámetro @IdContacto a la consulta SQL
                    sqlCm.Parameters.AddWithValue("@IdContacto", IdContacto);

                    // Ejecutamos la consulta de eliminación
                    int rowsAffected = sqlCm.ExecuteNonQuery();

                    // Verificamos si el contacto fue eliminado correctamente
                    if (rowsAffected > 0)
                    {
                        return Ok("Contacto eliminado correctamente");
                    }
                    else
                    {
                        return NotFound("Contacto no encontrado");
                    }
                }
            }
        }

        [HttpPut]
        [Route("UpdateContacto/{IdContacto}")]
        public IActionResult UpdateContacto(int IdContacto, [FromBody] Contacto contactoActualizado)
        {
            // Definimos la consulta de actualización, incluyendo el campo 'Leido'
            string Query = @"
                    UPDATE Contacto 
                    SET NombreCompleto = @NombreCompleto, 
                        Telefono = @Telefono, 
                        Mail = @Mail, 
                        Mensaje = @Mensaje,
                        Leido = @Leido
                    WHERE IdContacto = @IdContacto";

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCm = new SqlCommand(Query, sqlConn))
                {
                    // Agregamos los parámetros con los valores actualizados, incluido 'Leido'
                    sqlCm.Parameters.AddWithValue("@IdContacto", IdContacto);
                    sqlCm.Parameters.AddWithValue("@NombreCompleto", contactoActualizado.NombreCompleto);
                    sqlCm.Parameters.AddWithValue("@Telefono", contactoActualizado.Telefono);
                    sqlCm.Parameters.AddWithValue("@Mail", contactoActualizado.Mail);
                    sqlCm.Parameters.AddWithValue("@Mensaje", contactoActualizado.Mensaje);
                    sqlCm.Parameters.AddWithValue("@Leido", contactoActualizado.Leido);

                    

                    // Ejecutamos la consulta de actualización
                    int rowsAffected = sqlCm.ExecuteNonQuery();

                    // Verificamos si la actualización fue exitosa
                    if (rowsAffected > 0)
                    {
                        return Ok("Contacto actualizado correctamente");
                    }
                    else
                    {
                        return NotFound("Contacto no encontrado");
                    }
                }
            }
        }




    }
}
