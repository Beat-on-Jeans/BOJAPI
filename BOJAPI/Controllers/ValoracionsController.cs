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


        [ResponseType(typeof(IEnumerable<Actuacion>))]
        [HttpGet]
        [Route("api/Valoraciones/isNewRatting/{userID}")]
        public async Task<IHttpActionResult> GetUpcomingNewActuacion(int userID)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            var today = DateTime.UtcNow.Date;

            var actuacionesAcabadas = await db.Actuacion
                                            .Where(a => ((a.Creador_ID == userID || a.Finalizador_ID == userID) && a.Estado == 3) && a.Fecha < today)
                                            .ToListAsync();

            var otrosUsuarios = actuacionesAcabadas
                                .SelectMany(a => new[] { a.Creador_ID, a.Finalizador_ID })
                                .Where(id => id != userID)
                                .Distinct()
                                .ToList();

            foreach (var actuaciones in actuacionesAcabadas)
            {
                Valoracion valoracion_Creador = new Valoracion
                {
                    Valor = null,
                    Valorador_ID = actuaciones.Creador_ID,
                    Valorado_ID = actuaciones.Finalizador_ID,
                };

                Valoracion valoracion_Finalizador = new Valoracion
                {
                    Valor = null,
                    Valorador_ID = actuaciones.Finalizador_ID,
                    Valorado_ID = actuaciones.Creador_ID,
                };

                actuaciones.Estado = 4;

                db.Entry(actuaciones).State = EntityState.Modified;
                db.SaveChanges();

                db.Valoracion.Add(valoracion_Creador);
                db.SaveChanges();

                db.Valoracion.Add(valoracion_Finalizador);
                db.SaveChanges();
            }

            var valoracionesPendientes = await db.Valoracion
                                           .Where(v => v.Valorador_ID == userID && v.Valor == null)
                                           .ToListAsync();


            if (valoracionesPendientes == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(valoracionesPendientes);
            }
            return result;
        }

        // PUT: api/Valoracions
        [ResponseType(typeof(Valoracion))]
        [HttpPut]
        [Route("api/Valoracions/{rating}")]
        public async Task<IHttpActionResult> PostValoracion(Valoracion valoracion)
        {
            IHttpActionResult result;
            String missatge = "";
            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Entry(valoracion).State = EntityState.Modified;
                result = StatusCode(HttpStatusCode.NoContent);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                    missatge = Clases.Utilities.MissatgeError(sqlException);
                    result = BadRequest(missatge);
                }

                var valoracionTotal = await db.Valoracion
                                        .Where(v => v.Valorado_ID == valoracion.Valorado_ID && v.Valor != null)
                                        .Select(v => v.Valor)
                                        .ToListAsync();

                var ratteduser = await db.UsuarioMobil.FindAsync(valoracion.Valorado_ID);

                ratteduser.ValoracionTotal = valoracionTotal.Average();

                db.Entry(ratteduser).State = EntityState.Modified;
                result = StatusCode(HttpStatusCode.NoContent);
                try
                {
                    await db.SaveChangesAsync();
                    result = Ok(valoracion);
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