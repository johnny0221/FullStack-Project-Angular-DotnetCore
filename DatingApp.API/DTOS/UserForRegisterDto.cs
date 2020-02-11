using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTOS
{
    public class UserForRegisterDto
    {
        //The request's pass in username must not be blank.
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "You must Submit a password more than 4 chars.")]
        public string Password { get; set; }

         [Required]
         public string Gender { get; set; }

         [Required]
         public string KnownAs { get; set; }
         [Required]
         public DateTime DateOfBirth { get; set; }

         [Required]
         public string City { get; set; }

         [Required]
         public string Country { get; set; }

         [Required]
         public DateTime Created { get; set; }

         [Required]
         public DateTime LastActive { get; set; }

        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }


    }
}