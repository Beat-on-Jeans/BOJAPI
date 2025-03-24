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
    public class NotificacionesController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Notificaciones
        public IQueryable<Notificaciones> GetNotificaciones()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Notificaciones;
        }

        // GET: api/Notificaciones/5
        [ResponseType(typeof(Notificaciones))]
        public async Task<IHttpActionResult> GetNotificaciones(int id)
        {
            IHttpActionResult result;
            Notificaciones _notificaciones = await db.Notificaciones.FindAsync(id);
            if (_notificaciones == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_notificaciones);
            }
            return result;
        }

        // PUT: api/Notificaciones/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNotificaciones(int id, Notificaciones _notificaciones)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _notificaciones.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_notificaciones).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!NotificacionesExists(id))
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

        // POST: api/Notificaciones
        [ResponseType(typeof(Notificaciones))]
        public async Task<IHttpActionResult> PostNotificaciones(Notificaciones _notificaciones)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Notificaciones.Add(_notificaciones);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _notificaciones.ID }, _notificaciones);
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

        // DELETE: api/Notificaciones/5
        [ResponseType(typeof(Notificaciones))]
        public async Task<IHttpActionResult> DeleteNotificaciones(int id)
        {
            IHttpActionResult result;

            Notificaciones _notificaciones = await db.Notificaciones.FindAsync(id);
            if (_notificaciones == null)
            {
                result = NotFound();
            }
            else
            {
                db.Notificaciones.Remove(_notificaciones);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_notificaciones);
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

        private bool NotificacionesExists(int id)
        {
            return db.Notificaciones.Count(e => e.ID == id) > 0;
        }
    }
}