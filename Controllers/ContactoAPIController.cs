using Microsoft.AspNetCore.Mvc;
using ContactoApi.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContactoApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ContactoAPIController : ControllerBase
    {
        private readonly string _connectionString;

        public ContactoAPIController(IConfiguration configuration)
        {
           
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        [Route("GetListaContactos")]
        public List<ContactoAPI> GetListaContacto()
        {
            List<ContactoAPI> lc = new List<ContactoAPI>();

            string Query= "Select IdContacto,NombreCompleto,Telefono,Mail,Mensaje,FechaEnvioMensaje,Leido, FechaMensajeLeido from Contacto";

            SqlConnection sqlConn = new SqlConnection(_connectionString);
            sqlConn.Open();
            SqlCommand sqlCm = new SqlCommand(Query, sqlConn);
            SqlDataReader dr = sqlCm.ExecuteReader();

            while (dr.Read())
            {
                ContactoAPI contacto = new ContactoAPI();
                contacto.IdContacto = int.Parse(dr[0].ToString());
                contacto.NombreCompleto = dr[1].ToString();
                contacto.Telefono = dr[2].ToString();
                contacto.Mail = dr[3].ToString();
                contacto.Mensaje = dr[4].ToString();
                contacto.FechaEnvioMensaje = dr[5].ToString();
                var valorLeido = dr[6].ToString();
                contacto.Leido = bool.TryParse(valorLeido, out bool resultado) ? resultado : false;
                contacto.FechaMensajeLeido = dr[7].ToString();
                lc.Add(contacto);


            }
            sqlConn.Close();
            return lc; 
            
        }
        //
        [HttpGet]
        [Route("GetContacto/{IdContacto?}")]
        public IActionResult GetIdContacto(int? IdContacto = null)
        {
            // Validar si no se proporciona un ID
            if (!IdContacto.HasValue)
            {
                return BadRequest("No hay un IdContacto señalado como parámetro");
            }

            ContactoAPI contacto = null; // Cambiar de List<ContactoAPI> a un solo objeto ContactoAPI
            string Query = "SELECT IdContacto, NombreCompleto, Telefono, Mail, Mensaje, FechaEnvioMensaje, Leido, FechaMensajeLeido FROM Contacto WHERE IdContacto = @IdContacto";

            using (SqlConnection sqlConn = new SqlConnection(_connectionString))
            {
                sqlConn.Open();
                using (SqlCommand sqlCm = new SqlCommand(Query, sqlConn))
                {
                    sqlCm.Parameters.AddWithValue("@IdContacto", IdContacto.Value);

                    using (SqlDataReader dr = sqlCm.ExecuteReader())
                    {
                        // Leer el primer registro
                        if (dr.Read())
                        {
                            contacto = new ContactoAPI
                            {
                                IdContacto = int.Parse(dr[0].ToString()),
                                NombreCompleto = dr[1].ToString(),
                                Telefono = dr[2].ToString(),
                                Mail = dr[3].ToString(),
                                Mensaje = dr[4].ToString(),
                                FechaEnvioMensaje = dr[5].ToString(),
                                Leido = bool.TryParse(dr[6].ToString(), out bool resultado) && resultado,
                                FechaMensajeLeido = dr[7].ToString()
                            };
                        }
                    }
                }
            }

            // Retornar el contacto encontrado o un 404 si no se encontró
            if (contacto == null)
            {
                return NotFound("No se encontró un contacto con el Id proporcionado.");
            }

            return Ok(contacto); // Devuelve el contacto encontrado
        }

        [HttpPost]
        [Route("InsertContacto")]
        public IActionResult InsertContacto([FromBody] ContactoAPI nuevoContacto)
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
                    // Añadimos los parámetros sin los dos últimos campos, por lo antes mencionado.
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
        public IActionResult UpdateContacto(int IdContacto, [FromBody] ContactoAPI contactoActualizado)
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
