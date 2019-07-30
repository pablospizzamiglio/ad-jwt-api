using Microsoft.AspNetCore.Authorization;

namespace ad_jwt_api.Policies
{
    public class CountryRequirement : IAuthorizationRequirement
    {
        public string Country { get; private set; }
        public CountryRequirement(string country)
        {
            Country = country;
        }
    }
}
