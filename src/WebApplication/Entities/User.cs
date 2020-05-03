using System;

namespace WebApplication.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Key { get; set; }
        public bool HasTwoFactorAuth { get { return !string.IsNullOrEmpty(Key); } }

        public void HashPasword()
        {
            Password = BCrypt.Net.BCrypt.HashPassword(Password);
        }        

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}
