using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WareHouseApi.DbContexts;

namespace WareHouseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            if (loginModel is null)
            {
                return Unauthorized
                    
                    (new { message = "loginModel is null" });
            }
            string password = loginModel.Password;
            string login = loginModel.Login;
            if (password != "nothing is true everything is permitted" || login != "assassin")
            { 
                return Unauthorized(new { message = "Данные введены некорректно!" });
            }
            return Ok(GetToken(login + DateTime.Now.ToString()));
        }

        public class LoginModel
        {
            public string Login { get; set; } = "";
            public string Password { get; set; } = "";

        }

        private string GetToken(string name)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.UTF8.GetBytes(Global.SecretKey);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, name)
            }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tok = tokenHandler.CreateToken(tokenDescriptor);
            string? token = tokenHandler.WriteToken(tok);
            return token;
        }
    }
}
