using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarParts.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage ="لازم تكتب اسمك")]
        [Display(Name ="اسمك")]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "لازم تكتب اسم الوالد")]
        [Display(Name = "اسم الوالد")]
        [MaxLength(255)]
        public string LastName { get; set; }
    }
}
