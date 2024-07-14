using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetIdentityApp.Models;
using NetIdentityApp.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;

namespace NetIdentityApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<AppRole> _roleManager;

        public UsersController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var users = _userManager.Users;
            return View(users);
        }



        public async Task<IActionResult> Edit(string? userName)
        {
            if (userName == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
            var model = new UserUpdateViewModel()
            {
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                Id = user.Id,
                SelectedRoles = await _userManager.GetRolesAsync(user)
            };
            return View(model);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(string userName, UserUpdateViewModel model, string id)
        {
            ViewBag.Roles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
            if (!ModelState.IsValid)
            {
                
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.Email = model.Email ?? user.Email;
            user.UserName = model.UserName ?? user.UserName;
            user.FullName = model.FullName ?? user.FullName;
            var result = await _userManager.UpdateAsync(user);
            if (model.SelectedRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);
            }

            if (result.Succeeded && !String.IsNullOrEmpty(model.Password))
            {
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, model.Password!);
            }

            if (!result.Succeeded)
            {
                foreach (IdentityError eror in result.Errors)
                {
                    ModelState.AddModelError("", eror.Description);
                    return View(model);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string? userName)
        {
            if (userName == null)
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
    }
}
