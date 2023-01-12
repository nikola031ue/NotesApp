using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NotesApp.API.Contexts;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System;

namespace NotesApp.API.Services
{
    public class JwtTokenManager : IJwtTokenManager
    {
        private readonly IConfiguration _configuration;
        private UserContext _context;

        public JwtTokenManager(IConfiguration configuration, UserContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string Authenticate(string email/*, string password*/)
        {


            if (!_context.Users.Any(x => x.Email.Equals(email)/* && x.Password.Equals(password)*/))
            {
                throw new ArgumentNullException();
            }

            var key = _configuration.GetValue<string>("JwtConfig:Key");
            var keyBytes = Encoding.ASCII.GetBytes(key);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
