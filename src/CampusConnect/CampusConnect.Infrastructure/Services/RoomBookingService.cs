using CampusConnect.Application.DTOs.RoomBooking;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using CampusConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Infrastructure.Services;

public class RoomBookingService : IRoomBookingService
{
    private readonly ApplicationDbContext _context;

    private const int BOOKING_START_HOUR = 8;  // 8 AM
    private const int BOOKING_END_HOUR = 20;   // 8 PM

    public RoomBookingService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RoomBookingRequestDto> CreateBookingRequestAsync(CreateRoomBookingRequest request, int userId)
    {
        var room = await _context.Rooms
            .Include(r => r.Building)
            .FirstOrDefaultAsync(r => r.Id == request.RoomId && r.IsActive);

        if (room == null)
            throw new Exception("Room not found");

        // Validate booking time is within allowed hours (8 AM - 8 PM)
        ValidateBookingTime(request.StartTime, request.EndTime);

        var hasConflict = await CheckScheduleConflictAsync(request.RoomId, request.StartTime, request.EndTime, null);
        if (hasConflict)
            throw new Exception("Sala este deja rezervată în acest interval orar. Vă rugăm să alegeți alt interval.");

        var bookingRequest = new RoomBookingRequest
        {
            Title = request.Title,
            Description = request.Description,
            RoomId = request.RoomId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            RecurrencePattern = request.RecurrencePattern,
            RecurrenceEndDate = request.RecurrenceEndDate,
            RequestedByUserId = userId,
            Status = BookingRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.RoomBookingRequests.Add(bookingRequest);
        await _context.SaveChangesAsync();

        await NotifyAllAdminsAsync(bookingRequest.Id);

        return await MapToDto(bookingRequest);
    }

    public async Task<IEnumerable<RoomBookingRequestDto>> GetPendingRequestsAsync()
    {
        var requests = await _context.RoomBookingRequests
            .Where(r => r.Status == BookingRequestStatus.Pending && r.IsActive)
            .Include(r => r.Room)
                .ThenInclude(room => room.Building)
            .Include(r => r.RequestedByUser)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();

        return await Task.WhenAll(requests.Select(MapToDto));
    }

    public async Task<IEnumerable<RoomBookingRequestDto>> GetMyRequestsAsync(int userId)
    {
        var requests = await _context.RoomBookingRequests
            .Where(r => r.RequestedByUserId == userId && r.IsActive)
            .Include(r => r.Room)
                .ThenInclude(room => room.Building)
            .Include(r => r.RequestedByUser)
            .Include(r => r.ReviewedByAdmin)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return await Task.WhenAll(requests.Select(MapToDto));
    }

    public async Task<RoomBookingRequestDto?> GetRequestByIdAsync(int id)
    {
        var request = await _context.RoomBookingRequests
            .Include(r => r.Room)
                .ThenInclude(room => room.Building)
            .Include(r => r.RequestedByUser)
            .Include(r => r.ReviewedByAdmin)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

        if (request == null) return null;

        return await MapToDto(request);
    }

    public async Task<RoomBookingRequestDto> ApproveRequestAsync(int requestId, int adminId)
    {
        var request = await _context.RoomBookingRequests
            .Include(r => r.Room)
                .ThenInclude(room => room.Building)
            .Include(r => r.RequestedByUser)
            .FirstOrDefaultAsync(r => r.Id == requestId && r.IsActive);

        if (request == null)
            throw new Exception("Booking request not found");

        if (request.Status != BookingRequestStatus.Pending)
            throw new Exception("This request has already been reviewed");

        var hasConflict = await CheckScheduleConflictAsync(request.RoomId, request.StartTime, request.EndTime, requestId);
        if (hasConflict)
            throw new Exception("The room is now booked for this time slot. Cannot approve.");

        var schedule = new Schedule
        {
            Title = request.Title,
            Description = request.Description,
            RoomId = request.RoomId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            RecurrencePattern = request.RecurrencePattern,
            RecurrenceEndDate = request.RecurrenceEndDate,
            CreatedByProfessorId = request.RequestedByUserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Schedules.Add(schedule);

        request.Status = BookingRequestStatus.Approved;
        request.ReviewedByAdminId = adminId;
        request.ReviewedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await NotifyUserAsync(request.RequestedByUserId, $"Your booking request for '{request.Title}' has been approved!", "RoomBookingRequest", requestId);

        return await MapToDto(request);
    }

    public async Task<RoomBookingRequestDto> RejectRequestAsync(int requestId, int adminId, string? rejectionReason)
    {
        var request = await _context.RoomBookingRequests
            .Include(r => r.Room)
                .ThenInclude(room => room.Building)
            .Include(r => r.RequestedByUser)
            .FirstOrDefaultAsync(r => r.Id == requestId && r.IsActive);

        if (request == null)
            throw new Exception("Booking request not found");

        if (request.Status != BookingRequestStatus.Pending)
            throw new Exception("This request has already been reviewed");

        request.Status = BookingRequestStatus.Rejected;
        request.ReviewedByAdminId = adminId;
        request.ReviewedAt = DateTime.UtcNow;
        request.RejectionReason = rejectionReason;

        await _context.SaveChangesAsync();

        var message = $"Your booking request for '{request.Title}' has been rejected.";
        if (!string.IsNullOrEmpty(rejectionReason))
            message += $" Reason: {rejectionReason}";

        await NotifyUserAsync(request.RequestedByUserId, message, "RoomBookingRequest", requestId);

        return await MapToDto(request);
    }

    public async Task<bool> DeleteRequestAsync(int requestId, int userId)
    {
        var request = await _context.RoomBookingRequests
            .FirstOrDefaultAsync(r => r.Id == requestId && r.RequestedByUserId == userId && r.IsActive);

        if (request == null)
            return false;

        if (request.Status != BookingRequestStatus.Pending)
            throw new Exception("Cannot delete a request that has already been reviewed");

        request.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    private async Task<bool> CheckScheduleConflictAsync(int roomId, DateTime startTime, DateTime endTime, int? excludeRequestId)
    {
        var hasScheduleConflict = await _context.Schedules
            .AnyAsync(s => s.RoomId == roomId && s.IsActive &&
                          ((s.StartTime < endTime && s.EndTime > startTime)));

        if (hasScheduleConflict)
            return true;

        var hasPendingConflict = await _context.RoomBookingRequests
            .AnyAsync(r => r.RoomId == roomId &&
                          r.IsActive &&
                          r.Status == BookingRequestStatus.Pending &&
                          (excludeRequestId == null || r.Id != excludeRequestId) &&
                          ((r.StartTime < endTime && r.EndTime > startTime)));

        return hasPendingConflict;
    }

    private void ValidateBookingTime(DateTime startTime, DateTime endTime)
    {
        // Check if end time is after start time
        if (endTime <= startTime)
            throw new Exception("Ora de terminare trebuie să fie după ora de începere.");

        // Check if booking is within allowed hours (8 AM - 8 PM)
        if (startTime.Hour < BOOKING_START_HOUR || startTime.Hour >= BOOKING_END_HOUR)
            throw new Exception($"Rezervările sunt permise doar între {BOOKING_START_HOUR}:00 și {BOOKING_END_HOUR}:00. Ora de începere este în afara intervalului permis.");

        if (endTime.Hour < BOOKING_START_HOUR || endTime.Hour > BOOKING_END_HOUR || (endTime.Hour == BOOKING_END_HOUR && endTime.Minute > 0))
            throw new Exception($"Rezervările sunt permise doar între {BOOKING_START_HOUR}:00 și {BOOKING_END_HOUR}:00. Ora de terminare este în afara intervalului permis.");

        // Check if booking spans multiple days
        if (startTime.Date != endTime.Date)
            throw new Exception("Rezervările nu pot să se întindă pe mai multe zile.");
    }

    private async Task NotifyAllAdminsAsync(int requestId)
    {
        var adminRoleId = await _context.Roles
            .Where(r => r.Name == "Admin")
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        if (adminRoleId == 0)
            return;

        var adminIds = await _context.UserRoles
            .Where(ur => ur.RoleId == adminRoleId)
            .Select(ur => ur.UserId)
            .ToListAsync();

        var request = await _context.RoomBookingRequests
            .Include(r => r.Room)
            .Include(r => r.RequestedByUser)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request == null)
            return;

        var notifications = adminIds.Select(adminId => new Notification
        {
            UserId = adminId,
            Message = $"New room booking request from {request.RequestedByUser.FirstName} {request.RequestedByUser.LastName} for {request.Room.Name}",
            RelatedEntityType = "RoomBookingRequest",
            RelatedEntityId = requestId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        });

        _context.Notifications.AddRange(notifications);
        await _context.SaveChangesAsync();
    }

    private async Task NotifyUserAsync(int userId, string message, string entityType, int entityId)
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            RelatedEntityType = entityType,
            RelatedEntityId = entityId,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    private async Task<RoomBookingRequestDto> MapToDto(RoomBookingRequest request)
    {
        if (request.Room == null)
        {
            await _context.Entry(request).Reference(r => r.Room).LoadAsync();
            await _context.Entry(request.Room!).Reference(r => r.Building).LoadAsync();
        }

        if (request.RequestedByUser == null)
        {
            await _context.Entry(request).Reference(r => r.RequestedByUser).LoadAsync();
        }

        if (request.ReviewedByAdminId.HasValue && request.ReviewedByAdmin == null)
        {
            await _context.Entry(request).Reference(r => r.ReviewedByAdmin).LoadAsync();
        }

        return new RoomBookingRequestDto
        {
            Id = request.Id,
            Title = request.Title,
            Description = request.Description,
            RoomId = request.RoomId,
            RoomName = request.Room?.Name ?? string.Empty,
            BuildingId = request.Room?.BuildingId ?? 0,
            BuildingName = request.Room?.Building?.Name ?? string.Empty,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            RecurrencePattern = request.RecurrencePattern,
            RecurrenceEndDate = request.RecurrenceEndDate,
            RequestedByUserId = request.RequestedByUserId,
            RequestedByUserName = $"{request.RequestedByUser?.FirstName} {request.RequestedByUser?.LastName}",
            RequestedByUserEmail = request.RequestedByUser?.Email ?? string.Empty,
            Status = request.Status,
            ReviewedByAdminId = request.ReviewedByAdminId,
            ReviewedByAdminName = request.ReviewedByAdmin != null
                ? $"{request.ReviewedByAdmin.FirstName} {request.ReviewedByAdmin.LastName}"
                : null,
            ReviewedAt = request.ReviewedAt,
            RejectionReason = request.RejectionReason,
            CreatedAt = request.CreatedAt
        };
    }
}
