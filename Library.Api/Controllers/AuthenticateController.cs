using Library.Api.Models.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library.Api.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public AuthenticateController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpPost("token", Name = nameof(GenerateToken))]
        public IActionResult GenerateToken(LoginUser loginUser)
        {
            if (loginUser.UserName != "demouser" || loginUser.Password != "demopassword")
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginUser.UserName)
            };

            var tokenConfigSection = Configuration.GetSection("Security:Token");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigSection["key"]));
            var signCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: tokenConfigSection["Issuer"],
                audience: tokenConfigSection["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signCredential);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                expiration = TimeZoneInfo.ConvertTimeFromUtc(jwtToken.ValidTo, TimeZoneInfo.Local)
            });
        }
    }
}
