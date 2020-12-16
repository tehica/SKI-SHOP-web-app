using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    // ITokenService -> Core.Interfaces
    public class TokenService : ITokenService
    {
         
        private readonly IConfiguration _config;

        // secure key that we can use to encrypt what we are going to use to sign this key from our server
        // this signature is important because this is what allows our server to trust the token that
        // the client sends up to us
        // Token will not be saved in DB, token is generated from the server, server is going to assing 
        // this token

        // SymmetricSecurityKey is type of encryption where only one key (secret key) which we're gonna to store
        // on our server is used to both encrypt and decrypt our signature in the token
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            // SymmetricSecurityKey takes a byte array
            // we need to encode key from string of text to byte array
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]));
        }

        // each user is going to have a list of their claims inside this token
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                // these claims inside token are going to be able to be decoded by the client
                // so if the user has that token he will be able to look inside the token and he will
                // be able to see their properties follow email and display name
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.DisplayName)
            };

            // credentials
            // SecurityAlgorithms.HmacSha512Signature is largest level of encryption
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // this is where we store our claims
                // DateTime.Now.AddDays(7) - this is the time that token is valid for seven days 
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["Token:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
