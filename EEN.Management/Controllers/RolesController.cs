using EEN.Management.Data;
using Identity.Data;
using Identity.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class RolesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        List<IdentityRole> roles = await _roleManager.Roles.ToListAsync();
        return View(roles);
    }

    [HttpPost]
    public async Task<IActionResult> AddRole(string? roleName)
    {
        if (roleName != null)
        {
            await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(string? id)
    {
        if (id == null) return NotFound();
        
        IdentityRole? role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        return View(role);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string? id)
    {
        if (id == null) return NotFound();
        
        IdentityRole? role = await _roleManager.FindByIdAsync(id);
        if (role != null)
        {
            await _roleManager.DeleteAsync(role);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: Users/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null) return NotFound();

        IdentityRole? role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        return View(role);
    }

    // POST: Users/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] IdentityRole roleDto)
    {
        if (id != roleDto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                IdentityRole? role = await _roleManager.FindByIdAsync(roleDto.Id);

                if (role != null)
                {
                    role.Name = roleDto.Name?.Trim();
                    role.NormalizedName = roleDto.Name?.Trim().ToUpper();
                    await _roleManager.UpdateAsync(role);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await RoleExists(roleDto.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(roleDto);
    }

    private async Task<bool> RoleExists(string id)
    {
        return await _roleManager.FindByIdAsync(id) != null;
    }
}