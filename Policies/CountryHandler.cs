using ad_jwt_api.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ad_jwt_api.Policies
{
    public class CountryHandler : AuthorizationHandler<CountryRequirement>
    {
        private readonly IOptions<JwtOptions> _jwtOptions;

        public CountryHandler(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            CountryRequirement requirement)
        {
            if (!context.User.HasClaim(c => 
                c.Issuer == _jwtOptions.Value.ValidIssuer && c.Type == ClaimTypes.Country))
            {
                return Task.CompletedTask;
            }

            string country = context.User.FindFirst(c =>
                c.Issuer == _jwtOptions.Value.ValidIssuer && c.Type == ClaimTypes.Country).Value;
            if (string.IsNullOrWhiteSpace(country))
            {
                return Task.CompletedTask;
            }

            if (country == requirement.Country)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
