using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotNet7.ExpenseTrackerApi.Middleware;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public AuthorizationMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? authHeader = context.Request.Headers["Api_Key"];
        string requestPath = context.Request.Path;

        if (requestPath == "/api/account/register" || requestPath == "/api/account/login")
        {
            await _next(context);
            return;
        }

        if (!string.IsNullOrEmpty(authHeader) && authHeader is not null && authHeader.StartsWith("Bearer"))
        {
            string[] header_and_token = authHeader.Split(' ');
            string header = header_and_token[0];
            string token = header_and_token[1];
            string privateKey = _configuration.GetSection("EncryptionKey").Value!;
            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(privateKey);

            TokenValidationParameters parameters = new()
            {
                RequireExpirationTime = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken securityToken);

            if (principal is not null)
            {
                await _next(context);
                return;
            }
            else
            {
                context.Response.StatusCode = 401;
                return;
            }
        }
        else
        {
            context.Response.StatusCode = 401;
            return;
        }
    }
}