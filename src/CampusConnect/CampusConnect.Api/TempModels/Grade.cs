using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class Grade
{
    public int Id { get; set; }

    public int SubjectId { get; set; }

    public int StudentId { get; set; }

    public decimal Value { get; set; }

    public string? Comments { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedByProfessorId { get; set; }

    public virtual User CreatedByProfessor { get; set; } = null!;

    public virtual User Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
