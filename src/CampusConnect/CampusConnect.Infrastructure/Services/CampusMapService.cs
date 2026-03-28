using CampusConnect.Application.DTOs.CampusMap;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using CampusConnect.Domain.Services;
using CampusConnect.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Infrastructure.Services;

public class CampusMapService : ICampusMapService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CampusMapService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    // Building operations
    public async Task<IEnumerable<BuildingDto>> GetAllBuildingsAsync()
    {
        var buildings = await _context.Buildings
            .Where(b => b.IsActive)
            .Include(b => b.Rooms.Where(r => r.IsActive))
            .ToListAsync();

        return buildings.Select(b => new BuildingDto
        {
            Id = b.Id,
            Name = b.Name,
            Description = b.Description,
            Address = b.Address,
            Latitude = b.Latitude,
            Longitude = b.Longitude,
            GeoJsonPolygon = b.GeoJsonPolygon,
            RoomsCount = b.Rooms.Count
        });
    }

    public async Task<BuildingDto?> GetBuildingByIdAsync(int id)
    {
        var building = await _context.Buildings
            .Include(b => b.Rooms.Where(r => r.IsActive))
            .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);

        if (building == null) return null;

        return new BuildingDto
        {
            Id = building.Id,
            Name = building.Name,
            Description = building.Description,
            Address = building.Address,
            Latitude = building.Latitude,
            Longitude = building.Longitude,
            GeoJsonPolygon = building.GeoJsonPolygon,
            RoomsCount = building.Rooms.Count
        };
    }

    public async Task<BuildingDto> CreateBuildingAsync(CreateBuildingRequest request)
    {
        var building = new Building
        {
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            GeoJsonPolygon = request.GeoJsonPolygon,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Buildings.Add(building);
        await _context.SaveChangesAsync();

        return new BuildingDto
        {
            Id = building.Id,
            Name = building.Name,
            Description = building.Description,
            Address = building.Address,
            Latitude = building.Latitude,
            Longitude = building.Longitude,
            GeoJsonPolygon = building.GeoJsonPolygon,
            RoomsCount = 0
        };
    }

    public async Task<BuildingDto> UpdateBuildingAsync(int id, UpdateBuildingRequest request)
    {
        var building = await _context.Buildings.FindAsync(id);
        if (building == null) throw new Exception("Building not found");

        if (request.Name != null) building.Name = request.Name;
        if (request.Description != null) building.Description = request.Description;
        if (request.Address != null) building.Address = request.Address;
        if (request.Latitude.HasValue) building.Latitude = request.Latitude.Value;
        if (request.Longitude.HasValue) building.Longitude = request.Longitude.Value;
        if (request.GeoJsonPolygon != null) building.GeoJsonPolygon = request.GeoJsonPolygon;

        await _context.SaveChangesAsync();

        var roomsCount = await _context.Rooms.CountAsync(r => r.BuildingId == id && r.IsActive);

        return new BuildingDto
        {
            Id = building.Id,
            Name = building.Name,
            Description = building.Description,
            Address = building.Address,
            Latitude = building.Latitude,
            Longitude = building.Longitude,
            GeoJsonPolygon = building.GeoJsonPolygon,
            RoomsCount = roomsCount
        };
    }

    public async Task<bool> DeleteBuildingAsync(int id)
    {
        var building = await _context.Buildings.FindAsync(id);
        if (building == null) return false;

        building.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    // Room operations
    public async Task<IEnumerable<RoomDto>> GetRoomsByBuildingAsync(int buildingId)
    {
        var rooms = await _context.Rooms
            .Include(r => r.Building)
            .Where(r => r.BuildingId == buildingId && r.IsActive)
            .ToListAsync();

        var roomDtos = new List<RoomDto>();
        foreach (var room in rooms)
        {
            var status = await GetRoomCurrentStatusAsync(room.Id);
            var (occupiedUntil, nextOccupiedAt) = await GetRoomOccupancyTimesAsync(room.Id);

            roomDtos.Add(new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Floor = room.Floor,
                Equipment = room.Equipment,
                BuildingId = room.BuildingId,
                BuildingName = room.Building.Name,
                CurrentStatus = status,
                OccupiedUntil = occupiedUntil,
                NextOccupiedAt = nextOccupiedAt
            });
        }

        return roomDtos;
    }

    public async Task<RoomDetailsDto?> GetRoomDetailsAsync(int roomId)
    {
        var room = await _context.Rooms
            .Include(r => r.Building)
            .FirstOrDefaultAsync(r => r.Id == roomId && r.IsActive);

        if (room == null) return null;

        var status = await GetRoomCurrentStatusAsync(roomId);
        var (occupiedUntil, nextOccupiedAt) = await GetRoomOccupancyTimesAsync(roomId);
        var todaySchedules = await GetRoomSchedulesTodayAsync(roomId);

        return new RoomDetailsDto
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            Floor = room.Floor,
            Equipment = room.Equipment,
            BuildingId = room.BuildingId,
            BuildingName = room.Building.Name,
            CurrentStatus = status,
            OccupiedUntil = occupiedUntil,
            NextOccupiedAt = nextOccupiedAt,
            TodaySchedules = todaySchedules.ToList()
        };
    }

    public async Task<RoomDto> CreateRoomAsync(CreateRoomRequest request)
    {
        var building = await _context.Buildings.FindAsync(request.BuildingId);
        if (building == null) throw new Exception("Building not found");

        var room = new Room
        {
            Name = request.Name,
            Capacity = request.Capacity,
            Floor = request.Floor,
            Equipment = request.Equipment,
            BuildingId = request.BuildingId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            Floor = room.Floor,
            Equipment = room.Equipment,
            BuildingId = room.BuildingId,
            BuildingName = building.Name,
            CurrentStatus = RoomStatus.Free,
            OccupiedUntil = null,
            NextOccupiedAt = null
        };
    }

    public async Task<RoomDto> UpdateRoomAsync(int id, UpdateRoomRequest request)
    {
        var room = await _context.Rooms.Include(r => r.Building).FirstOrDefaultAsync(r => r.Id == id);
        if (room == null) throw new Exception("Room not found");

        if (request.Name != null) room.Name = request.Name;
        if (request.Capacity.HasValue) room.Capacity = request.Capacity;
        if (request.Floor != null) room.Floor = request.Floor;
        if (request.Equipment != null) room.Equipment = request.Equipment;

        await _context.SaveChangesAsync();

        var status = await GetRoomCurrentStatusAsync(id);
        var (occupiedUntil, nextOccupiedAt) = await GetRoomOccupancyTimesAsync(id);

        return new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            Floor = room.Floor,
            Equipment = room.Equipment,
            BuildingId = room.BuildingId,
            BuildingName = room.Building.Name,
            CurrentStatus = status,
            OccupiedUntil = occupiedUntil,
            NextOccupiedAt = nextOccupiedAt
        };
    }

    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return false;

        room.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    // Room availability (real-time calculation)
    public async Task<RoomStatus> GetRoomCurrentStatusAsync(int roomId)
    {
        var now = DateTime.Now;
        var today = now.Date;

        var schedules = await _context.Schedules
            .Where(s => s.RoomId == roomId && s.IsActive)
            .ToListAsync();

        // Get today's effective schedules (including weekly recurrence and approved reservations)
        var todaySchedules = GetEffectiveSchedulesForDate(schedules, today);

        // Check if currently occupied
        var currentSchedule = todaySchedules.FirstOrDefault(s =>
            s.StartTime <= now && s.EndTime > now);
        if (currentSchedule != null)
            return RoomStatus.Occupied;

        // Check if occupied in next 30 minutes
        var soonSchedule = todaySchedules.FirstOrDefault(s =>
            s.StartTime > now && s.StartTime <= now.AddMinutes(30));
        if (soonSchedule != null)
            return RoomStatus.OccupiedSoon;

        return RoomStatus.Free;
    }

    public async Task<IEnumerable<RoomDto>> GetAvailableRoomsNowAsync()
    {
        var allRooms = await _context.Rooms
            .Include(r => r.Building)
            .Where(r => r.IsActive)
            .ToListAsync();

        var availableRooms = new List<RoomDto>();
        foreach (var room in allRooms)
        {
            var status = await GetRoomCurrentStatusAsync(room.Id);
            if (status == RoomStatus.Free)
            {
                var (occupiedUntil, nextOccupiedAt) = await GetRoomOccupancyTimesAsync(room.Id);
                availableRooms.Add(new RoomDto
                {
                    Id = room.Id,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Floor = room.Floor,
                    Equipment = room.Equipment,
                    BuildingId = room.BuildingId,
                    BuildingName = room.Building.Name,
                    CurrentStatus = status,
                    OccupiedUntil = occupiedUntil,
                    NextOccupiedAt = nextOccupiedAt
                });
            }
        }

        return availableRooms;
    }

    // Schedule operations
    public async Task<IEnumerable<ScheduleDto>> GetRoomSchedulesAsync(int roomId, DateTime? date)
    {
        var targetDate = date?.Date ?? DateTime.Today;

        // Get all schedules (including approved reservations which are now in the Schedules table)
        var schedules = await _context.Schedules
            .Include(s => s.Room)
                .ThenInclude(r => r.Building)
            .Include(s => s.CreatedByProfessor)
            .Where(s => s.RoomId == roomId && s.IsActive)
            .ToListAsync();

        var effectiveSchedules = GetEffectiveSchedulesForDate(schedules, targetDate);
        var now = DateTime.Now;

        var scheduleDtos = effectiveSchedules.Select(s => new ScheduleDto
        {
            Id = s.Id,
            Title = s.Title,
            Description = s.Description,
            RoomId = s.RoomId,
            RoomName = s.Room.Name,
            BuildingId = s.Room.BuildingId,
            BuildingName = s.Room.Building.Name,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            RecurrencePattern = s.RecurrencePattern,
            RecurrenceEndDate = s.RecurrenceEndDate,
            ProfessorName = $"{s.CreatedByProfessor.FirstName} {s.CreatedByProfessor.LastName}",
            IsCurrentlyActive = s.StartTime <= now && s.EndTime > now
        }).OrderBy(s => s.StartTime);

        return scheduleDtos;
    }

    public async Task<IEnumerable<ScheduleDto>> GetRoomSchedulesTodayAsync(int roomId)
    {
        return await GetRoomSchedulesAsync(roomId, DateTime.Today);
    }

    public async Task<ScheduleDto> CreateScheduleAsync(CreateScheduleRequest request)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null) throw new UnauthorizedAccessException("User not authenticated");

        // Validate booking time (8 AM - 8 PM)
        if (!IsValidBookingTime(request.StartTime, request.EndTime))
        {
            throw new InvalidOperationException("Rezervările pot fi făcute doar între orele 8:00 și 20:00.");
        }

        // Check for overlapping schedules
        var isAvailable = await IsTimeSlotAvailableAsync(request.RoomId, request.StartTime, request.EndTime);
        if (!isAvailable)
        {
            throw new InvalidOperationException("Intervalul orar selectat se suprapune cu o rezervare existentă.");
        }

        var schedule = new Schedule
        {
            Title = request.Title,
            Description = request.Description,
            RoomId = request.RoomId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            RecurrencePattern = request.RecurrencePattern,
            RecurrenceEndDate = request.RecurrenceEndDate,
            CreatedByProfessorId = userId.Value,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();

        var room = await _context.Rooms.Include(r => r.Building).FirstAsync(r => r.Id == request.RoomId);
        var professor = await _context.Users.FindAsync(userId.Value);

        return new ScheduleDto
        {
            Id = schedule.Id,
            Title = schedule.Title,
            Description = schedule.Description,
            RoomId = schedule.RoomId,
            RoomName = room.Name,
            BuildingId = room.BuildingId,
            BuildingName = room.Building.Name,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            RecurrencePattern = schedule.RecurrencePattern,
            RecurrenceEndDate = schedule.RecurrenceEndDate,
            ProfessorName = $"{professor!.FirstName} {professor.LastName}",
            IsCurrentlyActive = false
        };
    }

    public async Task<ScheduleDto> UpdateScheduleAsync(int id, UpdateScheduleRequest request)
    {
        var schedule = await _context.Schedules
            .Include(s => s.Room)
                .ThenInclude(r => r.Building)
            .Include(s => s.CreatedByProfessor)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (schedule == null) throw new Exception("Schedule not found");

        if (request.Title != null) schedule.Title = request.Title;
        if (request.Description != null) schedule.Description = request.Description;
        if (request.StartTime.HasValue) schedule.StartTime = request.StartTime.Value;
        if (request.EndTime.HasValue) schedule.EndTime = request.EndTime.Value;
        if (request.RecurrencePattern != null) schedule.RecurrencePattern = request.RecurrencePattern;
        if (request.RecurrenceEndDate.HasValue) schedule.RecurrenceEndDate = request.RecurrenceEndDate;

        await _context.SaveChangesAsync();

        var now = DateTime.Now;
        return new ScheduleDto
        {
            Id = schedule.Id,
            Title = schedule.Title,
            Description = schedule.Description,
            RoomId = schedule.RoomId,
            RoomName = schedule.Room.Name,
            BuildingId = schedule.Room.BuildingId,
            BuildingName = schedule.Room.Building.Name,
            StartTime = schedule.StartTime,
            EndTime = schedule.EndTime,
            RecurrencePattern = schedule.RecurrencePattern,
            RecurrenceEndDate = schedule.RecurrenceEndDate,
            ProfessorName = $"{schedule.CreatedByProfessor.FirstName} {schedule.CreatedByProfessor.LastName}",
            IsCurrentlyActive = schedule.StartTime <= now && schedule.EndTime > now
        };
    }

    public async Task<bool> DeleteScheduleAsync(int id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null) return false;

        schedule.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    // Room reservation operations
    public async Task<RoomReservationDto> CreateReservationAsync(CreateReservationRequest request)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null) throw new UnauthorizedAccessException("User not authenticated");

        // Validate booking time (8 AM - 8 PM)
        if (!IsValidBookingTime(request.StartTime, request.EndTime))
        {
            throw new InvalidOperationException("Rezervările pot fi făcute doar între orele 8:00 și 20:00.");
        }

        // Check for overlapping schedules or approved reservations
        var isAvailable = await IsTimeSlotAvailableAsync(request.RoomId, request.StartTime, request.EndTime);
        if (!isAvailable)
        {
            throw new InvalidOperationException("Intervalul orar selectat se suprapune cu o rezervare existentă.");
        }

        var reservation = new RoomReservation
        {
            Title = request.Title,
            Description = request.Description,
            RoomId = request.RoomId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            RequestedByUserId = userId.Value,
            Status = ReservationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.RoomReservations.Add(reservation);

        // Create notifications for all admins
        var admins = await _userManager.GetUsersInRoleAsync("Admin");
        var user = await _context.Users.FindAsync(userId.Value);
        var room = await _context.Rooms.Include(r => r.Building).FirstAsync(r => r.Id == request.RoomId);

        foreach (var admin in admins)
        {
            var notification = new Notification
            {
                UserId = admin.Id,
                Message = $"Cerere nouă de rezervare sală: {room.Name} ({room.Building.Name}) de la {user!.FirstName} {user.LastName} pentru {request.StartTime:dd.MM.yyyy HH:mm} - {request.EndTime:HH:mm}",
                RelatedEntityType = "RoomReservation",
                RelatedEntityId = reservation.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync();

        return new RoomReservationDto
        {
            Id = reservation.Id,
            Title = reservation.Title,
            Description = reservation.Description,
            RoomId = reservation.RoomId,
            RoomName = room.Name,
            BuildingId = room.BuildingId,
            BuildingName = room.Building.Name,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime,
            RequestedByUserId = reservation.RequestedByUserId,
            RequestedByUserName = $"{user!.FirstName} {user.LastName}",
            Status = reservation.Status.ToString(),
            CreatedAt = reservation.CreatedAt
        };
    }

    public async Task<IEnumerable<RoomReservationDto>> GetPendingReservationsAsync()
    {
        var reservations = await _context.RoomReservations
            .Include(r => r.Room)
                .ThenInclude(r => r.Building)
            .Include(r => r.RequestedByUser)
            .Where(r => r.Status == ReservationStatus.Pending)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

        return reservations.Select(r => new RoomReservationDto
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            RoomId = r.RoomId,
            RoomName = r.Room.Name,
            BuildingId = r.Room.BuildingId,
            BuildingName = r.Room.Building.Name,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            RequestedByUserId = r.RequestedByUserId,
            RequestedByUserName = $"{r.RequestedByUser.FirstName} {r.RequestedByUser.LastName}",
            Status = r.Status.ToString(),
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<IEnumerable<RoomReservationDto>> GetUserReservationsAsync()
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null) throw new UnauthorizedAccessException("User not authenticated");

        var reservations = await _context.RoomReservations
            .Include(r => r.Room)
                .ThenInclude(r => r.Building)
            .Include(r => r.RequestedByUser)
            .Include(r => r.ProcessedByAdmin)
            .Where(r => r.RequestedByUserId == userId.Value)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reservations.Select(r => new RoomReservationDto
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            RoomId = r.RoomId,
            RoomName = r.Room.Name,
            BuildingId = r.Room.BuildingId,
            BuildingName = r.Room.Building.Name,
            StartTime = r.StartTime,
            EndTime = r.EndTime,
            RequestedByUserId = r.RequestedByUserId,
            RequestedByUserName = $"{r.RequestedByUser.FirstName} {r.RequestedByUser.LastName}",
            Status = r.Status.ToString(),
            ProcessedByAdminId = r.ProcessedByAdminId,
            ProcessedByAdminName = r.ProcessedByAdmin != null ? $"{r.ProcessedByAdmin.FirstName} {r.ProcessedByAdmin.LastName}" : null,
            RejectionReason = r.RejectionReason,
            CreatedAt = r.CreatedAt,
            ProcessedAt = r.ProcessedAt
        });
    }

    public async Task<RoomReservationDto> ProcessReservationAsync(int reservationId, ProcessReservationRequest request)
    {
        var adminId = _currentUserService.GetCurrentUserId();
        if (adminId == null) throw new UnauthorizedAccessException("User not authenticated");

        var reservation = await _context.RoomReservations
            .Include(r => r.Room)
                .ThenInclude(r => r.Building)
            .Include(r => r.RequestedByUser)
            .FirstOrDefaultAsync(r => r.Id == reservationId);

        if (reservation == null) throw new Exception("Reservation not found");
        if (reservation.Status != ReservationStatus.Pending) throw new InvalidOperationException("Reservation is not pending");

        var admin = await _context.Users.FindAsync(adminId.Value);

        if (request.Approve)
        {
            // Check if time slot is still available
            var isAvailable = await IsTimeSlotAvailableAsync(reservation.RoomId, reservation.StartTime, reservation.EndTime, reservationId);
            if (!isAvailable)
            {
                throw new InvalidOperationException("Intervalul orar nu mai este disponibil. Există o suprapunere cu o altă rezervare.");
            }

            reservation.Status = ReservationStatus.Approved;
            reservation.ProcessedByAdminId = adminId.Value;
            reservation.ProcessedAt = DateTime.UtcNow;

            // Create schedule from the approved reservation
            var schedule = new Schedule
            {
                Title = reservation.Title,
                Description = reservation.Description,
                RoomId = reservation.RoomId,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                CreatedByProfessorId = adminId.Value, // Admin who approved
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Schedules.Add(schedule);

            // Notify user of approval
            var approvalNotification = new Notification
            {
                UserId = reservation.RequestedByUserId,
                Message = $"Cererea dvs. de rezervare pentru sala {reservation.Room.Name} ({reservation.Room.Building.Name}) pe {reservation.StartTime:dd.MM.yyyy HH:mm} - {reservation.EndTime:HH:mm} a fost APROBATĂ.",
                RelatedEntityType = "RoomReservation",
                RelatedEntityId = reservation.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(approvalNotification);
        }
        else
        {
            reservation.Status = ReservationStatus.Rejected;
            reservation.ProcessedByAdminId = adminId.Value;
            reservation.ProcessedAt = DateTime.UtcNow;
            reservation.RejectionReason = request.RejectionReason;

            // Notify user of rejection
            var rejectionNotification = new Notification
            {
                UserId = reservation.RequestedByUserId,
                Message = $"Cererea dvs. de rezervare pentru sala {reservation.Room.Name} ({reservation.Room.Building.Name}) pe {reservation.StartTime:dd.MM.yyyy HH:mm} - {reservation.EndTime:HH:mm} a fost RESPINSĂ. Motiv: {request.RejectionReason ?? "Nespecificat"}",
                RelatedEntityType = "RoomReservation",
                RelatedEntityId = reservation.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(rejectionNotification);
        }

        await _context.SaveChangesAsync();

        return new RoomReservationDto
        {
            Id = reservation.Id,
            Title = reservation.Title,
            Description = reservation.Description,
            RoomId = reservation.RoomId,
            RoomName = reservation.Room.Name,
            BuildingId = reservation.Room.BuildingId,
            BuildingName = reservation.Room.Building.Name,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime,
            RequestedByUserId = reservation.RequestedByUserId,
            RequestedByUserName = $"{reservation.RequestedByUser.FirstName} {reservation.RequestedByUser.LastName}",
            Status = reservation.Status.ToString(),
            ProcessedByAdminId = reservation.ProcessedByAdminId,
            ProcessedByAdminName = $"{admin!.FirstName} {admin.LastName}",
            RejectionReason = reservation.RejectionReason,
            CreatedAt = reservation.CreatedAt,
            ProcessedAt = reservation.ProcessedAt
        };
    }

    public async Task<bool> CancelReservationAsync(int reservationId)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null) throw new UnauthorizedAccessException("User not authenticated");

        var reservation = await _context.RoomReservations.FindAsync(reservationId);
        if (reservation == null) return false;

        // Only the user who created the reservation can cancel it (if pending)
        if (reservation.RequestedByUserId != userId.Value)
            throw new UnauthorizedAccessException("You can only cancel your own reservations");

        if (reservation.Status != ReservationStatus.Pending)
            throw new InvalidOperationException("Only pending reservations can be cancelled");

        _context.RoomReservations.Remove(reservation);
        await _context.SaveChangesAsync();
        return true;
    }

    // Validation helpers
    public async Task<bool> IsTimeSlotAvailableAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeReservationId = null)
    {
        var targetDate = startTime.Date;

        // Check against existing schedules
        var schedules = await _context.Schedules
            .Where(s => s.RoomId == roomId && s.IsActive)
            .ToListAsync();

        var effectiveSchedules = GetEffectiveSchedulesForDate(schedules, targetDate);

        foreach (var schedule in effectiveSchedules)
        {
            // Check for overlap: two intervals overlap if one starts before the other ends
            if (startTime < schedule.EndTime && endTime > schedule.StartTime)
            {
                return false;
            }
        }

        // Check against pending/approved reservations for the same date
        var reservations = await _context.RoomReservations
            .Where(r => r.RoomId == roomId &&
                        r.Status != ReservationStatus.Rejected &&
                        r.StartTime.Date == targetDate &&
                        (excludeReservationId == null || r.Id != excludeReservationId))
            .ToListAsync();

        foreach (var reservation in reservations)
        {
            if (startTime < reservation.EndTime && endTime > reservation.StartTime)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsValidBookingTime(DateTime startTime, DateTime endTime)
    {
        // Valid booking hours: 8:00 AM to 8:00 PM (20:00)
        var startHour = startTime.Hour;
        var endHour = endTime.Hour;
        var endMinute = endTime.Minute;

        // Start must be >= 8:00
        if (startHour < 8) return false;

        // End must be <= 20:00 (8 PM)
        if (endHour > 20 || (endHour == 20 && endMinute > 0)) return false;

        // End must be after start
        if (endTime <= startTime) return false;

        // Start and end must be on the same day
        if (startTime.Date != endTime.Date) return false;

        return true;
    }

    // Helper methods
    private List<Schedule> GetEffectiveSchedulesForDate(List<Schedule> schedules, DateTime date)
    {
        var effectiveSchedules = new List<Schedule>();

        foreach (var schedule in schedules)
        {
            // Check if date is before the schedule starts or after recurrence ends
            if (date < schedule.StartTime.Date)
                continue;

            if (schedule.RecurrenceEndDate.HasValue && date > schedule.RecurrenceEndDate.Value.Date)
                continue;

            bool shouldInclude = false;

            if (string.IsNullOrEmpty(schedule.RecurrencePattern))
            {
                // One-time schedule
                shouldInclude = schedule.StartTime.Date == date;
            }
            else if (schedule.RecurrencePattern == "Daily")
            {
                // Daily recurrence - every day from start to end
                shouldInclude = true;
            }
            else if (schedule.RecurrencePattern == "Weekly")
            {
                // Weekly recurrence - same day of week
                shouldInclude = schedule.StartTime.DayOfWeek == date.DayOfWeek;
            }
            else if (schedule.RecurrencePattern == "BiWeekly")
            {
                // BiWeekly recurrence - every 2 weeks on the same day
                if (schedule.StartTime.DayOfWeek == date.DayOfWeek)
                {
                    var daysDiff = (date - schedule.StartTime.Date).Days;
                    shouldInclude = daysDiff >= 0 && daysDiff % 14 == 0;
                }
            }
            else if (schedule.RecurrencePattern == "Monthly")
            {
                // Monthly recurrence - same day of month
                shouldInclude = schedule.StartTime.Day == date.Day;
            }

            if (shouldInclude)
            {
                var effectiveSchedule = new Schedule
                {
                    Id = schedule.Id,
                    Title = schedule.Title,
                    Description = schedule.Description,
                    RoomId = schedule.RoomId,
                    Room = schedule.Room,
                    StartTime = date.Add(schedule.StartTime.TimeOfDay),
                    EndTime = date.Add(schedule.EndTime.TimeOfDay),
                    RecurrencePattern = schedule.RecurrencePattern,
                    RecurrenceEndDate = schedule.RecurrenceEndDate,
                    CreatedByProfessorId = schedule.CreatedByProfessorId,
                    CreatedByProfessor = schedule.CreatedByProfessor,
                    IsActive = schedule.IsActive,
                    CreatedAt = schedule.CreatedAt
                };
                effectiveSchedules.Add(effectiveSchedule);
            }
        }

        return effectiveSchedules;
    }

    private async Task<(DateTime? occupiedUntil, DateTime? nextOccupiedAt)> GetRoomOccupancyTimesAsync(int roomId)
    {
        var now = DateTime.Now;
        var today = now.Date;

        var schedules = await _context.Schedules
            .Where(s => s.RoomId == roomId && s.IsActive)
            .ToListAsync();

        var todaySchedules = GetEffectiveSchedulesForDate(schedules, today)
            .OrderBy(s => s.StartTime)
            .ToList();

        // Find current schedule
        var currentSchedule = todaySchedules.FirstOrDefault(s =>
            s.StartTime <= now && s.EndTime > now);

        DateTime? occupiedUntil = currentSchedule?.EndTime;

        // Find next schedule
        var nextSchedule = todaySchedules.FirstOrDefault(s => s.StartTime > now);
        DateTime? nextOccupiedAt = nextSchedule?.StartTime;

        return (occupiedUntil, nextOccupiedAt);
    }
}
