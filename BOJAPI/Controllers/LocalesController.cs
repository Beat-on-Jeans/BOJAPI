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
    public class LocalesController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Locales
        public IQueryable<Locales> GetLocales()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Locales;
        }

        // GET: api/Locales/5
        [ResponseType(typeof(Locales))]
        public async Task<IHttpActionResult> GetLocales(int id)
        {
            IHttpActionResult result;
            Locales _locales = await db.Locales.FindAsync(id);
            if (_locales == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_locales);
            }
            return result;
        }

        // PUT: api/Locales/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLocales(int id, Locales _locales)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _locales.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_locales).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LocalesExists(id))
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

        // POST: api/Locales
        [ResponseType(typeof(Locales))]
        public async Task<IHttpActionResult> PostLocales(Locales _locales)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Locales.Add(_locales);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _locales.ID }, _locales);
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

        // DELETE: api/Locales/5
        [ResponseType(typeof(Locales))]
        public async Task<IHttpActionResult> DeleteLocales(int id)
        {
            IHttpActionResult result;

            Locales _locales = await db.Locales.FindAsync(id);
            if (_locales == null)
            {
                result = NotFound();
            }
            else
            {
                db.Locales.Remove(_locales);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_locales);
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

        private bool LocalesExists(int id)
        {
            return db.Locales.Count(e => e.ID == id) > 0;
        }
    }
}