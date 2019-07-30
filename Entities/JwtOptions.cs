using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ad_jwt_api.Entities
{
    public class JwtOptions
    {
        public string SecurityKey { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public SymmetricSecurityKey SymmetricSecurityKey =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
            //new SymmetricSecurityKey(Convert.FromBase64String(SecurityKey));
        public SigningCredentials SigningCredentials => 
            new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
    }

}