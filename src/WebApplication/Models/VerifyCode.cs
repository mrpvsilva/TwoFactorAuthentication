using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
namespace WebApplication.Models
{
    public class VerifyCode : IRequest<Auth>
    {
        public string Code { get; set; }
        public Guid Hash { get; set; }
    }
}
