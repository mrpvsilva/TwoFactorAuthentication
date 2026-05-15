using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication.Jwt
{
    public class SigningConfigurations
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations(string privateKeyPem)
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);
            Key = new RsaSecurityKey(rsa);
            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256);
        }
    }
}
