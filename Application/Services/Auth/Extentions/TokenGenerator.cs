using Dal;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TemplateWebApi;

namespace Application.Services.UserAuth
{
    public static class TokenGenerator
    {
        public static string CreateJwtToken(User user, string JWTKey)
        {
            var claims = new[]
            {
                new Claim(UserClaimTypes.UserId, user.Id), new Claim(UserClaimTypes.UserEmail, user.UserName)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JWTKey));

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddSeconds(60),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static async Task<RefreshToken> GenerateRefreshToken(string userId, AppDbContext context)
        {
            var token = GenerateRefreshToken(userId);

            while (await context.RefreshTokens.AnyAsync(x => x.Token == token.Token))
            {
                token = GenerateRefreshToken(userId);
            }

            return token;
        }

        private static RefreshToken GenerateRefreshToken(string userId)
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(30),
                UserId = userId
            };
        }
    }
}
