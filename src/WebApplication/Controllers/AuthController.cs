using System.Text.Encodings.Web;
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
            
            //string uri = string.Format("otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6", _urlEncoder.Encode("TwoFactAuth"), _urlEncoder.Encode("teste@email.com"), hash);

            return Ok();
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

    
}