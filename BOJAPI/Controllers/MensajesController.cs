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
    public class MensajesController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Mensajes
        public IQueryable<Mensajes> GetMensajes()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Mensajes;
        }

        [HttpGet]
        [Route("api/Mensajes/Mensajes/{chat_ID}")]
        // GET: api/Chats/5
        public async Task<IHttpActionResult> GetChatMensajes(int chat_ID)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            try
            {
                var mensajes = await db.Mensajes
                             .Where(c => c.Chat_ID == chat_ID)
                             .ToListAsync();

                if (mensajes == null)
                {
                    result = NotFound();
                }
                else
                {

                    result = Ok(mensajes);
                }
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }
            return result;
        }

        // GET: api/Mensajes/5
        [ResponseType(typeof(Mensajes))]
        public async Task<IHttpActionResult> GetMensajes(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            Mensajes _mensajes = await db.Mensajes.FindAsync(id);

            if (_mensajes == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_mensajes);
            }
            return result;
        }

        // PUT: api/Mensajes/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMensajes(int id, Mensajes _mensajes)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _mensajes.ID)
                {
                    result = BadRequest();
                }
                else
                {

                    db.Entry(_mensajes).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MensajesExists(id))
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

        // POST: api/Mensajes
        [ResponseType(typeof(Mensajes))]
        public async Task<IHttpActionResult> PostMensajes(Mensajes _mensajes)
        {
            IHttpActionResult result;
            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Mensajes.Add(_mensajes);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _mensajes.ID }, _mensajes);

                    var usuario = await db.Chats.FindAsync(_mensajes.Chat_ID);
                    if (_mensajes.Emisor_ID == usuario.UsuarioMobil_Local_ID)
                    {
                        await Clases.Utilities.SetNotifications(db,usuario.UsuarioMobil_Musico_ID, 1);
                    }
                    else
                    {
                        await Clases.Utilities.SetNotifications(db,usuario.UsuarioMobil_Local_ID, 1);
                    }
                    
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

        // DELETE: api/Mensajes/5
        [ResponseType(typeof(Mensajes))]
        public async Task<IHttpActionResult> DeleteMensajes(int id)
        {
            IHttpActionResult result;

            Mensajes mensajes = await db.Mensajes.FindAsync(id);
            if (mensajes == null)
            {
                result = NotFound();
            }
            else
            {

                db.Mensajes.Remove(mensajes);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = Ok(mensajes);
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

        private bool MensajesExists(int id)
        {
            return db.Mensajes.Count(e => e.ID == id) > 0;
        }
    }
}