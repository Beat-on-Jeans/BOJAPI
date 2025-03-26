using System;
using System.Collections.Generic;
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

                foreach (var file in provider.Contents)
                {
                    var fileName = file.Headers.ContentDisposition.FileName.Trim('"');
                    var buffer = await file.ReadAsByteArrayAsync();

                    string filePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Imgs"), fileName);
                    File.WriteAllBytes(filePath, buffer);
                }

                return Ok("Image uploaded successfully");
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
