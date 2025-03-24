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
    public class TipoIncidenciasController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/TipoIncidencias
        public IQueryable<TipoIncidencia> GetTipoIncidencia()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.TipoIncidencia;
        }

        // GET: api/TipoIncidencias/5
        [ResponseType(typeof(TipoIncidencia))]
        public async Task<IHttpActionResult> GetTipoIncidencia(int id)
        {
            IHttpActionResult result;
            TipoIncidencia _tipoIncidencia = await db.TipoIncidencia.FindAsync(id);
            if (_tipoIncidencia == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_tipoIncidencia);
            }
            return result;
        }

        // PUT: api/TipoIncidencias/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTipoIncidencia(int id, TipoIncidencia _tipoIncidencia)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _tipoIncidencia.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_tipoIncidencia).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TipoIncidenciaExists(id))
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

        // POST: api/TipoIncidencias
        [ResponseType(typeof(TipoIncidencia))]
        public async Task<IHttpActionResult> PostTipoIncidencia(TipoIncidencia _tipoIncidencia)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.TipoIncidencia.Add(_tipoIncidencia);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _tipoIncidencia.ID }, _tipoIncidencia);
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

        // DELETE: api/TipoIncidencias/5
        [ResponseType(typeof(TipoIncidencia))]
        public async Task<IHttpActionResult> DeleteTipoIncidencia(int id)
        {
            IHttpActionResult result;

            TipoIncidencia _tipoIncidencia = await db.TipoIncidencia.FindAsync(id);
            if (_tipoIncidencia == null)
            {
                result = NotFound();
            }
            else
            {
                db.TipoIncidencia.Remove(_tipoIncidencia);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_tipoIncidencia);
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

        private bool TipoIncidenciaExists(int id)
        {
            return db.TipoIncidencia.Count(e => e.ID == id) > 0;
        }
    }
}