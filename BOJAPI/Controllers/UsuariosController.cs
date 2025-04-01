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

        // GET: api/Usuarios/{ID}
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

        // POST: api/User
        [HttpPost]
        [Route("api/User")]
        public async Task<IHttpActionResult> CreateUser(UsuarioRecibido usuarioRecibido)
        {
            // Paso 1: Crear el objeto Usuario
            var usuario = new Usuarios

            {
                ROL_ID = usuarioRecibido.ROL_ID,
                Nombre = usuarioRecibido.Nombre,
                Correo = usuarioRecibido.Correo,
                Contrasena = usuarioRecibido.Contrasena
            };

            // Paso 2: Añadir el usuario a la base de datos
            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync(); // Guarda el usuario en la tabla Usuarios

            Usuarios newUser = db.Usuarios.FirstOrDefault(c => c.Correo == usuarioRecibido.Correo);

            // Paso 3: Crear el objeto UsuarioMobil y asignarle el Notificacion_ID
            var usuarioMobil = new UsuarioMobil
            {
                Notificacion_ID = null,  // Aquí asignamos el ID del Usuario recién creado
                ROL_ID = usuarioRecibido.ROL_ID,
                Url_Imagen = usuarioRecibido.Url_Imagen,
                ValoracionTotal = null,
                Ubicacion = usuarioRecibido.Ubicacion,
                Usuario_ID = newUser.ID,
                Descripcion = null
            };

            // Paso 4: Añadir el UsuarioMobil a la base de datos
            db.UsuarioMobil.Add(usuarioMobil);
            await db.SaveChangesAsync();  // Guarda el usuario movil en la tabla UsuarioMobil

            return Ok(usuario);  // Devolver el objeto Usuario creado
        }

        // POST: api/User/Login
        [HttpPost]
        [Route("api/User/Login")]
        public async Task<IHttpActionResult> Login(UsuarioRecibido usuarioRecibido)
        {
            // Paso 2: Buscar el usuario en la base de datos por correo
            var usuario = await db.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == usuarioRecibido.Correo);

            if (usuario == null)
            {
                return Unauthorized(); // No existe un usuario con ese correo
            }

            // Paso 3: Validar si la contraseña coincide (esto es solo un ejemplo, asegúrate de usar hashing para contraseñas)
            if (usuario.Contrasena != usuarioRecibido.Contrasena)
            {
                return Unauthorized(); // Contraseña incorrecta
            }

            // Paso 4: Buscar el usuario en la tabla UsuarioMobil usando el ID de Usuario
            var usuarioMobil = await db.UsuarioMobil
                .FirstOrDefaultAsync(um => um.Usuario_ID == usuario.ID);

            // Paso 5: Devolver toda la información del usuario junto con los datos de UsuarioMobil
            var usuarioRecibidoCompleto = new UsuarioRecibido
            {
                ID = usuarioMobil.ID,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo,
                Contrasena = usuario.Contrasena,
                ROL_ID = (int)usuario.ROL_ID,
                Url_Imagen = usuarioMobil.Url_Imagen,
                Ubicacion = usuarioMobil.Ubicacion
            };

            return Ok(usuarioRecibidoCompleto);
        }




        // GET: api/Usuarios/Matches_Locales/{Ubicacion}/{userID}
        [HttpGet]
        [Route("api/Usuarios/Matches_Locales/{Ubicacion}/{userID}")]
        public async Task<IHttpActionResult> GetLocalMatchesUser(String ubicacion, int userID)
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            try
            {
                var userMatch = await (from u in db.Usuarios
                                       join um in db.UsuarioMobil on u.ID equals um.Usuario_ID
                                       join gu in db.Generos_Usuarios on um.ID equals gu.Usuario_Id into generosJoin
                                       from gu in generosJoin.DefaultIfEmpty()
                                       join gm in db.Generos_Musicales on gu.Genero_Id equals gm.ID into generosMusicalesJoin
                                       from gm in generosMusicalesJoin.DefaultIfEmpty()
                                       join m in db.Matches on um.ID equals m.UsuarioMobil_Local_ID into matchesJoin
                                       from m in matchesJoin.DefaultIfEmpty()
                                       where um.Ubicacion.ToLower().Contains(ubicacion.ToLower()) 
                                               && u.ROL_ID == 2
                                               && (m == null || m.Estado < 3)
                                       group new { u, um, gm } by new
                                       {
                                           um.ID,
                                           u.Nombre,
                                           um.Descripcion,
                                           um.Url_Imagen
                                       } into usuarioGroup
                                       select new
                                       {
                                           usuarioGroup.Key.ID,
                                           usuarioGroup.Key.Nombre,
                                           usuarioGroup.Key.Descripcion,
                                           Generos = usuarioGroup.Where(g => g.gm != null).Select(g => g.gm.Nombre_Genero).Distinct().ToList(),
                                           usuarioGroup.Key.Url_Imagen
                                       }).ToListAsync();

                if (userMatch == null)
                {
                    result = NotFound();
                }
                else
                {

                    var localIDs = await (from u in db.Usuarios
                                          join um in db.UsuarioMobil on u.ID equals um.Usuario_ID
                                          join m in db.Matches on um.ID equals m.UsuarioMobil_Musico_ID
                                          where um.Usuario_ID == userID
                                          select m.UsuarioMobil_Local_ID).ToListAsync();




                    if (localIDs != null)
                    {
                        // Filtrar userMatch, eliminando los que tienen ID en matchedIds
                        userMatch = userMatch.Where(um => !localIDs.Contains(um.ID)).ToList();

                    }

                    result = Ok(userMatch);
                }

            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }
            return result;
        }

        // GET: api/Usuarios/Matches_Music/{Ubicacion}
        [HttpGet]
        [Route("api/Usuarios/Matches_Music/{Ubicacion}/{userID}")]
        public async Task<IHttpActionResult> GetMusicMatchesUser(String ubicacion, int userID)
        {
            var location = Regex.Split(ubicacion, @"\s*,\s*");
            string ciudad = location[location.Length - 1].ToLower();
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;
            try
            {
                var userMatch = await (from u in db.Usuarios
                                       join um in db.UsuarioMobil on u.ID equals um.Usuario_ID
                                       join gu in db.Generos_Usuarios on um.ID equals gu.Usuario_Id into generosJoin
                                       from gu in generosJoin.DefaultIfEmpty()
                                       join gm in db.Generos_Musicales on gu.Genero_Id equals gm.ID into generosMusicalesJoin
                                       from gm in generosMusicalesJoin.DefaultIfEmpty()
                                       join m in db.Matches on um.ID equals m.UsuarioMobil_Musico_ID into matchesJoin
                                       from m in matchesJoin.DefaultIfEmpty()
                                       where um.Ubicacion.ToLower().Contains(ciudad) 
                                             && u.ROL_ID == 1
                                             && (m == null || m.Estado < 3)
                                       group new { u, um, gm } by new
                                       {
                                           um.ID,
                                           u.Nombre,
                                           um.Descripcion,
                                           um.Url_Imagen
                                       } into usuarioGroup
                                       select new
                                       {
                                           usuarioGroup.Key.ID,
                                           usuarioGroup.Key.Nombre,
                                           usuarioGroup.Key.Descripcion,
                                           Generos = usuarioGroup.Where(g => g.gm != null).Select(g => g.gm.Nombre_Genero).Distinct().ToList(),
                                           usuarioGroup.Key.Url_Imagen
                                       }).ToListAsync();

                if (userMatch == null)
                {
                    result = NotFound();
                }
                result = Ok(userMatch);
            }
            catch (Exception ex)
            {
                result = InternalServerError(ex);
            }
            return result;
        }


        // GET: api/Usuarios/Musicos
        [ResponseType(typeof(IEnumerable<UsuarioRecibido>))]
        [HttpGet]
        [Route("api/Usuarios/Musicos")]
        public async Task<IHttpActionResult> GetAllMusicos()
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            // Buscar los músicos en base al rol
            var musicos = await (from usuario in db.Usuarios
                                 join usuarioMobil in db.UsuarioMobil
                                 on usuario.ID equals usuarioMobil.Usuario_ID // Join usando Usuario_ID de UsuarioMobil
                                 where usuario.ROL_ID == 1 // Filtramos por rolId = 1 (músicos)
                                 select new UsuarioRecibido
                                 {
                                     ID = usuario.ID,
                                     Nombre = usuario.Nombre, // Nombre de Usuario (de la tabla Usuarios)
                                     Correo = null, // Correo a null
                                     Contrasena = null, // Contraseña a null
                                     ROL_ID = (int)usuario.ROL_ID,
                                     Url_Imagen = usuarioMobil.Url_Imagen, // La URL de imagen de UsuarioMobil
                                     Ubicacion = usuarioMobil.Ubicacion // La ubicación de UsuarioMobil
                                 }).ToListAsync();

            if (musicos == null || musicos.Count == 0)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(musicos);
            }
            return result;
        }

        // GET: api/Usuarios/Locales
        [ResponseType(typeof(IEnumerable<UsuarioRecibido>))]
        [HttpGet]
        [Route("api/Usuarios/Locales")]
        public async Task<IHttpActionResult> GetAllLocales()
        {
            IHttpActionResult result;
            db.Configuration.LazyLoadingEnabled = false;

            // Buscar los locales en base al rol
            var locales = await (from usuario in db.Usuarios
                                 join usuarioMobil in db.UsuarioMobil
                                 on usuario.ID equals usuarioMobil.Usuario_ID // Join usando Usuario_ID de UsuarioMobil
                                 where usuario.ROL_ID == 2 // Filtramos por rolId = 2 (locales)
                                 select new UsuarioRecibido
                                 {
                                     ID = usuario.ID,
                                     Nombre = usuario.Nombre, // Nombre de Usuario (de la tabla Usuarios)
                                     Correo = null, // Correo a null
                                     Contrasena = null, // Contraseña a null
                                     ROL_ID = (int)usuario.ROL_ID,
                                     Url_Imagen = usuarioMobil.Url_Imagen, // La URL de imagen de UsuarioMobil
                                     Ubicacion = usuarioMobil.Ubicacion // La ubicación de UsuarioMobil
                                 }).ToListAsync();

            if (locales == null || locales.Count == 0)
            {
                result = NotFound();
            }
            else
            {
                result = Ok(locales);
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

