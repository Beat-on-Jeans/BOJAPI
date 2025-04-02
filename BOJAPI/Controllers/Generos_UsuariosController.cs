using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BOJAPI.Models;
using BOJAPI.Clases;
using Microsoft.Ajax.Utilities;

namespace BOJAPI.Controllers
{
    public class Generos_UsuariosController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Generos_Usuarios
        public IQueryable<Generos_Usuarios> GetGeneros_Usuarios()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Generos_Usuarios;
        }

        // GET: api/Generos_Usuarios/5
        [ResponseType(typeof(Generos_Usuarios))]
        public async Task<IHttpActionResult> GetGeneros_Usuarios(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            Generos_Usuarios _generos_Usuarios = await db.Generos_Usuarios.FindAsync(id);

            if (_generos_Usuarios == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_generos_Usuarios);
            }
            return result;
        }


        [HttpPut]
        [Route("api/GenerosUsuarios/ActualizarGeneros/{usuarioId}")]

        public async Task<IHttpActionResult> ActualizarGenerosUsuario(int usuarioId, [FromBody] List<int> generosIds)
        {
            // Validación básica
            if (generosIds == null)
            {
                return BadRequest("La lista de géneros no puede ser nula");
            }

            try
            {
                // Verificar si el usuario existe
                var usuarioExiste = await db.Usuarios.AnyAsync(u => u.ID == usuarioId);
                if (!usuarioExiste)
                {
                    return NotFound();
                }

                // Obtener géneros actuales del usuario
                var generosActuales = await db.Generos_Usuarios
                    .Where(gu => gu.Usuario_Id == usuarioId)
                    .ToListAsync();

                // Convertir a lista de IDs para comparación
                var generosActualesIds = generosActuales.Select(gu => gu.Genero_Id).ToList();

                // Identificar cambios necesarios
                var generosAEliminar = generosActuales
                    .Where(gu => gu.Genero_Id.HasValue && !generosIds.Contains(gu.Genero_Id.Value))  // Corregido Genero_id a Genero_Id
                    .ToList();

                var generosAAñadir = generosIds
                    .Where(id => !generosActualesIds.Contains(id))  // Corregido Contair a Contains
                    .ToList();

                // Validar que los géneros a añadir existan
                var generosExistentes = await db.Generos_Musicales
                    .Where(g => generosAAñadir.Contains(g.ID))
                    .Select(g => g.ID)
                    .ToListAsync();

                var generosNoExistentes = generosAAñadir.Except(generosExistentes).ToList();
                if (generosNoExistentes.Any())
                {
                    return BadRequest($"Los siguientes IDs de género no existen: {string.Join(", ", generosNoExistentes)}");
                }

                // Ejecutar transacción
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Eliminar relaciones que ya no aplican
                        db.Generos_Usuarios.RemoveRange(generosAEliminar);

                        // Crear nuevas relaciones
                        foreach (var generoId in generosAAñadir)
                        {
                            db.Generos_Usuarios.Add(new Generos_Usuarios
                            {
                                Usuario_Id = usuarioId,
                                Genero_Id = generoId,
                            });
                        }

                        await db.SaveChangesAsync();
                        transaction.Commit();

                        return Ok(new
                        {
                            Success = true,
                            Message = "Géneros actualizados correctamente"
                        });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return InternalServerError(new Exception("Error al actualizar géneros", ex));
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/GenerosUsuarios/ObtenerGeneros/{usuarioId}")]
        public async Task<IHttpActionResult> ObtenerGenerosUsuario(int usuarioId)
        {
            try
            {
                // Verificar si el usuario existe
                var usuario = await db.Usuarios.FindAsync(usuarioId);
                if (usuario == null)
                {
                    return NotFound();
                }

                // Obtener géneros musicales asociados al usuario
                var generos = await db.Generos_Usuarios
                    .Where(gu => gu.Usuario_Id == usuarioId)
                    .Join(db.Generos_Musicales,
                        gu => gu.Genero_Id,
                        g => g.ID,
                        (gu, g) => new
                        {
                            g.ID,
                            g.Nombre_Genero
                        })
                    .ToListAsync();

                return Ok(generos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }



        [HttpGet]
        [Route("api/MusicGenders/{userID}")]
        public async Task<IHttpActionResult> GetMusicGendersChats(int userID)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            try
            {
                // Proyectar solo los géneros necesarios usando un DTO
                var generos = await db.Generos_Usuarios
                            .Where(c => c.Usuario_Id == userID)
                            .Select(c => new GeneroDTO
                            {
                                Nombre_Genero = c.Generos_Musicales.Nombre_Genero
                            })
                            .ToListAsync();

                if (generos == null || !generos.Any())
                {
                    result = NotFound();
                }
                else
                {
                    result = Ok(generos); // Devuelve solo los géneros en formato DTO
                }
            }
            catch (Exception ex)
            {
                // Registro de la excepción para su seguimiento
                result = InternalServerError(ex);
            }
            return result;
        }


        // PUT: api/Generos_Usuarios/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGeneros_Usuarios(int id, Generos_Usuarios _generos_Usuarios)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _generos_Usuarios.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_generos_Usuarios).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!Generos_UsuariosExists(id))
                        {
                            result = NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    catch (DbUpdateException ex)
                    {
                        SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                        missatge = Clases.Utilities.MissatgeError(sqlException);
                        result = BadRequest(missatge);
                    }
                }
            }
            return result;
        }

        // POST: api/Generos_Usuarios
        [ResponseType(typeof(Generos_Usuarios))]
        public async Task<IHttpActionResult> PostGeneros_Usuarios(Generos_Usuarios _generos_Usuarios)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {

                db.Generos_Usuarios.Add(_generos_Usuarios);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _generos_Usuarios.ID }, _generos_Usuarios);
                }
                catch (DbUpdateException ex)
                {
                    SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                    missatge = Clases.Utilities.MissatgeError(sqlException);
                    result = BadRequest(missatge);
                }
            }
            return result;
        }

        // DELETE: api/Generos_Usuarios/5
        [ResponseType(typeof(Generos_Usuarios))]
        public async Task<IHttpActionResult> DeleteGeneros_Usuarios(int id)
        {
            IHttpActionResult result;
            Generos_Usuarios _generos_Usuarios = await db.Generos_Usuarios.FindAsync(id);
            if (_generos_Usuarios == null)
            {
                result = NotFound();
            }
            else
            {
                db.Generos_Usuarios.Remove(_generos_Usuarios);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_generos_Usuarios);
                }
                catch (DbUpdateException ex)
                {
                    SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                    missatge = Clases.Utilities.MissatgeError(sqlException);
                    result = BadRequest(missatge);
                }
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool Generos_UsuariosExists(int id)
        {
            return db.Generos_Usuarios.Count(e => e.ID == id) > 0;
        }
    }
}