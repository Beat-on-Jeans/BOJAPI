using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
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


        // GET: api/Actuacions/5
        [ResponseType(typeof(IEnumerable<Actuacion>))]
        [HttpGet]
        [Route("api/Actuacions/GetUpcomingNewActuacion/{creatorID}/{userID}")]
        public async Task<IHttpActionResult> GetUpcomingNewActuacion(int creatorID, int userID)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            var actuacionesPendintesConf = await db.Actuacion
                                            .Where(a => a.Creador_ID == creatorID && a.Finalizador_ID == userID && a.Estado < 3)
                                            .ToListAsync();

            if (actuacionesPendintesConf == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(actuacionesPendintesConf);
            }
            return result;
        }


        // GET: api/Actuacions/5
        [ResponseType(typeof(IEnumerable<Actuacion>))]
        [HttpGet]
        [Route("api/Actuacions/GetUserActuaciones/{userID}")]
        public async Task<IHttpActionResult> GetActuacions(int userID)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;


            var resultados = (from a in db.Actuacion
                                join umCreador in db.UsuarioMobil on a.Creador_ID equals umCreador.ID
                                join uCreador in db.Usuarios on umCreador.Usuario_ID equals uCreador.ID
                                join umFinalizador in db.UsuarioMobil on a.Finalizador_ID equals umFinalizador.ID
                                join uFinalizador in db.Usuarios on umFinalizador.Usuario_ID equals uFinalizador.ID
                                where a.Creador_ID == userID || a.Finalizador_ID == userID
                                select new
                                {
                                    Fechas = a.Fecha,
                                    NombreCreador = uCreador.Nombre,
                                    Imagen = umCreador.Url_Imagen,
                                    NombreFinalizador = uFinalizador.Nombre,
                                }).ToList();

            if (resultados == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(resultados);
            }
            return result;
        }


        [HttpPut]
        [Route("api/Actuacions/CreateEvent/{event}")]
        public async Task<IHttpActionResult> CreateActuacion(Actuacion _actuacion)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Entry(_actuacion).State = EntityState.Modified;
                result = StatusCode(HttpStatusCode.NoContent);
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

        // POST: api/Actuacion
        [HttpPost]
        [ResponseType(typeof(Mensajes))]
        public async Task<IHttpActionResult> PostMensajes(Actuacion _actuacion)
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
                    await Clases.Utilities.SetNotifications(db, _actuacion.Finalizador_ID, 5);
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
        [HttpDelete]
        [ResponseType(typeof(Actuacion))]
        public async Task<IHttpActionResult> DeleteActuacion(Actuacion _actuacion)
        {
            IHttpActionResult result;
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