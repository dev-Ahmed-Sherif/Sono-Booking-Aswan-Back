using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SonoTracker.Domain.Entities.Tracker;
using SonoTracker.Domain.Entities.Lookups;
using SonoTracker.Domain.Entities.Identity;
using SonoTracker.Domain.Entities.Attachments;
using SonoTracker.Domain.Entities.TrackerNotification;

namespace SonoTracker.Infrastructure.Context
{
    public partial class SonoTrackerDbContext(
            DbContextOptions<SonoTrackerDbContext> options)
        : IdentityDbContext<User, Role, string>(options)
    {

        #region Floating Unit

        public virtual DbSet<FloatingUnit> FloatingUnits { get; set; }
        public virtual DbSet<FloatingUnitOrganization> FloatingUnitOrganizations { get; set; }
        public virtual DbSet<FloatingUnitStaff> FloatingUnitStaffs { get; set; }

        #endregion

        #region Organization

        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<OrganizationStaff> OrganizationStaffs { get; set; }

        #endregion

        #region Trip

        public virtual DbSet<TripInformation> TripInformations { get; set; }
        public virtual DbSet<TripGeo> TripGeos { get; set; }
        public virtual DbSet<TripMarina> TripMarinas { get; set; }
        public virtual DbSet<TripNationality> TripNationalities { get; set; }
        public virtual DbSet<TripPassengerAttachment> TripPassengerAttachments { get; set; }

        #endregion

        #region Marina

        public virtual DbSet<TouristMarina> TouristMarinas { get; set; }
        public virtual DbSet<TouristMarinaOrganization> TouristMarinaOrganizations { get; set; }
        public virtual DbSet<TouristMarinaLicenseApplication> TouristMarinaLicenseApplications { get; set; }

        #endregion

        #region Inspection, Maintenance & Accident

        public virtual DbSet<Inspection> Inspections { get; set; }
        public virtual DbSet<InspectionClause> InspectionClauses { get; set; }
        public virtual DbSet<InspectionFloatingUnitClause> InspectionFloatingUnitClauses { get; set; }
        public virtual DbSet<InspectionAttachment> InspectionAttachments { get; set; }
        public virtual DbSet<Maintenance> Maintenances { get; set; }
        public virtual DbSet<MaintenanceAttachment> MaintenanceAttachments { get; set; }
        public virtual DbSet<Accident> Accidents { get; set; }
        public virtual DbSet<AccidentOrganization> AccidentOrganizations { get; set; }
        public virtual DbSet<AccidentAttachment> AccidentAttachments { get; set; }

        #endregion

        #region Identity & Auth

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        #endregion

        #region Lookups – Core

        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<AccidentType> AccidentTypes { get; set; }
        public virtual DbSet<InspectionType> InspectionTypes { get; set; }
        public virtual DbSet<MaintenanceType> MaintenanceTypes { get; set; }
        public virtual DbSet<Nationality> Nationalities { get; set; }
        public virtual DbSet<OrganizationCategory> OrganizationCategories { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<UnitType> UnitTypes { get; set; }

        #endregion

        #region Lookups – Geography

        public virtual DbSet<Governorate> Governorates { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Town> Towns { get; set; }
        public virtual DbSet<GeoPoint> GeoPoints { get; set; }

        #endregion

        #region Messaging & Notifications

        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<MessagingGroup> MessagingGroups { get; set; }
        public virtual DbSet<NotificationGroup> NotificationGroups { get; set; }

        #endregion

        #region Model & Relations

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            ConfigureFloatingUnitRelations(modelBuilder);
            ConfigureOrganizationRelations(modelBuilder);
            ConfigureTripRelations(modelBuilder);
            ConfigureMarinaRelations(modelBuilder);
            ConfigureInspectionMaintenanceAccidentRelations(modelBuilder);
            ConfigureIdentityAndNotificationRelations(modelBuilder);
            ConfigureLookupRelations(modelBuilder);
        }

        private static void ConfigureFloatingUnitRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FloatingUnit>()
                .Property(f => f.LicenseNumber)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<FloatingUnit>()
                .Property(f => f.Length)
                .IsRequired();

            modelBuilder.Entity<FloatingUnit>()
                .Property(f => f.Width)
                .IsRequired();

            modelBuilder.Entity<FloatingUnit>()
                .Property(f => f.ManufactureYear)
                .HasColumnType("date");

            modelBuilder.Entity<FloatingUnit>()
                .Property(f => f.LastMaintenanceDate)
                .HasColumnType("date");

            modelBuilder.Entity<FloatingUnit>()
                .Property(f => f.NextMaintenanceDate)
                .HasColumnType("date");

            modelBuilder.Entity<FloatingUnit>()
                .Property(f => f.ImageUrl)
                .IsRequired()
                .HasMaxLength(250);

            modelBuilder.Entity<FloatingUnit>()
                .Property(f => f.IsAccepted)
                .IsRequired();

            modelBuilder.Entity<FloatingUnit>()
                .Property(f => f.UnitTypeId)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<FloatingUnit>()
                .HasOne(f => f.UnitType)
                .WithMany(u => u.FloatingUnits)
                .HasForeignKey(f => f.UnitTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FloatingUnit>()
                .HasOne(f => f.Governorate)
                .WithMany()
                .HasForeignKey(f => f.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FloatingUnitOrganization>()
                .HasOne(fo => fo.FloatingUnit)
                .WithMany(f => f.FloatingUnitOrganizations)
                .HasForeignKey(fo => fo.FloatingUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FloatingUnitOrganization>()
                .HasOne(fo => fo.Organization)
                .WithMany(o => o.FloatingUnitOrganizations)
                .HasForeignKey(fo => fo.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FloatingUnitStaff>()
                .HasOne(s => s.FloatingUnit)
                .WithMany(f => f.FloatingUnitStaffs)
                .HasForeignKey(s => s.FloatingUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FloatingUnitStaff>()
                .HasOne(s => s.Governorate)
                .WithMany()
                .HasForeignKey(s => s.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureOrganizationRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organization>()
                .HasOne(o => o.OrganizationCategory)
                .WithMany(c => c.Organizations)
                .HasForeignKey(o => o.OrganizationCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Organization>()
                .HasOne(o => o.Nationality)
                .WithMany(n => n.Organizations)
                .HasForeignKey(o => o.NationalityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Organization>()
                .HasOne(o => o.Governorate)
                .WithMany()
                .HasForeignKey(o => o.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationStaff>()
                .HasOne(s => s.Organization)
                .WithMany(o => o.OrganizationStaffs)
                .HasForeignKey(s => s.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Accident.OrganizationId was removed; Accident→Organization is now via AccidentOrganization junction.
            modelBuilder.Entity<Organization>()
                .Ignore(o => o.Accidents);

            modelBuilder.Entity<OrganizationStaff>()
                .Property(s => s.NationalId)
                .IsRequired()
                .HasMaxLength(14);

            modelBuilder.Entity<TouristMarinaOrganization>()
                .HasOne(mo => mo.Organization)
                .WithMany(o => o.TouristMarinaOrganizations)
                .HasForeignKey(mo => mo.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureTripRelations(ModelBuilder modelBuilder)
        {
            // Keep DB table names consistent with the entity/type names.
            // This avoids drift when earlier migrations mapped different table names.
            modelBuilder.Entity<Attachment>()
                .ToTable("Attachments");

            modelBuilder.Entity<TripNationality>()
                .ToTable("TripNationalities");

            modelBuilder.Entity<TripMarina>()
                .ToTable("TripMarinas");

            modelBuilder.Entity<TripInformation>()
                .HasOne(t => t.FloatingUnit)
                .WithMany(f => f.TripInformations)
                .HasForeignKey(t => t.FloatingUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TripInformation>()
                .HasOne(t => t.Route)
                .WithMany(r => r.TripInformations)
                .HasForeignKey(t => t.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TripInformation>()
                .HasOne(t => t.Governorate)
                .WithMany()
                .HasForeignKey(t => t.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TripGeo>()
                .HasOne(tg => tg.GeoPoint)
                .WithMany()
                .HasForeignKey(tg => tg.GeoPointId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TripNationality>()
                .HasOne(nt => nt.TripInformation)
                .WithMany(t => t.NationalityTrips)
                .HasForeignKey(nt => nt.TripInformationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TripMarina>()
                .HasOne(mt => mt.TripInformation)
                .WithMany(t => t.TripMarinas)
                .HasForeignKey(mt => mt.TripInformationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TripMarina>()
                .HasOne(mt => mt.TouristMarina)
                .WithMany()
                .HasForeignKey(mt => mt.TouristMarinaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure DB table matches entity name.
            modelBuilder.Entity<TripPassengerAttachment>()
                .ToTable("TripPassengerAttachments")
                .HasOne(a => a.TripInformation)
                .WithMany(t => t.TripPassengerAttachments)
                .HasForeignKey(a => a.TripInformationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TripGeo>()
                .HasOne(tg => tg.TripInformation)
                .WithMany()
                .HasForeignKey(tg => tg.TripInformationId)
                .OnDelete(DeleteBehavior.Restrict);

        }

        private static void ConfigureMarinaRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TouristMarinaOrganization>()
                .ToTable("TouristMarinaOrganizations");

            modelBuilder.Entity<TouristMarina>()
                .Property(tm => tm.MarinaAddress)
                .HasMaxLength(250);

            modelBuilder.Entity<TouristMarina>()
                .Property(tm => tm.ImageUrl)
                .HasMaxLength(250);

            modelBuilder.Entity<TouristMarina>()
                .Property(tm => tm.Note)
                .HasMaxLength(250);

            modelBuilder.Entity<TouristMarina>()
                .Property(tm => tm.NorthSide)
                .HasMaxLength(50);

            modelBuilder.Entity<TouristMarina>()
                .Property(tm => tm.SouthSide)
                .HasMaxLength(50);

            modelBuilder.Entity<TouristMarina>()
                .HasOne(tm => tm.City)
                .WithMany()
                .HasForeignKey(tm => tm.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TouristMarina>()
                .HasOne(tm => tm.GeoPoint)
                .WithMany(g => g.TouristMarinas)
                .HasForeignKey(tm => tm.GeoPointId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TouristMarina>()
                .HasOne(tm => tm.Governorate)
                .WithMany()
                .HasForeignKey(tm => tm.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TouristMarinaOrganization>()
                .HasOne(mo => mo.TouristMarina)
                .WithMany(tm => tm.TouristMarinaOwners)
                .HasForeignKey(mo => mo.TouristMarinaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TouristMarinaLicenseApplication>()
                .ToTable("TouristMarinaLicenseApplications");

            modelBuilder.Entity<TouristMarinaLicenseApplication>()
                .HasOne(la => la.FromOrganization)
                .WithMany(o => o.TouristMarinaLicenseApplication)
                .HasForeignKey(la => la.FromOrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TouristMarinaLicenseApplication>()
                .HasOne(la => la.ToOrganization)
                .WithMany()
                .HasForeignKey(la => la.ToOrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TouristMarinaLicenseApplication>()
                .HasOne(la => la.Governorate)
                .WithMany()
                .HasForeignKey(la => la.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureInspectionMaintenanceAccidentRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inspection>()
                .Property(i => i.InspectionDate)
                .HasColumnType("date");

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.InspectionType)
                .WithMany(t => t.Inspections)
                .HasForeignKey(i => i.InspectionTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.FloatingUnit)
                .WithMany(f => f.Inspections)
                .HasForeignKey(i => i.FloatingUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.Organization)
                .WithMany(o => o.Inspections)
                .HasForeignKey(i => i.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.Governorate)
                .WithMany()
                .HasForeignKey(i => i.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionClause>()
                .HasOne(c => c.InspectionType)
                .WithMany()
                .HasForeignKey(c => c.InspectionTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionClause>()
                .HasOne(c => c.Parent)
                .WithMany()
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionClause>()
                .HasOne(c => c.Governorate)
                .WithMany()
                .HasForeignKey(c => c.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionFloatingUnitClause>()
                .HasOne(ic => ic.Inspection)
                .WithMany(i => i.InspectionFloatingUnitClauses)
                .HasForeignKey(ic => ic.InspectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionFloatingUnitClause>()
                .HasOne(ic => ic.InspectionClause)
                .WithMany(c => c.InspectionFloatingUnitClauses)
                .HasForeignKey(ic => ic.InspectionClauseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Maintenance>()
                .Property(m => m.MaintenanceDate)
                .HasColumnType("date");

            modelBuilder.Entity<Maintenance>()
                .Property(m => m.NextMaintenanceDate)
                .HasColumnType("date");

            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.MaintenanceType)
                .WithMany(mt => mt.Maintenances)
                .HasForeignKey(m => m.MaintenanceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.FloatingUnit)
                .WithMany(f => f.Maintenances)
                .HasForeignKey(m => m.FloatingUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.Governorate)
                .WithMany()
                .HasForeignKey(m => m.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Accident>()
                .Property(a => a.AccidentDate)
                .HasColumnType("date");

            modelBuilder.Entity<Accident>()
                .Property(a => a.ResponseDate)
                .HasColumnType("date");

            modelBuilder.Entity<Accident>()
                .HasOne(a => a.AccidentType)
                .WithMany(t => t.Accidents)
                .HasForeignKey(a => a.AccidentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Accident>()
                .HasOne(a => a.FloatingUnit)
                .WithMany(f => f.Accidents)
                .HasForeignKey(a => a.FloatingUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Accident>()
                .HasOne(a => a.Governorate)
                .WithMany()
                .HasForeignKey(a => a.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccidentOrganization>()
                .HasOne(ao => ao.Accident)
                .WithMany(a => a.AccidentOrganizations)
                .HasForeignKey(ao => ao.AccidentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccidentOrganization>()
                .HasOne(ao => ao.Organization)
                .WithMany()
                .HasForeignKey(ao => ao.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccidentAttachment>()
                .HasOne(aa => aa.Accident)
                .WithMany(a => a.AccidentAttachments)
                .HasForeignKey(aa => aa.AccidentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccidentAttachment>()
                .HasOne(aa => aa.Attachment)
                .WithMany()
                .HasForeignKey(aa => aa.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionAttachment>()
                .HasOne(ia => ia.Inspection)
                .WithMany(i => i.InspectionAttachments)
                .HasForeignKey(ia => ia.InspectionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionAttachment>()
                .HasOne(ia => ia.Attachment)
                .WithMany()
                .HasForeignKey(ia => ia.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceAttachment>()
                .HasOne(ma => ma.Maintenance)
                .WithMany(m => m.MaintenanceAttachments)
                .HasForeignKey(ma => ma.MaintenanceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceAttachment>()
                .HasOne(ma => ma.Attachment)
                .WithMany()
                .HasForeignKey(ma => ma.AttachmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureLookupRelations(ModelBuilder modelBuilder)
        {
            // Governorate -> City -> Town hierarchy
            modelBuilder.Entity<City>()
                .HasOne(c => c.Governorate)
                .WithMany(g => g.Cities)
                .HasForeignKey(c => c.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Town>()
                .HasOne(t => t.City)
                .WithMany(c => c.Towns)
                .HasForeignKey(t => t.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GeoPoint>()
                .HasOne(g => g.Governorate)
                .WithMany()
                .HasForeignKey(g => g.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccidentType>()
                .HasOne(x => x.Governorate)
                .WithMany()
                .HasForeignKey(x => x.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionType>()
                .HasOne(x => x.Governorate)
                .WithMany()
                .HasForeignKey(x => x.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MaintenanceType>()
                .HasOne(x => x.Governorate)
                .WithMany()
                .HasForeignKey(x => x.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationCategory>()
                .HasOne(x => x.Governorate)
                .WithMany()
                .HasForeignKey(x => x.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Route>()
                .HasOne(x => x.Governorate)
                .WithMany()
                .HasForeignKey(x => x.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnitType>()
                .HasOne(x => x.Governorate)
                .WithMany()
                .HasForeignKey(x => x.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Nationality relationships (Organization configured in ConfigureOrganizationRelations)

            modelBuilder.Entity<Nationality>()
                .HasMany(n => n.FloatingUnitStaffs)
                .WithOne(s => s.Nationality)
                .HasForeignKey(s => s.NationalityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Nationality>()
                .HasMany(n => n.TripNationalities)
                .WithOne(nt => nt.Nationality)
                .HasForeignKey(nt => nt.NationalityId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private static void ConfigureIdentityAndNotificationRelations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>()
                        .Ignore(r => r.ConcurrencyStamp);

            modelBuilder.Entity<User>()
                        .Ignore(r => r.ConcurrencyStamp);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Governorate)
                .WithMany()
                .HasForeignKey(u => u.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            // User -> RefreshTokens
            modelBuilder.Entity<User>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notifications
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Sender)
                .WithMany()
                .HasForeignKey(n => n.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Receiver)
                .WithMany()
                .HasForeignKey(n => n.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.NotificationGroup)
                .WithMany(g => g.Notifications)
                .HasForeignKey(n => n.NotificationGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Governorate)
                .WithMany()
                .HasForeignKey(n => n.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Messages
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.MessagingGroup)
                .WithMany(g => g.Messages)
                .HasForeignKey(m => m.MessagingGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Governorate)
                .WithMany()
                .HasForeignKey(m => m.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MessagingGroup>()
                .HasOne(g => g.Governorate)
                .WithMany()
                .HasForeignKey(g => g.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NotificationGroup>()
                .HasOne(g => g.Governorate)
                .WithMany()
                .HasForeignKey(g => g.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        #endregion
    }
}
