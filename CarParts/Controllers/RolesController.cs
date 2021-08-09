using CarParts.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarParts.Controllers
{
    [Authorize(Roles = "مسؤول")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        // GET: Roles
        public ActionResult Index()
        {
            var roles = roleManager.Roles.ToList();
            List<RoleViewModel> models = new List<RoleViewModel>();
            foreach(var role in roles)
            {
                models.Add(new RoleViewModel { 
                    Id = role.Id,
                    Name = role.Name
                });
            }
            return View(models);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IdentityRole role)
        {
            if(role.Name == "" || role.Name == null)
            {
                ViewBag.RoleNameRequiredErrorMessage = "لازم تكتب اسم الدور";
                return View(role);
            }
            var roles = roleManager.Roles.ToList();
            foreach(var item in roles)
            {
                if(item.Name == role.Name)
                {
                    ViewBag.RoleExist = "هذا الدور موجود بالفعل";
                    return View(role);
                }

            }
            await roleManager.CreateAsync(role);
            return RedirectToAction(nameof(Index));
        }

        // GET: Roles/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if(role != null)
            {
                RoleViewModel model = new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name
                };
                return View(model);
            }
            else
            {
                return Content("لا يوجد دور بهذه الصفات");
            }

        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string id, RoleViewModel role)
        {
            if(id != role.Id)
            {
                return BadRequest();
            }
            if (role.Name == "" || role.Name == null)
            {
                return View(role);
            }
            try
            {
                var roleInDb = await roleManager.FindByIdAsync(role.Id);
                if(roleInDb == null)
                {
                    return BadRequest();
                }
                roleInDb.Name = role.Name;

                var result = await roleManager.UpdateAsync(roleInDb);

                if (!result.Succeeded)
                {
                    return View(role);
                }
            }
            catch
            {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Roles/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role != null)
            {
                RoleViewModel model = new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name
                };
                return View(model);
            }
            else
            {
                return Content("لا يوجد دور بهذه الصفات");
            }
        }

        // POST: Roles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string id, RoleViewModel role)
        {
            try
            {
                if(id == null || id != role.Id)
                {
                    return BadRequest();
                }
                var roleInDb = await roleManager.FindByIdAsync(id);
                var result = await roleManager.DeleteAsync(roleInDb);
                if (!result.Succeeded)
                {
                    return View(role);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(role);
            }
        }
    }
}
