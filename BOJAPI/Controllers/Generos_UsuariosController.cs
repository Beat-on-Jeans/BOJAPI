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