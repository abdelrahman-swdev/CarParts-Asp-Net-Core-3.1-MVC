using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarParts.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }


        [Required(ErrorMessage = "لازم تكتب الاسم الاول للمستخدم")]
        [Display(Name = "اسم المستخدم الاول")]
        [MaxLength(255)]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "لازم تكتب الاسم الثاني للمستخدم")]
        [Display(Name = "اسم المستخدم الثاني")]
        [MaxLength(255)]
        public string LastName { get; set; }


        [Required(ErrorMessage = "لازم تكتب البريد الالكتروني")]
        [EmailAddress]
        [Display(Name = "البريد الالكتروني")]
        public string Email { get; set; }

        [Display(Name = "دور هذا المستخدم")]
        public string RoleId { get; set; }

        public IEnumerable<IdentityRole> roles { get; set; }
    }
}
