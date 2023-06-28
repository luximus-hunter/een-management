using Identity.Enums;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class UserRolesController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRolesController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        List<IdentityUser> users = await _userManager.Users.ToListAsync();
        List<UserRolesViewModel> userRolesViewModel = new();
        foreach (IdentityUser user in users)
        {
            UserRolesViewModel thisViewModel = new();
            thisViewModel.UserId = user.Id;
            thisViewModel.Email = user.Email;
            thisViewModel.UserName = user.UserName;
            thisViewModel.Roles = await GetUserRoles(user);
            userRolesViewModel.Add(thisViewModel);
        }
        return View(userRolesViewModel);
    }
    private async Task<List<string>> GetUserRoles(IdentityUser user)
    {
        return new List<string>(await _userManager.GetRolesAsync(user));
    }
    public async Task<IActionResult> Manage(string userId)
    {
        ViewBag.userId = userId;
        IdentityUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
            return View("NotFound");
        }

        if (user.UserName != null) ViewBag.UserName = user.UserName;
        List<ManageUserRolesViewModel> model = new();
        foreach (IdentityRole role in _roleManager.Roles)
        {
            ManageUserRolesViewModel userRolesViewModel = new()
            {
                RoleId = role.Id,
                RoleName = role.Name
            };
            if (role.Name != null && await _userManager.IsInRoleAsync(user, role.Name))
            {
                userRolesViewModel.Selected = true;
            }
            else
            {
                userRolesViewModel.Selected = false;
            }
            model.Add(userRolesViewModel);
        }
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> Manage(List<ManageUserRolesViewModel> model, string userId)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return View();
        }
        IList<string> roles = await _userManager.GetRolesAsync(user);
        IdentityResult result = await _userManager.RemoveFromRolesAsync(user, roles);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot remove user existing roles");
            return View(model);
        }
        result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName)!);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot add selected roles to user");
            return View(model);
        }
        return RedirectToAction("Index");
    }
}