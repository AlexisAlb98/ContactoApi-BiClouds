using Microsoft.AspNetCore.Mvc;
using ContactoApi.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("GetListaContactos")]
        public IActionResult GetListaContacto()
        {
            var lc = new List<ContactoAPI>();
            string query = "SELECT IdContacto, NombreCompleto, Telefono, Mail, Mensaje, FechaEnvioMensaje, Leido, FechaMensajeLeido FROM Contacto";

            try
            {
                using (var sqlConn = new SqlConnection(_connectionString))
                {
                    sqlConn.Open();
                    using (var sqlCm = new SqlCommand(query, sqlConn))
                    using (var dr = sqlCm.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var contacto = new ContactoAPI
                            {
                                IdContacto = int.Parse(dr["IdContacto"].ToString()),
                                NombreCompleto = dr["NombreCompleto"].ToString(),
                                Telefono = dr["Telefono"].ToString(),
                                Mail = dr["Mail"].ToString(),
                                Mensaje = dr["Mensaje"].ToString(),
                                FechaEnvioMensaje = dr["FechaEnvioMensaje"].ToString(),
                                Leido = bool.TryParse(dr["Leido"].ToString(), out bool resultado) && resultado,
                                FechaMensajeLeido = dr["FechaMensajeLeido"].ToString()
                            };
                            lc.Add(contacto);
                        }
                    }
                }
                return Ok(lc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la lista de contactos.", details = ex.Message });
            }

        }

        [HttpGet("GetContacto/{IdContacto}")]
        public IActionResult GetIdContacto(int IdContacto)
        {
            ContactoAPI contacto = null;
            string query = "SELECT IdContacto, NombreCompleto, Telefono, Mail, Mensaje, FechaEnvioMensaje, Leido, FechaMensajeLeido FROM Contacto WHERE IdContacto = @IdContacto";

            try
            {
                using (var sqlConn = new SqlConnection(_connectionString))
                {
                    sqlConn.Open();
                    using (var sqlCm = new SqlCommand(query, sqlConn))
                    {
                        sqlCm.Parameters.AddWithValue("@IdContacto", IdContacto);

                        using (var dr = sqlCm.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                contacto = new ContactoAPI
                                {
                                    IdContacto = int.Parse(dr["IdContacto"].ToString()),
                                    NombreCompleto = dr["NombreCompleto"].ToString(),
                                    Telefono = dr["Telefono"].ToString(),
                                    Mail = dr["Mail"].ToString(),
                                    Mensaje = dr["Mensaje"].ToString(),
                                    FechaEnvioMensaje = dr["FechaEnvioMensaje"].ToString(),
                                    Leido = bool.TryParse(dr["Leido"].ToString(), out bool resultado) && resultado,
                                    FechaMensajeLeido = dr["FechaMensajeLeido"].ToString()
                                };
                            }
                        }
                    }
                }

                if (contacto == null)
                {
                    return NotFound(new { message = "No se encontró un contacto con el Id proporcionado." });
                }

                return Ok(contacto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el contacto.", details = ex.Message });
            }
        }

        [HttpPost("InsertContacto")]
        public IActionResult InsertContacto([FromBody] ContactoAPI nuevoContacto)
        {
            if (nuevoContacto == null)
                return BadRequest(new { message = "Los datos del contacto no pueden estar vacíos." });

            string query = @"INSERT INTO Contacto (NombreCompleto, Telefono, Mail, Mensaje)
                             OUTPUT INSERTED.IdContacto
                             VALUES (@NombreCompleto, @Telefono, @Mail, @Mensaje)";

            try
            {
                using (var sqlConn = new SqlConnection(_connectionString))
                {
                    sqlConn.Open();
                    using (var sqlCm = new SqlCommand(query, sqlConn))
                    {
                        sqlCm.Parameters.AddWithValue("@NombreCompleto", nuevoContacto.NombreCompleto);
                        sqlCm.Parameters.AddWithValue("@Telefono", nuevoContacto.Telefono);
                        sqlCm.Parameters.AddWithValue("@Mail", nuevoContacto.Mail);
                        sqlCm.Parameters.AddWithValue("@Mensaje", nuevoContacto.Mensaje);

                        int newId = (int)sqlCm.ExecuteScalar();
                        return CreatedAtAction(nameof(GetIdContacto), new { IdContacto = newId }, new { message = "Contacto insertado correctamente", IdContacto = newId });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al insertar el contacto.", details = ex.Message });
            }
        }

        [HttpDelete("DeleteContacto/{IdContacto}")]
        public IActionResult DeleteContacto(int IdContacto)
        {
            string query = "DELETE FROM Contacto WHERE IdContacto = @IdContacto";

            try
            {
                using (var sqlConn = new SqlConnection(_connectionString))
                {
                    sqlConn.Open();
                    using (var sqlCm = new SqlCommand(query, sqlConn))
                    {
                        sqlCm.Parameters.AddWithValue("@IdContacto", IdContacto);
                        int rowsAffected = sqlCm.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Contacto eliminado correctamente" });
                        }
                        else
                        {
                            return NotFound(new { message = "Contacto no encontrado" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el contacto.", details = ex.Message });
            }
        }

        [HttpPut("UpdateContacto/{IdContacto}")]
        public IActionResult UpdateContacto(int IdContacto, [FromBody] ContactoAPI contactoActualizado)
        {
            string query = @"
                UPDATE Contacto 
                SET NombreCompleto = @NombreCompleto, 
                    Telefono = @Telefono, 
                    Mail = @Mail, 
                    Mensaje = @Mensaje,
                    Leido = @Leido
                WHERE IdContacto = @IdContacto";

            try
            {
                using (var sqlConn = new SqlConnection(_connectionString))
                {
                    sqlConn.Open();
                    using (var sqlCm = new SqlCommand(query, sqlConn))
                    {
                        sqlCm.Parameters.AddWithValue("@IdContacto", IdContacto);
                        sqlCm.Parameters.AddWithValue("@NombreCompleto", contactoActualizado.NombreCompleto);
                        sqlCm.Parameters.AddWithValue("@Telefono", contactoActualizado.Telefono);
                        sqlCm.Parameters.AddWithValue("@Mail", contactoActualizado.Mail);
                        sqlCm.Parameters.AddWithValue("@Mensaje", contactoActualizado.Mensaje);
                        sqlCm.Parameters.AddWithValue("@Leido", contactoActualizado.Leido);

                        int rowsAffected = sqlCm.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { message = "Contacto actualizado correctamente" });
                        }
                        else
                        {
                            return NotFound(new { message = "Contacto no encontrado" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el contacto.", details = ex.Message });
            }
        }
    }
}
