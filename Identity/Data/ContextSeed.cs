using Identity.Enums;
using Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Data;

public static class ContextSeed
{
    public static async Task SeedRolesAsync(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        //Seed Roles
        await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Moderator.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
    }

    public static async Task SeedDefaultUsers(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        //Seed Default Admin
        IdentityUser defaultAdmin = new()
        {
            UserName = "admin",
            Email = "admin@mail.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };
        if (userManager.Users.All(u => u.Id != defaultAdmin.Id))
        {
            IdentityUser? user = await userManager.FindByEmailAsync(defaultAdmin.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultAdmin, "@Admin123");
                await userManager.AddToRoleAsync(defaultAdmin, Roles.Basic.ToString());
                await userManager.AddToRoleAsync(defaultAdmin, Roles.Moderator.ToString());
                await userManager.AddToRoleAsync(defaultAdmin, Roles.Admin.ToString());
            }
        }

        //Seed Default Moderator
        IdentityUser defaultModerator = new()
        {
            UserName = "moderator",
            Email = "moderator@mail.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };
        if (userManager.Users.All(u => u.Id != defaultModerator.Id))
        {
            IdentityUser? user = await userManager.FindByEmailAsync(defaultModerator.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultModerator, "@Moderator123");
                await userManager.AddToRoleAsync(defaultModerator, Roles.Basic.ToString());
                await userManager.AddToRoleAsync(defaultModerator, Roles.Moderator.ToString());
            }
        }

        //Seed Default User
        IdentityUser defaultUser = new()
        {
            UserName = "user",
            Email = "user@mail.com",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };
        if (userManager.Users.All(u => u.Id != defaultUser.Id))
        {
            IdentityUser? user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "@User123");
                await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
            }
        }
    }
}