using CampusConnect.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CampusConnect.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IUserService _userService;
    private readonly IGradeService _gradeService;

    public DocumentService(IUserService userService, IGradeService gradeService)
    {
        _userService = userService;
        _gradeService = gradeService;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateEnrollmentCertificateAsync(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header().Element(c => ComposeHeader(c, "ADEVERINTA"));

                page.Content().Element(c => ComposeEnrollmentContent(c, user));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    public async Task<byte[]> GenerateTranscriptAsync(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var gradesResponse = await _gradeService.GetStudentGradesGroupedAsync(userId);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(c => ComposeHeader(c, "SITUATIE SCOLARA"));

                page.Content().Element(c => ComposeTranscriptContent(c, user, gradesResponse));

                page.Footer().Element(ComposeFooter);
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, string documentTitle)
    {
        container.Column(column =>
        {
            // University header
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("ROMANIA").Bold().FontSize(10);
                    col.Item().Text("MINISTERUL EDUCATIEI").FontSize(9);
                });

                row.RelativeItem(2).Column(col =>
                {
                    col.Item().AlignCenter().Text("UNIVERSITATEA DIN BUCURESTI").Bold().FontSize(14);
                    col.Item().AlignCenter().Text("FACULTATEA DE MATEMATICA SI INFORMATICA").FontSize(10);
                    col.Item().AlignCenter().Text("Str. Academiei nr. 14, Sector 1, Bucuresti").FontSize(8).FontColor(Colors.Grey.Darken1);
                    col.Item().AlignCenter().Text("Tel: 021-314.35.08 | E-mail: secretariat@fmi.unibuc.ro").FontSize(8).FontColor(Colors.Grey.Darken1);
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().AlignRight().Text($"Nr. {GenerateDocNumber()}").FontSize(9);
                    col.Item().AlignRight().Text($"Din {DateTime.Now:dd.MM.yyyy}").FontSize(9);
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Blue.Darken3);

            // Document title
            column.Item().PaddingTop(20).AlignCenter().Text(documentTitle).Bold().FontSize(16).FontColor(Colors.Blue.Darken3);
        });
    }

    private void ComposeEnrollmentContent(IContainer container, dynamic user)
    {
        container.PaddingTop(30).Column(column =>
        {
            column.Spacing(15);

            column.Item().Text(text =>
            {
                text.Span("Se adevereste ca ").FontSize(12);
                text.Span($"{user.LastName} {user.FirstName}").Bold().FontSize(12);
                text.Span(", avand numarul matricol ").FontSize(12);
                text.Span($"{user.StudentId ?? "N/A"}").Bold().FontSize(12);
                text.Span(", este student(a) in anul universitar ").FontSize(12);
                text.Span("2025-2026").Bold().FontSize(12);
                text.Span(" la Facultatea de Matematica si Informatica, Universitatea din Bucuresti.").FontSize(12);
            });

            column.Item().PaddingTop(10).Text(text =>
            {
                text.Span("Forma de invatamant: ").FontSize(12);
                text.Span("ZI (cu frecventa)").Bold().FontSize(12);
            });

            column.Item().Text(text =>
            {
                text.Span("Ciclul de studii: ").FontSize(12);
                text.Span("LICENTA").Bold().FontSize(12);
            });

            column.Item().Text(text =>
            {
                text.Span("Domeniul de studii: ").FontSize(12);
                text.Span("INFORMATICA").Bold().FontSize(12);
            });

            column.Item().Text(text =>
            {
                text.Span("Programul de studii: ").FontSize(12);
                text.Span("INFORMATICA").Bold().FontSize(12);
            });

            column.Item().Text(text =>
            {
                text.Span("Statut: ").FontSize(12);
                text.Span("Buget").Bold().FontSize(12);
            });

            column.Item().PaddingTop(20).Text("Prezenta adeverinta se elibereaza pentru a-i servi la cerere.").FontSize(11).Italic();

            // Signatures section
            column.Item().PaddingTop(50).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("DECAN,").Bold().FontSize(11);
                    col.Item().PaddingTop(5).Text("Prof. univ. dr.").FontSize(10);
                    col.Item().Text("_______________________").FontSize(10);
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().AlignRight().Text("SECRETAR SEF,").Bold().FontSize(11);
                    col.Item().PaddingTop(5).AlignRight().Text("_______________________").FontSize(10);
                });
            });
        });
    }

    private void ComposeTranscriptContent(IContainer container, dynamic user, dynamic gradesResponse)
    {
        container.PaddingTop(20).Column(column =>
        {
            column.Spacing(10);

            // Student info
            column.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(info =>
            {
                info.Item().Row(row =>
                {
                    row.RelativeItem().Text(text =>
                    {
                        text.Span("Student: ").FontSize(10);
                        text.Span($"{user.LastName} {user.FirstName}").Bold().FontSize(10);
                    });
                    row.RelativeItem().AlignRight().Text(text =>
                    {
                        text.Span("Nr. Matricol: ").FontSize(10);
                        text.Span($"{user.StudentId ?? "N/A"}").Bold().FontSize(10);
                    });
                });
                info.Item().Text(text =>
                {
                    text.Span("Program de studii: ").FontSize(10);
                    text.Span("Informatica - Licenta").Bold().FontSize(10);
                });
            });

            // Grades by year
            var subjectGrades = gradesResponse.SubjectGrades as IEnumerable<dynamic>;
            if (subjectGrades != null && subjectGrades.Any())
            {
                var gradesByYear = subjectGrades
                    .GroupBy(g => (int)g.Year)
                    .OrderBy(g => g.Key);

                foreach (var yearGroup in gradesByYear)
                {
                    column.Item().PaddingTop(15).Text($"Anul {yearGroup.Key}").Bold().FontSize(12).FontColor(Colors.Blue.Darken2);

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // Nr
                            columns.ConstantColumn(70);  // Cod
                            columns.RelativeColumn(3);   // Disciplina
                            columns.RelativeColumn(2);   // Profesor
                            columns.ConstantColumn(50);  // Nota
                            columns.ConstantColumn(60);  // Data
                        });

                        // Header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Nr.").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Cod").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Disciplina").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(5).Text("Profesor").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(5).AlignCenter().Text("Nota").FontColor(Colors.White).Bold().FontSize(9);
                            header.Cell().Background(Colors.Blue.Darken3).Padding(5).AlignCenter().Text("Data").FontColor(Colors.White).Bold().FontSize(9);
                        });

                        int nr = 1;
                        foreach (var subject in yearGroup)
                        {
                            var grades = subject.Grades as IEnumerable<dynamic>;
                            var avgGrade = subject.AverageGrade;
                            var latestGrade = grades?.OrderByDescending(g => (DateTime)g.CreatedAt).FirstOrDefault();

                            var bgColor = nr % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                            table.Cell().Background(bgColor).Padding(4).Text($"{nr}").FontSize(9);
                            table.Cell().Background(bgColor).Padding(4).Text($"{subject.SubjectCode}").FontSize(9);
                            table.Cell().Background(bgColor).Padding(4).Text($"{subject.SubjectName}").FontSize(9);
                            table.Cell().Background(bgColor).Padding(4).Text($"{subject.ProfessorName}").FontSize(9);
                            table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(avgGrade != null ? $"{avgGrade:F2}" : "-").Bold().FontSize(9);
                            table.Cell().Background(bgColor).Padding(4).AlignCenter().Text(latestGrade != null ? ((DateTime)latestGrade.CreatedAt).ToString("dd.MM.yyyy") : "-").FontSize(9);

                            nr++;
                        }
                    });
                }

                // Calculate overall average
                var allAverages = subjectGrades
                    .Where(s => s.AverageGrade != null)
                    .Select(s => (decimal)s.AverageGrade)
                    .ToList();

                if (allAverages.Any())
                {
                    var overallAvg = allAverages.Average();
                    column.Item().PaddingTop(15).AlignRight().Text(text =>
                    {
                        text.Span("Media generala: ").FontSize(11);
                        text.Span($"{overallAvg:F2}").Bold().FontSize(14).FontColor(Colors.Blue.Darken3);
                    });
                }
            }
            else
            {
                column.Item().PaddingTop(20).AlignCenter().Text("Nu exista note inregistrate.").Italic().FontColor(Colors.Grey.Darken1);
            }

            // Signatures
            column.Item().PaddingTop(40).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("DECAN,").Bold().FontSize(10);
                    col.Item().PaddingTop(30).Text("_______________________").FontSize(9);
                });

                row.RelativeItem().AlignCenter().Column(col =>
                {
                    col.Item().AlignCenter().Text("SECRETAR,").Bold().FontSize(10);
                    col.Item().PaddingTop(30).AlignCenter().Text("_______________________").FontSize(9);
                });

                row.RelativeItem().AlignRight().Column(col =>
                {
                    col.Item().AlignRight().Text("DIRECTOR DEPARTAMENT,").Bold().FontSize(10);
                    col.Item().PaddingTop(30).AlignRight().Text("_______________________").FontSize(9);
                });
            });
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().PaddingTop(10).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten1);
            column.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("Document generat electronic prin ").FontSize(8).FontColor(Colors.Grey.Darken1);
                    text.Span("CampusConnect").Bold().FontSize(8).FontColor(Colors.Blue.Darken2);
                });
                row.RelativeItem().AlignRight().Text($"Generat la: {DateTime.Now:dd.MM.yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Darken1);
            });
            column.Item().AlignCenter().Text("Acest document este valabil fara semnatura si stampila conform Legii nr. 455/2001").FontSize(7).FontColor(Colors.Grey.Darken1);
        });
    }

    private string GenerateDocNumber()
    {
        return $"{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
    }
}