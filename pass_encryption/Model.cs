using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ConsoleApp.SQLite
{
    public class passManagerContext : DbContext
    {
        public DbSet<Username> Usernames { get; set; }
        public DbSet<Password> Passwords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=passwords.db");
        }
    }

    public class Username
    {
        public int UsernameId { get; set; }
        public string Name { get; set; }
        public string MainPassword { get; set; }
        public string Salt { get; set; }
    }

    public class Password
    {
        public int PasswordId { get; set; }
        public string hashedPass { get; set; }
        public string tag { get; set; }
        public int UsernameId { get; set; }
    }
}