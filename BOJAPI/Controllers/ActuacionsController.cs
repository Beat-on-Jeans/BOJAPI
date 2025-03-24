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
    public class ActuacionsController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Actuacions
        public IQueryable<Actuacion> GetActuacion()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Actuacion;
        }

        // GET: api/Actuacions/5
        [ResponseType(typeof(Actuacion))]
        public async Task<IHttpActionResult> GetActuacion(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            Actuacion _actuacion = await db.Actuacion.FindAsync(id);

            if (_actuacion == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_actuacion);
            }
            return result;
        }

        // PUT: api/Actuacions/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutActuacion(int id, Actuacion _actuacion)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _actuacion.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_actuacion).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ActuacionExists(id))
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

        // POST: api/Actuacions
        [ResponseType(typeof(Actuacion))]
        public async Task<IHttpActionResult> PostActuacion(Actuacion _actuacion)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {

                db.Actuacion.Add(_actuacion);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _actuacion.ID }, _actuacion);
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

        // DELETE: api/Actuacions/5
        [ResponseType(typeof(Actuacion))]
        public async Task<IHttpActionResult> DeleteActuacion(int id)
        {
            IHttpActionResult result;
            Actuacion _actuacion = await db.Actuacion.FindAsync(id);
            if (_actuacion == null)
            {
                result = NotFound();
            }
            else
            {
                db.Actuacion.Remove(_actuacion);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_actuacion);
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

        private bool ActuacionExists(int id)
        {
            return db.Actuacion.Count(e => e.ID == id) > 0;
        }
    }
}