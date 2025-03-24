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
using System.Web.Http.Results;
using BOJAPI.Models;

namespace BOJAPI.Controllers
{
    public class ValoracionsController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Valoracions
        public IQueryable<Valoracion> GetValoracion()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Valoracion;
        }

        // GET: api/Valoracions/5
        [ResponseType(typeof(Valoracion))]
        public async Task<IHttpActionResult> GetValoracion(int id)
        {
            IHttpActionResult result;
            Valoracion _valoracion = await db.Valoracion.FindAsync(id);
            if (_valoracion == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_valoracion);
            }
            return result;
        }

        // PUT: api/Valoracions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutValoracion(int id, Valoracion _valoracion)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _valoracion.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_valoracion).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ValoracionExists(id))
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

        // POST: api/Valoracions
        [ResponseType(typeof(Valoracion))]
        public async Task<IHttpActionResult> PostValoracion(Valoracion _valoracion)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Valoracion.Add(_valoracion);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _valoracion.ID }, _valoracion);
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

        // DELETE: api/Valoracions/5
        [ResponseType(typeof(Valoracion))]
        public async Task<IHttpActionResult> DeleteValoracion(int id)
        {
            IHttpActionResult result;

            Valoracion _valoracion = await db.Valoracion.FindAsync(id);
            if (_valoracion == null)
            {
                result = NotFound();
            }
            else
            {
                db.Valoracion.Remove(_valoracion);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_valoracion);
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

        private bool ValoracionExists(int id)
        {
            return db.Valoracion.Count(e => e.ID == id) > 0;
        }
    }
}