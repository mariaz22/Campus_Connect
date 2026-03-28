using CampusConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Infrastructure.Data;


public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>

{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<ApplicationUser> Users { get; set; }

    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<SavedAnnouncement> SavedAnnouncements { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventParticipant> EventParticipants { get; set; }
    public DbSet<SavedEvent> SavedEvents { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<GroupTask> GroupTasks { get; set; }
    public DbSet<CourseMaterial> CourseMaterials { get; set; }
    public DbSet<GroupAnnouncement> GroupAnnouncements { get; set; }
    public DbSet<SavedTask> SavedTasks { get; set; }
    public DbSet<CategorySubscription> CategorySubscriptions { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Building> Buildings { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<RoomBookingRequest> RoomBookingRequests { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<LibraryFolder> LibraryFolders { get; set; } 
    public DbSet<LibraryItem> LibraryItems { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Grade> Grades { get; set; }
    public DbSet<RoomReservation> RoomReservations { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.StudentId)
                .HasMaxLength(50);

            entity.Property(e => e.ProfilePictureUrl)
                .HasMaxLength(500);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(e => e.Email)
                .IsUnique();
        });

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole<int>>().ToTable("Roles");
        builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");

        // 1. Seed Roluri
        var adminRole = new IdentityRole<int> 
        { 
            Id = 1, 
            Name = "Admin", 
            NormalizedName = "ADMIN",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        var userRole = new IdentityRole<int> 
        { 
            Id = 2, 
            Name = "User", 
            NormalizedName = "USER",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        var professorRole = new IdentityRole<int> 
        { 
            Id = 3, 
            Name = "Professor", 
            NormalizedName = "PROFESSOR",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        builder.Entity<IdentityRole<int>>().HasData(adminRole, userRole, professorRole);

        // 2. Creare utilizatori default
        var hasher = new PasswordHasher<ApplicationUser>();

        var admin1 = CreateUser(10, "admin1@unibuc.ro", "Andrei", "Popescu", hasher);
        var admin2 = CreateUser(11, "admin2@unibuc.ro", "Maria", "Ionescu", hasher);
        var user1 = CreateStudent(12, "student1@s.unibuc.ro", "Ion", "Vasilescu", "STD2024001", hasher);
        var user2 = CreateStudent(13, "student2@s.unibuc.ro", "Elena", "Georgescu", "STD2024002", hasher);
        var professor1 = CreateUser(14, "anastasia.ispas@s.unibuc.ro", "Anastasia", "Ispas", hasher);
        var professor2 = CreateUser(15, "irina-maria.istrate@s.unibuc.ro", "Irina-Maria", "Istrate", hasher);

        builder.Entity<ApplicationUser>().HasData(admin1, admin2, user1, user2, professor1, professor2);

        // 3. Atribuire roluri utilizatorilor
        builder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int> { UserId = 10, RoleId = 1 }, // Admin 1
            new IdentityUserRole<int> { UserId = 11, RoleId = 1 }, // Admin 2
            new IdentityUserRole<int> { UserId = 12, RoleId = 2 }, // User 1
            new IdentityUserRole<int> { UserId = 13, RoleId = 2 }, // User 2
            new IdentityUserRole<int> { UserId = 14, RoleId = 3 }, // Professor 1
            new IdentityUserRole<int> { UserId = 15, RoleId = 3 }  // Professor 2
        );

        // 4. Configurare SavedAnnouncement
        builder.Entity<SavedAnnouncement>(entity =>
        {
            entity.HasIndex(sa => new { sa.UserId, sa.AnnouncementId }).IsUnique();
            
            entity.HasOne(sa => sa.User)
                  .WithMany()
                  .HasForeignKey(sa => sa.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sa => sa.Announcement)
                  .WithMany()
                  .HasForeignKey(sa => sa.AnnouncementId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // 5. Configurare Event
        builder.Entity<Event>()
            .HasOne(e => e.Organizer)
            .WithMany() 
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        // 6. Configurare EventParticipant
        builder.Entity<EventParticipant>()
            .HasKey(ep => new { ep.UserId, ep.EventId });

        builder.Entity<EventParticipant>()
            .HasOne(ep => ep.Event)
            .WithMany(e => e.Participants)
            .HasForeignKey(ep => ep.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<EventParticipant>()
            .HasOne(ep => ep.User)
            .WithMany(u => u.EventsJoined)
            .HasForeignKey(ep => ep.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        // 7. Configurare Group
        builder.Entity<Group>(entity =>
        {
            entity.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(g => g.Subject)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(g => g.Description)
                .HasMaxLength(1000);

            entity.HasOne(g => g.Professor)
                .WithMany()
                .HasForeignKey(g => g.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 8. Configurare GroupMember
        builder.Entity<GroupMember>(entity =>
        {
            entity.HasIndex(gm => new { gm.UserId, gm.GroupId }).IsUnique();

            entity.HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(gm => gm.User)
                .WithMany()
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 9. Configurare GroupTask
        builder.Entity<GroupTask>(entity =>
        {
            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(300);

            entity.Property(t => t.Description)
                .HasMaxLength(2000);

            entity.HasOne(t => t.Group)
                .WithMany(g => g.Tasks)
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(t => t.CreatedByProfessor)
                .WithMany()
                .HasForeignKey(t => t.CreatedByProfessorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 10. Configurare SavedTask
        builder.Entity<SavedTask>(entity =>
        {
            entity.HasIndex(st => new { st.UserId, st.TaskId }).IsUnique();

            entity.HasOne(st => st.User)
                .WithMany()
                .HasForeignKey(st => st.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(st => st.Task)
                .WithMany(t => t.SavedByUsers)
                .HasForeignKey(st => st.TaskId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        // 10a. Configurare CourseMaterial
        builder.Entity<CourseMaterial>(entity =>
        {
            entity.Property(cm => cm.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(cm => cm.Description)
                .HasMaxLength(1000);

            entity.Property(cm => cm.FileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(cm => cm.FileUrl)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(cm => cm.FileType)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(cm => cm.Group)
                .WithMany(g => g.CourseMaterials)
                .HasForeignKey(cm => cm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cm => cm.UploadedByProfessor)
                .WithMany()
                .HasForeignKey(cm => cm.UploadedByProfessorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 10a. Configurare GroupAnnouncement
        builder.Entity<GroupAnnouncement>(entity =>
        {
            entity.HasOne(ga => ga.Group)
                .WithMany()
                .HasForeignKey(ga => ga.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ga => ga.Announcement)
                .WithMany()
                .HasForeignKey(ga => ga.AnnouncementId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ga => ga.ForwardedByProfessor)
                .WithMany()
                .HasForeignKey(ga => ga.ForwardedByProfessorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(ga => new { ga.GroupId, ga.AnnouncementId });
        });

        // 11. Configurare SavedEvent
        builder.Entity<SavedEvent>(entity =>
        {
            entity.HasIndex(se => new { se.UserId, se.EventId }).IsUnique();

            entity.HasOne(se => se.User)
                .WithMany()
                .HasForeignKey(se => se.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(se => se.Event)
                .WithMany()
                .HasForeignKey(se => se.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 12. Configurare Building
        builder.Entity<Building>(entity =>
        {
            entity.Property(b => b.Name).IsRequired().HasMaxLength(200);
            entity.Property(b => b.Address).IsRequired().HasMaxLength(500);
            entity.Property(b => b.Description).HasMaxLength(1000);
            entity.Property(b => b.Latitude).IsRequired();
            entity.Property(b => b.Longitude).IsRequired();
            entity.Property(b => b.GeoJsonPolygon).HasColumnType("nvarchar(max)");

            entity.HasIndex(b => b.Name);
        });

        // 13. Configurare Room
        builder.Entity<Room>(entity =>
        {
            entity.Property(r => r.Name).IsRequired().HasMaxLength(100);
            entity.Property(r => r.Floor).HasMaxLength(50);
            entity.Property(r => r.Equipment).HasMaxLength(1000);

            entity.HasOne(r => r.Building)
                .WithMany(b => b.Rooms)
                .HasForeignKey(r => r.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => new { r.BuildingId, r.Name });
        });

        // 14. Configurare Schedule
        builder.Entity<Schedule>(entity =>
        {
            entity.Property(s => s.Title).IsRequired().HasMaxLength(200);
            entity.Property(s => s.Description).HasMaxLength(1000);
            entity.Property(s => s.RecurrencePattern).HasMaxLength(50);

            entity.HasOne(s => s.Room)
                .WithMany(r => r.Schedules)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.CreatedByProfessor)
                .WithMany()
                .HasForeignKey(s => s.CreatedByProfessorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(s => new { s.RoomId, s.StartTime, s.EndTime });
        });

        // 15. Configurare RoomBookingRequest
        builder.Entity<RoomBookingRequest>(entity =>
        {
            entity.Property(r => r.Title).IsRequired().HasMaxLength(200);
            entity.Property(r => r.Description).HasMaxLength(1000);
            entity.Property(r => r.RecurrencePattern).HasMaxLength(50);
            entity.Property(r => r.RejectionReason).HasMaxLength(500);

            entity.HasOne(r => r.Room)
                .WithMany()
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.RequestedByUser)
                .WithMany()
                .HasForeignKey(r => r.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.ReviewedByAdmin)
                .WithMany()
                .HasForeignKey(r => r.ReviewedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => new { r.RoomId, r.StartTime, r.EndTime });
            entity.HasIndex(r => r.RequestedByUserId);
            entity.HasIndex(r => r.Status);
        });

        // 16. Seed Campus Map Data (UniBuc)
        SeedCampusMapData(builder);
    }

    private static void SeedCampusMapData(ModelBuilder builder)
    {
        // Buildings - Real UniBuc Faculties with precise GPS coordinates verified from official sources
        var buildings = new[]
        {
            // Chimie/FAA - Regina Elisabeta 4-12 (same building)
            new Building { Id = 1, Name = "Facultatea de Administrație și Afaceri", Address = "B-dul Regina Elisabeta nr. 4-12, etaj 1, sector 3, București", Latitude = 44.43472, Longitude = 26.10072, Description = "FAA - Sediu în clădirea Chimiei", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Building { Id = 2, Name = "Facultatea de Biologie", Address = "Splaiul Independenței nr. 91-95, sector 5, București, 050095", Latitude = 44.43530, Longitude = 26.06326, Description = "Facultatea de Biologie", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Building { Id = 3, Name = "Facultatea de Chimie", Address = "Bd. Regina Elisabeta nr. 4-12, sector 3, București, 030018", Latitude = 44.43472, Longitude = 26.10072, Description = "Facultatea de Chimie", IsActive = true, CreatedAt = DateTime.UtcNow },
            // Drept - Kogălniceanu (verified coordinates)
            new Building { Id = 4, Name = "Facultatea de Drept", Address = "Bd. Mihail Kogălniceanu nr. 36-46, sector 5, București, 050107", Latitude = 44.435241, Longitude = 26.082077, Description = "Facultatea de Drept", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Building { Id = 5, Name = "Facultatea de Filosofie", Address = "Splaiul Independenței nr. 204, sector 6, București, 060024", Latitude = 44.43471, Longitude = 26.04824, Description = "Facultatea de Filosofie", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Building { Id = 6, Name = "Facultatea de Fizică", Address = "Str. Atomiștilor nr. 405, Măgurele, Ilfov, 077125", Latitude = 44.34834, Longitude = 26.03128, Description = "Facultatea de Fizică - Campus Măgurele", IsActive = true, CreatedAt = DateTime.UtcNow },
            // Geografie - Bălcescu 1
            new Building { Id = 7, Name = "Facultatea de Geografie", Address = "Bd. Nicolae Bălcescu nr. 1, sector 1, București, 010041", Latitude = 44.43654, Longitude = 26.10189, Description = "Facultatea de Geografie", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Building { Id = 8, Name = "Facultatea de Geologie și Geofizică", Address = "Str. Traian Vuia nr. 6, sector 2, București, 020956", Latitude = 44.45167, Longitude = 26.07901, Description = "Facultatea de Geologie și Geofizică", IsActive = true, CreatedAt = DateTime.UtcNow },
            // FMI & Istorie - Academiei 14 (same building)
            new Building { Id = 9, Name = "Facultatea de Istorie", Address = "Str. Academiei nr. 14, București", Latitude = 44.43584, Longitude = 26.09683, Description = "Facultatea de Istorie", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Building { Id = 10, Name = "Facultatea de Jurnalism și Științele Comunicării", Address = "Bd. Iuliu Maniu nr. 1-3, Complex Leu, Corp A, etaj 6, sector 6, București", Latitude = 44.43891, Longitude = 26.04321, Description = "FJSC - Complex Leu", IsActive = true, CreatedAt = DateTime.UtcNow },
            // FLLS & Litere - Edgar Quinet 5-7 (verified coordinates for Litere)
            new Building { Id = 11, Name = "Facultatea de Limbi și Literaturi Străine", Address = "Str. Edgar Quinet nr. 5-7, sector 1, București, 010017", Latitude = 44.43583, Longitude = 26.10081, Description = "FLLS", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Building { Id = 12, Name = "Facultatea de Litere", Address = "Str. Edgar Quinet nr. 5-7, sector 1, București, 010017", Latitude = 44.43583, Longitude = 26.10081, Description = "Facultatea de Litere", IsActive = true, CreatedAt = DateTime.UtcNow },
            // FMI - Academiei 14
            new Building { Id = 13, Name = "Facultatea de Matematică și Informatică", Address = "Str. Academiei nr. 14, sector 1, București, 010014", Latitude = 44.43584, Longitude = 26.09683, Description = "FMI", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Building { Id = 14, Name = "Facultatea de Psihologie și Științele Educației", Address = "Șos. Panduri nr. 90-91, București", Latitude = 44.43221, Longitude = 26.06892, Description = "FPSE", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Building { Id = 15, Name = "Facultatea de Sociologie și Asistență Socială", Address = "Bd. Schitu Măgureanu nr. 9, București", Latitude = 44.43342, Longitude = 26.09421, Description = "SAS", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Building { Id = 16, Name = "Facultatea de Științe Politice", Address = "Calea Plevnei nr. 59, București, 010223", Latitude = 44.44521, Longitude = 26.08392, Description = "FSP", IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        builder.Entity<Building>().HasData(buildings);

        // Rooms - 10 rooms per faculty
        var rooms = new[]
        {
            // FAA (Building 1)
            new Room { Id = 1, Name = "A101", BuildingId = 1, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 2, Name = "A102", BuildingId = 1, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 3, Name = "A103", BuildingId = 1, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 4, Name = "A104", BuildingId = 1, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 5, Name = "A105", BuildingId = 1, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 6, Name = "S201", BuildingId = 1, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 7, Name = "S202", BuildingId = 1, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 8, Name = "S203", BuildingId = 1, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 9, Name = "Lab301", BuildingId = 1, Floor = "Etaj 3", Capacity = 25, Equipment = "Computere", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 10, Name = "Lab302", BuildingId = 1, Floor = "Etaj 3", Capacity = 25, Equipment = "Computere", IsActive = true, CreatedAt = DateTime.UtcNow },

            // Biologie (Building 2)
            new Room { Id = 11, Name = "Bio101", BuildingId = 2, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 12, Name = "Bio102", BuildingId = 2, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 13, Name = "Bio103", BuildingId = 2, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 14, Name = "Bio104", BuildingId = 2, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 15, Name = "Bio105", BuildingId = 2, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 16, Name = "LabBio201", BuildingId = 2, Floor = "Etaj 2", Capacity = 30, Equipment = "Echipament laborator", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 17, Name = "LabBio202", BuildingId = 2, Floor = "Etaj 2", Capacity = 30, Equipment = "Echipament laborator", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 18, Name = "LabBio203", BuildingId = 2, Floor = "Etaj 2", Capacity = 30, Equipment = "Echipament laborator", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 19, Name = "AmfBio1", BuildingId = 2, Floor = "Parter", Capacity = 200, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 20, Name = "AmfBio2", BuildingId = 2, Floor = "Parter", Capacity = 150, Equipment = "Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },

            // Chimie (Building 3)
            new Room { Id = 21, Name = "Ch101", BuildingId = 3, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 22, Name = "Ch102", BuildingId = 3, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 23, Name = "Ch103", BuildingId = 3, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 24, Name = "Ch104", BuildingId = 3, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 25, Name = "Ch105", BuildingId = 3, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 26, Name = "LabCh201", BuildingId = 3, Floor = "Etaj 2", Capacity = 25, Equipment = "Echipament chimie", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 27, Name = "LabCh202", BuildingId = 3, Floor = "Etaj 2", Capacity = 25, Equipment = "Echipament chimie", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 28, Name = "LabCh203", BuildingId = 3, Floor = "Etaj 2", Capacity = 25, Equipment = "Echipament chimie", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 29, Name = "AmfCh1", BuildingId = 3, Floor = "Parter", Capacity = 180, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 30, Name = "AmfCh2", BuildingId = 3, Floor = "Parter", Capacity = 150, Equipment = "Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },

            // Drept (Building 4)
            new Room { Id = 31, Name = "D101", BuildingId = 4, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 32, Name = "D102", BuildingId = 4, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 33, Name = "D103", BuildingId = 4, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 34, Name = "D104", BuildingId = 4, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 35, Name = "D105", BuildingId = 4, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 36, Name = "AmfD1", BuildingId = 4, Floor = "Parter", Capacity = 300, Equipment = "Sistem audio-video complet", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 37, Name = "AmfD2", BuildingId = 4, Floor = "Parter", Capacity = 250, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 38, Name = "SemD201", BuildingId = 4, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 39, Name = "SemD202", BuildingId = 4, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 40, Name = "SemD203", BuildingId = 4, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },

            // Filosofie (Building 5)
            new Room { Id = 41, Name = "Filo101", BuildingId = 5, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 42, Name = "Filo102", BuildingId = 5, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 43, Name = "Filo103", BuildingId = 5, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 44, Name = "Filo104", BuildingId = 5, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 45, Name = "Filo105", BuildingId = 5, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 46, Name = "AmfFilo1", BuildingId = 5, Floor = "Parter", Capacity = 150, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 47, Name = "AmfFilo2", BuildingId = 5, Floor = "Parter", Capacity = 120, Equipment = "Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 48, Name = "SemFilo201", BuildingId = 5, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 49, Name = "SemFilo202", BuildingId = 5, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 50, Name = "SemFilo203", BuildingId = 5, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },

            // Fizică (Building 6)
            new Room { Id = 51, Name = "Fiz101", BuildingId = 6, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 52, Name = "Fiz102", BuildingId = 6, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 53, Name = "Fiz103", BuildingId = 6, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 54, Name = "Fiz104", BuildingId = 6, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 55, Name = "Fiz105", BuildingId = 6, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 56, Name = "LabFiz201", BuildingId = 6, Floor = "Etaj 2", Capacity = 30, Equipment = "Echipament fizică", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 57, Name = "LabFiz202", BuildingId = 6, Floor = "Etaj 2", Capacity = 30, Equipment = "Echipament fizică", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 58, Name = "LabFiz203", BuildingId = 6, Floor = "Etaj 2", Capacity = 30, Equipment = "Echipament fizică", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 59, Name = "AmfFiz1", BuildingId = 6, Floor = "Parter", Capacity = 200, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 60, Name = "AmfFiz2", BuildingId = 6, Floor = "Parter", Capacity = 150, Equipment = "Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },

            // Geografie (Building 7)
            new Room { Id = 61, Name = "Geo101", BuildingId = 7, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 62, Name = "Geo102", BuildingId = 7, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 63, Name = "Geo103", BuildingId = 7, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 64, Name = "Geo104", BuildingId = 7, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 65, Name = "Geo105", BuildingId = 7, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 66, Name = "LabGeo201", BuildingId = 7, Floor = "Etaj 2", Capacity = 25, Equipment = "Hărți, Computere GIS", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 67, Name = "LabGeo202", BuildingId = 7, Floor = "Etaj 2", Capacity = 25, Equipment = "Computere GIS", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 68, Name = "SemGeo203", BuildingId = 7, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 69, Name = "AmfGeo1", BuildingId = 7, Floor = "Parter", Capacity = 180, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 70, Name = "AmfGeo2", BuildingId = 7, Floor = "Parter", Capacity = 120, Equipment = "Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },

            // Geologie și Geofizică (Building 8)
            new Room { Id = 71, Name = "GG101", BuildingId = 8, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 72, Name = "GG102", BuildingId = 8, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 73, Name = "GG103", BuildingId = 8, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 74, Name = "GG104", BuildingId = 8, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 75, Name = "GG105", BuildingId = 8, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 76, Name = "LabGG201", BuildingId = 8, Floor = "Etaj 2", Capacity = 25, Equipment = "Echipament geologic", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 77, Name = "LabGG202", BuildingId = 8, Floor = "Etaj 2", Capacity = 25, Equipment = "Echipament geofizic", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 78, Name = "SemGG203", BuildingId = 8, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 79, Name = "AmfGG1", BuildingId = 8, Floor = "Parter", Capacity = 150, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 80, Name = "AmfGG2", BuildingId = 8, Floor = "Parter", Capacity = 120, Equipment = "Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },

            // Istorie (Building 9)
            new Room { Id = 81, Name = "Ist101", BuildingId = 9, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 82, Name = "Ist102", BuildingId = 9, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 83, Name = "Ist103", BuildingId = 9, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 84, Name = "Ist104", BuildingId = 9, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 85, Name = "Ist105", BuildingId = 9, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 86, Name = "SemIst201", BuildingId = 9, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 87, Name = "SemIst202", BuildingId = 9, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 88, Name = "SemIst203", BuildingId = 9, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 89, Name = "AmfIst1", BuildingId = 9, Floor = "Parter", Capacity = 150, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 90, Name = "AmfIst2", BuildingId = 9, Floor = "Parter", Capacity = 120, Equipment = "Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },

            // FJSC (Building 10)
            new Room { Id = 91, Name = "J101", BuildingId = 10, Floor = "Etaj 6", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 92, Name = "J102", BuildingId = 10, Floor = "Etaj 6", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 93, Name = "J103", BuildingId = 10, Floor = "Etaj 6", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 94, Name = "J104", BuildingId = 10, Floor = "Etaj 6", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 95, Name = "J105", BuildingId = 10, Floor = "Etaj 6", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 96, Name = "LabMedia201", BuildingId = 10, Floor = "Etaj 7", Capacity = 25, Equipment = "Camere, Echipament video", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 97, Name = "LabMedia202", BuildingId = 10, Floor = "Etaj 7", Capacity = 25, Equipment = "Echipament audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 98, Name = "SemPR203", BuildingId = 10, Floor = "Etaj 7", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 99, Name = "AmfJ1", BuildingId = 10, Floor = "Etaj 6", Capacity = 100, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 100, Name = "StudioJ2", BuildingId = 10, Floor = "Etaj 7", Capacity = 20, Equipment = "Studio TV/Radio", IsActive = true, CreatedAt = DateTime.UtcNow },

            // FLLS (Building 11)
            new Room { Id = 101, Name = "LLS101", BuildingId = 11, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 102, Name = "LLS102", BuildingId = 11, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 103, Name = "LLS103", BuildingId = 11, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 104, Name = "LLS104", BuildingId = 11, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 105, Name = "LLS105", BuildingId = 11, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 106, Name = "LabLingv201", BuildingId = 11, Floor = "Etaj 2", Capacity = 25, Equipment = "Echipament limbi străine", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 107, Name = "LabLingv202", BuildingId = 11, Floor = "Etaj 2", Capacity = 25, Equipment = "Computere, Software lingvistic", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 108, Name = "SemLLS203", BuildingId = 11, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 109, Name = "AmfLLS1", BuildingId = 11, Floor = "Parter", Capacity = 150, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 110, Name = "AmfLLS2", BuildingId = 11, Floor = "Parter", Capacity = 120, Equipment = "Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },

            // Litere (Building 12)
            new Room { Id = 111, Name = "Lit101", BuildingId = 12, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 112, Name = "Lit102", BuildingId = 12, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 113, Name = "Lit103", BuildingId = 12, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 114, Name = "Lit104", BuildingId = 12, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 115, Name = "Lit105", BuildingId = 12, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 116, Name = "SemLit201", BuildingId = 12, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 117, Name = "SemLit202", BuildingId = 12, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 118, Name = "SemLit203", BuildingId = 12, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 119, Name = "AmfLit1", BuildingId = 12, Floor = "Parter", Capacity = 200, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 120, Name = "AmfLit2", BuildingId = 12, Floor = "Parter", Capacity = 150, Equipment = "Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },

            // FMI (Building 13)
            new Room { Id = 121, Name = "Amf. Spiru Haret", BuildingId = 13, Floor = "Parter", Capacity = 300, Equipment = "Proiector, Sistem audio premium", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 122, Name = "Amf. Gheorghe Țițeica", BuildingId = 13, Floor = "Etaj 3", Capacity = 250, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 123, Name = "Amf. Simion Stoilow", BuildingId = 13, Floor = "Etaj 1", Capacity = 200, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 124, Name = "Amf. Dimitrie Pompeiu", BuildingId = 13, Floor = "Etaj 2", Capacity = 180, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 125, Name = "Lab FMI 1", BuildingId = 13, Floor = "Etaj 1", Capacity = 30, Equipment = "30 Computere, Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 126, Name = "Lab FMI 2", BuildingId = 13, Floor = "Etaj 1", Capacity = 30, Equipment = "30 Computere, Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 127, Name = "Lab FMI 3", BuildingId = 13, Floor = "Etaj 1", Capacity = 30, Equipment = "30 Computere, Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 128, Name = "S101", BuildingId = 13, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 129, Name = "S102", BuildingId = 13, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 130, Name = "S103", BuildingId = 13, Floor = "Etaj 1", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 161, Name = "S1", BuildingId = 13, Floor = "Parter", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 162, Name = "S3", BuildingId = 13, Floor = "Parter", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 163, Name = "S201", BuildingId = 13, Floor = "Etaj 2", Capacity = 50, IsActive = true, CreatedAt = DateTime.UtcNow },

            // FPSE (Building 14)
            new Room { Id = 131, Name = "Psi101", BuildingId = 14, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 132, Name = "Psi102", BuildingId = 14, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 133, Name = "Psi103", BuildingId = 14, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 134, Name = "Psi104", BuildingId = 14, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 135, Name = "Psi105", BuildingId = 14, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 136, Name = "LabPsi201", BuildingId = 14, Floor = "Etaj 2", Capacity = 20, Equipment = "Echipament psihologie", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 137, Name = "LabPsi202", BuildingId = 14, Floor = "Etaj 2", Capacity = 20, Equipment = "Computere, Software psiho", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 138, Name = "SemEdu203", BuildingId = 14, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 139, Name = "AmfPsi1", BuildingId = 14, Floor = "Parter", Capacity = 150, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 140, Name = "AmfPsi2", BuildingId = 14, Floor = "Parter", Capacity = 120, Equipment = "Proiector", IsActive = true, CreatedAt = DateTime.UtcNow },

            // SAS (Building 15)
            new Room { Id = 141, Name = "SAS101", BuildingId = 15, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 142, Name = "SAS102", BuildingId = 15, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 143, Name = "SAS103", BuildingId = 15, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 144, Name = "SAS104", BuildingId = 15, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 145, Name = "SAS105", BuildingId = 15, Floor = "Etaj 1", Capacity = 40, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 146, Name = "SemSAS201", BuildingId = 15, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 147, Name = "SemSAS202", BuildingId = 15, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 148, Name = "SemSAS203", BuildingId = 15, Floor = "Etaj 2", Capacity = 25, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 149, Name = "AmfSAS1", BuildingId = 15, Floor = "Parter", Capacity = 150, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 150, Name = "LabSAS2", BuildingId = 15, Floor = "Etaj 2", Capacity = 25, Equipment = "Computere, Software SPSS", IsActive = true, CreatedAt = DateTime.UtcNow },

            // FSP (Building 16)
            new Room { Id = 151, Name = "FSP101", BuildingId = 16, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 152, Name = "FSP102", BuildingId = 16, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 153, Name = "FSP103", BuildingId = 16, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 154, Name = "FSP104", BuildingId = 16, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 155, Name = "FSP105", BuildingId = 16, Floor = "Etaj 1", Capacity = 45, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 156, Name = "SemFSP201", BuildingId = 16, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 157, Name = "SemFSP202", BuildingId = 16, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 158, Name = "SemFSP203", BuildingId = 16, Floor = "Etaj 2", Capacity = 30, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 159, Name = "AmfFSP1", BuildingId = 16, Floor = "Parter", Capacity = 180, Equipment = "Proiector, Sistem audio", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Room { Id = 160, Name = "LabFSP2", BuildingId = 16, Floor = "Etaj 2", Capacity = 25, Equipment = "Computere", IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        builder.Entity<Room>().HasData(rooms);

        // Schedules - Sample schedules for demonstration
        var today = DateTime.Today;
        var schedules = new[]
        {
            // FMI - Today's schedules
            new Schedule
            {
                Id = 1,
                Title = "Curs Inginerie Software",
                Description = "Principii de inginerie software și design patterns",
                RoomId = 121, // Amf. Spiru Haret
                StartTime = today.AddHours(10),
                EndTime = today.AddHours(12),
                CreatedByProfessorId = 14,
                RecurrencePattern = "Weekly",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Schedule
            {
                Id = 2,
                Title = "Seminar Baze de Date",
                Description = "Lucru cu SQL și modelare baze de date",
                RoomId = 125, // Lab FMI 1
                StartTime = today.AddHours(14),
                EndTime = today.AddHours(16),
                CreatedByProfessorId = 15,
                RecurrencePattern = "Weekly",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Schedule
            {
                Id = 3,
                Title = "Curs Algoritmi și Structuri de Date",
                Description = "Algoritmi de sortare și căutare",
                RoomId = 122, // Amf. Gheorghe Țițeica
                StartTime = today.AddHours(8),
                EndTime = today.AddHours(10),
                CreatedByProfessorId = 14,
                RecurrencePattern = "Weekly",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },

            // Drept - Today
            new Schedule
            {
                Id = 4,
                Title = "Curs Drept Civil",
                Description = "Dreptul persoanelor și al familiei",
                RoomId = 36, // AmfD1
                StartTime = today.AddHours(12),
                EndTime = today.AddHours(14),
                CreatedByProfessorId = 15,
                RecurrencePattern = "Weekly",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },

            // ========== CURSURI 30-31 IANUARIE 2026 ==========
            // FAA (Building 1) - Rooms 1, 2
            new Schedule { Id = 5, Title = "Curs Administrație Publică", Description = "Principii de administrație publică", RoomId = 1, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 6, Title = "Seminar Politici Publice", Description = "Analiza politicilor publice", RoomId = 2, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 7, Title = "Curs Management Public", Description = "Fundamente de management public", RoomId = 1, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 8, Title = "Seminar Drept Administrativ", Description = "Aspecte practice", RoomId = 2, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // Biologie (Building 2) - Rooms 11, 12
            new Schedule { Id = 9, Title = "Curs Biologie Celulară", Description = "Structura celulei eucariote", RoomId = 11, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 10, Title = "Laborator Microbiologie", Description = "Tehnici de cultivare", RoomId = 12, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 11, Title = "Curs Genetică", Description = "Genetica moleculară", RoomId = 11, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 12, Title = "Seminar Ecologie", Description = "Ecosisteme terestre", RoomId = 12, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // Chimie (Building 3) - Rooms 21, 22
            new Schedule { Id = 13, Title = "Curs Chimie Organică", Description = "Reacții de sinteză", RoomId = 21, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 14, Title = "Laborator Chimie Analitică", Description = "Metode spectroscopice", RoomId = 22, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 15, Title = "Curs Biochimie", Description = "Metabolismul celular", RoomId = 21, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 16, Title = "Seminar Chimie Fizică", Description = "Termodinamică", RoomId = 22, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // Drept (Building 4) - Rooms 31, 32
            new Schedule { Id = 17, Title = "Curs Drept Penal", Description = "Infracțiuni și pedepse", RoomId = 31, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 18, Title = "Seminar Drept Constituțional", Description = "Constituția României", RoomId = 32, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 19, Title = "Curs Drept European", Description = "Instituții UE", RoomId = 31, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 20, Title = "Seminar Drept Comercial", Description = "Societăți comerciale", RoomId = 32, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // Filosofie (Building 5) - Rooms 41, 42
            new Schedule { Id = 21, Title = "Curs Istoria Filosofiei", Description = "Filosofia antică greacă", RoomId = 41, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 22, Title = "Seminar Etică", Description = "Teorii etice contemporane", RoomId = 42, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 23, Title = "Curs Logică", Description = "Logica formală", RoomId = 41, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 24, Title = "Seminar Epistemologie", Description = "Teoria cunoașterii", RoomId = 42, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // Fizică (Building 6) - Rooms 51, 52
            new Schedule { Id = 25, Title = "Curs Mecanică Cuantică", Description = "Principii fundamentale", RoomId = 51, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 26, Title = "Laborator Optică", Description = "Experimente de optică", RoomId = 52, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 27, Title = "Curs Termodinamică", Description = "Legile termodinamicii", RoomId = 51, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 28, Title = "Seminar Electrodinamică", Description = "Câmpuri electromagnetice", RoomId = 52, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // Geografie (Building 7) - Rooms 61, 62
            new Schedule { Id = 29, Title = "Curs Geografie Fizică", Description = "Relieful terestru", RoomId = 61, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 30, Title = "Laborator Cartografie", Description = "Tehnici cartografice", RoomId = 62, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 31, Title = "Curs Climatologie", Description = "Sistemul climatic global", RoomId = 61, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 32, Title = "Seminar GIS", Description = "Sisteme informaționale geografice", RoomId = 62, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // Geologie (Building 8) - Rooms 71, 72
            new Schedule { Id = 33, Title = "Curs Mineralogie", Description = "Clasificarea mineralelor", RoomId = 71, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 34, Title = "Laborator Petrografie", Description = "Analiza rocilor", RoomId = 72, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 35, Title = "Curs Geofizică", Description = "Metode geofizice", RoomId = 71, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 36, Title = "Seminar Paleontologie", Description = "Fosile și evoluție", RoomId = 72, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // Istorie (Building 9) - Rooms 81, 82
            new Schedule { Id = 37, Title = "Curs Istorie Antică", Description = "Civilizații antice", RoomId = 81, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 38, Title = "Seminar Istorie Medievală", Description = "Europa medievală", RoomId = 82, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 39, Title = "Curs Istorie Modernă", Description = "Revoluții și națiuni", RoomId = 81, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 40, Title = "Seminar Istorie Contemporană", Description = "Secolul XX", RoomId = 82, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // FJSC (Building 10) - Rooms 91, 92
            new Schedule { Id = 41, Title = "Curs Teoria Comunicării", Description = "Modele de comunicare", RoomId = 91, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 42, Title = "Laborator Jurnalism Digital", Description = "Tehnici multimedia", RoomId = 92, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 43, Title = "Curs Relații Publice", Description = "Strategii de PR", RoomId = 91, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 44, Title = "Seminar Publicitate", Description = "Campanii publicitare", RoomId = 92, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // FLLS (Building 11) - Rooms 101, 102
            new Schedule { Id = 45, Title = "Curs Limba Engleză", Description = "Gramatică avansată", RoomId = 101, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 46, Title = "Seminar Limba Franceză", Description = "Conversație", RoomId = 102, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 47, Title = "Curs Limba Germană", Description = "Literatură germană", RoomId = 101, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 48, Title = "Seminar Limba Spaniolă", Description = "Cultură hispanică", RoomId = 102, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // Litere (Building 12) - Rooms 111, 112
            new Schedule { Id = 49, Title = "Curs Literatură Română", Description = "Clasici români", RoomId = 111, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 50, Title = "Seminar Lingvistică", Description = "Analiză lingvistică", RoomId = 112, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 51, Title = "Curs Literatură Universală", Description = "Curente literare", RoomId = 111, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 52, Title = "Seminar Teoria Literaturii", Description = "Analiză text", RoomId = 112, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // FMI (Building 13) - Rooms 121, 122
            new Schedule { Id = 53, Title = "Curs Programare Orientată Obiect", Description = "Design patterns", RoomId = 121, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 54, Title = "Laborator Rețele de Calculatoare", Description = "Protocoale de rețea", RoomId = 122, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 55, Title = "Curs Inteligență Artificială", Description = "Machine Learning", RoomId = 121, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 56, Title = "Seminar Baze de Date Avansate", Description = "SQL și NoSQL", RoomId = 122, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // FPSE (Building 14) - Rooms 131, 132
            new Schedule { Id = 57, Title = "Curs Psihologie Generală", Description = "Procese cognitive", RoomId = 131, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 58, Title = "Seminar Psihologie Socială", Description = "Dinamica grupurilor", RoomId = 132, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 59, Title = "Curs Pedagogie", Description = "Teorii educaționale", RoomId = 131, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 60, Title = "Seminar Psihologia Dezvoltării", Description = "Etape de dezvoltare", RoomId = 132, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // SAS (Building 15) - Rooms 141, 142
            new Schedule { Id = 61, Title = "Curs Sociologie Generală", Description = "Teorii sociologice", RoomId = 141, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 62, Title = "Seminar Asistență Socială", Description = "Intervenție socială", RoomId = 142, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 63, Title = "Curs Metodologia Cercetării", Description = "Metode de cercetare", RoomId = 141, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 64, Title = "Seminar Politici Sociale", Description = "Sistemul de protecție", RoomId = 142, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },

            // FSP (Building 16) - Rooms 151, 152
            new Schedule { Id = 65, Title = "Curs Științe Politice", Description = "Teorii politice", RoomId = 151, StartTime = new DateTime(2026, 1, 30, 8, 0, 0), EndTime = new DateTime(2026, 1, 30, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 66, Title = "Seminar Relații Internaționale", Description = "Diplomație", RoomId = 152, StartTime = new DateTime(2026, 1, 30, 10, 0, 0), EndTime = new DateTime(2026, 1, 30, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 67, Title = "Curs Studii Europene", Description = "Integrare europeană", RoomId = 151, StartTime = new DateTime(2026, 1, 31, 8, 0, 0), EndTime = new DateTime(2026, 1, 31, 10, 0, 0), CreatedByProfessorId = 14, IsActive = true, CreatedAt = DateTime.UtcNow },
            new Schedule { Id = 68, Title = "Seminar Politici Comparate", Description = "Sisteme politice", RoomId = 152, StartTime = new DateTime(2026, 1, 31, 10, 0, 0), EndTime = new DateTime(2026, 1, 31, 12, 0, 0), CreatedByProfessorId = 15, IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        builder.Entity<Schedule>().HasData(schedules);
        // 17. Configurare Achievement
        builder.Entity<Achievement>(entity =>
        {
            entity.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(a => a.Icon)
                .IsRequired()
                .HasMaxLength(100);
        });

        // 18. Configurare UserAchievement
        builder.Entity<UserAchievement>(entity =>
        {
            entity.HasIndex(ua => new { ua.UserId, ua.AchievementId }).IsUnique();

            entity.HasOne(ua => ua.User)
                .WithMany(u => u.UserAchievements)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ua => ua.Achievement)
                .WithMany(a => a.UserAchievements)
                .HasForeignKey(ua => ua.AchievementId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 19. Configurare UserActivity
        builder.Entity<UserActivity>(entity =>
        {
            entity.Property(a => a.ActivityType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(a => a.EntityName)
                .HasMaxLength(300);

            entity.Property(a => a.Description)
                .HasMaxLength(500);

            entity.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => new { a.UserId, a.CreatedAt });
        });

        // 20. Seed Achievements
        builder.Entity<Achievement>().HasData(
            new Achievement
            {
                Id = 1,
                Title = "First Steps",
                Description = "Complete your first task",
                Icon = "🎯",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Achievement
            {
                Id = 2,
                Title = "Task Master",
                Description = "Complete 5 tasks",
                Icon = "⭐",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Achievement
            {
                Id = 3,
                Title = "Task Legend",
                Description = "Complete 10 tasks",
                Icon = "🏆",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Achievement
            {
                Id = 4,
                Title = "Team Player",
                Description = "Join your first group",
                Icon = "👥",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Achievement
            {
                Id = 5,
                Title = "Social Butterfly",
                Description = "Attend your first event",
                Icon = "🦋",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );

        // Subject configuration
        builder.Entity<Subject>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
            entity.Property(s => s.Code).IsRequired().HasMaxLength(50);
            entity.Property(s => s.Description).HasMaxLength(1000);
            
            entity.HasOne(s => s.Professor)
                .WithMany()
                .HasForeignKey(s => s.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(s => s.Code).IsUnique();
            entity.HasIndex(s => s.ProfessorId);
        });

        // Grade configuration
        builder.Entity<Grade>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Value).HasColumnType("decimal(4,2)");
            entity.Property(g => g.Comments).HasMaxLength(500);
            
            entity.HasOne(g => g.Subject)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(g => g.Student)
                .WithMany()
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(g => g.CreatedByProfessor)
                .WithMany()
                .HasForeignKey(g => g.CreatedByProfessorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(g => new { g.SubjectId, g.StudentId, g.CreatedAt });
        });
    }

    private static ApplicationUser CreateUser(int id, string email, string firstName, string lastName, PasswordHasher<ApplicationUser> hasher)
    {
        var user = new ApplicationUser
        {
            Id = id,
            UserName = email,
            NormalizedUserName = email.ToUpper(),
            Email = email,
            NormalizedEmail = email.ToUpper(),
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = hasher.HashPassword(user, "Parola@123");
        return user;
    }

    private static ApplicationUser CreateStudent(int id, string email, string firstName, string lastName, string studentId, PasswordHasher<ApplicationUser> hasher)
    {
        var user = new ApplicationUser
        {
            Id = id,
            UserName = email,
            NormalizedUserName = email.ToUpper(),
            Email = email,
            NormalizedEmail = email.ToUpper(),
            FirstName = firstName,
            LastName = lastName,
            StudentId = studentId,
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = hasher.HashPassword(user, "Parola@123");
        return user;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    base.OnConfiguring(optionsBuilder);
    // Ignoră eroarea de model pending pentru a permite actualizarea bazei
    optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
}
}
