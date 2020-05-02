using System;
using System.Web;
using OtpNet;

namespace WebApplication.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Key { get; set; }
        public bool HasTwoFactorAuth { get { return !string.IsNullOrEmpty(Key); } }

        public string AuthenticatorUri
        {
            get
            {
                if (HasTwoFactorAuth)
                    return null;

                return string.Format("otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6", UrlEncode("TwoFactAuth"), UrlEncode(Email), GenerateKey());
            }
        }

        private string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        public void HashPasword()
        {
            Password = BCrypt.Net.BCrypt.HashPassword(Password);
        }
        private string GenerateKey()
        {
            return Base32Encoding.ToString(KeyGeneration.GenerateRandomKey());
        }

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}
