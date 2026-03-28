using CampusConnect.Application.DTOs.Grades;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using CampusConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Infrastructure.Services;

public class SubjectService : ISubjectService
{
    private readonly ApplicationDbContext _context;

    public SubjectService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SubjectDto> CreateSubjectAsync(int professorId, CreateSubjectRequest request)
    {
        // Check if code already exists
        var existingSubject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Code == request.Code);

        if (existingSubject != null)
        {
            throw new InvalidOperationException("A subject with this code already exists.");
        }

        var subject = new Subject
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Year = request.Year,
            ProfessorId = professorId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        return await GetSubjectDtoAsync(subject.Id);
    }

    public async Task<SubjectDto> UpdateSubjectAsync(int subjectId, int professorId, UpdateSubjectRequest request)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == subjectId && s.ProfessorId == professorId);

        if (subject == null)
        {
            throw new InvalidOperationException("Subject not found or you don't have permission to update it.");
        }

        subject.Name = request.Name;
        subject.Description = request.Description;
        subject.Year = request.Year;

        await _context.SaveChangesAsync();

        return await GetSubjectDtoAsync(subject.Id);
    }

    public async Task<bool> DeleteSubjectAsync(int subjectId, int professorId)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Id == subjectId && s.ProfessorId == professorId);

        if (subject == null)
        {
            return false;
        }

        subject.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<SubjectDto?> GetSubjectByIdAsync(int subjectId)
    {
        return await GetSubjectDtoAsync(subjectId);
    }

    public async Task<List<SubjectDto>> GetSubjectsByProfessorAsync(int professorId)
    {
        return await _context.Subjects
            .Where(s => s.ProfessorId == professorId && s.IsActive)
            .Include(s => s.Professor)
            .Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                Year = s.Year,
                ProfessorId = s.ProfessorId,
                ProfessorName = $"{s.Professor.FirstName} {s.Professor.LastName}",
                CreatedAt = s.CreatedAt,
                IsActive = s.IsActive
            })
            .OrderBy(s => s.Year)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<List<SubjectDto>> GetAllSubjectsAsync()
    {
        return await _context.Subjects
            .Where(s => s.IsActive)
            .Include(s => s.Professor)
            .Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                Description = s.Description,
                Year = s.Year,
                ProfessorId = s.ProfessorId,
                ProfessorName = $"{s.Professor.FirstName} {s.Professor.LastName}",
                CreatedAt = s.CreatedAt,
                IsActive = s.IsActive
            })
            .OrderBy(s => s.Year)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    private async Task<SubjectDto> GetSubjectDtoAsync(int subjectId)
    {
        var subject = await _context.Subjects
            .Include(s => s.Professor)
            .FirstOrDefaultAsync(s => s.Id == subjectId);

        if (subject == null)
        {
            throw new InvalidOperationException("Subject not found.");
        }

        return new SubjectDto
        {
            Id = subject.Id,
            Name = subject.Name,
            Code = subject.Code,
            Description = subject.Description,
            Year = subject.Year,
            ProfessorId = subject.ProfessorId,
            ProfessorName = $"{subject.Professor.FirstName} {subject.Professor.LastName}",
            CreatedAt = subject.CreatedAt,
            IsActive = subject.IsActive
        };
    }
}
