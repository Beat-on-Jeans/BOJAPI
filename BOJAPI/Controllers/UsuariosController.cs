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
using Microsoft.Ajax.Utilities;

namespace BOJAPI.Controllers
{
    public class UsuariosController : ApiController
    {
        private dam05Entities1 db = new dam05Entities1();

        // GET: api/Usuarios
        public IQueryable<Usuarios> GetUsuarios()
        {
            db.Configuration.LazyLoadingEnabled = false;
            return db.Usuarios;
        }

        // GET: api/Usuarios/5
        [ResponseType(typeof(Usuarios))]
        public async Task<IHttpActionResult> GetUsuarios(int id)
        {
            IHttpActionResult result;
            Usuarios _usuarios = await db.Usuarios.FindAsync(id);
            if (_usuarios == null)
            {
                result = NotFound();
            }
            else
            {

                result = Ok(_usuarios);
            }
            return result;
        }

        [HttpGet]
        [Route("api/Usuarios/{userID}")]
        public async Task<IHttpActionResult> GetUser(int userID)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            try
            {
                var user = await db.Usuarios.FirstOrDefaultAsync(c => c.ID == userID);

                if (user == null)
                {
                    result = NotFound();
                }
                else
                {
                    result = Ok(user);
                }
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }
            return result;
        }

        // PUT: api/Usuarios/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUsuarios(int id, Usuarios _usuarios)
        {

            IHttpActionResult result;
            String missatge = "";

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                if (id != _usuarios.ID)
                {
                    result = BadRequest();
                }
                else
                {
                    db.Entry(_usuarios).State = EntityState.Modified;
                    result = StatusCode(HttpStatusCode.NoContent);
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UsuariosExists(id))
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

        // POST: api/Usuarios
        [ResponseType(typeof(Usuarios))]
        public async Task<IHttpActionResult> PostUsuarios(Usuarios _usuarios)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Usuarios.Add(_usuarios);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _usuarios.ID }, _usuarios);
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


        // POST: api/Usuarios
        [HttpPost]
        [Route("api/Usuarios/CreateUsuarioMobils/{_usuarios}")]
        public async Task<IHttpActionResult> CreateUsuarioMobil(Usuarios _usuarios, UsuarioMobil _usuarioMobil)
        {
            IHttpActionResult result;

            if (!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            else
            {
                db.Usuarios.Add(_usuarios);
                String missatge = "";
                try
                {
                    await db.SaveChangesAsync();
                    result = CreatedAtRoute("DefaultApi", new { id = _usuarios.ID }, _usuarios);
                    db.UsuarioMobil.Add(_usuarioMobil);
                    if(_usuarioMobil.ROL_ID == 1)
                    {

                    }
                    else
                    {

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

        // DELETE: api/Usuarios/5
        [ResponseType(typeof(Usuarios))]
        public async Task<IHttpActionResult> DeleteUsuarios(int id)
        {
            IHttpActionResult result;

            Usuarios _usuarios = await db.Usuarios.FindAsync(id);
            if (_usuarios == null)
            {
                result = NotFound();
            }
            else
            {
                db.Usuarios.Remove(_usuarios);
                String missatge = "";
                try
                {

                    await db.SaveChangesAsync();
                    result = Ok(_usuarios);
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

        private bool UsuariosExists(int id)
        {
            return db.Usuarios.Count(e => e.ID == id) > 0;
        }
    }
}