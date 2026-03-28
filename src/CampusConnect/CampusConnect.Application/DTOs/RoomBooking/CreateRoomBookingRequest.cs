namespace CampusConnect.Application.DTOs.RoomBooking;

public class CreateRoomBookingRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? RecurrencePattern { get; set; }
    public DateTime? RecurrenceEndDate { get; set; }
}
