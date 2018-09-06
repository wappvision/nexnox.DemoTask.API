
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace nexnox.DemoTask.API.Data
{
    public class ShaJwtTokenProvider<TUser> : ITokenProvider<TUser> where TUser : IdentityUser
    {
        private SecurityKey _key;
        private string _algorithm;
        private string _issuer;
        private string _audience;

        public ShaJwtTokenProvider(string issuer, string audience, string key)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            _algorithm = SecurityAlgorithms.HmacSha256Signature;
            _issuer = issuer;
            _audience = audience;
        }

        public string CreateToken(TUser user, DateTime expiry, List<Claim> claims)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(user.UserName, "jwt"));

            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaims(claims);

            SecurityToken token = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor
            {
                Audience = _audience,
                Issuer = _issuer,
                SigningCredentials = new SigningCredentials(_key, _algorithm),
                Expires = expiry.ToUniversalTime(),
                Subject = identity
            });

            return tokenHandler.WriteToken(token);
        }

    }
}
