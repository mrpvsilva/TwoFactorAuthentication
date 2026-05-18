using System;

namespace WebApplication.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public User User { get; set; }

        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;

        public RefreshToken()
        {
            Id = Guid.NewGuid();
        }
    }
}
