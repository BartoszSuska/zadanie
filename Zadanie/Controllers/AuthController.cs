using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zadanie.Data;
using Zadanie.DTO;
using Zadanie.Models;

namespace Zadanie.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
        }

        //Generowanie tokenu JWT
        private string GenerateJwtToken(User user) {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //Sprawdzanie poprawności danych logowania
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto) {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user==null)
                return Unauthorized("Invalid email.");

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                return Unauthorized("Invalid password.");

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }
    }
}
