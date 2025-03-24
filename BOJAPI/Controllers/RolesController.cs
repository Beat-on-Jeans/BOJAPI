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
    public class RolesController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Roles
        public IQueryable<Roles> GetRoles()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Roles;
        }

        // GET: api/Roles/5
        [ResponseType(typeof(Roles))]
        public async Task<IHttpActionResult> GetRoles(int id)
        {
            IHttpActionResult result;
            Roles _roles = await db.Roles.FindAsync(id);
            if (_roles == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_roles);
            }
            return result;
        }

        // PUT: api/Roles/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutRoles(int id, Roles _roles)
        {
            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _roles.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_roles).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RolesExists(id))
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

        // POST: api/Roles
        [ResponseType(typeof(Roles))]
        public async Task<IHttpActionResult> PostRoles(Roles _roles)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Roles.Add(_roles);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _roles.ID }, _roles);
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

        // DELETE: api/Roles/5
        [ResponseType(typeof(Roles))]
        public async Task<IHttpActionResult> DeleteRoles(int id)
        {
            IHttpActionResult result;

            Roles _roles = await db.Roles.FindAsync(id);
            if (_roles == null)
            {
                result = NotFound();
            }
            else
            {
                db.Roles.Remove(_roles);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_roles);
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

        private bool RolesExists(int id)
        {
            return db.Roles.Count(e => e.ID == id) > 0;
        }
    }
}