using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vizsgaremek.Models;

namespace vizsgaremek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost("GetSalt/{FelhasznaloNev}")]
        public async Task<IActionResult> GetSalt(string FelhasznaloNev)
        {
            using(var context = new VizsgaremekContext())
            {
                try
                {
                    User res = await context.Users.FirstOrDefaultAsync(x=> x.FelhasznaloNev == FelhasznaloNev);
                    if (res == null)
                    {
                        return NotFound("Nem található felhasználó.");
                    }
                    else
                    {
                        return Ok(res.Salt);
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            using (var context = new VizsgaremekContext())
            {
                try
                {
                    string Hash = Program.CreateSHA256(loginDTO.TmpHash);
                    User loggedUser = await context.Users.FirstOrDefaultAsync(x=> x.Hash == Hash && x.FelhasznaloNev == loginDTO.LoginName);
                    if (loggedUser != null && loggedUser.Aktiv == 1)
                    {
                        string token = Guid.NewGuid().ToString();
                        lock(Program.LoggedInUsers)
                        {
                            Program.LoggedInUsers.Add(token, loggedUser);
                            return Ok(new User()
                            {
                                FelhasznaloNev = loginDTO.LoginName,
                                Email = loggedUser.Email,
                                Jogosultsag = loggedUser.Jogosultsag,
                                ProfilKepUtvonal = loggedUser.ProfilKepUtvonal,
                            }
                            );
                        }
                    }
                    else 
                    {
                        return NotFound();
                    }
                    
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
