using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class User
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? StudentId { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string Email { get; set; } = null!;

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public virtual ICollection<CourseMaterial> CourseMaterials { get; set; } = new List<CourseMaterial>();

    public virtual ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Grade> GradeCreatedByProfessors { get; set; } = new List<Grade>();

    public virtual ICollection<Grade> GradeStudents { get; set; } = new List<Grade>();

    public virtual ICollection<GroupAnnouncement> GroupAnnouncements { get; set; } = new List<GroupAnnouncement>();

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<GroupTask> GroupTasks { get; set; } = new List<GroupTask>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<RoomBookingRequest> RoomBookingRequestRequestedByUsers { get; set; } = new List<RoomBookingRequest>();

    public virtual ICollection<RoomBookingRequest> RoomBookingRequestReviewedByAdmins { get; set; } = new List<RoomBookingRequest>();

    public virtual ICollection<SavedAnnouncement> SavedAnnouncements { get; set; } = new List<SavedAnnouncement>();

    public virtual ICollection<SavedEvent> SavedEvents { get; set; } = new List<SavedEvent>();

    public virtual ICollection<SavedTask> SavedTasks { get; set; } = new List<SavedTask>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();

    public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();

    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();

    public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();

    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
