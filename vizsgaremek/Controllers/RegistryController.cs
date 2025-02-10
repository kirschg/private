using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vizsgaremek.Models;

namespace vizsgaremek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistryController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Registry(User user)
        {
            using (var context = new VizsgaremekContext())
            {
                try
                {
                    if (context.Users.FirstOrDefault(x=>x.FelhasznaloNev == user.FelhasznaloNev) is not null)
                    {
                        return BadRequest("A felhasználó név már foglalt.");
                    }
                    if (context.Users.FirstOrDefault(x => x.Email == user.Email) is not null)
                    {
                        return BadRequest("Ez az email cím már használatban van.");
                    }
                    user.Jogosultsag = 0;
                    user.Aktiv = 0;
                    user.Hash = Program.CreateSHA256(user.Hash);
                    await context.Users.AddAsync(user);
                    await context.SaveChangesAsync();
                    Program.SendEmail(user.Email, "Regisztráció", $"https://localhost:7189/api/Registry?felhasznaloNev={user.FelhasznaloNev}&email={user.Email}");
                    return Ok("Sikeres regisztráció. Ellenőrizze az emailjeit és véglegesítse a regisztrációt!");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> EndOfTheRegistry(string felhasznaloNev, string email)
        {
            using (var context = new VizsgaremekContext())
            {
                try
                {
                    User user = await context.Users.FirstOrDefaultAsync(x=> x.FelhasznaloNev == felhasznaloNev && x.Email == email);
                    if (user == null)
                    {
                        return BadRequest("Sikertelen a regisztráció befejezése.");
                    }
                    user.Aktiv = 1;
                    context.Users.Update(user);
                    await context.SaveChangesAsync();
                    return Ok("A regisztráció sikeresen befejeződött.");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}
