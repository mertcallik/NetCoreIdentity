using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetIdentityApp.Models;
using System.Data;

namespace NetIdentityApp.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RolesController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles =await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppRole model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = new AppRole()
            {
                Name = model.Name
            };
          var result=await _roleManager.CreateAsync(role);
          if (!result.Succeeded)
          {
              foreach (IdentityError err in result.Errors)
              {
                  ModelState.AddModelError("", err.Description);
                  return View(model);
              }
          }
            return RedirectToAction("Index");
        }

        public async Task<IList<AppUser>> TakeUsersFromRole(AppRole role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role.Name);
            return users;
        }
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role==null)
            {
                return NotFound();
            }
            ViewBag.Users = await TakeUsersFromRole(role);


            return View(role);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(AppRole model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = TakeUsersFromRole(model);
                return View(model);
            }

            var role = await _roleManager.FindByIdAsync(model.Id);
            var roleValidator = new RoleValidator<AppRole>();
          var rolevalidationresult= await roleValidator.ValidateAsync(_roleManager, model);
          if (!rolevalidationresult.Succeeded)
          {
              rolevalidationresult.Errors.ToList().ForEach(err =>
              {
                  ModelState.AddModelError("", err.Description);

              });
              ViewBag.Users =await TakeUsersFromRole(model);
                return View(model);
          }
          role.Name = model.Name;

          var result = await _roleManager.UpdateAsync(role);
          if (!result.Succeeded)
          {
              result.Errors.ToList().ForEach(err =>
              {
                  ModelState.AddModelError("", err.Description);
              });
              ViewBag.Users =await TakeUsersFromRole(model);
                return View(model);
          }
          return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteFromRole(string UserName,string RoleId)
        {
            var role = await _roleManager.FindByIdAsync(RoleId);
           var result= await _userManager.RemoveFromRoleAsync(await _userManager.FindByNameAsync(UserName), role.Name);
           if (!result.Succeeded)
           {
               result.Errors.ToList().ForEach(err =>
               {
                   ModelState.AddModelError("", err.Description);
               });
           }
            return RedirectToAction("Index");
        }
    }
}
