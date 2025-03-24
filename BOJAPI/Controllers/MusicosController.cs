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
    public class MusicosController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Musicos
        public IQueryable<Musicos> GetMusicos()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Musicos;
        }

        // GET: api/Musicos/5
        [ResponseType(typeof(Musicos))]
        public async Task<IHttpActionResult> GetMusicos(int id)
        {
            IHttpActionResult result;
            Musicos _musicos = await db.Musicos.FindAsync(id);
            if (_musicos == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_musicos);
            }
            return result;
        }

        // PUT: api/Musicos/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMusicos(int id, Musicos _musicos)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _musicos.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_musicos).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MusicosExists(id))
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

        // POST: api/Musicos
        [ResponseType(typeof(Musicos))]
        public async Task<IHttpActionResult> PostMusicos(Musicos _musicos)
        {
            IHttpActionResult result;
            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Musicos.Add(_musicos);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _musicos.ID }, _musicos);
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

        // DELETE: api/Musicos/5
        [ResponseType(typeof(Musicos))]
        public async Task<IHttpActionResult> DeleteMusicos(int id)
        {
            IHttpActionResult result;

            Musicos _musicos = await db.Musicos.FindAsync(id);
            if (_musicos == null)
            {
                result = NotFound();
            }
            else
            {
                db.Musicos.Remove(_musicos);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_musicos);
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

        private bool MusicosExists(int id)
        {
            return db.Musicos.Count(e => e.ID == id) > 0;
        }
    }
}