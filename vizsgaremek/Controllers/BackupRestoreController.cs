using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities;
using System.Diagnostics.Eventing.Reader;
using vizsgaremek.Models;

namespace vizsgaremek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupRestoreController : ControllerBase
    {
        IWebHostEnvironment _env;

        public BackupRestoreController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [Route("Backup/{Uid},{fileName}")]
        [HttpGet]
        public async Task<ActionResult> SQLBackup(string uId, string fileName)
        {
            if (Program.LoggedInUsers.ContainsKey(uId) && Program.LoggedInUsers[uId].Jogosultsag == 9)
            {
                string hibauzenet = "";
                using (var context = new VizsgaremekContext())
                {
                    string sqlDataSource = context.Database.GetConnectionString()!;
                    MySqlCommand command = new MySqlCommand();
                    MySqlBackup backup = new MySqlBackup(command);
                    using (MySqlConnection myconnection = new MySqlConnection(sqlDataSource))
                    {
                        try
                        {
                            command.Connection = myconnection;
                            myconnection.Open();
                            var filePath = "SQLBackupRestore/" + fileName;
                            backup.ExportToFile(filePath);
                            myconnection.Close();
                            if (System.IO.File.Exists(filePath))
                            {
                                return File("text/plain", "error.txt");
                            }
                            else
                            {
                                hibauzenet = "Nincs ilyen fájl!";
                                byte[] bytes = new byte[hibauzenet.Length];
                                for (int i = 0; i < hibauzenet.Length; i++)
                                {
                                    bytes[i] = Convert.ToByte(hibauzenet[i]);
                                }
                                return File(bytes, "text/plain", "error.txt");
                            }
                        }
                        catch (Exception ex)
                        {
                            return new JsonResult(ex.Message);
                        }
                    }
                }
            }
            else
            {
                return Unauthorized("Az elérés nem engedélyezett.");
            }
        }

        [Route("Restore/{uId}")]
        [HttpPost]
        public JsonResult SQLRestore(string uId)
        {
            if (Program.LoggedInUsers.ContainsKey(uId) && Program.LoggedInUsers[uId].Jogosultsag == 9)
            {
                try
                {
                    var context = new VizsgaremekContext();
                    string sqlDataSource = context.Database.GetConnectionString();
                    var httpRequest = Request.Form;
                    var postedFile = httpRequest.Files[0];
                    string fileName = postedFile.FileName;
                    string subFolder = "/SqlBackupRestore/";
                    var filePath = _env.ContentRootPath + subFolder + fileName;
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }
                    MySqlCommand mycommand = new MySqlCommand();
                    MySqlBackup restore = new MySqlBackup(mycommand);
                    using (MySqlConnection mySqlConnection = new MySqlConnection(sqlDataSource))
                    {
                        try
                        {
                            mycommand.Connection = mySqlConnection;
                            mySqlConnection.Open();
                            restore.ImportFromFile(filePath);
                            System.IO.File.Delete(filePath);
                            mySqlConnection.Close();
                            return new JsonResult("A visszatöltés sikeres."); 
                        }
                        catch (Exception)
                        {
                            return new JsonResult("Mentésfájl feltöltve, de a szerver nem elérhető.");
                        }
                    }
                }
                catch (Exception)
                {
                    return new JsonResult("Nem sikerült a feltöltés.");
                }
            }
            else
            {
                return new JsonResult("Nem engedélyezett a hozzáférés.");
            }
        }
    }
}
