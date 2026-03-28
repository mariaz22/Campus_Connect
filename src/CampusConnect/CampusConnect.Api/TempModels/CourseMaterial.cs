using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class CourseMaterial
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string FileName { get; set; } = null!;

    public string FileUrl { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public long FileSize { get; set; }

    public int GroupId { get; set; }

    public int UploadedByProfessorId { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual User UploadedByProfessor { get; set; } = null!;
}
