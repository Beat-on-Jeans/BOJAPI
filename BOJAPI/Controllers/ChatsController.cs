﻿using System;
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
using System.Web.UI.WebControls;
using BOJAPI.Models;

namespace BOJAPI.Controllers
{
    public class ChatsController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Chats
        public IQueryable<Chats> GetChats()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Chats;
        }

        // GET: api/Chats/5
        [ResponseType(typeof(Chats))]
        public async Task<IHttpActionResult> GetChats(int id)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            Chats _chats = await db.Chats.FindAsync(id);
            if (_chats == null)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(_chats);
            }
            return result;
        }

        [HttpGet]
        [Route("api/LocalChatsUser/{userID}")]
        public async Task<IHttpActionResult> GetLocalUserChats(int userID)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            try
            {
                var chats = await db.Chats
                             .Include(c => c.Mensajes)
                             .Where(c => c.UsuarioMobil_Local_ID == userID)
                             .ToListAsync();

                if (chats == null || !chats.Any())
                {
                    result = NotFound();
                }
                else
                {

                    result = Ok(chats);
                }
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }
            return result;
        }

        [HttpGet]
        [Route("api/MusicChatsUser/{userID}")]
        public async Task<IHttpActionResult> GetMusicUserChats(int userID)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            try
            {
                var chats = await db.Chats
                             .Include(c => c.Mensajes)
                             .Where(c => c.UsuarioMobil_Musico_ID == userID)
                             .ToListAsync();

                if (chats == null || !chats.Any())
                {
                    result = NotFound();
                }
                else
                {

                    result = Ok(chats);
                }
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }
            return result;
        }


        [HttpGet]
        [Route("api/Chat/{chatID}")]
        public async Task<IHttpActionResult> GetChat(int chatID)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            try
            {
                var chat = await db.Chats
                    .Include(c => c.Mensajes)
                    .FirstOrDefaultAsync(c => c.ID == chatID);

                if (chat == null)
                {
                    result = NotFound();
                }
                else
                {
                    result = Ok(chat);
                }
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }
            return result;
        }


        // PUT: api/Chats/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutChats(int id, Chats _chats)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {

                if (id != _chats.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_chats).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ChatsExists(id))
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

        // POST: api/Chats
        [ResponseType(typeof(Chats))]
        public async Task<IHttpActionResult> PostChats(Chats _chats)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Chats.Add(_chats);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _chats.ID }, _chats);
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

        // DELETE: api/Chats/5
        [ResponseType(typeof(Chats))]
        public async Task<IHttpActionResult> DeleteChats(int id)
        {
            IHttpActionResult result;
            Chats _chats = await db.Chats.FindAsync(id);
            if (_chats == null)
            {
                result = NotFound();
            }
            else
            {
                String missatge = "";
                db.Chats.Remove(_chats);
                try
                {
                    await db.SaveChangesAsync();
                    result = Ok(_chats);
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

        private bool ChatsExists(int id)
        {
            return db.Chats.Count(e => e.ID == id) > 0;
        }
    }
}