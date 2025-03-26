using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BOJAPI.Controllers
{
    public class UploadController : ApiController
    {
        [HttpPost]
        [Route("api/upload")]
        public async Task<IHttpActionResult> UploadImage()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return BadRequest("Unsupported media type");
            }

            try
            {
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                // Directorio donde se almacenarán las imágenes
                string directoryPath = HttpContext.Current.Server.MapPath("~/Imgs");

                // Asegura que la carpeta existe
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                foreach (var file in provider.Contents)
                {
                    var fileName = file.Headers.ContentDisposition.FileName.Trim('"');

                    // Validación de tipo MIME (solo imágenes)
                    var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/jpg" };
                    if (!allowedMimeTypes.Contains(file.Headers.ContentType.MediaType))
                    {
                        return BadRequest("Invalid file type. Only images are allowed.");
                    }

                    var buffer = await file.ReadAsByteArrayAsync();

                    // Genera un nombre único para evitar sobrescribir archivos
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                    string filePath = Path.Combine(directoryPath, uniqueFileName);

                    // Guarda el archivo en el servidor
                    File.WriteAllBytes(filePath, buffer);

                    // 🔥 Retornar la URL de la imagen subida
                    return Ok(new
                    {
                        message = "Image uploaded successfully",
                        fileName = uniqueFileName,
                        url = $"http://10.0.1.140/ApiBOJ/Imgs/{uniqueFileName}"
                    });
                }

                return BadRequest("No file uploaded.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("api/download/{fileName}")]
        public IHttpActionResult DownloadImage(string fileName)
        {
            try
            {
                string filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Imgs"), fileName);

                if (!File.Exists(filePath))
                {
                    return NotFound();
                }

                byte[] fileBytes = File.ReadAllBytes(filePath);
                MemoryStream stream = new MemoryStream(fileBytes);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(stream)
                };

                response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg"); // Ajusta el tipo MIME según el formato de la imagen
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName
                };

                return ResponseMessage(response);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
