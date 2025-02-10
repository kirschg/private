using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace vizsgaremek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        [HttpPost("{uId}")]
        public IActionResult Logout(string uId) 
        {
            if (Program.LoggedInUsers.ContainsKey(uId))
            {
                Program.LoggedInUsers.Remove(uId);
                return Ok("Sikeres Kijelentkezés.");
            }
            else
            {
                return NotFound("Nem található felhasználó ezzel a tokennel!");
            }
            
        }
    }
}
