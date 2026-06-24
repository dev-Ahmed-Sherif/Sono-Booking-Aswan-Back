using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.BusinessNotification;
using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Domain.Entities.Lookups;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Infrastructure.Context;
public partial class SonoBookingDbContext(
            DbContextOptions<SonoBookingDbContext> options)
        : IdentityDbContext<User, Role, string>(options)
{
    public virtual DbSet<AllowedDayBeforeReservation> AllowedDayBeforeReservations { get; set; }

    public virtual DbSet<Apartment> Apartments { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Approval> Approvals { get; set; }

    public virtual DbSet<Bed> Beds { get; set; }

    public virtual DbSet<Companion> Companions { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Extension> Extensions { get; set; }

    public virtual DbSet<Leader> Leaders { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessagingGroup> MessagingGroups { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Relationship> Relationships { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<RequestParticipant> RequestParticipants { get; set; }

    public virtual DbSet<RequestUnit> RequestUnits { get; set; }

    public virtual DbSet<RequestAttach> RequestAttaches { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<UnitImage> UnitImages { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>()
                    .Ignore(r => r.ConcurrencyStamp);

        modelBuilder.Entity<User>()
                    .Ignore(r => r.ConcurrencyStamp);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Employee)
            .WithMany()
            .HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(14000);

            entity.HasOne(d => d.Sender).WithMany()
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Receiver).WithMany()
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(14000);

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("system");

            entity.Property(e => e.ReferenceId)
                .HasMaxLength(50);

            entity.HasOne(d => d.Sender).WithMany()
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Receiver).WithMany()
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MessagingGroup>(entity =>
        {
            entity.ToTable("MessagingGroups");

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(280);
        });

        modelBuilder.Entity<Apartment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Apartmen__3214EC0721231588");

            entity.ToTable("Apartments", "units");

            entity.HasIndex(e => e.Gender, "IDX_Apartments_Gender");

            entity.HasIndex(e => e.Status, "IDX_Apartments_Status");

            entity.Property(e => e.AllocationType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10)
                .HasDefaultValue(AllocationType.Fixed);
            entity.Property(e => e.ApartmentNumber)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.ApartmentTypeId)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.BuildingNumber)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.CityId)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.DetailedAddress)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.Floor)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.Gender)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10)
                .HasDefaultValue(Gender.Male);
            entity.Property(e => e.GovernorateId)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(UnitStatus.Available);
            entity.Property(e => e.Street)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.ApartmentType).WithMany()
                .HasForeignKey(d => d.ApartmentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.City).WithMany()
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Governorate).WithMany()
                .HasForeignKey(d => d.GovernorateId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Approval>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Approval__3214EC0734F8A54C");

            entity.ToTable("Approvals", "booking");

            entity.HasIndex(e => e.RequestId, "UX_Approvals_Request").IsUnique();

            entity.Property(e => e.Decision)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.DecisionDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.Leader).WithMany(p => p.Approvals)
                .HasForeignKey(d => d.LeaderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Approvals_Leader");

            entity.HasOne(d => d.Request).WithOne(p => p.Approval)
                .HasForeignKey<Approval>(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Approvals_Request");
        });

        modelBuilder.Entity<Bed>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Beds__3214EC079A1B9CD4");

            entity.ToTable("Beds", "units");

            entity.HasIndex(e => e.RoomId, "IDX_Beds_RoomId");

            entity.HasIndex(e => e.Status, "IDX_Beds_Status");

            entity.HasIndex(e => new { e.RoomId, e.BedNumber }, "UX_Bed_Number_Per_Room").IsUnique();

            entity.Property(e => e.BedNumber)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.Dimensions)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(UnitStatus.Available);

            entity.HasOne(d => d.Room).WithMany(p => p.Beds)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_Beds_Rooms");
        });

        modelBuilder.Entity<Companion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Companio__3214EC0728FDF18A");

            entity.ToTable("Companions");

            entity.HasIndex(e => e.DocumentNumber, "UX_Companions_DocumentNumber").IsUnique();

            entity.Property(e => e.DocumentImageUrl)
                .IsRequired()
                .HasMaxLength(140);
            entity.Property(e => e.DocumentNumber)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.DocumentType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Gender)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);

            entity.HasOne(d => d.Relationship).WithMany()
                .HasForeignKey(d => d.RelationshipId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Companions_Relationship");

            entity.HasOne(d => d.User).WithMany(p => p.Companions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Companions_Users");
        });

        modelBuilder.Entity<AllowedDayBeforeReservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AllowedDayBeforeReservation");

            entity.ToTable("AllowedDayBeforeReservation");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.NameAr)
                .HasMaxLength(280)
                .IsRequired();

            entity.Property(e => e.NameEn)
                .HasMaxLength(280);

            entity.Property(e => e.Code)
                .HasMaxLength(35)
                .IsRequired();

            entity.Property(e => e.NumofDays)
                .IsRequired();
        });

        modelBuilder.Entity<Relationship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Relationships");

            entity.ToTable("Relationships");

            entity.Property(e => e.Id)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.NameAr)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(e => e.NameEn)
                .HasMaxLength(200)
                .IsRequired();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Employees");

            entity.ToTable("Employees");

            entity.HasIndex(e => e.NationalId, "UX_Employees_NationalId").IsUnique();

            entity.Property(e => e.Id)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.NationalId)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Leader>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Leaders__3214EC070064AA06");

            entity.ToTable("Leaders", "booking");

            entity.HasIndex(e => e.IsActive, "IDX_Leaders_IsActive");

            entity.HasIndex(e => new { e.FullName, e.Position }, "UX_Leaders_Name_Position").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Position)
                .IsRequired()
                .HasMaxLength(150);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC0797960E75");

            entity.ToTable("Payments", "booking");

            entity.HasIndex(e => e.ReservationId, "UX_Payments_ReservationId").IsUnique();

            entity.HasIndex(e => e.PaymentStatus, "IDX_Payments_Status");

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.Property(e => e.PaymentStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(PaymentStatus.Pending);
            entity.Property(e => e.TransactionReference).HasMaxLength(100);

            entity.HasOne(d => d.Reservation)
                .WithOne(p => p.Payment)
                .HasForeignKey<Payment>(d => d.ReservationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Payments_Reservation");
        });

        modelBuilder.Entity<Extension>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Extensions", "booking");

            entity.HasIndex(e => e.ReservationId, "IDX_Extensions_ReservationId");

            entity.HasIndex(e => e.UserId, "IDX_Extensions_UserId");

            entity.HasIndex(e => e.Status, "IDX_Extensions_Status");

            entity.Property(e => e.ApprovedAt).HasColumnType("datetime");

            entity.Property(e => e.RejectionReason).HasMaxLength(500);

            entity.Property(e => e.EndDate)
                .HasColumnType("date");

            entity.Property(e => e.ExtensionAllocationType)
                .HasColumnType("int");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(Status.Pending);

            entity.HasOne(d => d.Reservation).WithMany()
                .HasForeignKey(d => d.ReservationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Extensions_Reservation");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Extensions_User");

            entity.HasOne(d => d.ApprovedBy).WithMany()
                .HasForeignKey(d => d.ApprovedById)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Extensions_ApprovedBy");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Requests__3214EC07B2D76077");

            entity.ToTable("Requests", "booking");

            entity.HasIndex(e => e.RequestNumber, "UX_Request_Number").IsUnique();

            entity.Property(e => e.ApprovedAt).HasColumnType("datetime");

            entity.Property(e => e.RejectionReason).HasMaxLength(500);

            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.RequestNumber)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(e => e.StartDate)
                .HasColumnType("date");

            entity.Property(e => e.EndDate)
                .HasColumnType("date");

            entity.Property(e => e.RequestAllocationType)
                .HasColumnType("int");

            entity.Property(e => e.RequestCatagory)
                .IsRequired()
                .HasColumnType("int");

            entity.Property(e => e.PreviousRequestId)
                .HasMaxLength(50);

            entity.HasIndex(e => e.PreviousRequestId, "IDX_Requests_PreviousRequestId");

            entity.Property(e => e.RequestTypeId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(Status.Pending);

            entity.HasOne(d => d.User).WithMany(p => p.Requests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ApprovedBy).WithMany()
                .HasForeignKey(d => d.ApprovedById)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(d => d.RequestType).WithMany()
                .HasForeignKey(d => d.RequestTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PreviousRequest).WithMany()
                .HasForeignKey(d => d.PreviousRequestId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Requests_PreviousRequest");
        });

        modelBuilder.Entity<RequestParticipant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RequestP__3214EC07565E9385");

            entity.ToTable("RequestParticipants", "booking");

            entity.HasIndex(e => e.RequestId, "IDX_RequestParticipants_RequestId");

            entity.HasOne(d => d.Companion).WithMany(p => p.RequestParticipants)
                .HasForeignKey(d => d.CompanionId)
                .HasConstraintName("FK_RequestParticipants_Companion");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestParticipants)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_RequestParticipants_Request");

        });

        modelBuilder.Entity<RequestUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RequestU__3214EC07A33A8588");

            entity.ToTable("RequestUnits", "booking");

            entity.HasIndex(e => e.RequestId, "IDX_RequestUnits_RequestId");

            entity.HasOne(d => d.Apartment).WithMany(p => p.RequestUnits)
                .HasForeignKey(d => d.ApartmentId)
                .HasConstraintName("FK_RequestUnits_Apartment");

            entity.HasOne(d => d.Bed).WithMany(p => p.RequestUnits)
                .HasForeignKey(d => d.BedId)
                .HasConstraintName("FK_RequestUnits_Bed");

            entity.HasOne(d => d.Request).WithMany(p => p.RequestUnits)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_RequestUnits_Request");

            entity.HasOne(d => d.Room).WithMany(p => p.RequestUnits)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_RequestUnits_Room");
        });

        modelBuilder.Entity<RequestAttach>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RequestA__3214EC07A1B2C3D4");

            entity.ToTable("RequestAttaches", "booking");

            entity.HasIndex(e => e.RequestId, "IDX_RequestAttaches_RequestId");

            entity.HasIndex(e => e.AttachmentId, "IDX_RequestAttaches_AttachmentId");

            entity.HasIndex(e => new { e.RequestId, e.AttachmentId }, "UX_RequestAttaches_Request_Attachment")
                .IsUnique();

            entity.Property(e => e.AttachmentId)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.RequestId)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Request).WithMany(p => p.RequestAttaches)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_RequestAttaches_Request");

            entity.HasOne(d => d.Attachment).WithMany()
                .HasForeignKey(d => d.AttachmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RequestAttaches_Attachment");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reservat__3214EC07909291A0");

            entity.ToTable("Reservations", "booking");

            entity.HasIndex(e => new { e.StartDate, e.EndDate }, "IDX_Reservations_Dates");

            entity.HasIndex(e => e.RequestId, "IDX_Reservations_RequestId");

            entity.HasIndex(e => e.Status, "IDX_Reservations_Status");

            entity.Property(e => e.ActualCheckOutDate).HasColumnType("datetime");
            entity.Property(e => e.CheckInDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(ReservationStatus.Reserved);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CancelationReason)
                .HasMaxLength(700)
                .IsRequired(false);

            entity.HasOne(d => d.Request).WithOne(p => p.Reservation)
                .HasForeignKey<Reservation>(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservations_Request");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Rooms__3214EC07E6B4CB77");

            entity.ToTable("Rooms", "units");

            entity.HasIndex(e => e.ApartmentId, "IDX_Rooms_ApartmentId");

            entity.HasIndex(e => e.Status, "IDX_Rooms_Status");

            entity.HasIndex(e => new { e.ApartmentId, e.RoomNumber }, "UX_Room_Number_Per_Apartment").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RoomNumber)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.RoomTypeId)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(UnitStatus.Available);

            entity.HasOne(d => d.Apartment).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.ApartmentId)
                .HasConstraintName("FK_Rooms_Apartments");

            entity.HasOne(d => d.RoomType).WithMany()
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UnitImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UnitImag__3214EC074E4C76EA");

            entity.ToTable("UnitImages", "units");

            entity.HasIndex(e => e.ApartmentId, "IDX_UnitImages_ApartmentId");

            entity.HasIndex(e => e.BedId, "IDX_UnitImages_BedId");

            entity.HasIndex(e => e.RoomId, "IDX_UnitImages_RoomId");

            entity.HasIndex(e => new { e.ApartmentId, e.RoomId, e.BedId, e.AttachmentId }, "UX_UnitImages_Unique").IsUnique();

            entity.HasOne(d => d.Apartment).WithMany(p => p.UnitImages)
                .HasForeignKey(d => d.ApartmentId)
                .HasConstraintName("FK_UnitImages_Apartment");

            entity.HasOne(d => d.Bed).WithMany(p => p.UnitImages)
                .HasForeignKey(d => d.BedId)
                .HasConstraintName("FK_UnitImages_Bed");

            entity.HasOne(d => d.Room).WithMany(p => p.UnitImages)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_UnitImages_Room");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attachme__3214EC074E4C76EA");
            
            entity.ToTable("Attachments");

            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(250);
            entity.Property(e => e.Extension)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Url)
                .IsRequired()
                .HasMaxLength(250);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
