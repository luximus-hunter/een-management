using Identity.Data;
using Identity.Enums;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers;

public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Users
    [Authorize(Roles = nameof(Roles.Admin) + "," + nameof(Roles.Moderator))]
    public async Task<IActionResult> Index()
    {
        return View(await _userManager.Users.ToListAsync());
    }

    // GET: Users/Edit/5
    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<IActionResult> Edit(string? id)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [Authorize(Roles = nameof(Roles.Admin))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,UserName")] IdentityUser userDto)
    {
        if (id != userDto.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                IdentityUser? user = await _userManager.FindByIdAsync(userDto.Id);

                IdentityResult setUsernameResult = await _userManager.SetUserNameAsync(user, userDto.UserName);
                if (!setUsernameResult.Succeeded)
                {
                    setUsernameResult.Errors.ToList().ForEach(e => Console.WriteLine(e.Description));
                    Console.WriteLine("Unexpected error when trying to set username.");
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UserExists(userDto.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(userDto);
    }

    // GET: Users/Delete/5
    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<IActionResult> Delete(string? id)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = nameof(Roles.Admin))]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> UserExists(string id)
    {
        return await _userManager.FindByIdAsync(id) != null;
    }
}