using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using CarParts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CarParts.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notyfService;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, INotyfService notyfService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyfService = notyfService;
        }

        [Display(Name ="البريد الالكتروني")]
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {

            [Required(ErrorMessage = "لازم تكتب الاسم الاول")]
            [Display(Name = "الاسم الاول")]
            [MaxLength(255)]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "لازم تكتب اسم العائلة")]
            [Display(Name = "اسم العائلة")]
            [MaxLength(255)]
            public string LastName { get; set; }

            [Required(ErrorMessage = "لازم تكتب كلمة السر لتاكيد التعديل")]
            [StringLength(100, ErrorMessage = "كلمة السر ليست قوية", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "اكتب كلمة السر لتاكيد التعديلات")]
            public string Password { get; set; }

            [Display(Name ="الدور")]
            public string RoleName { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var roleName = await _userManager.GetRolesAsync(user);

            Username = userName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleName = roleName.First()
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var flag = await _userManager.CheckPasswordAsync(user, Input.Password);
            if (flag)
            {

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }

                await _signInManager.RefreshSignInAsync(user);
                _notyfService.Success("تم تعديل حسابك بنجاح");
                return RedirectToPage();
            }
            else
            {
                await LoadAsync(user);
                _notyfService.Error("كلمة السر غير صحيحة لن تتم عملية التعديل", 5);
                return Page();
            }

            /*var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }*/
        }
    }
}
