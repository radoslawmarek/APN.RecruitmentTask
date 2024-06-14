using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace APN.RecruitmentTask.Api.Endpoints;

public static class AuthenticationEndpoint
{
    private static readonly string[] EndpointTags = new[] { "Authentication" };
    
    public static void AddAuthenticationEndpoints(this WebApplication application)
    {
        application.MapGet("/api/token", () =>
        {
            var secretKey = application.Configuration["Authentication:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("Secret key is missing");
            }
            
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "Bookstore User"),
                }),
                Expires = DateTime.UtcNow.AddYears(10), 
                Audience = "Bookstore",
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return Results.Ok(new { token });
        })
        .WithName("GetToken")
        .WithDescription("Get JWT token")
        .WithTags(EndpointTags);
    }
}