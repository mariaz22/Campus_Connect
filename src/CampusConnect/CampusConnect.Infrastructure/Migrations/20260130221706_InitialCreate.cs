using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CampusConnect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Announcements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Announcements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    GeoJsonPolygon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CategorySubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategorySubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LibraryFolders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryFolders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedEntityId = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StudentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    Floor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Equipment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BuildingId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LibraryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StoredFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LibraryItems_LibraryFolders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "LibraryFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrganizerId = table.Column<int>(type: "int", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Users_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProfessorId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Users_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavedAnnouncements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AnnouncementId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedAnnouncements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedAnnouncements_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedAnnouncements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ProfessorId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Users_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAchievements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AchievementId = table.Column<int>(type: "int", nullable: false),
                    UnlockedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAchievements_Achievements_AchievementId",
                        column: x => x.AchievementId,
                        principalTable: "Achievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAchievements_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoomBookingRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecurrencePattern = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RecurrenceEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestedByUserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewedByAdminId = table.Column<int>(type: "int", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomBookingRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomBookingRequests_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomBookingRequests_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomBookingRequests_Users_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedByUserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProcessedByAdminId = table.Column<int>(type: "int", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomReservations_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomReservations_Users_ProcessedByAdminId",
                        column: x => x.ProcessedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomReservations_Users_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecurrencePattern = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RecurrenceEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByProfessorId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Schedules_Users_CreatedByProfessorId",
                        column: x => x.CreatedByProfessorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventParticipants",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipants", x => new { x.UserId, x.EventId });
                    table.ForeignKey(
                        name: "FK_EventParticipants_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SavedEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedEvents_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedEvents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UploadedByProfessorId = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMaterials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMaterials_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMaterials_Users_UploadedByProfessorId",
                        column: x => x.UploadedByProfessorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupAnnouncements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    AnnouncementId = table.Column<int>(type: "int", nullable: false),
                    ForwardedByProfessorId = table.Column<int>(type: "int", nullable: false),
                    ForwardedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAnnouncements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAnnouncements_Announcements_AnnouncementId",
                        column: x => x.AnnouncementId,
                        principalTable: "Announcements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAnnouncements_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAnnouncements_Users_ForwardedByProfessorId",
                        column: x => x.ForwardedByProfessorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByProfessorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupTasks_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupTasks_Users_CreatedByProfessorId",
                        column: x => x.CreatedByProfessorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByProfessorId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grades_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grades_Users_CreatedByProfessorId",
                        column: x => x.CreatedByProfessorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Grades_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavedTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedTasks_GroupTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "GroupTasks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SavedTasks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Achievements",
                columns: new[] { "Id", "CreatedAt", "Description", "Icon", "IsActive", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 30, 22, 17, 5, 358, DateTimeKind.Utc).AddTicks(1781), "Complete your first task", "🎯", true, "First Steps" },
                    { 2, new DateTime(2026, 1, 30, 22, 17, 5, 358, DateTimeKind.Utc).AddTicks(2132), "Complete 5 tasks", "⭐", true, "Task Master" },
                    { 3, new DateTime(2026, 1, 30, 22, 17, 5, 358, DateTimeKind.Utc).AddTicks(2135), "Complete 10 tasks", "🏆", true, "Task Legend" },
                    { 4, new DateTime(2026, 1, 30, 22, 17, 5, 358, DateTimeKind.Utc).AddTicks(2137), "Join your first group", "👥", true, "Team Player" },
                    { 5, new DateTime(2026, 1, 30, 22, 17, 5, 358, DateTimeKind.Utc).AddTicks(2139), "Attend your first event", "🦋", true, "Social Butterfly" }
                });

            migrationBuilder.InsertData(
                table: "Buildings",
                columns: new[] { "Id", "Address", "CreatedAt", "Description", "GeoJsonPolygon", "IsActive", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, "B-dul Regina Elisabeta nr. 4-12, etaj 1, sector 3, București", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7762), "FAA - Sediu în clădirea Chimiei", null, true, 44.434719999999999, 26.100719999999999, "Facultatea de Administrație și Afaceri" },
                    { 2, "Splaiul Independenței nr. 91-95, sector 5, București, 050095", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7898), "Facultatea de Biologie", null, true, 44.435299999999998, 26.06326, "Facultatea de Biologie" },
                    { 3, "Bd. Regina Elisabeta nr. 4-12, sector 3, București, 030018", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7900), "Facultatea de Chimie", null, true, 44.434719999999999, 26.100719999999999, "Facultatea de Chimie" },
                    { 4, "Bd. Mihail Kogălniceanu nr. 36-46, sector 5, București, 050107", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7903), "Facultatea de Drept", null, true, 44.435240999999998, 26.082077000000002, "Facultatea de Drept" },
                    { 5, "Splaiul Independenței nr. 204, sector 6, București, 060024", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7905), "Facultatea de Filosofie", null, true, 44.434710000000003, 26.04824, "Facultatea de Filosofie" },
                    { 6, "Str. Atomiștilor nr. 405, Măgurele, Ilfov, 077125", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7907), "Facultatea de Fizică - Campus Măgurele", null, true, 44.34834, 26.031279999999999, "Facultatea de Fizică" },
                    { 7, "Bd. Nicolae Bălcescu nr. 1, sector 1, București, 010041", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7909), "Facultatea de Geografie", null, true, 44.436540000000001, 26.101890000000001, "Facultatea de Geografie" },
                    { 8, "Str. Traian Vuia nr. 6, sector 2, București, 020956", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7911), "Facultatea de Geologie și Geofizică", null, true, 44.45167, 26.07901, "Facultatea de Geologie și Geofizică" },
                    { 9, "Str. Academiei nr. 14, București", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7913), "Facultatea de Istorie", null, true, 44.435839999999999, 26.096830000000001, "Facultatea de Istorie" },
                    { 10, "Bd. Iuliu Maniu nr. 1-3, Complex Leu, Corp A, etaj 6, sector 6, București", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7916), "FJSC - Complex Leu", null, true, 44.43891, 26.043209999999998, "Facultatea de Jurnalism și Științele Comunicării" },
                    { 11, "Str. Edgar Quinet nr. 5-7, sector 1, București, 010017", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7930), "FLLS", null, true, 44.435830000000003, 26.100809999999999, "Facultatea de Limbi și Literaturi Străine" },
                    { 12, "Str. Edgar Quinet nr. 5-7, sector 1, București, 010017", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7932), "Facultatea de Litere", null, true, 44.435830000000003, 26.100809999999999, "Facultatea de Litere" },
                    { 13, "Str. Academiei nr. 14, sector 1, București, 010014", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7934), "FMI", null, true, 44.435839999999999, 26.096830000000001, "Facultatea de Matematică și Informatică" },
                    { 14, "Șos. Panduri nr. 90-91, București", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7936), "FPSE", null, true, 44.432209999999998, 26.068919999999999, "Facultatea de Psihologie și Științele Educației" },
                    { 15, "Bd. Schitu Măgureanu nr. 9, București", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7946), "SAS", null, true, 44.433419999999998, 26.09421, "Facultatea de Sociologie și Asistență Socială" },
                    { 16, "Calea Plevnei nr. 59, București, 010223", new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(7948), "FSP", null, true, 44.445210000000003, 26.083919999999999, "Facultatea de Științe Politice" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "dd6fa6d7-7f35-48bb-b8d7-38d880bbae73", "Admin", "ADMIN" },
                    { 2, "4fc6752a-9c95-4254-8369-6af0615e0c3a", "User", "USER" },
                    { 3, "18286468-3180-4c12-9494-9492cf61e3ad", "Professor", "PROFESSOR" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "Email", "EmailConfirmed", "FirstName", "LastLoginAt", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePictureUrl", "SecurityStamp", "StudentId", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { 10, 0, "4e2f5d85-0d80-438f-8bde-1bafb2d1da8d", new DateTime(2026, 1, 30, 22, 17, 4, 979, DateTimeKind.Utc).AddTicks(4631), null, "admin1@unibuc.ro", true, "Andrei", null, "Popescu", false, null, "ADMIN1@UNIBUC.RO", "ADMIN1@UNIBUC.RO", "AQAAAAIAAYagAAAAEEuGk8skaOuUS84a3Ot0fqP+flxWV/r7WmHuLoo4iOwZ5SAVUslns/SmHCCDPn+1Lg==", null, false, null, "225e929c-8225-4b29-a4fd-1536f7bc28d7", null, false, "admin1@unibuc.ro" },
                    { 11, 0, "4cf36c1a-8e64-4bc9-b760-ee87b636e775", new DateTime(2026, 1, 30, 22, 17, 5, 37, DateTimeKind.Utc).AddTicks(7061), null, "admin2@unibuc.ro", true, "Maria", null, "Ionescu", false, null, "ADMIN2@UNIBUC.RO", "ADMIN2@UNIBUC.RO", "AQAAAAIAAYagAAAAEMZKMl+VwJhfO1SL4bIoS64jGK2U/3/9/SiLOtJJh4y1EuKb6MtJGQ9tWuoF2ahoOg==", null, false, null, "dda8f9aa-8d48-4f77-838c-6724fcdbcef6", null, false, "admin2@unibuc.ro" },
                    { 12, 0, "86b7ddf4-716f-4a26-b645-691f60020df6", new DateTime(2026, 1, 30, 22, 17, 5, 94, DateTimeKind.Utc).AddTicks(5371), null, "student1@s.unibuc.ro", true, "Ion", null, "Vasilescu", false, null, "STUDENT1@S.UNIBUC.RO", "STUDENT1@S.UNIBUC.RO", "AQAAAAIAAYagAAAAEJNc2Uq1eAqZ9/P9aH88Gfcqqujx8pmNUQWUN+DSeU7Fm+C0cwc+RLo8zISCjADoWA==", null, false, null, "59dc5a49-fddd-4d65-8b50-b578d53ce705", "STD2024001", false, "student1@s.unibuc.ro" },
                    { 13, 0, "2da18f49-3075-4b13-b9dc-0d338e717c02", new DateTime(2026, 1, 30, 22, 17, 5, 155, DateTimeKind.Utc).AddTicks(4603), null, "student2@s.unibuc.ro", true, "Elena", null, "Georgescu", false, null, "STUDENT2@S.UNIBUC.RO", "STUDENT2@S.UNIBUC.RO", "AQAAAAIAAYagAAAAEODWHtlyTgxnSDM8FZp5/5LDswiMNGPgLP2uz6Q6nwDvsqm38cqd8a4WxAZevZYhog==", null, false, null, "8768a6ac-9d14-4bff-b98a-4b665b97e85b", "STD2024002", false, "student2@s.unibuc.ro" },
                    { 14, 0, "d1eed499-7c21-427c-811e-065e45d161d5", new DateTime(2026, 1, 30, 22, 17, 5, 211, DateTimeKind.Utc).AddTicks(6210), null, "anastasia.ispas@s.unibuc.ro", true, "Anastasia", null, "Ispas", false, null, "ANASTASIA.ISPAS@S.UNIBUC.RO", "ANASTASIA.ISPAS@S.UNIBUC.RO", "AQAAAAIAAYagAAAAEEF9TwxU73tPsy3NPBBOCOYlEENaXuKiiQtb1quR0ndL7rs/Rh1JtADYhmNPF/V8Sg==", null, false, null, "c1fff5a5-3750-43e5-a5ca-ededbb889c28", null, false, "anastasia.ispas@s.unibuc.ro" },
                    { 15, 0, "d5fecae0-71a3-4cf7-bb9a-37f9da75012a", new DateTime(2026, 1, 30, 22, 17, 5, 269, DateTimeKind.Utc).AddTicks(968), null, "irina-maria.istrate@s.unibuc.ro", true, "Irina-Maria", null, "Istrate", false, null, "IRINA-MARIA.ISTRATE@S.UNIBUC.RO", "IRINA-MARIA.ISTRATE@S.UNIBUC.RO", "AQAAAAIAAYagAAAAEO1rg2EfX8yF0gdPflX/TLF/Y9ZjIyxel8Mwk+JqV9zCCTVeSuMeV8x+o5/K4rQL8w==", null, false, null, "729d8cbe-8348-4a60-9289-eee3330fcc6b", null, false, "irina-maria.istrate@s.unibuc.ro" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "BuildingId", "Capacity", "CreatedAt", "Equipment", "Floor", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 1, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9648), null, "Etaj 1", true, "A101" },
                    { 2, 1, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9749), null, "Etaj 1", true, "A102" },
                    { 3, 1, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9752), null, "Etaj 1", true, "A103" },
                    { 4, 1, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9754), null, "Etaj 1", true, "A104" },
                    { 5, 1, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9756), null, "Etaj 1", true, "A105" },
                    { 6, 1, 30, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9759), null, "Etaj 2", true, "S201" },
                    { 7, 1, 30, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9761), null, "Etaj 2", true, "S202" },
                    { 8, 1, 30, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9763), null, "Etaj 2", true, "S203" },
                    { 9, 1, 25, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9876), "Computere", "Etaj 3", true, "Lab301" },
                    { 10, 1, 25, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9879), "Computere", "Etaj 3", true, "Lab302" },
                    { 11, 2, 50, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9881), null, "Etaj 1", true, "Bio101" },
                    { 12, 2, 50, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9883), null, "Etaj 1", true, "Bio102" },
                    { 13, 2, 50, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9885), null, "Etaj 1", true, "Bio103" },
                    { 14, 2, 50, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9887), null, "Etaj 1", true, "Bio104" },
                    { 15, 2, 50, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9889), null, "Etaj 1", true, "Bio105" },
                    { 16, 2, 30, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9892), "Echipament laborator", "Etaj 2", true, "LabBio201" },
                    { 17, 2, 30, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9894), "Echipament laborator", "Etaj 2", true, "LabBio202" },
                    { 18, 2, 30, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9896), "Echipament laborator", "Etaj 2", true, "LabBio203" },
                    { 19, 2, 200, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9899), "Proiector, Sistem audio", "Parter", true, "AmfBio1" },
                    { 20, 2, 150, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9918), "Proiector", "Parter", true, "AmfBio2" },
                    { 21, 3, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9921), null, "Etaj 1", true, "Ch101" },
                    { 22, 3, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9923), null, "Etaj 1", true, "Ch102" },
                    { 23, 3, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9935), null, "Etaj 1", true, "Ch103" },
                    { 24, 3, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9938), null, "Etaj 1", true, "Ch104" },
                    { 25, 3, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9940), null, "Etaj 1", true, "Ch105" },
                    { 26, 3, 25, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9942), "Echipament chimie", "Etaj 2", true, "LabCh201" },
                    { 27, 3, 25, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9944), "Echipament chimie", "Etaj 2", true, "LabCh202" },
                    { 28, 3, 25, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9946), "Echipament chimie", "Etaj 2", true, "LabCh203" },
                    { 29, 3, 180, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9949), "Proiector, Sistem audio", "Parter", true, "AmfCh1" },
                    { 30, 3, 150, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9951), "Proiector", "Parter", true, "AmfCh2" },
                    { 31, 4, 50, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9953), null, "Etaj 1", true, "D101" },
                    { 32, 4, 50, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9955), null, "Etaj 1", true, "D102" },
                    { 33, 4, 50, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9957), null, "Etaj 1", true, "D103" },
                    { 34, 4, 50, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9959), null, "Etaj 1", true, "D104" },
                    { 35, 4, 50, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9961), null, "Etaj 1", true, "D105" },
                    { 36, 4, 300, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9964), "Sistem audio-video complet", "Parter", true, "AmfD1" },
                    { 37, 4, 250, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9975), "Proiector, Sistem audio", "Parter", true, "AmfD2" },
                    { 38, 4, 30, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9977), null, "Etaj 2", true, "SemD201" },
                    { 39, 4, 30, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9979), null, "Etaj 2", true, "SemD202" },
                    { 40, 4, 30, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9981), null, "Etaj 2", true, "SemD203" },
                    { 41, 5, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9983), null, "Etaj 1", true, "Filo101" },
                    { 42, 5, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9985), null, "Etaj 1", true, "Filo102" },
                    { 43, 5, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9988), null, "Etaj 1", true, "Filo103" },
                    { 44, 5, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9990), null, "Etaj 1", true, "Filo104" },
                    { 45, 5, 40, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9992), null, "Etaj 1", true, "Filo105" },
                    { 46, 5, 150, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9994), "Proiector, Sistem audio", "Parter", true, "AmfFilo1" },
                    { 47, 5, 120, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9996), "Proiector", "Parter", true, "AmfFilo2" },
                    { 48, 5, 25, new DateTime(2026, 1, 30, 22, 17, 5, 352, DateTimeKind.Utc).AddTicks(9999), null, "Etaj 2", true, "SemFilo201" },
                    { 49, 5, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(1), null, "Etaj 2", true, "SemFilo202" },
                    { 50, 5, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(3), null, "Etaj 2", true, "SemFilo203" },
                    { 51, 6, 50, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(5), null, "Etaj 1", true, "Fiz101" },
                    { 52, 6, 50, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(16), null, "Etaj 1", true, "Fiz102" },
                    { 53, 6, 50, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(18), null, "Etaj 1", true, "Fiz103" },
                    { 54, 6, 50, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(21), null, "Etaj 1", true, "Fiz104" },
                    { 55, 6, 50, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(23), null, "Etaj 1", true, "Fiz105" },
                    { 56, 6, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(25), "Echipament fizică", "Etaj 2", true, "LabFiz201" },
                    { 57, 6, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(27), "Echipament fizică", "Etaj 2", true, "LabFiz202" },
                    { 58, 6, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(29), "Echipament fizică", "Etaj 2", true, "LabFiz203" },
                    { 59, 6, 200, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(32), "Proiector, Sistem audio", "Parter", true, "AmfFiz1" },
                    { 60, 6, 150, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(34), "Proiector", "Parter", true, "AmfFiz2" },
                    { 61, 7, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(36), null, "Etaj 1", true, "Geo101" },
                    { 62, 7, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(38), null, "Etaj 1", true, "Geo102" },
                    { 63, 7, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(41), null, "Etaj 1", true, "Geo103" },
                    { 64, 7, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(43), null, "Etaj 1", true, "Geo104" },
                    { 65, 7, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(45), null, "Etaj 1", true, "Geo105" },
                    { 66, 7, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(47), "Hărți, Computere GIS", "Etaj 2", true, "LabGeo201" },
                    { 67, 7, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(58), "Computere GIS", "Etaj 2", true, "LabGeo202" },
                    { 68, 7, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(60), null, "Etaj 2", true, "SemGeo203" },
                    { 69, 7, 180, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(62), "Proiector, Sistem audio", "Parter", true, "AmfGeo1" },
                    { 70, 7, 120, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(65), "Proiector", "Parter", true, "AmfGeo2" },
                    { 71, 8, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(67), null, "Etaj 1", true, "GG101" },
                    { 72, 8, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(69), null, "Etaj 1", true, "GG102" },
                    { 73, 8, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(71), null, "Etaj 1", true, "GG103" },
                    { 74, 8, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(73), null, "Etaj 1", true, "GG104" },
                    { 75, 8, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(75), null, "Etaj 1", true, "GG105" },
                    { 76, 8, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(78), "Echipament geologic", "Etaj 2", true, "LabGG201" },
                    { 77, 8, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(80), "Echipament geofizic", "Etaj 2", true, "LabGG202" },
                    { 78, 8, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(82), null, "Etaj 2", true, "SemGG203" },
                    { 79, 8, 150, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(84), "Proiector, Sistem audio", "Parter", true, "AmfGG1" },
                    { 80, 8, 120, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(87), "Proiector", "Parter", true, "AmfGG2" },
                    { 81, 9, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(97), null, "Etaj 1", true, "Ist101" },
                    { 82, 9, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(100), null, "Etaj 1", true, "Ist102" },
                    { 83, 9, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(102), null, "Etaj 1", true, "Ist103" },
                    { 84, 9, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(104), null, "Etaj 1", true, "Ist104" },
                    { 85, 9, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(106), null, "Etaj 1", true, "Ist105" },
                    { 86, 9, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(108), null, "Etaj 2", true, "SemIst201" },
                    { 87, 9, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(110), null, "Etaj 2", true, "SemIst202" },
                    { 88, 9, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(112), null, "Etaj 2", true, "SemIst203" },
                    { 89, 9, 150, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(123), "Proiector, Sistem audio", "Parter", true, "AmfIst1" },
                    { 90, 9, 120, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(125), "Proiector", "Parter", true, "AmfIst2" },
                    { 91, 10, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(127), null, "Etaj 6", true, "J101" },
                    { 92, 10, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(129), null, "Etaj 6", true, "J102" },
                    { 93, 10, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(131), null, "Etaj 6", true, "J103" },
                    { 94, 10, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(134), null, "Etaj 6", true, "J104" },
                    { 95, 10, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(145), null, "Etaj 6", true, "J105" },
                    { 96, 10, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(148), "Camere, Echipament video", "Etaj 7", true, "LabMedia201" },
                    { 97, 10, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(150), "Echipament audio", "Etaj 7", true, "LabMedia202" },
                    { 98, 10, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(152), null, "Etaj 7", true, "SemPR203" },
                    { 99, 10, 100, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(154), "Proiector, Sistem audio", "Etaj 6", true, "AmfJ1" },
                    { 100, 10, 20, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(156), "Studio TV/Radio", "Etaj 7", true, "StudioJ2" },
                    { 101, 11, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(159), null, "Etaj 1", true, "LLS101" },
                    { 102, 11, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(161), null, "Etaj 1", true, "LLS102" },
                    { 103, 11, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(163), null, "Etaj 1", true, "LLS103" },
                    { 104, 11, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(165), null, "Etaj 1", true, "LLS104" },
                    { 105, 11, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(167), null, "Etaj 1", true, "LLS105" },
                    { 106, 11, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(170), "Echipament limbi străine", "Etaj 2", true, "LabLingv201" },
                    { 107, 11, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(172), "Computere, Software lingvistic", "Etaj 2", true, "LabLingv202" },
                    { 108, 11, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(174), null, "Etaj 2", true, "SemLLS203" },
                    { 109, 11, 150, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(176), "Proiector, Sistem audio", "Parter", true, "AmfLLS1" },
                    { 110, 11, 120, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(187), "Proiector", "Parter", true, "AmfLLS2" },
                    { 111, 12, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(189), null, "Etaj 1", true, "Lit101" },
                    { 112, 12, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(191), null, "Etaj 1", true, "Lit102" },
                    { 113, 12, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(193), null, "Etaj 1", true, "Lit103" },
                    { 114, 12, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(196), null, "Etaj 1", true, "Lit104" },
                    { 115, 12, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(198), null, "Etaj 1", true, "Lit105" },
                    { 116, 12, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(200), null, "Etaj 2", true, "SemLit201" },
                    { 117, 12, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(202), null, "Etaj 2", true, "SemLit202" },
                    { 118, 12, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(204), null, "Etaj 2", true, "SemLit203" },
                    { 119, 12, 200, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(206), "Proiector, Sistem audio", "Parter", true, "AmfLit1" },
                    { 120, 12, 150, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(209), "Proiector", "Parter", true, "AmfLit2" },
                    { 121, 13, 300, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(211), "Proiector, Sistem audio premium", "Parter", true, "Amf. Spiru Haret" },
                    { 122, 13, 250, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(213), "Proiector, Sistem audio", "Etaj 3", true, "Amf. Gheorghe Țițeica" },
                    { 123, 13, 200, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(215), "Proiector, Sistem audio", "Etaj 1", true, "Amf. Simion Stoilow" },
                    { 124, 13, 180, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(226), "Proiector, Sistem audio", "Etaj 2", true, "Amf. Dimitrie Pompeiu" },
                    { 125, 13, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(229), "30 Computere, Proiector", "Etaj 1", true, "Lab FMI 1" },
                    { 126, 13, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(231), "30 Computere, Proiector", "Etaj 1", true, "Lab FMI 2" },
                    { 127, 13, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(233), "30 Computere, Proiector", "Etaj 1", true, "Lab FMI 3" },
                    { 128, 13, 50, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(235), null, "Etaj 1", true, "S101" },
                    { 129, 13, 50, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(237), null, "Etaj 1", true, "S102" },
                    { 130, 13, 50, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(240), null, "Etaj 1", true, "S103" },
                    { 131, 14, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(248), null, "Etaj 1", true, "Psi101" },
                    { 132, 14, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(250), null, "Etaj 1", true, "Psi102" },
                    { 133, 14, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(252), null, "Etaj 1", true, "Psi103" },
                    { 134, 14, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(254), null, "Etaj 1", true, "Psi104" },
                    { 135, 14, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(257), null, "Etaj 1", true, "Psi105" },
                    { 136, 14, 20, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(269), "Echipament psihologie", "Etaj 2", true, "LabPsi201" },
                    { 137, 14, 20, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(271), "Computere, Software psiho", "Etaj 2", true, "LabPsi202" },
                    { 138, 14, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(273), null, "Etaj 2", true, "SemEdu203" },
                    { 139, 14, 150, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(275), "Proiector, Sistem audio", "Parter", true, "AmfPsi1" },
                    { 140, 14, 120, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(277), "Proiector", "Parter", true, "AmfPsi2" },
                    { 141, 15, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(280), null, "Etaj 1", true, "SAS101" },
                    { 142, 15, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(282), null, "Etaj 1", true, "SAS102" },
                    { 143, 15, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(284), null, "Etaj 1", true, "SAS103" },
                    { 144, 15, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(286), null, "Etaj 1", true, "SAS104" },
                    { 145, 15, 40, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(288), null, "Etaj 1", true, "SAS105" },
                    { 146, 15, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(290), null, "Etaj 2", true, "SemSAS201" },
                    { 147, 15, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(292), null, "Etaj 2", true, "SemSAS202" },
                    { 148, 15, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(294), null, "Etaj 2", true, "SemSAS203" },
                    { 149, 15, 150, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(296), "Proiector, Sistem audio", "Parter", true, "AmfSAS1" },
                    { 150, 15, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(307), "Computere, Software SPSS", "Etaj 2", true, "LabSAS2" },
                    { 151, 16, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(310), null, "Etaj 1", true, "FSP101" },
                    { 152, 16, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(312), null, "Etaj 1", true, "FSP102" },
                    { 153, 16, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(314), null, "Etaj 1", true, "FSP103" },
                    { 154, 16, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(321), null, "Etaj 1", true, "FSP104" },
                    { 155, 16, 45, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(324), null, "Etaj 1", true, "FSP105" },
                    { 156, 16, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(326), null, "Etaj 2", true, "SemFSP201" },
                    { 157, 16, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(328), null, "Etaj 2", true, "SemFSP202" },
                    { 158, 16, 30, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(330), null, "Etaj 2", true, "SemFSP203" },
                    { 159, 16, 180, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(332), "Proiector, Sistem audio", "Parter", true, "AmfFSP1" },
                    { 160, 16, 25, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(335), "Computere", "Etaj 2", true, "LabFSP2" },
                    { 161, 13, 50, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(242), null, "Parter", true, "S1" },
                    { 162, 13, 50, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(244), null, "Parter", true, "S3" },
                    { 163, 13, 50, new DateTime(2026, 1, 30, 22, 17, 5, 353, DateTimeKind.Utc).AddTicks(246), null, "Etaj 2", true, "S201" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 10 },
                    { 1, 11 },
                    { 2, 12 },
                    { 2, 13 },
                    { 3, 14 },
                    { 3, 15 }
                });

            migrationBuilder.InsertData(
                table: "Schedules",
                columns: new[] { "Id", "CreatedAt", "CreatedByProfessorId", "Description", "EndTime", "IsActive", "RecurrenceEndDate", "RecurrencePattern", "RoomId", "StartTime", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2201), 14, "Principii de inginerie software și design patterns", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Local), true, null, "Weekly", 121, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Local), "Curs Inginerie Software" },
                    { 2, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2305), 15, "Lucru cu SQL și modelare baze de date", new DateTime(2026, 1, 31, 16, 0, 0, 0, DateTimeKind.Local), true, null, "Weekly", 125, new DateTime(2026, 1, 31, 14, 0, 0, 0, DateTimeKind.Local), "Seminar Baze de Date" },
                    { 3, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2321), 14, "Algoritmi de sortare și căutare", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Local), true, null, "Weekly", 122, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Local), "Curs Algoritmi și Structuri de Date" },
                    { 4, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2324), 15, "Dreptul persoanelor și al familiei", new DateTime(2026, 1, 31, 14, 0, 0, 0, DateTimeKind.Local), true, null, "Weekly", 36, new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Local), "Curs Drept Civil" },
                    { 5, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2352), 14, "Principii de administrație publică", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 1, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Administrație Publică" },
                    { 6, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2354), 15, "Analiza politicilor publice", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 2, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Politici Publice" },
                    { 7, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2357), 14, "Fundamente de management public", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 1, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Management Public" },
                    { 8, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2359), 15, "Aspecte practice", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 2, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Drept Administrativ" },
                    { 9, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2362), 14, "Structura celulei eucariote", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 11, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Biologie Celulară" },
                    { 10, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2364), 15, "Tehnici de cultivare", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 12, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Laborator Microbiologie" },
                    { 11, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2367), 14, "Genetica moleculară", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 11, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Genetică" },
                    { 12, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2369), 15, "Ecosisteme terestre", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 12, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Ecologie" },
                    { 13, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2383), 14, "Reacții de sinteză", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 21, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Chimie Organică" },
                    { 14, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2386), 15, "Metode spectroscopice", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 22, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Laborator Chimie Analitică" },
                    { 15, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2389), 14, "Metabolismul celular", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 21, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Biochimie" },
                    { 16, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2391), 15, "Termodinamică", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 22, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Chimie Fizică" },
                    { 17, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2394), 14, "Infracțiuni și pedepse", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 31, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Drept Penal" },
                    { 18, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2396), 15, "Constituția României", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 32, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Drept Constituțional" },
                    { 19, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2398), 14, "Instituții UE", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 31, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Drept European" },
                    { 20, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2401), 15, "Societăți comerciale", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 32, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Drept Comercial" },
                    { 21, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2404), 14, "Filosofia antică greacă", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 41, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Istoria Filosofiei" },
                    { 22, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2406), 15, "Teorii etice contemporane", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 42, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Etică" },
                    { 23, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2419), 14, "Logica formală", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 41, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Logică" },
                    { 24, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2421), 15, "Teoria cunoașterii", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 42, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Epistemologie" },
                    { 25, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2424), 14, "Principii fundamentale", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 51, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Mecanică Cuantică" },
                    { 26, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2426), 15, "Experimente de optică", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 52, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Laborator Optică" },
                    { 27, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2429), 14, "Legile termodinamicii", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 51, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Termodinamică" },
                    { 28, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2431), 15, "Câmpuri electromagnetice", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 52, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Electrodinamică" },
                    { 29, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2434), 14, "Relieful terestru", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 61, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Geografie Fizică" },
                    { 30, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2436), 15, "Tehnici cartografice", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 62, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Laborator Cartografie" },
                    { 31, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2438), 14, "Sistemul climatic global", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 61, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Climatologie" },
                    { 32, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2441), 15, "Sisteme informaționale geografice", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 62, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar GIS" },
                    { 33, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2453), 14, "Clasificarea mineralelor", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 71, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Mineralogie" },
                    { 34, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2456), 15, "Analiza rocilor", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 72, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Laborator Petrografie" },
                    { 35, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2458), 14, "Metode geofizice", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 71, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Geofizică" },
                    { 36, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2460), 15, "Fosile și evoluție", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 72, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Paleontologie" },
                    { 37, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2463), 14, "Civilizații antice", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 81, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Istorie Antică" },
                    { 38, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2465), 15, "Europa medievală", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 82, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Istorie Medievală" },
                    { 39, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2467), 14, "Revoluții și națiuni", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 81, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Istorie Modernă" },
                    { 40, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2470), 15, "Secolul XX", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 82, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Istorie Contemporană" },
                    { 41, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2472), 14, "Modele de comunicare", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 91, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Teoria Comunicării" },
                    { 42, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2474), 15, "Tehnici multimedia", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 92, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Laborator Jurnalism Digital" },
                    { 43, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2488), 14, "Strategii de PR", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 91, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Relații Publice" },
                    { 44, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2490), 15, "Campanii publicitare", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 92, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Publicitate" },
                    { 45, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2492), 14, "Gramatică avansată", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 101, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Limba Engleză" },
                    { 46, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2495), 15, "Conversație", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 102, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Limba Franceză" },
                    { 47, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2497), 14, "Literatură germană", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 101, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Limba Germană" },
                    { 48, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2500), 15, "Cultură hispanică", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 102, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Limba Spaniolă" },
                    { 49, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2502), 14, "Clasici români", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 111, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Literatură Română" },
                    { 50, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2504), 15, "Analiză lingvistică", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 112, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Lingvistică" },
                    { 51, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2507), 14, "Curente literare", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 111, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Literatură Universală" },
                    { 52, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2509), 15, "Analiză text", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 112, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Teoria Literaturii" },
                    { 53, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2523), 14, "Design patterns", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 121, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Programare Orientată Obiect" },
                    { 54, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2525), 15, "Protocoale de rețea", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 122, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Laborator Rețele de Calculatoare" },
                    { 55, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2528), 14, "Machine Learning", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 121, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Inteligență Artificială" },
                    { 56, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2530), 15, "SQL și NoSQL", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 122, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Baze de Date Avansate" },
                    { 57, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2532), 14, "Procese cognitive", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 131, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Psihologie Generală" },
                    { 58, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2535), 15, "Dinamica grupurilor", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 132, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Psihologie Socială" },
                    { 59, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2537), 14, "Teorii educaționale", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 131, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Pedagogie" },
                    { 60, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2539), 15, "Etape de dezvoltare", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 132, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Psihologia Dezvoltării" },
                    { 61, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2541), 14, "Teorii sociologice", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 141, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Sociologie Generală" },
                    { 62, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2544), 15, "Intervenție socială", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 142, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Asistență Socială" },
                    { 63, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2558), 14, "Metode de cercetare", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 141, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Metodologia Cercetării" },
                    { 64, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2560), 15, "Sistemul de protecție", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 142, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Politici Sociale" },
                    { 65, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2562), 14, "Teorii politice", new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 151, new DateTime(2026, 1, 30, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Științe Politice" },
                    { 66, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2565), 15, "Diplomație", new DateTime(2026, 1, 30, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 152, new DateTime(2026, 1, 30, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Relații Internaționale" },
                    { 67, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2567), 14, "Integrare europeană", new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 151, new DateTime(2026, 1, 31, 8, 0, 0, 0, DateTimeKind.Unspecified), "Curs Studii Europene" },
                    { 68, new DateTime(2026, 1, 30, 22, 17, 5, 355, DateTimeKind.Utc).AddTicks(2585), 15, "Sisteme politice", new DateTime(2026, 1, 31, 12, 0, 0, 0, DateTimeKind.Unspecified), true, null, null, 152, new DateTime(2026, 1, 31, 10, 0, 0, 0, DateTimeKind.Unspecified), "Seminar Politici Comparate" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_Name",
                table: "Buildings",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMaterials_GroupId",
                table: "CourseMaterials",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMaterials_UploadedByProfessorId",
                table: "CourseMaterials",
                column: "UploadedByProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_EventId",
                table: "EventParticipants",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_OrganizerId",
                table: "Events",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_CreatedByProfessorId",
                table: "Grades",
                column: "CreatedByProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_StudentId",
                table: "Grades",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_SubjectId_StudentId_CreatedAt",
                table: "Grades",
                columns: new[] { "SubjectId", "StudentId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupAnnouncements_AnnouncementId",
                table: "GroupAnnouncements",
                column: "AnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAnnouncements_ForwardedByProfessorId",
                table: "GroupAnnouncements",
                column: "ForwardedByProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAnnouncements_GroupId_AnnouncementId",
                table: "GroupAnnouncements",
                columns: new[] { "GroupId", "AnnouncementId" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupId",
                table: "GroupMembers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_UserId_GroupId",
                table: "GroupMembers",
                columns: new[] { "UserId", "GroupId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ProfessorId",
                table: "Groups",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupTasks_CreatedByProfessorId",
                table: "GroupTasks",
                column: "CreatedByProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupTasks_GroupId",
                table: "GroupTasks",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryItems_FolderId",
                table: "LibraryItems",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoomBookingRequests_RequestedByUserId",
                table: "RoomBookingRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomBookingRequests_ReviewedByAdminId",
                table: "RoomBookingRequests",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomBookingRequests_RoomId_StartTime_EndTime",
                table: "RoomBookingRequests",
                columns: new[] { "RoomId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_RoomBookingRequests_Status",
                table: "RoomBookingRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_ProcessedByAdminId",
                table: "RoomReservations",
                column: "ProcessedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_RequestedByUserId",
                table: "RoomReservations",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_RoomId",
                table: "RoomReservations",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BuildingId_Name",
                table: "Rooms",
                columns: new[] { "BuildingId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedAnnouncements_AnnouncementId",
                table: "SavedAnnouncements",
                column: "AnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedAnnouncements_UserId_AnnouncementId",
                table: "SavedAnnouncements",
                columns: new[] { "UserId", "AnnouncementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedEvents_EventId",
                table: "SavedEvents",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedEvents_UserId_EventId",
                table: "SavedEvents",
                columns: new[] { "UserId", "EventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedTasks_TaskId",
                table: "SavedTasks",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedTasks_UserId_TaskId",
                table: "SavedTasks",
                columns: new[] { "UserId", "TaskId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_CreatedByProfessorId",
                table: "Schedules",
                column: "CreatedByProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_RoomId_StartTime_EndTime",
                table: "Schedules",
                columns: new[] { "RoomId", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_Code",
                table: "Subjects",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_ProfessorId",
                table: "Subjects",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_AchievementId",
                table: "UserAchievements",
                column: "AchievementId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_UserId_AchievementId",
                table: "UserAchievements",
                columns: new[] { "UserId", "AchievementId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_UserId_CreatedAt",
                table: "UserActivities",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategorySubscriptions");

            migrationBuilder.DropTable(
                name: "CourseMaterials");

            migrationBuilder.DropTable(
                name: "EventParticipants");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "GroupAnnouncements");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "LibraryItems");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "RoomBookingRequests");

            migrationBuilder.DropTable(
                name: "RoomReservations");

            migrationBuilder.DropTable(
                name: "SavedAnnouncements");

            migrationBuilder.DropTable(
                name: "SavedEvents");

            migrationBuilder.DropTable(
                name: "SavedTasks");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "UserAchievements");

            migrationBuilder.DropTable(
                name: "UserActivities");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "LibraryFolders");

            migrationBuilder.DropTable(
                name: "Announcements");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "GroupTasks");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
