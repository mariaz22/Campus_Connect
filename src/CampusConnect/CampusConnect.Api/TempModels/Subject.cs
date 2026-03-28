using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public int ProfessorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public int Year { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual User Professor { get; set; } = null!;
}
