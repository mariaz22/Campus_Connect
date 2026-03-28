namespace CampusConnect.Application.DTOs.Grades;

public class CreateSubjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Year { get; set; } = 1; // Anul de studiu: 1, 2 sau 3
}

public class UpdateSubjectRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Year { get; set; } = 1; // Anul de studiu: 1, 2 sau 3
}

public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Year { get; set; } = 1; // Anul de studiu: 1, 2 sau 3
    public int ProfessorId { get; set; }
    public string ProfessorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class CreateGradeRequest
{
    public int SubjectId { get; set; }
    public int StudentId { get; set; }
    public decimal Value { get; set; }
    public string? Comments { get; set; }
}

public class UpdateGradeRequest
{
    public decimal Value { get; set; }
    public string? Comments { get; set; }
}

public class GradeDto
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string? Comments { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int CreatedByProfessorId { get; set; }
    public string CreatedByProfessorName { get; set; } = string.Empty;
}

public class StudentGradesResponse
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentEmail { get; set; } = string.Empty;
    public List<SubjectGrades> SubjectGrades { get; set; } = new();
}

public class SubjectGrades
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectCode { get; set; } = string.Empty;
    public int Year { get; set; } = 1; // Anul de studiu
    public string ProfessorName { get; set; } = string.Empty;
    public List<GradeDto> Grades { get; set; } = new();
    public decimal? AverageGrade { get; set; }
}
