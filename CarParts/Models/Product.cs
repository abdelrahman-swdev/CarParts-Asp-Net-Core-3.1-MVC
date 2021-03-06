using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CarParts.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="لازم تكتب اسم المنتج")]
        [MaxLength(255)]
        [Display(Name = "اسم المنتج")]
        public string Name { get; set; }

        [Required(ErrorMessage = "لازم تكتب السعر")]
        [MaxLength(255)]
        [Display(Name = "السعر")]
        public string Price { get; set; }

        [Required(ErrorMessage ="لازم تكتب عدد قطع المتوفرة من هذا المنتج في المخزن")]
        [Display(Name ="العدد في المخزن")]
        public int NumberInStock { get; set; }

        [Required(ErrorMessage = "لازم تكتب وصف للمنتج")]
        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Display(Name ="صورة المنتج")]
        public string ProductPhotoPath { get; set; }

    }
}
