using System.Text.Json.Serialization;

namespace WebApplication.Models
{
    public class Auth
    {
        public string AccessToken { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }
    }
}
