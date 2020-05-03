using System;
using MediatR;
namespace WebApplication.Models
{
    public class VerifyCode : IRequest<Auth>
    {
        public string Code { get; set; }
        public Guid Hash { get; set; }
    }
}
