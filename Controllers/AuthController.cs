using ad_jwt_api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ad_jwt_api.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<JwtOptions> _jwtOptions;

        public AuthController(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
        }

        public IActionResult Login([FromBody] AuthRequest request)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.Username)
                || string.IsNullOrWhiteSpace(request.Username))
            {
                return BadRequest("Invalid credentials.");
            }

            using (var principalContext = new PrincipalContext(ContextType.Domain, "domain"))
            {
                if (!principalContext.ValidateCredentials(request.Username, request.Password))
                {
                    return Unauthorized();
                }

                var user = UserPrincipal.FindByIdentity(principalContext, request.Username);
                DirectoryEntry directoryEntry = user.GetUnderlyingObject() as DirectoryEntry;
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.GivenName, user.GivenName),
                    new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Country, directoryEntry.Properties["c"].Value.ToString())
                };

                var token = new JwtSecurityToken(
                    issuer: _jwtOptions.Value.ValidIssuer,
                    audience: _jwtOptions.Value.ValidAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(30),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: _jwtOptions.Value.SigningCredentials
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }
    }
}
