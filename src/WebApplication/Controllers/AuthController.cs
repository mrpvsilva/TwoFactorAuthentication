using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OtpNet;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UrlEncoder _urlEncoder;

        public AuthController(UrlEncoder urlEncoder)
        {
            _urlEncoder = urlEncoder;
        }


        [HttpGet]
        public IActionResult Generate()
        {
            var key = Encoding.UTF8.GetBytes("32E1DD25106D48D9BD1FDCC2B62DBAD8");
            var hash = Base32Encoding.ToString(key);
            string uri = string.Format("otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6", _urlEncoder.Encode("TwoFactAuth"), _urlEncoder.Encode("teste@email.com"), hash);

            return Ok(new { hash, uri });
        }

        [HttpPost]
        public IActionResult Verify([FromBody] Verify verify)
        {
            var otp = new Totp(Base32Encoding.ToBytes(verify.Hash));
            var computed = otp.ComputeTotp();
            var valid = otp.VerifyTotp(verify.Code, out long timeStepMatch);
            return Ok(new { valid, timeStepMatch, computed, verify.Code });
        }
    }

    public class Verify
    {
        public string Code { get; set; }
        public string Hash { get; set; }
    }

    public class Account
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}