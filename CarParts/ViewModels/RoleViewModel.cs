using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarParts.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage ="لازم تدخل اسم الدور")]
        [Display(Name ="الدور")]
        public string Name { get; set; }
    }
}
