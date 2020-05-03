using System;

namespace WebApplication.Models
{
    public class TwoFactAuth
    {
        public Guid Hash { get; set; }
        public bool HasTwoFactorAuth { get; set; }
        public string AuthenticatorUri { get; set; }
        public string SharedKey { get; set; }
    }
}
