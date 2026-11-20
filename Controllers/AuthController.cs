using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DSW_3P_PR01.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DSW_3P_PR01.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [AllowAnonymous]
   [HttpPost("login")]
   public IActionResult Login(LoginDto login)
   {
       // Usuario temporal para la práctica
       if (login.Usuario != "admin" ||
           login.Password != "QuestLog123!")
       {
           return Unauthorized("Credenciales incorrectas");
       }
       var jwt = _configuration.GetSection("Jwt");
       var key = Convert.FromBase64String(
           jwt["Key"] ?? throw new InvalidOperationException(
               "No se configuró Jwt:Key"));
       var claims = new[]
       {
           new Claim(
               ClaimTypes.Name,
               login.Usuario),
           new Claim(
               ClaimTypes.Role,
               "Administrador")
       };
       var credentials = new SigningCredentials(
           new SymmetricSecurityKey(key),
           SecurityAlgorithms.HmacSha256);
       var token = new JwtSecurityToken(
           issuer: jwt["Issuer"],
           audience: jwt["Audience"],
           claims: claims,
           expires: DateTime.UtcNow.AddMinutes(
               int.Parse(jwt["ExpirationMinutes"] ?? "30")),
           signingCredentials: credentials);
       var tokenString =
           new JwtSecurityTokenHandler().WriteToken(token);
       return Ok(new
       {
           access_token = tokenString,
           token_type = "Bearer",
           expires_in = int.Parse(
               jwt["ExpirationMinutes"] ?? "30") * 60
       });
   }
}