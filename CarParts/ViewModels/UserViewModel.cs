using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarParts.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "لازم تكتب الاسم الاول")]
        [Display(Name = "الاسم الاول")]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "لازم تكتب اسم العائلة")]
        [Display(Name = "اسم العائلة")]
        [MaxLength(255)]
        public string LastName { get; set; }


        [Required(ErrorMessage = "لازم تكتب البريد الالكتروني")]
        [EmailAddress]
        [Display(Name = "البريد الالكتروني")]
        public string Email { get; set; }

        [Required(ErrorMessage = "لازم تكتب كلمة السر")]
        [StringLength(100, ErrorMessage = "كلمة السر ليست قوية", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة السر")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تاكيد كلمة السر")]
        [Compare("Password", ErrorMessage = "كلمة السر غير متطابقه")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage ="لازم تحدد دور المستخدم")]
        [Display(Name ="حدد دور لهذا المستخدم")]
        public string RoleId { get; set; }

        public IEnumerable<IdentityRole> roles { get; set; }
    }
}
