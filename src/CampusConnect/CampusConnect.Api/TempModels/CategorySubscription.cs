using System;
using System.Collections.Generic;

namespace CampusConnect.Api.TempModels;

public partial class CategorySubscription
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Category { get; set; } = null!;
}
