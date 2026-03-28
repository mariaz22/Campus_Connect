using CampusConnect.Api.Controllers.Facilities;
using CampusConnect.Application.DTOs.CampusMap;
using CampusConnect.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace CampusConnect.Tests.Controllers.Facilities;

public class BuildingControllerTests
{
    private readonly Mock<ICampusMapService> _mockCampusMapService;
    private readonly BuildingController _controller;

    public BuildingControllerTests()
    {
        _mockCampusMapService = new Mock<ICampusMapService>();
        _controller = new BuildingController(_mockCampusMapService.Object);
    }

    private void SetupUserContext(int userId, string role = "User")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    #region GetAllBuildings Tests

    [Fact]
    public async Task GetAllBuildings_ReturnsOkWithBuildings()
    {
        // Arrange
        SetupUserContext(1);

        var buildings = new List<BuildingDto>
        {
            new BuildingDto
            {
                Id = 1,
                Name = "Facultatea de Matematică și Informatică",
                Address = "Str. Academiei nr. 14",
                Latitude = 44.4359,
                Longitude = 26.1000,
                RoomsCount = 50
            },
            new BuildingDto
            {
                Id = 2,
                Name = "Biblioteca Centrală",
                Address = "Bd. Regina Elisabeta nr. 4-12",
                Latitude = 44.4364,
                Longitude = 26.1023,
                RoomsCount = 20
            }
        };

        _mockCampusMapService
            .Setup(x => x.GetAllBuildingsAsync())
            .ReturnsAsync(buildings);

        // Act
        var result = await _controller.GetAllBuildings();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBuildings = okResult.Value.Should().BeAssignableTo<IEnumerable<BuildingDto>>().Subject;
        returnedBuildings.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllBuildings_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange
        SetupUserContext(1);

        _mockCampusMapService
            .Setup(x => x.GetAllBuildingsAsync())
            .ReturnsAsync(new List<BuildingDto>());

        // Act
        var result = await _controller.GetAllBuildings();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBuildings = okResult.Value.Should().BeAssignableTo<IEnumerable<BuildingDto>>().Subject;
        returnedBuildings.Should().BeEmpty();
    }

    #endregion

    #region GetBuildingById Tests

    [Fact]
    public async Task GetBuildingById_WithValidId_ReturnsOkWithBuilding()
    {
        // Arrange
        SetupUserContext(1);
        var buildingId = 1;

        var building = new BuildingDto
        {
            Id = buildingId,
            Name = "Facultatea de Matematică și Informatică",
            Description = "Clădirea principală a facultății",
            Address = "Str. Academiei nr. 14",
            Latitude = 44.4359,
            Longitude = 26.1000,
            RoomsCount = 50
        };

        _mockCampusMapService
            .Setup(x => x.GetBuildingByIdAsync(buildingId))
            .ReturnsAsync(building);

        // Act
        var result = await _controller.GetBuildingById(buildingId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBuilding = okResult.Value.Should().BeOfType<BuildingDto>().Subject;
        returnedBuilding.Id.Should().Be(buildingId);
        returnedBuilding.Name.Should().Be("Facultatea de Matematică și Informatică");
    }

    [Fact]
    public async Task GetBuildingById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        SetupUserContext(1);
        var buildingId = 999;

        _mockCampusMapService
            .Setup(x => x.GetBuildingByIdAsync(buildingId))
            .ReturnsAsync((BuildingDto?)null);

        // Act
        var result = await _controller.GetBuildingById(buildingId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region CreateBuilding Tests

    [Fact]
    public async Task CreateBuilding_AsAdmin_ReturnsCreatedAtAction()
    {
        // Arrange
        SetupUserContext(1, "Admin");

        var request = new CreateBuildingRequest
        {
            Name = "Clădire Nouă",
            Description = "Descriere clădire",
            Address = "Str. Nouă nr. 1",
            Latitude = 44.4400,
            Longitude = 26.1100
        };

        var createdBuilding = new BuildingDto
        {
            Id = 1,
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            RoomsCount = 0
        };

        _mockCampusMapService
            .Setup(x => x.CreateBuildingAsync(request))
            .ReturnsAsync(createdBuilding);

        // Act
        var result = await _controller.CreateBuilding(request);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedBuilding = createdResult.Value.Should().BeOfType<BuildingDto>().Subject;
        returnedBuilding.Name.Should().Be("Clădire Nouă");
    }

    [Fact]
    public async Task CreateBuilding_WithGeoJsonPolygon_ReturnsCreatedAtAction()
    {
        // Arrange
        SetupUserContext(1, "Admin");

        var geoJson = "{\"type\":\"Polygon\",\"coordinates\":[[[26.1,44.4],[26.11,44.4],[26.11,44.41],[26.1,44.41],[26.1,44.4]]]}";

        var request = new CreateBuildingRequest
        {
            Name = "Clădire cu Polygon",
            Address = "Str. Test nr. 1",
            Latitude = 44.4400,
            Longitude = 26.1100,
            GeoJsonPolygon = geoJson
        };

        var createdBuilding = new BuildingDto
        {
            Id = 1,
            Name = request.Name,
            Address = request.Address,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            GeoJsonPolygon = geoJson,
            RoomsCount = 0
        };

        _mockCampusMapService
            .Setup(x => x.CreateBuildingAsync(request))
            .ReturnsAsync(createdBuilding);

        // Act
        var result = await _controller.CreateBuilding(request);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var returnedBuilding = createdResult.Value.Should().BeOfType<BuildingDto>().Subject;
        returnedBuilding.GeoJsonPolygon.Should().Be(geoJson);
    }

    #endregion

    #region UpdateBuilding Tests

    [Fact]
    public async Task UpdateBuilding_AsAdmin_ReturnsOkWithUpdatedBuilding()
    {
        // Arrange
        SetupUserContext(1, "Admin");
        var buildingId = 1;

        var request = new UpdateBuildingRequest
        {
            Name = "Clădire Actualizată",
            Description = "Descriere actualizată",
            Address = "Str. Nouă nr. 2"
        };

        var updatedBuilding = new BuildingDto
        {
            Id = buildingId,
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            Latitude = 44.4359,
            Longitude = 26.1000,
            RoomsCount = 50
        };

        _mockCampusMapService
            .Setup(x => x.UpdateBuildingAsync(buildingId, request))
            .ReturnsAsync(updatedBuilding);

        // Act
        var result = await _controller.UpdateBuilding(buildingId, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBuilding = okResult.Value.Should().BeOfType<BuildingDto>().Subject;
        returnedBuilding.Name.Should().Be("Clădire Actualizată");
    }

    [Fact]
    public async Task UpdateBuilding_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        SetupUserContext(1, "Admin");
        var buildingId = 999;

        var request = new UpdateBuildingRequest
        {
            Name = "Test"
        };

        _mockCampusMapService
            .Setup(x => x.UpdateBuildingAsync(buildingId, request))
            .ThrowsAsync(new Exception("Building not found"));

        // Act
        var result = await _controller.UpdateBuilding(buildingId, request);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateBuilding_PartialUpdate_ReturnsOk()
    {
        // Arrange
        SetupUserContext(1, "Admin");
        var buildingId = 1;

        var request = new UpdateBuildingRequest
        {
            Description = "Doar descrierea actualizată"
        };

        var updatedBuilding = new BuildingDto
        {
            Id = buildingId,
            Name = "Clădire Originală",
            Description = request.Description,
            Address = "Str. Originală nr. 1",
            Latitude = 44.4359,
            Longitude = 26.1000,
            RoomsCount = 50
        };

        _mockCampusMapService
            .Setup(x => x.UpdateBuildingAsync(buildingId, request))
            .ReturnsAsync(updatedBuilding);

        // Act
        var result = await _controller.UpdateBuilding(buildingId, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedBuilding = okResult.Value.Should().BeOfType<BuildingDto>().Subject;
        returnedBuilding.Description.Should().Be("Doar descrierea actualizată");
    }

    #endregion

    #region DeleteBuilding Tests

    [Fact]
    public async Task DeleteBuilding_AsAdmin_ReturnsNoContent()
    {
        // Arrange
        SetupUserContext(1, "Admin");
        var buildingId = 1;

        _mockCampusMapService
            .Setup(x => x.DeleteBuildingAsync(buildingId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteBuilding(buildingId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteBuilding_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        SetupUserContext(1, "Admin");
        var buildingId = 999;

        _mockCampusMapService
            .Setup(x => x.DeleteBuildingAsync(buildingId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteBuilding(buildingId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion
}
