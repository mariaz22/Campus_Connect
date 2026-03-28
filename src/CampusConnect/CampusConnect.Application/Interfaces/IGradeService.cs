using CampusConnect.Application.DTOs.Grades;

namespace CampusConnect.Application.Interfaces;

public interface IGradeService
{
    Task<GradeDto> CreateGradeAsync(int professorId, CreateGradeRequest request);
    Task<GradeDto> UpdateGradeAsync(int gradeId, int professorId, UpdateGradeRequest request);
    Task<bool> DeleteGradeAsync(int gradeId, int professorId);
    Task<GradeDto?> GetGradeByIdAsync(int gradeId);
    Task<List<GradeDto>> GetGradesBySubjectAsync(int subjectId);
    Task<List<GradeDto>> GetGradesByStudentAsync(int studentId);
    Task<StudentGradesResponse> GetStudentGradesGroupedAsync(int studentId);
    Task<List<GradeDto>> GetGradesBySubjectAndStudentAsync(int subjectId, int studentId);
}
