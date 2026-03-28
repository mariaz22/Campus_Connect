namespace CampusConnect.Application.Interfaces;

public interface IDocumentService
{
    Task<byte[]> GenerateEnrollmentCertificateAsync(int userId);
    Task<byte[]> GenerateTranscriptAsync(int userId);
}