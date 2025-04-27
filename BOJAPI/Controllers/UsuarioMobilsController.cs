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
using BOJAPI.Clases;
using BOJAPI.Models;

namespace BOJAPI.Controllers
{
    public class UsuarioMobilsController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/UsuarioMobils
        public IQueryable<UsuarioMobil> GetUsuarioMobil()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.UsuarioMobil;
        }

        // GET: api/UsuarioMobils/5
        [ResponseType(typeof(UsuarioMobil))]
        public async Task<IHttpActionResult> GetUsuarioMobil(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            UsuarioMobil _usuarioMobil = await db.UsuarioMobil.FindAsync(id);
            if (_usuarioMobil == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_usuarioMobil);
            }
            return result;
        }

        // GET: api/UsuarioMobils/5/Descripcion
        [HttpGet]
        [Route("api/UsuarioMobils/{id}/Descripcion")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetDescripcionUsuario(int id)
        {
            try
            {
                // Buscar el usuario por ID
                var usuario = await db.UsuarioMobil
                    .Where(u => u.ID == id)
                    .Select(u => new { u.Descripcion })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound();
                }

                return Ok(usuario.Descripcion);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/UsuarioMobils/5/Descripcion
        [HttpGet]
        [Route("api/UsuarioMobils/{UserID}/Valoraciones")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> GetRattingUsuario(int UserID)
        {
            try
            {
                // Buscar el usuario por ID
                var usuario = await db.UsuarioMobil
                    .Where(u => u.ID == UserID)
                    .Select(u => new { u.ValoracionTotal })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return NotFound();
                }

                return Ok(usuario.ValoracionTotal);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpGet]
        [Route("api/UsuarioMobils/Notificaiones/{userID}")]
        [ResponseType(typeof(int))]
        public async Task<IHttpActionResult> GetnotificationsUsuario(int UserID)
        {
            IHttpActionResult result;
            try
            {
                // Buscar el usuario por ID
                var user = await db.UsuarioMobil
                    .Where(u => u.ID == UserID)
                    .FirstOrDefaultAsync();

                if (user.Notificacion_ID == null)
                {
                    result = NotFound();
                }
                else
                {
                    result = Ok(user.Notificacion_ID);
                    user.Notificacion_ID = null;
                    db.Entry(user).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }
            return result;
        }

        // PUT: api/UsuarioMobils/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUsuarioMobil(int id, UsuarioMobil _usuarioMobil)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _usuarioMobil.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_usuarioMobil).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UsuarioMobilExists(id))
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


        [HttpPut]
        [Route("api/UsuarioMobils/NotificaionesNULL/{userID}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNotificaionesUsuario(int id, [FromBody] string descripcion)
        {
            if (string.IsNullOrEmpty(descripcion))
            {
                descripcion = ("");
            }

            try
            {
                var usuario = await db.UsuarioMobil.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.Notificacion_ID = null;
                await db.SaveChangesAsync();

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }




        // POST: api/UsuarioMobils
        [ResponseType(typeof(UsuarioMobil))]
        public async Task<IHttpActionResult> PostUsuarioMobil(UsuarioMobil _usuarioMobil)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.UsuarioMobil.Add(_usuarioMobil);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _usuarioMobil.ID }, _usuarioMobil);
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

        // PUT: api/UsuarioMobils/5/Descripcion
        [HttpPut]
        [Route("api/UsuarioMobils/{id}/Descripcion")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutDescripcionUsuario(int id, [FromBody] string descripcion)
        {
            if (string.IsNullOrEmpty(descripcion))
            {
                descripcion = ("");
            }

            try
            {
                var usuario = await db.UsuarioMobil.FindAsync(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                usuario.Descripcion = descripcion;
                await db.SaveChangesAsync();

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // DELETE: api/UsuarioMobils/5
        [ResponseType(typeof(UsuarioMobil))]
        public async Task<IHttpActionResult> DeleteUsuarioMobil(int id)
        {
            IHttpActionResult result;

            UsuarioMobil _usuarioMobil = await db.UsuarioMobil.FindAsync(id);
            if (_usuarioMobil == null)
            {
                result = NotFound();
            }
            else
            {
                db.UsuarioMobil.Remove(_usuarioMobil);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_usuarioMobil);
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

        private bool UsuarioMobilExists(int id)
        {
            return db.UsuarioMobil.Count(e => e.ID == id) > 0;
        }
    }
}