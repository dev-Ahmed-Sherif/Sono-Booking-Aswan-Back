using Microsoft.AspNetCore.Identity;
using Serilog;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Identity;

namespace SonoTracker.Api.Seed
{
    /// <summary>
    /// Runs all database seed operations on startup.
    /// </summary>
    public static class DatabaseSeed
    {
        private const string SystemActor = "System";
        private const string SuperAdminRoleName = "SuperAdmin";
        private const string UserRoleName = "User";
        private const string SuperUserEmail = "super@sonobooking.com";
        private const string SuperUserPassword = "(as1+me2)";

        /// <summary>
        /// Seed base role (SuperAdmin) and super user on startup.
        /// </summary>
        public static async Task SeedIdentityAsync(IHost host)
        {
            try
            {
                await using var scope = host.Services.CreateAsyncScope();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                var now = DateTime.UtcNow;

                // Ensure SuperAdmin role exists
                Role? role = await roleManager.FindByNameAsync(SuperAdminRoleName);
                if (role == null)
                {
                    role = new Role
                    {
                        Name = SuperAdminRoleName,
                        NormalizedName = SuperAdminRoleName.ToUpperInvariant(),
                        NameAr = "مدير النظام",
                        CreatedAt = now,
                        ModifiedAt = now,
                        CreatedById = SystemActor,
                        CreatedBy = SystemActor,
                        ModifiedById = SystemActor,
                        ModifiedBy = SystemActor,
                        IsDeleted = false
                    };
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                        Log.Information("Seeded role: {RoleName}", SuperAdminRoleName);
                    else
                        Log.Warning("Failed to seed role SuperAdmin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                // Ensure User role exists (NameEn: User, NameAr: مستخدم)
                Role? userRole = await roleManager.FindByNameAsync(UserRoleName);
                if (userRole == null)
                {
                    userRole = new Role
                    {
                        Name = UserRoleName,
                        NormalizedName = UserRoleName.ToUpperInvariant(),
                        NameAr = "مستخدم",
                        CreatedAt = now,
                        ModifiedAt = now,
                        CreatedById = SystemActor,
                        CreatedBy = SystemActor,
                        ModifiedById = SystemActor,
                        ModifiedBy = SystemActor,
                        IsDeleted = false
                    };
                    var userRoleResult = await roleManager.CreateAsync(userRole);
                    if (userRoleResult.Succeeded)
                        Log.Information("Seeded role: {RoleName}", UserRoleName);
                    else
                        Log.Warning("Failed to seed role User: {Errors}", string.Join(", ", userRoleResult.Errors.Select(e => e.Description)));
                }

                // Ensure super user exists
                User? user = await userManager.FindByEmailAsync(SuperUserEmail);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = SuperUserEmail,
                        Email = SuperUserEmail,
                        EmailConfirmed = true,
                        FullName = "مدير النظام",
                        Gender = Gender.Male,
                        BirthDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        CreatedAt = now,
                        ModifiedAt = now,
                        CreatedById = SystemActor,
                        CreatedBy = SystemActor,
                        ModifiedById = SystemActor,
                        ModifiedBy = SystemActor,
                        IsDeleted = false
                    };
                    var createResult = await userManager.CreateAsync(user, SuperUserPassword);
                    if (createResult.Succeeded)
                    {
                        if (role != null && !await userManager.IsInRoleAsync(user, SuperAdminRoleName))
                        {
                            await userManager.AddToRoleAsync(user, SuperAdminRoleName);
                            Log.Information("Seeded super user and assigned role SuperAdmin: {Email}", SuperUserEmail);
                        }
                        else
                            Log.Information("Seeded super user: {Email}", SuperUserEmail);
                    }
                    else
                        Log.Warning("Failed to seed super user: {Errors}", string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Identity seed (role/user) skipped or failed");
            }
        }
    }
}
