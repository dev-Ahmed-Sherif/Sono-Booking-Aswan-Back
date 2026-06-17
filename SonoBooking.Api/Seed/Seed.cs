using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Infrastructure.Context;
using SonoBooking.Infrastructure.DataInitializer;

namespace SonoBooking.Api.Seed
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
        private const string LeaderUserEmail = "leader@sonobooking.com";
        private const string ReceptionStaffUserEmail = "reception@sonobooking.com";
        private const string SuperAdminSeedPassword = "(as1+me2)";
        private const string SeedUserPassword = "12345";

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

                await EnsureRoleAsync(roleManager, RoleNames.Leader, "قائد", now);
                await EnsureRoleAsync(roleManager, RoleNames.ReceptionStaff, "موظف استقبال", now);

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
                    var createResult = await userManager.CreateAsync(user, SuperAdminSeedPassword);
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

                await EnsureSeedUserAsync(
                    userManager,
                    LeaderUserEmail,
                    "قائد",
                    RoleNames.Leader,
                    now);

                await EnsureSeedUserAsync(
                    userManager,
                    ReceptionStaffUserEmail,
                    "موظف استقبال",
                    RoleNames.ReceptionStaff,
                    now);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Identity seed (role/user) skipped or failed");
            }
        }

        private static async Task EnsureRoleAsync(
            RoleManager<Role> roleManager,
            string roleName,
            string nameAr,
            DateTime now)
        {
            if (await roleManager.FindByNameAsync(roleName) != null)
                return;

            var role = new Role
            {
                Name = roleName,
                NormalizedName = roleName.ToUpperInvariant(),
                NameAr = nameAr,
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
                Log.Information("Seeded role: {RoleName}", roleName);
            else
                Log.Warning("Failed to seed role {RoleName}: {Errors}", roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        private static async Task EnsureSeedUserAsync(
            UserManager<User> userManager,
            string email,
            string fullName,
            string roleName,
            DateTime now)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = fullName,
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

                var createResult = await userManager.CreateAsync(user, SeedUserPassword);
                if (!createResult.Succeeded)
                {
                    Log.Warning("Failed to seed user {Email}: {Errors}", email, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    return;
                }

                Log.Information("Seeded user: {Email}", email);
            }

            if (!await userManager.IsInRoleAsync(user, roleName))
            {
                await userManager.AddToRoleAsync(user, roleName);
                Log.Information("Assigned role {RoleName} to user: {Email}", roleName, email);
            }
        }

        /// <summary>
        /// Seed lookup data from json files on startup.
        /// </summary>
        public static async Task SeedLookupsAsync(IHost host)
        {
            try
            {
                await using var scope = host.Services.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SonoBookingDbContext>();
                var dataInitializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();

                if (!await dbContext.Set<Domain.Entities.Lookups.Governorate>().AnyAsync())
                {
                    var governorates = dataInitializer.SeedGovernoratesAsync();
                    await dbContext.Set<Domain.Entities.Lookups.Governorate>().AddRangeAsync(governorates);
                }

                if (!await dbContext.Set<Domain.Entities.Lookups.City>().AnyAsync())
                {
                    var cities = dataInitializer.SeedCitiesAsync();
                    await dbContext.Set<Domain.Entities.Lookups.City>().AddRangeAsync(cities);
                }

                if (!await dbContext.Relationships.AnyAsync())
                {
                    var relationships = dataInitializer.SeedRelationshipsAsync();
                    await dbContext.Relationships.AddRangeAsync(relationships);
                }

                if (!await dbContext.Set<Domain.Entities.Lookups.ApartmentType>().AnyAsync())
                {
                    var apartmentTypes = dataInitializer.SeedApartmentTypesAsync();
                    await dbContext.Set<Domain.Entities.Lookups.ApartmentType>().AddRangeAsync(apartmentTypes);
                }

                if (!await dbContext.Set<Domain.Entities.Lookups.RoomType>().AnyAsync())
                {
                    var roomTypes = dataInitializer.SeedRoomTypesAsync();
                    await dbContext.Set<Domain.Entities.Lookups.RoomType>().AddRangeAsync(roomTypes);
                }

                if (!await dbContext.Set<Domain.Entities.Lookups.RequestType>().AnyAsync())
                {
                    var requestTypes = dataInitializer.SeedRequestTypesAsync();
                    await dbContext.Set<Domain.Entities.Lookups.RequestType>().AddRangeAsync(requestTypes);
                }

                if (!await dbContext.Roles.AnyAsync())
                {
                    var roles = dataInitializer.SeedRolesAsync();
                    await dbContext.Roles.AddRangeAsync(roles);
                }

                var seedEmployees = dataInitializer.SeedEmployeesAsync().ToList();
                if (seedEmployees.Count > 0)
                {
                    var existingNationalIds = await dbContext.Employees
                        .Where(e => !e.IsDeleted)
                        .Select(e => e.NationalId)
                        .ToListAsync();

                    var employeesToAdd = seedEmployees
                        .Where(e => !string.IsNullOrWhiteSpace(e.NationalId))
                        .Select(e =>
                        {
                            e.NationalId = e.NationalId.Trim();
                            return e;
                        })
                        .Where(e => !existingNationalIds.Contains(e.NationalId))
                        .ToList();

                    if (employeesToAdd.Count > 0)
                        await dbContext.Employees.AddRangeAsync(employeesToAdd);
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Lookup seed skipped or failed");
            }
        }
    }
}
