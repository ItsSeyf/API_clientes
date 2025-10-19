using MiAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly string secretKey;
        private readonly Contexto _db;
        public AutenticacionController(IConfiguration configuration, Contexto contexto)
        {
            secretKey = configuration.GetSection("settings").GetSection("secretKey").ToString();
            _db = contexto;
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] Usuarios user)
        {
            try
            {
                var usuario = await _db.Usuarios.FirstOrDefaultAsync(x=>x.username == user.username && x.pass == user.pass);
                if(usuario == null)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new { mensaje = "Usuario o contraseña incorrectos" });

                }
                var Keybytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.username));
                var TokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Keybytes), SecurityAlgorithms.HmacSha256Signature)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(TokenDescriptor);
                string tokencreado = tokenHandler.WriteToken(tokenConfig);
                return Ok(new {token = tokencreado, expira = TokenDescriptor.Expires});
            }catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { token = "", expira = "" });
            }
        }
    }
}
