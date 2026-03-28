using CampusConnect.Application.DTOs.RoomBooking;

namespace CampusConnect.Application.Interfaces;

public interface IRoomBookingService
{
    Task<RoomBookingRequestDto> CreateBookingRequestAsync(CreateRoomBookingRequest request, int userId);
    Task<IEnumerable<RoomBookingRequestDto>> GetPendingRequestsAsync();
    Task<IEnumerable<RoomBookingRequestDto>> GetMyRequestsAsync(int userId);
    Task<RoomBookingRequestDto?> GetRequestByIdAsync(int id);
    Task<RoomBookingRequestDto> ApproveRequestAsync(int requestId, int adminId);
    Task<RoomBookingRequestDto> RejectRequestAsync(int requestId, int adminId, string? rejectionReason);
    Task<bool> DeleteRequestAsync(int requestId, int userId);
}
