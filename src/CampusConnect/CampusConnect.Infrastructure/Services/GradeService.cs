using CampusConnect.Application.DTOs.Grades;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using CampusConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Infrastructure.Services;

public class GradeService : IGradeService
{
    private readonly ApplicationDbContext _context;

    public GradeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GradeDto> CreateGradeAsync(int professorId, CreateGradeRequest request)
    {
        // Verify subject belongs to professor
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == request.SubjectId && s.ProfessorId == professorId);

        if (subject == null)
        {
            throw new InvalidOperationException("Subject not found or you don't have permission to add grades.");
        }

        // Verify student exists
        var student = await _context.Users.FindAsync(request.StudentId);
        if (student == null)
        {
            throw new InvalidOperationException("Student not found.");
        }

        var grade = new Grade
        {
            SubjectId = request.SubjectId,
            StudentId = request.StudentId,
            Value = request.Value,
            Comments = request.Comments,
            CreatedByProfessorId = professorId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Grades.Add(grade);
        await _context.SaveChangesAsync();

        return await GetGradeDtoAsync(grade.Id);
    }

    public async Task<GradeDto> UpdateGradeAsync(int gradeId, int professorId, UpdateGradeRequest request)
    {
        var grade = await _context.Grades
            .Include(g => g.Subject)
            .FirstOrDefaultAsync(g => g.Id == gradeId && g.Subject.ProfessorId == professorId);

        if (grade == null)
        {
            throw new InvalidOperationException("Grade not found or you don't have permission to update it.");
        }

        grade.Value = request.Value;
        grade.Comments = request.Comments;
        grade.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetGradeDtoAsync(grade.Id);
    }

    public async Task<bool> DeleteGradeAsync(int gradeId, int professorId)
    {
        var grade = await _context.Grades
            .Include(g => g.Subject)
            .FirstOrDefaultAsync(g => g.Id == gradeId && g.Subject.ProfessorId == professorId);

        if (grade == null)
        {
            return false;
        }

        _context.Grades.Remove(grade);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<GradeDto?> GetGradeByIdAsync(int gradeId)
    {
        return await GetGradeDtoAsync(gradeId);
    }

    public async Task<List<GradeDto>> GetGradesBySubjectAsync(int subjectId)
    {
        return await _context.Grades
            .Where(g => g.SubjectId == subjectId)
            .Include(g => g.Subject)
            .Include(g => g.Student)
            .Include(g => g.CreatedByProfessor)
            .Select(g => new GradeDto
            {
                Id = g.Id,
                SubjectId = g.SubjectId,
                SubjectName = g.Subject.Name,
                SubjectCode = g.Subject.Code,
                StudentId = g.StudentId,
                StudentName = $"{g.Student.FirstName} {g.Student.LastName}",
                StudentEmail = g.Student.Email,
                Value = g.Value,
                Comments = g.Comments,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                CreatedByProfessorId = g.CreatedByProfessorId,
                CreatedByProfessorName = $"{g.CreatedByProfessor.FirstName} {g.CreatedByProfessor.LastName}"
            })
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<GradeDto>> GetGradesByStudentAsync(int studentId)
    {
        return await _context.Grades
            .Where(g => g.StudentId == studentId)
            .Include(g => g.Subject)
            .Include(g => g.Student)
            .Include(g => g.CreatedByProfessor)
            .Select(g => new GradeDto
            {
                Id = g.Id,
                SubjectId = g.SubjectId,
                SubjectName = g.Subject.Name,
                SubjectCode = g.Subject.Code,
                StudentId = g.StudentId,
                StudentName = $"{g.Student.FirstName} {g.Student.LastName}",
                StudentEmail = g.Student.Email,
                Value = g.Value,
                Comments = g.Comments,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                CreatedByProfessorId = g.CreatedByProfessorId,
                CreatedByProfessorName = $"{g.CreatedByProfessor.FirstName} {g.CreatedByProfessor.LastName}"
            })
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<StudentGradesResponse> GetStudentGradesGroupedAsync(int studentId)
    {
        var student = await _context.Users.FindAsync(studentId);
        if (student == null)
        {
            throw new InvalidOperationException("Student not found.");
        }

        var grades = await _context.Grades
            .Where(g => g.StudentId == studentId)
            .Include(g => g.Subject)
                .ThenInclude(s => s.Professor)
            .Include(g => g.CreatedByProfessor)
            .OrderBy(g => g.Subject.Name)
            .ThenByDescending(g => g.CreatedAt)
            .ToListAsync();

        var groupedGrades = grades
            .GroupBy(g => new { g.SubjectId, g.Subject.Name, g.Subject.Code, g.Subject.Year, ProfessorName = $"{g.Subject.Professor.FirstName} {g.Subject.Professor.LastName}" })
            .Select(group => new SubjectGrades
            {
                SubjectId = group.Key.SubjectId,
                SubjectName = group.Key.Name,
                SubjectCode = group.Key.Code,
                Year = group.Key.Year,
                ProfessorName = group.Key.ProfessorName,
                Grades = group.Select(g => new GradeDto
                {
                    Id = g.Id,
                    SubjectId = g.SubjectId,
                    SubjectName = g.Subject.Name,
                    SubjectCode = g.Subject.Code,
                    StudentId = g.StudentId,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    StudentEmail = student.Email,
                    Value = g.Value,
                    Comments = g.Comments,
                    CreatedAt = g.CreatedAt,
                    UpdatedAt = g.UpdatedAt,
                    CreatedByProfessorId = g.CreatedByProfessorId,
                    CreatedByProfessorName = $"{g.CreatedByProfessor.FirstName} {g.CreatedByProfessor.LastName}"
                }).ToList(),
                AverageGrade = group.Any() ? Math.Round(group.Average(g => g.Value), 2) : null
            })
            .ToList();

        return new StudentGradesResponse
        {
            StudentId = studentId,
            StudentName = $"{student.FirstName} {student.LastName}",
            StudentEmail = student.Email,
            SubjectGrades = groupedGrades
        };
    }

    public async Task<List<GradeDto>> GetGradesBySubjectAndStudentAsync(int subjectId, int studentId)
    {
        return await _context.Grades
            .Where(g => g.SubjectId == subjectId && g.StudentId == studentId)
            .Include(g => g.Subject)
            .Include(g => g.Student)
            .Include(g => g.CreatedByProfessor)
            .Select(g => new GradeDto
            {
                Id = g.Id,
                SubjectId = g.SubjectId,
                SubjectName = g.Subject.Name,
                SubjectCode = g.Subject.Code,
                StudentId = g.StudentId,
                StudentName = $"{g.Student.FirstName} {g.Student.LastName}",
                StudentEmail = g.Student.Email,
                Value = g.Value,
                Comments = g.Comments,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt,
                CreatedByProfessorId = g.CreatedByProfessorId,
                CreatedByProfessorName = $"{g.CreatedByProfessor.FirstName} {g.CreatedByProfessor.LastName}"
            })
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    private async Task<GradeDto> GetGradeDtoAsync(int gradeId)
    {
        var grade = await _context.Grades
            .Include(g => g.Subject)
            .Include(g => g.Student)
            .Include(g => g.CreatedByProfessor)
            .FirstOrDefaultAsync(g => g.Id == gradeId);

        if (grade == null)
        {
            throw new InvalidOperationException("Grade not found.");
        }

        return new GradeDto
        {
            Id = grade.Id,
            SubjectId = grade.SubjectId,
            SubjectName = grade.Subject.Name,
            SubjectCode = grade.Subject.Code,
            StudentId = grade.StudentId,
            StudentName = $"{grade.Student.FirstName} {grade.Student.LastName}",
            StudentEmail = grade.Student.Email,
            Value = grade.Value,
            Comments = grade.Comments,
            CreatedAt = grade.CreatedAt,
            UpdatedAt = grade.UpdatedAt,
            CreatedByProfessorId = grade.CreatedByProfessorId,
            CreatedByProfessorName = $"{grade.CreatedByProfessor.FirstName} {grade.CreatedByProfessor.LastName}"
        };
    }
}