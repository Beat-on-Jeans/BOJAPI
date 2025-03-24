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
    public class SoportesController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Soportes
        public IQueryable<Soporte> GetSoporte()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Soporte;
        }

        // GET: api/Soportes/5
        [ResponseType(typeof(Soporte))]
        public async Task<IHttpActionResult> GetSoporte(int id)
        {
            IHttpActionResult result;
            Soporte _soporte = await db.Soporte.FindAsync(id);
            if (_soporte == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_soporte);
            }
            return result;
        }

        // PUT: api/Soportes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSoporte(int id, Soporte _soporte)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _soporte.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_soporte).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!SoporteExists(id))
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

        // POST: api/Soportes
        [ResponseType(typeof(Soporte))]
        public async Task<IHttpActionResult> PostSoporte(Soporte _soporte)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Soporte.Add(_soporte);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _soporte.ID }, _soporte);
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

        // DELETE: api/Soportes/5
        [ResponseType(typeof(Soporte))]
        public async Task<IHttpActionResult> DeleteSoporte(int id)
        {
            IHttpActionResult result;

            Soporte _soporte = await db.Soporte.FindAsync(id);
            if (_soporte == null)
            {
                result = NotFound();
            }
            else
            {
                db.Soporte.Remove(_soporte);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_soporte);
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

        private bool SoporteExists(int id)
        {
            return db.Soporte.Count(e => e.ID == id) > 0;
        }
    }
}