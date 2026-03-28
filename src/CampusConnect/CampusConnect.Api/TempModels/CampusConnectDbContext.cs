using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Api.TempModels;

public partial class CampusConnectDbContext : DbContext
{
    public CampusConnectDbContext(DbContextOptions<CampusConnectDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Achievement> Achievements { get; set; }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<Building> Buildings { get; set; }

    public virtual DbSet<CategorySubscription> CategorySubscriptions { get; set; }

    public virtual DbSet<CourseMaterial> CourseMaterials { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventParticipant> EventParticipants { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupAnnouncement> GroupAnnouncements { get; set; }

    public virtual DbSet<GroupMember> GroupMembers { get; set; }

    public virtual DbSet<GroupTask> GroupTasks { get; set; }

    public virtual DbSet<LibraryFolder> LibraryFolders { get; set; }

    public virtual DbSet<LibraryItem> LibraryItems { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomBookingRequest> RoomBookingRequests { get; set; }

    public virtual DbSet<SavedAnnouncement> SavedAnnouncements { get; set; }

    public virtual DbSet<SavedEvent> SavedEvents { get; set; }

    public virtual DbSet<SavedTask> SavedTasks { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAchievement> UserAchievements { get; set; }

    public virtual DbSet<UserActivity> UserActivities { get; set; }

    public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Building>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Buildings_Name");

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<CourseMaterial>(entity =>
        {
            entity.HasIndex(e => e.GroupId, "IX_CourseMaterials_GroupId");

            entity.HasIndex(e => e.UploadedByProfessorId, "IX_CourseMaterials_UploadedByProfessorId");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FileType).HasMaxLength(50);
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Group).WithMany(p => p.CourseMaterials).HasForeignKey(d => d.GroupId);

            entity.HasOne(d => d.UploadedByProfessor).WithMany(p => p.CourseMaterials)
                .HasForeignKey(d => d.UploadedByProfessorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasIndex(e => e.OrganizerId, "IX_Events_OrganizerId");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events).HasForeignKey(d => d.OrganizerId);
        });

        modelBuilder.Entity<EventParticipant>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.EventId });

            entity.HasIndex(e => e.EventId, "IX_EventParticipants_EventId");

            entity.HasOne(d => d.Event).WithMany(p => p.EventParticipants).HasForeignKey(d => d.EventId);

            entity.HasOne(d => d.User).WithMany(p => p.EventParticipants)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasIndex(e => e.CreatedByProfessorId, "IX_Grades_CreatedByProfessorId");

            entity.HasIndex(e => e.StudentId, "IX_Grades_StudentId");

            entity.HasIndex(e => new { e.SubjectId, e.StudentId, e.CreatedAt }, "IX_Grades_SubjectId_StudentId_CreatedAt");

            entity.Property(e => e.Comments).HasMaxLength(500);
            entity.Property(e => e.Value).HasColumnType("decimal(4, 2)");

            entity.HasOne(d => d.CreatedByProfessor).WithMany(p => p.GradeCreatedByProfessors)
                .HasForeignKey(d => d.CreatedByProfessorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Student).WithMany(p => p.GradeStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Subject).WithMany(p => p.Grades).HasForeignKey(d => d.SubjectId);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasIndex(e => e.ProfessorId, "IX_Groups_ProfessorId");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Subject).HasMaxLength(200);

            entity.HasOne(d => d.Professor).WithMany(p => p.Groups)
                .HasForeignKey(d => d.ProfessorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GroupAnnouncement>(entity =>
        {
            entity.HasIndex(e => e.AnnouncementId, "IX_GroupAnnouncements_AnnouncementId");

            entity.HasIndex(e => e.ForwardedByProfessorId, "IX_GroupAnnouncements_ForwardedByProfessorId");

            entity.HasIndex(e => new { e.GroupId, e.AnnouncementId }, "IX_GroupAnnouncements_GroupId_AnnouncementId");

            entity.HasOne(d => d.Announcement).WithMany(p => p.GroupAnnouncements).HasForeignKey(d => d.AnnouncementId);

            entity.HasOne(d => d.ForwardedByProfessor).WithMany(p => p.GroupAnnouncements)
                .HasForeignKey(d => d.ForwardedByProfessorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Group).WithMany(p => p.GroupAnnouncements).HasForeignKey(d => d.GroupId);
        });

        modelBuilder.Entity<GroupMember>(entity =>
        {
            entity.HasIndex(e => e.GroupId, "IX_GroupMembers_GroupId");

            entity.HasIndex(e => new { e.UserId, e.GroupId }, "IX_GroupMembers_UserId_GroupId").IsUnique();

            entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers).HasForeignKey(d => d.GroupId);

            entity.HasOne(d => d.User).WithMany(p => p.GroupMembers).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<GroupTask>(entity =>
        {
            entity.HasIndex(e => e.CreatedByProfessorId, "IX_GroupTasks_CreatedByProfessorId");

            entity.HasIndex(e => e.GroupId, "IX_GroupTasks_GroupId");

            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Title).HasMaxLength(300);

            entity.HasOne(d => d.CreatedByProfessor).WithMany(p => p.GroupTasks)
                .HasForeignKey(d => d.CreatedByProfessorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Group).WithMany(p => p.GroupTasks).HasForeignKey(d => d.GroupId);
        });

        modelBuilder.Entity<LibraryFolder>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<LibraryItem>(entity =>
        {
            entity.HasIndex(e => e.FolderId, "IX_LibraryItems_FolderId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Folder).WithMany(p => p.LibraryItems).HasForeignKey(d => d.FolderId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_RoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasIndex(e => new { e.BuildingId, e.Name }, "IX_Rooms_BuildingId_Name");

            entity.Property(e => e.Equipment).HasMaxLength(1000);
            entity.Property(e => e.Floor).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Building).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.BuildingId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RoomBookingRequest>(entity =>
        {
            entity.HasIndex(e => e.RequestedByUserId, "IX_RoomBookingRequests_RequestedByUserId");

            entity.HasIndex(e => e.ReviewedByAdminId, "IX_RoomBookingRequests_ReviewedByAdminId");

            entity.HasIndex(e => new { e.RoomId, e.StartTime, e.EndTime }, "IX_RoomBookingRequests_RoomId_StartTime_EndTime");

            entity.HasIndex(e => e.Status, "IX_RoomBookingRequests_Status");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.RecurrencePattern).HasMaxLength(50);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.RequestedByUser).WithMany(p => p.RoomBookingRequestRequestedByUsers)
                .HasForeignKey(d => d.RequestedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReviewedByAdmin).WithMany(p => p.RoomBookingRequestReviewedByAdmins).HasForeignKey(d => d.ReviewedByAdminId);

            entity.HasOne(d => d.Room).WithMany(p => p.RoomBookingRequests)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SavedAnnouncement>(entity =>
        {
            entity.HasIndex(e => e.AnnouncementId, "IX_SavedAnnouncements_AnnouncementId");

            entity.HasIndex(e => new { e.UserId, e.AnnouncementId }, "IX_SavedAnnouncements_UserId_AnnouncementId").IsUnique();

            entity.HasOne(d => d.Announcement).WithMany(p => p.SavedAnnouncements).HasForeignKey(d => d.AnnouncementId);

            entity.HasOne(d => d.User).WithMany(p => p.SavedAnnouncements).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<SavedEvent>(entity =>
        {
            entity.HasIndex(e => e.EventId, "IX_SavedEvents_EventId");

            entity.HasIndex(e => new { e.UserId, e.EventId }, "IX_SavedEvents_UserId_EventId").IsUnique();

            entity.HasOne(d => d.Event).WithMany(p => p.SavedEvents).HasForeignKey(d => d.EventId);

            entity.HasOne(d => d.User).WithMany(p => p.SavedEvents).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<SavedTask>(entity =>
        {
            entity.HasIndex(e => e.TaskId, "IX_SavedTasks_TaskId");

            entity.HasIndex(e => new { e.UserId, e.TaskId }, "IX_SavedTasks_UserId_TaskId").IsUnique();

            entity.HasOne(d => d.Task).WithMany(p => p.SavedTasks)
                .HasForeignKey(d => d.TaskId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.SavedTasks).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasIndex(e => e.CreatedByProfessorId, "IX_Schedules_CreatedByProfessorId");

            entity.HasIndex(e => new { e.RoomId, e.StartTime, e.EndTime }, "IX_Schedules_RoomId_StartTime_EndTime");

            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.RecurrencePattern).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByProfessor).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.CreatedByProfessorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Room).WithMany(p => p.Schedules).HasForeignKey(d => d.RoomId);
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasIndex(e => e.Code, "IX_Subjects_Code").IsUnique();

            entity.HasIndex(e => e.ProfessorId, "IX_Subjects_ProfessorId");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(200);

            entity.HasOne(d => d.Professor).WithMany(p => p.Subjects)
                .HasForeignKey(d => d.ProfessorId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.ProfilePictureUrl).HasMaxLength(500);
            entity.Property(e => e.StudentId).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("UserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_UserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<UserAchievement>(entity =>
        {
            entity.HasIndex(e => e.AchievementId, "IX_UserAchievements_AchievementId");

            entity.HasIndex(e => new { e.UserId, e.AchievementId }, "IX_UserAchievements_UserId_AchievementId").IsUnique();

            entity.HasOne(d => d.Achievement).WithMany(p => p.UserAchievements).HasForeignKey(d => d.AchievementId);

            entity.HasOne(d => d.User).WithMany(p => p.UserAchievements).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.CreatedAt }, "IX_UserActivities_UserId_CreatedAt");

            entity.Property(e => e.ActivityType).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EntityName).HasMaxLength(300);
            entity.Property(e => e.EntityType).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.UserActivities).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_UserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.UserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_UserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.UserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.HasOne(d => d.User).WithMany(p => p.UserTokens).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
