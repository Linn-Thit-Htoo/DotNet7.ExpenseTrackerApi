using DotNet7.ExpenseTrackerApi.DbService.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotNet7.ExpenseTrackerApi.Services;

public class JwtService
{
    private readonly IConfiguration _configuration;
    private readonly AesService _aesService;

    public JwtService(IConfiguration configuration, AesService aesService)
    {
        _configuration = configuration;
        _aesService = aesService;
    }

    public string GenerateJWTToken(UserModel user)
    {
        List<Claim> claims = new()
        {
            new Claim("UserId", _aesService.EncryptString(user.UserId.ToString())),
            new Claim("UserName", _aesService.EncryptString(user.UserName)),
            new Claim("Email", _aesService.EncryptString(user.Email)),
            new Claim("UserRole", _aesService.EncryptString(user.UserRole))
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMonths(1),
            signingCredentials: signIn);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}