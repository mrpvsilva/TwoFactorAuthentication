using System;
using MediatR;

namespace WebApplication.Models
{
    public class SendEmailOtp : IRequest<bool>
    {
        public Guid Hash { get; set; }
    }
}
