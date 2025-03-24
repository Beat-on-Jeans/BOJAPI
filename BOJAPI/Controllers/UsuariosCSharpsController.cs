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

namespace BOJAPI.Controllers
{
    public class UsuariosCSharpsController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/UsuariosCSharps
        public IQueryable<UsuariosCSharp> GetUsuariosCSharp()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.UsuariosCSharp;
        }

        // GET: api/UsuariosCSharps/5
        [ResponseType(typeof(UsuariosCSharp))]
        public async Task<IHttpActionResult> GetUsuariosCSharp(int id)
        {
            IHttpActionResult result;
            UsuariosCSharp _usuariosCSharp = await db.UsuariosCSharp.FindAsync(id);
            if (_usuariosCSharp == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_usuariosCSharp);
            }
            return result;
        }

        // PUT: api/UsuariosCSharps/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUsuariosCSharp(int id, UsuariosCSharp _usuariosCSharp)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _usuariosCSharp.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_usuariosCSharp).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UsuariosCSharpExists(id))
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

        // POST: api/UsuariosCSharps
        [ResponseType(typeof(UsuariosCSharp))]
        public async Task<IHttpActionResult> PostUsuariosCSharp(UsuariosCSharp _usuariosCSharp)
        {
            IHttpActionResult result;
            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.UsuariosCSharp.Add(_usuariosCSharp);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _usuariosCSharp.ID }, _usuariosCSharp);
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

        // DELETE: api/UsuariosCSharps/5
        [ResponseType(typeof(UsuariosCSharp))]
        public async Task<IHttpActionResult> DeleteUsuariosCSharp(int id)
        {
            IHttpActionResult result;

            UsuariosCSharp _usuariosCSharp = await db.UsuariosCSharp.FindAsync(id);
            if (_usuariosCSharp == null)
            {
                result = NotFound();
            }
            else
            {
                db.UsuariosCSharp.Remove(_usuariosCSharp);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_usuariosCSharp);
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

        private bool UsuariosCSharpExists(int id)
        {
            return db.UsuariosCSharp.Count(e => e.ID == id) > 0;
        }
    }
}