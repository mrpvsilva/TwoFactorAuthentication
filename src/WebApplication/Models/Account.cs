using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace WebApplication.Models
{
    public class Account : IRequest<TwoFactAuth>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class TwoFactAuth
    {
        public string AuthenticatorUri { get; set; }
    }
}
