namespace CampusConnect.Application.DTOs.CampusMap;

public class BuildingDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? GeoJsonPolygon { get; set; }
    public int RoomsCount { get; set; }
}

public class CreateBuildingRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Address { get; set; }
    public required double Latitude { get; set; }
    public required double Longitude { get; set; }
    public string? GeoJsonPolygon { get; set; }
}

public class UpdateBuildingRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? GeoJsonPolygon { get; set; }
}
