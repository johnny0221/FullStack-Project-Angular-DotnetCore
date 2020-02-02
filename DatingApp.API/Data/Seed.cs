using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using DatingApp.API.models;
using System;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context)
        {
            if(!context.Users.Any())
            {
                var userdata = System.IO.File.ReadAllText("Data/UserSeedData.json");
                // map our json data into objects
                var users = JsonConvert.DeserializeObject<List<User>>(userdata);
                

                foreach(var user in users)
                {
                    byte[] passwordHash, passwordSalt;

                    CreatePasswordHash("password", out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Username = user.Username.ToLower();

                    context.Users.Add(user);
                }

                context.SaveChanges();
            }

        }
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //after doing all the logic inside the bracket, it will automatically call the dispose() method
            //Whenever you instantiate a class of the IDispose interface, you are recommended to use call dispose()
            //method after using all the logic to prevent memory leak.
            //In here the class we are instantiating here implements the IDisposable interface, so whenever we use
            //it in a single method we'd better call dispose whenever we're finished, and using() {} is a syntatic sugar
            //for doing it.
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}