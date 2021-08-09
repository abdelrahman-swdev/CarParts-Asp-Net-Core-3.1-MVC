using AspNetCoreHero.ToastNotification.Abstractions;
using CarParts.Areas.Identity.Pages.Account;
using CarParts.Models;
using CarParts.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarParts.Controllers
{
    [Authorize(Roles ="مسؤول")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly INotyfService notyfService;



        public AdminController(UserManager<ApplicationUser> _userManager,
            RoleManager<IdentityRole> _roleManager,
            SignInManager<ApplicationUser> _signInManager,
            ILogger<LogoutModel> logger, INotyfService notyfService)
        {
            userManager = _userManager;
            roleManager = _roleManager;
            signInManager = _signInManager;
            _logger = logger;
            this.notyfService = notyfService;

        }
        // GET: AdminController
        public ActionResult ListUsers()
        {
            var users = userManager.Users.Where(u => u.UserName != User.Identity.Name).ToList();
            return View(users);
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult CreateUser()
        {
            UserViewModel model = new UserViewModel {
                roles = roleManager.Roles.ToList()
            };
            
            return View(model);
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(UserViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var role = roleManager.Roles.FirstOrDefault(r => r.Id == model.RoleId);
                    if (role != null)
                    {
                        await userManager.AddToRoleAsync(user, role.Name);
                    }
                    notyfService.Success("تم اضافة المستخدم بنجاح");
                    return RedirectToAction("ListUsers", "Admin");
                }
                else
                {
                    notyfService.Error("حدث خطأ");
                    return Content("لم يتم اضافة المستخدم");
                }

            }
            else
            {
                model.roles = roleManager.Roles.ToList();
                return View(model);
            }
        }

        // GET: AdminController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                notyfService.Error("حدث خطأ");
                return Content("لا يوجد مستخدم بهذه المواصفات");
            }
            var roles = await userManager.GetRolesAsync(user);
            var role = await roleManager.FindByNameAsync(roles[0]);
            EditUserViewModel model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                RoleId = role.Id,
                roles = roleManager.Roles.ToList()
            };
            return View(model);
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var userInDb = await userManager.FindByIdAsync(model.Id);
                    if(userInDb == null)
                    {
                        notyfService.Error("حدث خطأ");
                        return Content("لا يوجد مستخدم بهذه المواصفات");
                    }

                    userInDb.FirstName = model.FirstName;
                    userInDb.LastName = model.LastName;
                    userInDb.Email = model.Email;
                    userInDb.UserName = model.Email;

                    var role = await roleManager.FindByIdAsync(model.RoleId);
                    if(role == null)
                    {
                        notyfService.Error("حدث خطأ");
                        return Content("لا يوجد دور بهذه المواصفات");
                    }

                    var result = await userManager.IsInRoleAsync(userInDb, role.Name);
                    if (!result)
                    {
                        var currentRoles = await userManager.GetRolesAsync(userInDb);
                        await userManager.RemoveFromRolesAsync(userInDb, currentRoles);
                        await userManager.AddToRoleAsync(userInDb, role.Name);
                    }

                    await userManager.UpdateAsync(userInDb);
                    notyfService.Success("تم تعديل المستخدم بنجاح");
                    return RedirectToAction(nameof(ListUsers));
                }
                else
                {
                    model.roles = roleManager.Roles.ToList();
                    return View(model);
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                return View(user);
            }
            notyfService.Error("حدث خطأ");
            return Content("لا يوجد مستخدم بهذه المواصفات");
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, ApplicationUser user)
        {
            try
            {
                if(id != user.Id)
                {
                    notyfService.Error("حدث خطأ");
                    return Content("لا يوجد مستخدم بهذه المواصفات");
                }
                var userInDb = await userManager.FindByIdAsync(user.Id);
                //await signInManager.SignOutAsync();
                //_logger.LogInformation("User logged out.");
                var result = await userManager.DeleteAsync(userInDb);
                if (result.Succeeded)
                {
                    notyfService.Success("تم حذف المستخدم بنجاح");
                    return RedirectToAction(nameof(ListUsers));
                }
                else
                {
                    notyfService.Error("حدث خطأ");
                    return Content("لم يتم حذف المستخدم");
                }
            }
            catch
            {
                return View();
            }
        }
    }
}
