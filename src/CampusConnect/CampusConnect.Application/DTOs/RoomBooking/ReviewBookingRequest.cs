namespace CampusConnect.Application.DTOs.RoomBooking;

public class ReviewBookingRequest
{
    public bool Approve { get; set; }
    public string? RejectionReason { get; set; }
}
