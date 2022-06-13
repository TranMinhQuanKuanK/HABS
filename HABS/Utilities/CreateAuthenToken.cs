using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class CreateAuthenToken
    {
        public static string GetToken(string role)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KeyForSignInSecret@1234"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: "http://localhost:2000",
                audience: "http://localhost:2000",
                claims: new List<Claim>() {
                    new Claim(ClaimTypes.Role, role)
                },
                expires: DateTime.Now.AddDays(7),
                signingCredentials: signinCredentials
            ); ;

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }
    }
}
