using System;
using MediatR;

namespace WebApplication.Models
{
    public class VerifyRegistrationOtp : IRequest<bool>
    {
        public Guid Hash { get; set; }
        public string Code { get; set; }
    }
}
