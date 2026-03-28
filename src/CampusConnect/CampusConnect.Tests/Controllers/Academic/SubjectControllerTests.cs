using CampusConnect.Api.Controllers.Academic;
using CampusConnect.Application.DTOs.Grades;
using CampusConnect.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace CampusConnect.Tests.Controllers.Academic;

public class SubjectControllerTests
{
    private readonly Mock<ISubjectService> _mockSubjectService;
    private readonly SubjectController _controller;

    public SubjectControllerTests()
    {
        _mockSubjectService = new Mock<ISubjectService>();
        _controller = new SubjectController(_mockSubjectService.Object);
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

    #region CreateSubject Tests

    [Fact]
    public async Task CreateSubject_WithValidRequest_ReturnsOkWithSubject()
    {
        // Arrange
        var professorId = 1;
        SetupUserContext(professorId, "Professor");

        var request = new CreateSubjectRequest
        {
            Name = "Programare Web",
            Code = "PW101",
            Description = "Introducere în dezvoltarea web",
            Year = 2
        };

        var expectedSubject = new SubjectDto
        {
            Id = 1,
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            Year = request.Year,
            ProfessorId = professorId,
            ProfessorName = "Prof. Ionescu"
        };

        _mockSubjectService
            .Setup(x => x.CreateSubjectAsync(professorId, request))
            .ReturnsAsync(expectedSubject);

        // Act
        var result = await _controller.CreateSubject(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSubject = okResult.Value.Should().BeOfType<SubjectDto>().Subject;
        returnedSubject.Name.Should().Be("Programare Web");
        returnedSubject.Code.Should().Be("PW101");
    }

    [Fact]
    public async Task CreateSubject_WithDuplicateCode_ReturnsBadRequest()
    {
        // Arrange
        var professorId = 1;
        SetupUserContext(professorId, "Professor");

        var request = new CreateSubjectRequest
        {
            Name = "Test Subject",
            Code = "EXISTING_CODE",
            Year = 1
        };

        _mockSubjectService
            .Setup(x => x.CreateSubjectAsync(professorId, request))
            .ThrowsAsync(new InvalidOperationException("Codul materiei există deja"));

        // Act
        var result = await _controller.CreateSubject(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region UpdateSubject Tests

    [Fact]
    public async Task UpdateSubject_WithValidRequest_ReturnsOkWithUpdatedSubject()
    {
        // Arrange
        var professorId = 1;
        var subjectId = 1;
        SetupUserContext(professorId, "Professor");

        var request = new UpdateSubjectRequest
        {
            Name = "Programare Web Avansată",
            Description = "Tehnici avansate de dezvoltare web",
            Year = 3
        };

        var updatedSubject = new SubjectDto
        {
            Id = subjectId,
            Name = request.Name,
            Description = request.Description,
            Year = request.Year
        };

        _mockSubjectService
            .Setup(x => x.UpdateSubjectAsync(subjectId, professorId, request))
            .ReturnsAsync(updatedSubject);

        // Act
        var result = await _controller.UpdateSubject(subjectId, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSubject = okResult.Value.Should().BeOfType<SubjectDto>().Subject;
        returnedSubject.Name.Should().Be("Programare Web Avansată");
    }

    [Fact]
    public async Task UpdateSubject_WithNonExistentId_ReturnsBadRequest()
    {
        // Arrange
        var professorId = 1;
        var subjectId = 999;
        SetupUserContext(professorId, "Professor");

        var request = new UpdateSubjectRequest { Name = "Test", Year = 1 };

        _mockSubjectService
            .Setup(x => x.UpdateSubjectAsync(subjectId, professorId, request))
            .ThrowsAsync(new InvalidOperationException("Materia nu există"));

        // Act
        var result = await _controller.UpdateSubject(subjectId, request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateSubject_WithoutPermission_ReturnsBadRequest()
    {
        // Arrange
        var professorId = 1;
        var subjectId = 1;
        SetupUserContext(professorId, "Professor");

        var request = new UpdateSubjectRequest { Name = "Test", Year = 1 };

        _mockSubjectService
            .Setup(x => x.UpdateSubjectAsync(subjectId, professorId, request))
            .ThrowsAsync(new InvalidOperationException("Nu aveți permisiunea să modificați această materie"));

        // Act
        var result = await _controller.UpdateSubject(subjectId, request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region DeleteSubject Tests

    [Fact]
    public async Task DeleteSubject_WithValidId_ReturnsOk()
    {
        // Arrange
        var professorId = 1;
        var subjectId = 1;
        SetupUserContext(professorId, "Professor");

        _mockSubjectService
            .Setup(x => x.DeleteSubjectAsync(subjectId, professorId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteSubject(subjectId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteSubject_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var professorId = 1;
        var subjectId = 999;
        SetupUserContext(professorId, "Professor");

        _mockSubjectService
            .Setup(x => x.DeleteSubjectAsync(subjectId, professorId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteSubject(subjectId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetSubject Tests

    [Fact]
    public async Task GetSubject_WithValidId_ReturnsOkWithSubject()
    {
        // Arrange
        var subjectId = 1;
        SetupUserContext(1);

        var subject = new SubjectDto
        {
            Id = subjectId,
            Name = "Algoritmi",
            Code = "ALG101",
            ProfessorName = "Prof. Popescu"
        };

        _mockSubjectService
            .Setup(x => x.GetSubjectByIdAsync(subjectId))
            .ReturnsAsync(subject);

        // Act
        var result = await _controller.GetSubject(subjectId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSubject = okResult.Value.Should().BeOfType<SubjectDto>().Subject;
        returnedSubject.Id.Should().Be(subjectId);
        returnedSubject.Name.Should().Be("Algoritmi");
    }

    [Fact]
    public async Task GetSubject_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var subjectId = 999;
        SetupUserContext(1);

        _mockSubjectService
            .Setup(x => x.GetSubjectByIdAsync(subjectId))
            .ReturnsAsync((SubjectDto?)null);

        // Act
        var result = await _controller.GetSubject(subjectId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetMySubjects Tests

    [Fact]
    public async Task GetMySubjects_ReturnsProfessorSubjects()
    {
        // Arrange
        var professorId = 1;
        SetupUserContext(professorId, "Professor");

        var subjects = new List<SubjectDto>
        {
            new SubjectDto { Id = 1, Name = "Programare", ProfessorId = professorId },
            new SubjectDto { Id = 2, Name = "Baze de Date", ProfessorId = professorId },
            new SubjectDto { Id = 3, Name = "Rețele", ProfessorId = professorId }
        };

        _mockSubjectService
            .Setup(x => x.GetSubjectsByProfessorAsync(professorId))
            .ReturnsAsync(subjects);

        // Act
        var result = await _controller.GetMySubjects();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSubjects = okResult.Value.Should().BeAssignableTo<List<SubjectDto>>().Subject;
        returnedSubjects.Should().HaveCount(3);
        returnedSubjects.Should().OnlyContain(s => s.ProfessorId == professorId);
    }

    [Fact]
    public async Task GetMySubjects_WhenNoSubjects_ReturnsEmptyList()
    {
        // Arrange
        var professorId = 1;
        SetupUserContext(professorId, "Professor");

        _mockSubjectService
            .Setup(x => x.GetSubjectsByProfessorAsync(professorId))
            .ReturnsAsync(new List<SubjectDto>());

        // Act
        var result = await _controller.GetMySubjects();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSubjects = okResult.Value.Should().BeAssignableTo<List<SubjectDto>>().Subject;
        returnedSubjects.Should().BeEmpty();
    }

    #endregion

    #region GetAllSubjects Tests

    [Fact]
    public async Task GetAllSubjects_ReturnsAllSubjects()
    {
        // Arrange
        SetupUserContext(1);

        var subjects = new List<SubjectDto>
        {
            new SubjectDto { Id = 1, Name = "Matematică", Year = 1 },
            new SubjectDto { Id = 2, Name = "Fizică", Year = 1 },
            new SubjectDto { Id = 3, Name = "Programare", Year = 1 },
            new SubjectDto { Id = 4, Name = "Algoritmi", Year = 2 }
        };

        _mockSubjectService
            .Setup(x => x.GetAllSubjectsAsync())
            .ReturnsAsync(subjects);

        // Act
        var result = await _controller.GetAllSubjects();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSubjects = okResult.Value.Should().BeAssignableTo<List<SubjectDto>>().Subject;
        returnedSubjects.Should().HaveCount(4);
    }

    [Fact]
    public async Task GetAllSubjects_WhenEmpty_ReturnsEmptyList()
    {
        // Arrange
        SetupUserContext(1);

        _mockSubjectService
            .Setup(x => x.GetAllSubjectsAsync())
            .ReturnsAsync(new List<SubjectDto>());

        // Act
        var result = await _controller.GetAllSubjects();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSubjects = okResult.Value.Should().BeAssignableTo<List<SubjectDto>>().Subject;
        returnedSubjects.Should().BeEmpty();
    }

    #endregion
}
