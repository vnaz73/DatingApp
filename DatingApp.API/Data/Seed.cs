using System.Collections.Generic;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DatingApp.Api.Data
{
    public class Seed
    {
        private readonly DataContext _context;

        public Seed(DataContext context)
        {
            _context = context;

        }

        public void SeedUsers()
        {
            var userData = System.IO.File.ReadAllText("data/userseeddata.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            foreach(var user in users)
                {
                    byte[] PasswordHash, PasswordSalt;
                    createPasswordHash("password", out PasswordHash, out PasswordSalt);
                    user.PasswordHash = PasswordHash;
                    user.PasswordSalt = PasswordSalt;
                    user.UserName = user.UserName.ToLower();

                    _context.Users.Add(user);
                }
            _context.SaveChanges();    
        }
       
    private void createPasswordHash(string password, out byte[] hashpassword, out byte[] hashsalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                hashsalt = hmac.Key;
                hashpassword = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
    
}