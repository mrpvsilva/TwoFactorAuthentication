using System;
using MediatR;

namespace WebApplication.Models
{
    public class AddTwoFactAuth : IRequest<Auth>
    {
        public string AuthenticatorUri { get; set; }
        public string Code { get; set; }
        public Guid Hash { get; set; }
    }
}
