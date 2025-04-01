using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using BOJAPI.Clases;
using BOJAPI.Models;

namespace BOJAPI.Controllers
{
    public class MatchesController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Matches
        public IQueryable<Matches> GetMatches()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Matches;
        }

        // GET: api/Matches/5
        [ResponseType(typeof(Matches))]
        public async Task<IHttpActionResult> GetMatches(int id)
        {
            IHttpActionResult result;
            Matches _matches = await db.Matches.FindAsync(id);
            if (_matches == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_matches);
            }
            return result;
        }

        // PUT: api/Matches/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMatches(int id, Matches _matches)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != _matches.ID)
            {
                return BadRequest();
            }

            db.Entry(_matches).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Matches
        [ResponseType(typeof(Matches))]
        public async Task<IHttpActionResult> PostMatches(Matches _matches)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Matches.Add(_matches);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _matches.ID }, _matches);
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


     
        [HttpPost]
        [Route("api/Matches/{Local_ID}/{Musico_ID}")]
        public async Task<IHttpActionResult> CreateNewMatch(int Local_ID, int Musico_ID)
        {
            IHttpActionResult result;

            var isCurrent = await db.Matches.FirstOrDefaultAsync(m => m.UsuarioMobil_Local_ID == Local_ID && 
                                                                      m.UsuarioMobil_Musico_ID == Musico_ID);
            if (isCurrent == null)
            {

                Matches newMatch = new Matches
                {
                    Estado = 2,
                    UsuarioMobil_Local_ID = Local_ID,
                    UsuarioMobil_Musico_ID = Musico_ID
                };
                db.Matches.Add(newMatch);
                await db.SaveChangesAsync();
                result = Ok(newMatch);
            }
            else
            {
                Matches match = new Matches
                {
                    Estado = 3,
                    UsuarioMobil_Local_ID = Local_ID,
                    UsuarioMobil_Musico_ID = Musico_ID
                };
                db.Entry(match).State = EntityState.Modified;
                await db.SaveChangesAsync();

                Chats newChat = new Chats
                {
                    UsuarioMobil_Local_ID = Local_ID,
                    UsuarioMobil_Musico_ID = Musico_ID
                };
                db.Chats.Add(newChat);
                await db.SaveChangesAsync();

                result = Ok(match);
            }
            return result;
        }

        // DELETE: api/Matches/5
        [ResponseType(typeof(Matches))]
        public async Task<IHttpActionResult> DeleteMatches(int id)
        {
            IHttpActionResult result;

            Matches _matches = await db.Matches.FindAsync(id);
            if (_matches == null)
            {
                result = NotFound();
            }
            else
            {
                db.Matches.Remove(_matches);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_matches);
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

        private bool MatchesExists(int id)
        {
            return db.Matches.Count(e => e.ID == id) > 0;
        }
    }
}