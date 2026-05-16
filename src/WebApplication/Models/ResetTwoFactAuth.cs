using System;
using MediatR;

namespace WebApplication.Models
{
    public class ResetTwoFactAuth : IRequest<bool>
    {
        public Guid UserId { get; set; }
    }
}
