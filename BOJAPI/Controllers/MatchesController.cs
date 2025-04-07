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

        [ResponseType(typeof(List<Clases.Match>))] // Usa un DTO específico
        [HttpGet]
        [Route("api/Matches/GetUserMatches/{userId}")]
        public async Task<IHttpActionResult> GetUserMatches(int userId)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false; // Deshabilita proxies

                var userMatches = await db.Matches
                    .AsNoTracking()
                    .Where(m => m.Creador_ID == userId || m.Finalizador_ID == userId)
                    .Select(m => new Clases.Match // Proyecta a un DTO
                    {
                        ID = m.ID,
                        CreadorId = m.Creador_ID,
                        FinalizadorId = m.Finalizador_ID,
                        Estado = (int)m.Estado
                        // Mapea todas las propiedades necesarias
                    })
                    .ToListAsync();

                return Ok(userMatches);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
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
        [Route("api/Matches/{Creador_ID}/{Finalizador_ID}")]
        public async Task<IHttpActionResult> CreateNewMatch(int Creador_ID, int Finalizador_ID)
        {
            try
            {
                // Verificar si ya existe un match activo (estado < 3) entre estos usuarios
                var existingMatch = await db.Matches.FirstOrDefaultAsync(m =>
                    ((m.Creador_ID == Creador_ID && m.Finalizador_ID == Finalizador_ID) ||
                     (m.Creador_ID == Finalizador_ID && m.Finalizador_ID == Creador_ID))
                    && m.Estado < 3);

                if (existingMatch == null)
                {
                    // Crear nuevo match
                    var newMatch = new Matches
                    {
                        Estado = 2, // Estado "En espera"
                        Creador_ID = Creador_ID,
                        Finalizador_ID = Finalizador_ID
                    };

                    db.Matches.Add(newMatch);
                    await db.SaveChangesAsync();

                    return Ok(new
                    {
                        Message = "Match creado exitosamente",
                        Match = newMatch
                    });
                }
                else
                {
                    // Si el match existe pero no está finalizado (Estado < 3)
                    if (existingMatch.Estado == 2)
                    {
                        existingMatch.Estado = 3; // Finalizar match
                        await db.SaveChangesAsync();

                        // Crear chat automáticamente
                        var newChat = new Chats
                        {
                            UsuarioMobil_Local_ID = Creador_ID,
                            UsuarioMobil_Musico_ID = Finalizador_ID
                        };

                        db.Chats.Add(newChat);
                        await db.SaveChangesAsync();

                        return Ok(new
                        {
                            Message = "Match actualizado a estado 'Finalizado' y chat creado",
                            Match = existingMatch
                        });
                    }

                    return Ok(new
                    {
                        Message = "El match ya existe y está finalizado",
                        Match = existingMatch
                    });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("api/Matches/{Creador_ID}/{Finalizador_ID}")]
        public async Task<IHttpActionResult> UpdateMatchStatusToDislike(int Creador_ID, int Finalizador_ID)
        {
            try
            {
                // Buscar el match entre el creador y el finalizador
                var match = await db.Matches.FirstOrDefaultAsync(m =>
                    (m.Creador_ID == Creador_ID && m.Finalizador_ID == Finalizador_ID) ||
                    (m.Creador_ID == Finalizador_ID && m.Finalizador_ID == Creador_ID));

                if (match == null)
                {
                    // Si no se encuentra el match, crearlo
                    match = new Matches
                    {
                        Creador_ID = Creador_ID,
                        Finalizador_ID = Finalizador_ID,
                        Estado = 1 // Estado 1 indica que es un "dislike"
                    };

                    db.Matches.Add(match); // Añadir el nuevo match a la base de datos
                    await db.SaveChangesAsync(); // Guardar cambios

                    return Ok(new
                    {
                        Message = "Nuevo match creado y marcado como 'dislike' (Estado 1).",
                        Match = new
                        {
                            match.ID,
                            match.Creador_ID,
                            match.Finalizador_ID,
                            Estado = match.Estado
                        }
                    });
                }
                else
                {
                    // Si el match existe, comprobar si está en un estado válido para actualizarlo
                    if (match.Estado < 3) // Estado < 3 significa que el match no está finalizado
                    {
                        match.Estado = 1; // Cambiar el estado a "rechazado" o "dislike"

                        await db.SaveChangesAsync(); // Guardar cambios en la base de datos

                        return Ok(new
                        {
                            Message = "Match actualizado a estado 'rechazado' (Estado 1).",
                            Match = new
                            {
                                match.ID,
                                match.Creador_ID,
                                match.Finalizador_ID,
                                Estado = match.Estado
                            }
                        });
                    }
                    else
                    {
                        return BadRequest("El match ya está finalizado o tiene un estado definitivo.");
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new
                {
                    Message = "Hubo un error al procesar la solicitud.",
                    ErrorDetails = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }


        private IHttpActionResult InternalServerError(Exception ex)
        {
            // Aquí puedes registrar el error o hacer alguna otra acción si es necesario
            return Content(HttpStatusCode.InternalServerError, new
            {
                Message = "Hubo un error al procesar la solicitud.",
                ErrorDetails = ex.Message,
                StackTrace = ex.StackTrace // Opcional, puedes quitarlo si no quieres mostrar detalles sensibles
            });
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