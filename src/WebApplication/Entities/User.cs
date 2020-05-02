using System;
using OtpNet;

namespace WebApplication.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Key { get; set; }

        public void HashPasword()
        {
            Password = BCrypt.Net.BCrypt.HashPassword(Password);
        }
        public void GenerateKey()
        {
            Key = Base32Encoding.ToString(Guid.NewGuid().ToByteArray());
        }


        public User()
        {
            Id = Guid.NewGuid();
            GenerateKey();
        }
    }
}
