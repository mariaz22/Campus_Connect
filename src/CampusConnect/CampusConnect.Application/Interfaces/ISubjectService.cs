using CampusConnect.Application.DTOs.Grades;

namespace CampusConnect.Application.Interfaces;

public interface ISubjectService
{
    Task<SubjectDto> CreateSubjectAsync(int professorId, CreateSubjectRequest request);
    Task<SubjectDto> UpdateSubjectAsync(int subjectId, int professorId, UpdateSubjectRequest request);
    Task<bool> DeleteSubjectAsync(int subjectId, int professorId);
    Task<SubjectDto?> GetSubjectByIdAsync(int subjectId);
    Task<List<SubjectDto>> GetSubjectsByProfessorAsync(int professorId);
    Task<List<SubjectDto>> GetAllSubjectsAsync();
}
