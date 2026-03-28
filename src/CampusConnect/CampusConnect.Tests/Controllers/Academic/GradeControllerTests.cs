using CampusConnect.Api.Controllers.Academic;
using CampusConnect.Application.DTOs.Grades;
using CampusConnect.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace CampusConnect.Tests.Controllers.Academic;

public class GradeControllerTests
{
    private readonly Mock<IGradeService> _mockGradeService;
    private readonly GradeController _controller;

    public GradeControllerTests()
    {
        _mockGradeService = new Mock<IGradeService>();
        _controller = new GradeController(_mockGradeService.Object);
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

    #region CreateGrade Tests

    [Fact]
    public async Task CreateGrade_WithValidRequest_ReturnsOkWithGrade()
    {
        // Arrange
        var professorId = 1;
        SetupUserContext(professorId, "Professor");

        var request = new CreateGradeRequest
        {
            SubjectId = 1,
            StudentId = 2,
            Value = 9.5m,
            Comments = "Foarte bine!"
        };

        var expectedGrade = new GradeDto
        {
            Id = 1,
            SubjectId = request.SubjectId,
            StudentId = request.StudentId,
            Value = request.Value,
            Comments = request.Comments,
            CreatedByProfessorId = professorId
        };

        _mockGradeService
            .Setup(x => x.CreateGradeAsync(professorId, request))
            .ReturnsAsync(expectedGrade);

        // Act
        var result = await _controller.CreateGrade(request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedGrade = okResult.Value.Should().BeOfType<GradeDto>().Subject;
        returnedGrade.Value.Should().Be(9.5m);
        returnedGrade.StudentId.Should().Be(2);
    }

    [Fact]
    public async Task CreateGrade_WithInvalidSubject_ReturnsBadRequest()
    {
        // Arrange
        var professorId = 1;
        SetupUserContext(professorId, "Professor");

        var request = new CreateGradeRequest
        {
            SubjectId = 999, // Non-existent
            StudentId = 2,
            Value = 9.5m
        };

        _mockGradeService
            .Setup(x => x.CreateGradeAsync(professorId, request))
            .ThrowsAsync(new InvalidOperationException("Materia nu există"));

        // Act
        var result = await _controller.CreateGrade(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateGrade_WithInvalidGradeValue_ReturnsBadRequest()
    {
        // Arrange
        var professorId = 1;
        SetupUserContext(professorId, "Professor");

        var request = new CreateGradeRequest
        {
            SubjectId = 1,
            StudentId = 2,
            Value = 15m // Invalid - over 10
        };

        _mockGradeService
            .Setup(x => x.CreateGradeAsync(professorId, request))
            .ThrowsAsync(new InvalidOperationException("Nota trebuie să fie între 1 și 10"));

        // Act
        var result = await _controller.CreateGrade(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region UpdateGrade Tests

    [Fact]
    public async Task UpdateGrade_WithValidRequest_ReturnsOkWithUpdatedGrade()
    {
        // Arrange
        var professorId = 1;
        var gradeId = 1;
        SetupUserContext(professorId, "Professor");

        var request = new UpdateGradeRequest
        {
            Value = 10m,
            Comments = "Excelent!"
        };

        var updatedGrade = new GradeDto
        {
            Id = gradeId,
            Value = request.Value,
            Comments = request.Comments,
            UpdatedAt = DateTime.UtcNow
        };

        _mockGradeService
            .Setup(x => x.UpdateGradeAsync(gradeId, professorId, request))
            .ReturnsAsync(updatedGrade);

        // Act
        var result = await _controller.UpdateGrade(gradeId, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedGrade = okResult.Value.Should().BeOfType<GradeDto>().Subject;
        returnedGrade.Value.Should().Be(10m);
    }

    [Fact]
    public async Task UpdateGrade_WithNonExistentGrade_ReturnsBadRequest()
    {
        // Arrange
        var professorId = 1;
        var gradeId = 999;
        SetupUserContext(professorId, "Professor");

        var request = new UpdateGradeRequest { Value = 8m };

        _mockGradeService
            .Setup(x => x.UpdateGradeAsync(gradeId, professorId, request))
            .ThrowsAsync(new InvalidOperationException("Nota nu există"));

        // Act
        var result = await _controller.UpdateGrade(gradeId, request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region DeleteGrade Tests

    [Fact]
    public async Task DeleteGrade_WithValidId_ReturnsOk()
    {
        // Arrange
        var professorId = 1;
        var gradeId = 1;
        SetupUserContext(professorId, "Professor");

        _mockGradeService
            .Setup(x => x.DeleteGradeAsync(gradeId, professorId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteGrade(gradeId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeleteGrade_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var professorId = 1;
        var gradeId = 999;
        SetupUserContext(professorId, "Professor");

        _mockGradeService
            .Setup(x => x.DeleteGradeAsync(gradeId, professorId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteGrade(gradeId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetGrade Tests

    [Fact]
    public async Task GetGrade_WithValidId_ReturnsOkWithGrade()
    {
        // Arrange
        var gradeId = 1;
        SetupUserContext(1);

        var grade = new GradeDto
        {
            Id = gradeId,
            SubjectName = "Programare",
            Value = 9m,
            StudentName = "Ion Popescu"
        };

        _mockGradeService
            .Setup(x => x.GetGradeByIdAsync(gradeId))
            .ReturnsAsync(grade);

        // Act
        var result = await _controller.GetGrade(gradeId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedGrade = okResult.Value.Should().BeOfType<GradeDto>().Subject;
        returnedGrade.Id.Should().Be(gradeId);
    }

    [Fact]
    public async Task GetGrade_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var gradeId = 999;
        SetupUserContext(1);

        _mockGradeService
            .Setup(x => x.GetGradeByIdAsync(gradeId))
            .ReturnsAsync((GradeDto?)null);

        // Act
        var result = await _controller.GetGrade(gradeId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetGradesBySubject Tests

    [Fact]
    public async Task GetGradesBySubject_ReturnsOkWithGrades()
    {
        // Arrange
        var subjectId = 1;
        SetupUserContext(1, "Professor");

        var grades = new List<GradeDto>
        {
            new GradeDto { Id = 1, SubjectId = subjectId, Value = 8m },
            new GradeDto { Id = 2, SubjectId = subjectId, Value = 9m },
            new GradeDto { Id = 3, SubjectId = subjectId, Value = 10m }
        };

        _mockGradeService
            .Setup(x => x.GetGradesBySubjectAsync(subjectId))
            .ReturnsAsync(grades);

        // Act
        var result = await _controller.GetGradesBySubject(subjectId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedGrades = okResult.Value.Should().BeAssignableTo<List<GradeDto>>().Subject;
        returnedGrades.Should().HaveCount(3);
    }

    #endregion

    #region GetGradesByStudent Tests

    [Fact]
    public async Task GetGradesByStudent_AsStudent_OwnGrades_ReturnsOk()
    {
        // Arrange
        var studentId = 1;
        SetupUserContext(studentId, "User");

        var grades = new List<GradeDto>
        {
            new GradeDto { Id = 1, StudentId = studentId, Value = 9m }
        };

        _mockGradeService
            .Setup(x => x.GetGradesByStudentAsync(studentId))
            .ReturnsAsync(grades);

        // Act
        var result = await _controller.GetGradesByStudent(studentId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetGradesByStudent_AsStudent_OtherStudentGrades_ReturnsForbid()
    {
        // Arrange
        var currentUserId = 1;
        var otherStudentId = 2;
        SetupUserContext(currentUserId, "User");

        // Act
        var result = await _controller.GetGradesByStudent(otherStudentId);

        // Assert
        result.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task GetGradesByStudent_AsProfessor_AnyStudent_ReturnsOk()
    {
        // Arrange
        var professorId = 1;
        var studentId = 2;
        SetupUserContext(professorId, "Professor");

        var grades = new List<GradeDto>
        {
            new GradeDto { Id = 1, StudentId = studentId, Value = 8m }
        };

        _mockGradeService
            .Setup(x => x.GetGradesByStudentAsync(studentId))
            .ReturnsAsync(grades);

        // Act
        var result = await _controller.GetGradesByStudent(studentId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetGradesByStudent_AsAdmin_AnyStudent_ReturnsOk()
    {
        // Arrange
        var adminId = 1;
        var studentId = 2;
        SetupUserContext(adminId, "Admin");

        var grades = new List<GradeDto>
        {
            new GradeDto { Id = 1, StudentId = studentId, Value = 7m }
        };

        _mockGradeService
            .Setup(x => x.GetGradesByStudentAsync(studentId))
            .ReturnsAsync(grades);

        // Act
        var result = await _controller.GetGradesByStudent(studentId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region GetStudentGradesGrouped Tests

    [Fact]
    public async Task GetStudentGradesGrouped_ReturnsGroupedGrades()
    {
        // Arrange
        var studentId = 1;
        SetupUserContext(studentId, "User");

        var response = new StudentGradesResponse
        {
            StudentId = studentId,
            StudentName = "Ion Popescu",
            SubjectGrades = new List<SubjectGrades>
            {
                new SubjectGrades
                {
                    SubjectId = 1,
                    SubjectName = "Programare",
                    AverageGrade = 9.5m,
                    Grades = new List<GradeDto>
                    {
                        new GradeDto { Value = 9m },
                        new GradeDto { Value = 10m }
                    }
                }
            }
        };

        _mockGradeService
            .Setup(x => x.GetStudentGradesGroupedAsync(studentId))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetStudentGradesGrouped(studentId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResponse = okResult.Value.Should().BeOfType<StudentGradesResponse>().Subject;
        returnedResponse.SubjectGrades.Should().HaveCount(1);
        returnedResponse.SubjectGrades[0].AverageGrade.Should().Be(9.5m);
    }

    [Fact]
    public async Task GetStudentGradesGrouped_StudentNotFound_ReturnsNotFound()
    {
        // Arrange
        var studentId = 999;
        SetupUserContext(studentId, "User");

        _mockGradeService
            .Setup(x => x.GetStudentGradesGroupedAsync(studentId))
            .ThrowsAsync(new InvalidOperationException("Studentul nu a fost găsit"));

        // Act
        var result = await _controller.GetStudentGradesGrouped(studentId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region GetMyGrades Tests

    [Fact]
    public async Task GetMyGrades_ReturnsCurrentUserGrades()
    {
        // Arrange
        var studentId = 1;
        SetupUserContext(studentId, "User");

        var response = new StudentGradesResponse
        {
            StudentId = studentId,
            StudentName = "Maria Ionescu",
            SubjectGrades = new List<SubjectGrades>()
        };

        _mockGradeService
            .Setup(x => x.GetStudentGradesGroupedAsync(studentId))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetMyGrades();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResponse = okResult.Value.Should().BeOfType<StudentGradesResponse>().Subject;
        returnedResponse.StudentId.Should().Be(studentId);
    }

    #endregion
}
