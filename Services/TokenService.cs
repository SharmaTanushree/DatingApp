using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService(IConfiguration config) : ITokenService
    {
        public string CreateToken(AppUser user)
        {
           var tokenKey = config["TokenKey"] ?? throw new Exception("can not access token key from appsettings");
           if(tokenKey.Length < 64) throw new Exception("token key must be at least 64 characters long");

           var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
           var claims = new List<Claim>{
              new(ClaimTypes.NameIdentifier, user.UserName)
           };

           var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
           var tokenDescriptor = new SecurityTokenDescriptor{
               Subject = new ClaimsIdentity(claims),
               Expires = DateTime.Now.AddDays(7),
               SigningCredentials = creds
           };
           var tokenHandler = new JwtSecurityTokenHandler();
           var token = tokenHandler.CreateToken(tokenDescriptor);

           return tokenHandler.WriteToken(token);
        }
    }


}