using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace vizsgaremek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> FileUploadFTP()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string fileName = postedFile.FileName;
                string subfolder = "/";
                string filePath = "ftp://ftp.nethey.hu" + subfolder + fileName;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(filePath);
                request.Credentials = new NetworkCredential("kirschg", "saját jelszó");
                request.Method = "POST";
                await using(Stream ftpStream = request.GetRequestStream())
                {
                    postedFile.CopyTo(ftpStream);
                }
                return Ok($"Sikeres feltöltés {fileName}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
