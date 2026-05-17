using System;
using MediatR;

namespace WebApplication.Models
{
    public class VerifyEmailOtp : IRequest<Auth>
    {
        public Guid Hash { get; set; }
        public string Code { get; set; }
    }
}
