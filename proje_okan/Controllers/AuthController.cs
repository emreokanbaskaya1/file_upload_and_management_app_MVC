using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using proje_okan.Models;
using proje_okan.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace proje_okan.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public AuthController(IUserService userService, IConfiguration config)
        {
            _userService = userService;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            var user = _userService.Register(model.Username, model.Password);
            return Ok(user);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] RegisterModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);
            if (user == null)
                return Unauthorized(new { message = "Kullanıcı adı veya şifre hatalı." });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                return StatusCode(500, new { message = "JWT key is not configured." });
            }

            var keyBytes = Encoding.ASCII.GetBytes(key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _config["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = $"Bearer {jwt}" });
        }
    }
}
