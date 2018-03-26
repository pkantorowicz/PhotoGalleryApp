using System;
using System.Collections.Generic;

namespace Gallery.Data.Models
{
    public class User : IEntityBase
    {
        public User()
        {
            UserRoles = new List<UserRole>();
            UserTokens = new HashSet<UserToken>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string SerialNumber { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
        public bool IsLocked { get; set; }
        public DateTimeOffset? LastLoggedIn { get; set; }
        public DateTime DateCreated { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<UserToken> UserTokens { get; set; }
    }
}
