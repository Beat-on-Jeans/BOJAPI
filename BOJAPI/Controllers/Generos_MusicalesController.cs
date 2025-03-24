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
    public class Generos_MusicalesController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Generos_Musicales
        public IQueryable<Generos_Musicales> GetGeneros_Musicales()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Generos_Musicales;
        }

        // GET: api/Generos_Musicales/5
        [ResponseType(typeof(Generos_Musicales))]
        public async Task<IHttpActionResult> GetGeneros_Musicales(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            Generos_Musicales _generos_Musicales = await db.Generos_Musicales.FindAsync(id);
            if (_generos_Musicales == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_generos_Musicales);
            }
            return result;
        }

        // PUT: api/Generos_Musicales/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutGeneros_Musicales(int id, Generos_Musicales _generos_Musicales)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _generos_Musicales.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_generos_Musicales).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!Generos_MusicalesExists(id))
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

        // POST: api/Generos_Musicales
        [ResponseType(typeof(Generos_Musicales))]
        public async Task<IHttpActionResult> PostGeneros_Musicales(Generos_Musicales _generos_Musicales)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Generos_Musicales.Add(_generos_Musicales);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _generos_Musicales.ID }, _generos_Musicales);
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

        // DELETE: api/Generos_Musicales/5
        [ResponseType(typeof(Generos_Musicales))]
        public async Task<IHttpActionResult> DeleteGeneros_Musicales(int id)
        {
            IHttpActionResult result;

            Generos_Musicales _generos_Musicales = await db.Generos_Musicales.FindAsync(id);
            if (_generos_Musicales == null)
            {
                result = NotFound();
            }
            else
            {
                db.Generos_Musicales.Remove(_generos_Musicales);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_generos_Musicales);
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

        private bool Generos_MusicalesExists(int id)
        {
            return db.Generos_Musicales.Count(e => e.ID == id) > 0;
        }
    }
}