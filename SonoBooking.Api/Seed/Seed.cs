using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SonoBooking.Application.Services.Housing.Notifications;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Constants.BusinessNotification;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.BusinessNotification;
using SonoBooking.Domain.Entities.Housing;
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
        private const string LeaderSeedLeaderId = "019f0e10-3a9b-7a5c-9c9b-5b44b1a30001";
        private const string ReceptionStaffUserEmail = "reception@sonobooking.com";
        private const string OwnerUserEmail = "owner@sonobooking.com";
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

                await EnsureSeedUserAsync(
                    userManager,
                    OwnerUserEmail,
                    "صاحب الطلب",
                    RoleNames.User,
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

                if (!await dbContext.AllowedDayBeforeReservations.AnyAsync())
                {
                    var allowedDayBeforeReservations = dataInitializer.SeedAllowedDayBeforeReservationsAsync();
                    await dbContext.AllowedDayBeforeReservations.AddRangeAsync(allowedDayBeforeReservations);
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

                var seedNotificationGroups = dataInitializer.SeedNotificationGroupsAsync().ToList();
                if (seedNotificationGroups.Count > 0)
                {
                    var existingGroupCodes = await dbContext.Set<NotificationGroup>()
                        .Where(g => !g.IsDeleted)
                        .Select(g => g.Code)
                        .ToListAsync();

                    var groupsToAdd = seedNotificationGroups
                        .Where(g => !string.IsNullOrWhiteSpace(g.Code))
                        .Where(g => !existingGroupCodes.Contains(g.Code))
                        .ToList();

                    if (groupsToAdd.Count > 0)
                        await dbContext.Set<NotificationGroup>().AddRangeAsync(groupsToAdd);
                }

                var seedLeaders = dataInitializer.SeedLeadersAsync().ToList();
                if (seedLeaders.Count > 0)
                {
                    var existingLeaderIds = await dbContext.Leaders
                        .Where(l => !l.IsDeleted)
                        .Select(l => l.Id)
                        .ToListAsync();

                    var leadersToAdd = seedLeaders
                        .Where(l => !string.IsNullOrWhiteSpace(l.Id))
                        .Where(l => !existingLeaderIds.Contains(l.Id))
                        .ToList();

                    if (leadersToAdd.Count > 0)
                        await dbContext.Leaders.AddRangeAsync(leadersToAdd);
                }

                await dbContext.SaveChangesAsync();
                await BackfillRequestToLeaderAsync(dbContext);

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                await EnsureLeaderUserLeaderIdAsync(userManager);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Lookup seed skipped or failed");
            }
        }

        private static async Task BackfillRequestToLeaderAsync(SonoBookingDbContext dbContext)
        {
            try
            {
                string? defaultLeaderId = await dbContext.Leaders
                    .Where(l => !l.IsDeleted && l.IsActive)
                    .OrderBy(l => l.CreatedAt)
                    .Select(l => l.Id)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrWhiteSpace(defaultLeaderId))
                    return;

                await dbContext.Database.ExecuteSqlInterpolatedAsync(
                    $@"UPDATE [booking].[Requests]
                       SET [RequestToId] = {defaultLeaderId}
                       WHERE [RequestToId] IS NULL OR [RequestToId] = ''");
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "RequestToId backfill skipped");
            }
        }

        private static async Task EnsureLeaderUserLeaderIdAsync(UserManager<User> userManager)
        {
            try
            {
                User? user = await userManager.FindByEmailAsync(LeaderUserEmail);
                if (user == null)
                    return;

                if (string.Equals(user.LeaderId, LeaderSeedLeaderId, StringComparison.Ordinal))
                    return;

                user.LeaderId = LeaderSeedLeaderId;
                user.ModifiedAt = DateTime.UtcNow;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                    Log.Information("Assigned LeaderId {LeaderId} to seed user: {Email}", LeaderSeedLeaderId, LeaderUserEmail);
                else
                    Log.Warning(
                        "Failed to assign LeaderId to seed user {Email}: {Errors}",
                        LeaderUserEmail,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Leader user LeaderId seed skipped or failed");
            }
        }

        /// <summary>
        /// Seed sample bell notifications for local testing (Development only, idempotent).
        /// </summary>
        public static async Task SeedTestNotificationsAsync(IHost host)
        {
            try
            {
                await using var scope = host.Services.CreateAsyncScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<SonoBookingDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                const string seedMarkerNotificationId = "019f0e20-3a9b-7a5c-9c9b-5b44b1a10001";
                if (await dbContext.Notifications.AnyAsync(n => n.Id == seedMarkerNotificationId))
                    return;

                User? owner = await userManager.FindByEmailAsync(OwnerUserEmail);
                User? leader = await userManager.FindByEmailAsync(LeaderUserEmail);
                User? reception = await userManager.FindByEmailAsync(ReceptionStaffUserEmail);

                if (owner == null || leader == null || reception == null)
                {
                    Log.Warning("Test notification seed skipped: required seed users were not found");
                    return;
                }

                var now = DateTime.UtcNow;
                const string ownerLeaderGroupId = "019f0e10-3a9b-7a5c-9c9b-5b44b1a10001";
                const string leaderReceptionGroupId = "019f0e10-3a9b-7a5c-9c9b-5b44b1a10002";

                const string requestRef1 = "019f0e30-3a9b-7a5c-9c9b-5b44b1a20001";
                const string requestRef2 = "019f0e30-3a9b-7a5c-9c9b-5b44b1a20002";
                const string reservationRef1 = "019f0e30-3a9b-7a5c-9c9b-5b44b1a20003";
                const string reservationRef2 = "019f0e30-3a9b-7a5c-9c9b-5b44b1a20004";

                const string requestNumber1 = "REQ-2026-001";
                const string requestNumber2 = "REQ-2026-002";
                var requestDate = now.AddDays(-2);
                var checkIn = DateOnly.FromDateTime(now.AddDays(3));
                var checkOut = DateOnly.FromDateTime(now.AddDays(7));

                var notifications = new List<Notification>
                {
                    CreateSeedNotification(
                        "019f0e20-3a9b-7a5c-9c9b-5b44b1a10001",
                        HousingNotificationMessages.NewRequest(requestNumber1, owner.FullName ?? "صاحب الطلب", requestDate),
                        NotificationTypes.Request,
                        requestRef1,
                        owner.Id!,
                        leader.Id!,
                        ownerLeaderGroupId,
                        isRead: false,
                        now.AddDays(-2)),

                    CreateSeedNotification(
                        "019f0e20-3a9b-7a5c-9c9b-5b44b1a10002",
                        HousingNotificationMessages.NewRequest(requestNumber2, owner.FullName ?? "صاحب الطلب", requestDate.AddHours(3)),
                        NotificationTypes.Request,
                        requestRef2,
                        owner.Id!,
                        leader.Id!,
                        ownerLeaderGroupId,
                        isRead: true,
                        now.AddDays(-1)),

                    CreateSeedNotification(
                        "019f0e20-3a9b-7a5c-9c9b-5b44b1a10003",
                        HousingNotificationMessages.RequestApproved(requestNumber1, leader.FullName ?? "القائد"),
                        NotificationTypes.Request,
                        requestRef1,
                        leader.Id!,
                        owner.Id!,
                        ownerLeaderGroupId,
                        isRead: false,
                        now.AddHours(-6)),

                    CreateSeedNotification(
                        "019f0e20-3a9b-7a5c-9c9b-5b44b1a10004",
                        HousingNotificationMessages.RequestRejected(requestNumber2),
                        NotificationTypes.Request,
                        requestRef2,
                        leader.Id!,
                        owner.Id!,
                        ownerLeaderGroupId,
                        isRead: true,
                        now.AddDays(-1).AddHours(2)),

                    CreateSeedNotification(
                        "019f0e20-3a9b-7a5c-9c9b-5b44b1a10005",
                        HousingNotificationMessages.NewReservationForReception(requestNumber1, checkIn, checkOut),
                        NotificationTypes.Reservation,
                        reservationRef1,
                        leader.Id!,
                        reception.Id!,
                        leaderReceptionGroupId,
                        isRead: false,
                        now.AddHours(-3)),

                    CreateSeedNotification(
                        "019f0e20-3a9b-7a5c-9c9b-5b44b1a10006",
                        HousingNotificationMessages.NewReservationForReception(requestNumber2, checkIn.AddDays(1), checkOut.AddDays(1)),
                        NotificationTypes.Reservation,
                        reservationRef2,
                        leader.Id!,
                        reception.Id!,
                        leaderReceptionGroupId,
                        isRead: true,
                        now.AddDays(-1)),

                    CreateSeedNotification(
                        "019f0e20-3a9b-7a5c-9c9b-5b44b1a10007",
                        HousingNotificationMessages.ReservationStatusUpdated(requestNumber1, ReservationStatus.Reserved),
                        NotificationTypes.Reservation,
                        reservationRef1,
                        reception.Id!,
                        owner.Id!,
                        null,
                        isRead: false,
                        now.AddHours(-1)),

                    CreateSeedNotification(
                        "019f0e20-3a9b-7a5c-9c9b-5b44b1a10008",
                        HousingNotificationMessages.ReservationStatusUpdated(requestNumber1, ReservationStatus.Completed),
                        NotificationTypes.Reservation,
                        reservationRef1,
                        reception.Id!,
                        owner.Id!,
                        null,
                        isRead: true,
                        now.AddMinutes(-30)),
                };

                await dbContext.Notifications.AddRangeAsync(notifications);
                await dbContext.SaveChangesAsync();
                Log.Information("Seeded {Count} test bell notifications", notifications.Count);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Test notification seed skipped or failed");
            }
        }

        private static Notification CreateSeedNotification(
            string id,
            string content,
            string type,
            string referenceId,
            string senderId,
            string receiverId,
            string? notificationGroupId,
            bool isRead,
            DateTime createdAt)
        {
            return new Notification
            {
                Id = id,
                Content = content,
                Type = type,
                ReferenceId = referenceId,
                SenderId = senderId,
                ReceiverId = receiverId,
                NotificationGroupId = notificationGroupId,
                IsRead = isRead,
                CreatedAt = createdAt,
                ModifiedAt = createdAt,
                CreatedById = SystemActor,
                CreatedBy = SystemActor,
                ModifiedById = SystemActor,
                ModifiedBy = SystemActor,
                IsDeleted = false
            };
        }
    }
}
