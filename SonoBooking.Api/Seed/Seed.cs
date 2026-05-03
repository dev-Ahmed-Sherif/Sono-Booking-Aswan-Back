using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SonoBooking.Domain.Entities.Identity;
using SonoTracker.Infrastructure.Context;
using SonoTracker.Infrastructure.DataInitializer;

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
        private const string SuperUserEmail = "super@sonotracker.com";
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

        /// <summary>
        /// Seed nationalities from Nationalities.json into the database on startup.
        /// </summary>
        public static async Task SeedNationalitiesAsync(IHost host)
        {
            try
            {
                await using var scope = host.Services.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<SonoTrackerDbContext>();
                var initializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();

                var nationalities = initializer.SeedNationalitiesAsync().ToList();
                if (nationalities.Count == 0) return;

                var existingIds = await db.Nationalities.Select(x => x.Id).ToListAsync();
                var toAdd = nationalities.Where(n => !existingIds.Contains(n.Id)).ToList();
                if (toAdd.Count == 0) return;

                db.Nationalities.AddRange(toAdd);
                await db.SaveChangesAsync();
                Log.Information("Seeded {Count} nationalities from Nationalities.json", toAdd.Count);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Nationalities seed skipped or failed");
            }
        }

        /// <summary>
        /// Seed accident types from AccidentTypes.json into the database on startup.
        /// </summary>
        public static async Task SeedAccidentTypesAsync(IHost host)
        {
            try
            {
                await using var scope = host.Services.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<SonoTrackerDbContext>();
                var initializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();

                var accidentTypes = initializer.SeedAccidentTypesAsync().ToList();
                if (accidentTypes.Count == 0) return;

                var existingIds = await db.AccidentTypes.Select(x => x.Id).ToListAsync();
                var toAdd = accidentTypes.Where(a => !existingIds.Contains(a.Id)).ToList();
                if (toAdd.Count == 0) return;

                db.AccidentTypes.AddRange(toAdd);
                await db.SaveChangesAsync();
                Log.Information("Seeded {Count} accident types from AccidentTypes.json", toAdd.Count);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Accident types seed skipped or failed");
            }
        }

        /// <summary>
        /// Seed governorates from Governorates.json into the database on startup.
        /// </summary>
        public static async Task SeedGovernoratesAsync(IHost host)
        {
            try
            {
                await using var scope = host.Services.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<SonoTrackerDbContext>();
                var initializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();

                var governorates = initializer.SeedGovernoratesAsync().ToList();
                if (governorates.Count == 0) return;

                var existingIds = await db.Governorates.Select(x => x.Id).ToListAsync();
                var toAdd = governorates.Where(g => !existingIds.Contains(g.Id)).ToList();
                if (toAdd.Count == 0) return;

                db.Governorates.AddRange(toAdd);
                await db.SaveChangesAsync();
                Log.Information("Seeded {Count} governorates from Governorates.json", toAdd.Count);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Governorates seed skipped or failed");
            }
        }

        /// <summary>
        /// Seed cities from Cities.json into the database on startup.
        /// </summary>
        public static async Task SeedCitiesAsync(IHost host)
        {
            try
            {
                await using var scope = host.Services.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<SonoTrackerDbContext>();
                var initializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();

                var cities = initializer.SeedCitiesAsync().ToList();
                if (cities.Count == 0) return;

                var existingIds = await db.Cities.Select(x => x.Id).ToListAsync();
                var toAdd = cities.Where(c => !existingIds.Contains(c.Id)).ToList();
                if (toAdd.Count == 0) return;

                db.Cities.AddRange(toAdd);
                await db.SaveChangesAsync();
                Log.Information("Seeded {Count} cities from Cities.json", toAdd.Count);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Cities seed skipped or failed");
            }
        }
    }
}
