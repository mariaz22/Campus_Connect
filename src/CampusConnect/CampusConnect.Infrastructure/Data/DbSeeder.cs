using CampusConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CampusConnect.Infrastructure.Data;

public static class DbSeeder
{
    private const string DefaultPassword = "Student123!";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            // Check if already seeded (more than 10 users means demo data exists)
            var userCount = await context.Users.CountAsync();
            if (userCount > 10)
            {
                logger.LogInformation("Database already seeded with demo data. Skipping...");
                return;
            }

            logger.LogInformation("Starting database seeding...");

            // 1. Seed Roles
            await SeedRolesAsync(roleManager);

            // 2. Seed Professors
            var professors = await SeedProfessorsAsync(userManager);

            // 3. Seed Students
            var students = await SeedStudentsAsync(userManager);

            // 4. Seed Subjects
            var subjects = await SeedSubjectsAsync(context, professors);

            // 5. Seed Achievements
            await SeedAchievementsAsync(context);

            // 6. Seed Grades
            await SeedGradesAsync(context, subjects, students, professors);

            // 7. Seed Groups
            var groups = await SeedGroupsAsync(context, professors);

            // 8. Seed Group Members
            await SeedGroupMembersAsync(context, groups, students);

            // 9. Seed Group Tasks
            var tasks = await SeedGroupTasksAsync(context, groups, professors);

            // 10. Seed Events
            var events = await SeedEventsAsync(context, professors);

            // 11. Seed Event Participants
            await SeedEventParticipantsAsync(context, events, students);

            // 12. Seed Announcements
            await SeedAnnouncementsAsync(context, professors);

            // 13. Seed Library
            await SeedLibraryAsync(context, professors);

            // 14. Seed Course Materials
            await SeedCourseMaterialsAsync(context, groups, professors);

            // 15. Seed Saved Tasks
            await SeedSavedTasksAsync(context, tasks, students);

            // 16. Seed Group Announcements (Group Chat)
            await SeedGroupAnnouncementsAsync(context, groups);

            logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error seeding database");
            throw;
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
    {
        string[] roles = { "Admin", "User", "Professor" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }
    }

    private static async Task<Dictionary<string, ApplicationUser>> SeedProfessorsAsync(UserManager<ApplicationUser> userManager)
    {
        var professors = new Dictionary<string, ApplicationUser>();
        var professorData = new[]
        {
            ("Alexandru", "Popescu", "alexandru.popescu@unibuc.ro"),
            ("Maria", "Ionescu", "maria.ionescu@unibuc.ro"),
            ("Andrei", "Dumitrescu", "andrei.dumitrescu@unibuc.ro"),
            ("Elena", "Stanescu", "elena.stanescu@unibuc.ro"),
            ("Mihai", "Georgescu", "mihai.georgescu@unibuc.ro")
        };

        foreach (var (firstName, lastName, email) in professorData)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                professors[email] = existing;
                continue;
            }

            var user = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, DefaultPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Professor");
                professors[email] = user;
            }
        }

        return professors;
    }

    private static async Task<Dictionary<string, ApplicationUser>> SeedStudentsAsync(UserManager<ApplicationUser> userManager)
    {
        var students = new Dictionary<string, ApplicationUser>();
        var studentData = new[]
        {
            // Year 1
            ("Andreea", "Marin", "andreea.marin@s.unibuc.ro", "1234INFO1"),
            ("Bogdan", "Radu", "bogdan.radu@s.unibuc.ro", "1235INFO1"),
            ("Cristina", "Vasilescu", "cristina.vasilescu@s.unibuc.ro", "1236INFO1"),
            ("Daniel", "Popa", "daniel.popa@s.unibuc.ro", "1237INFO1"),
            // Year 2
            ("Elena", "Constantinescu", "elena.constantinescu@s.unibuc.ro", "1134INFO2"),
            ("Florin", "Dragomir", "florin.dragomir@s.unibuc.ro", "1135INFO2"),
            ("Gabriela", "Niculae", "gabriela.niculae@s.unibuc.ro", "1136INFO2"),
            ("Horia", "Moldovan", "horia.moldovan@s.unibuc.ro", "1137INFO2"),
            // Year 3
            ("Ioana", "Petrescu", "ioana.petrescu@s.unibuc.ro", "1034INFO3"),
            ("Vlad", "Tanase", "vlad.tanase@s.unibuc.ro", "1035INFO3"),
            ("Laura", "Stoica", "laura.stoica@s.unibuc.ro", "1036INFO3"),
            ("Matei", "Florescu", "matei.florescu@s.unibuc.ro", "1037INFO3")
        };

        foreach (var (firstName, lastName, email, studentId) in studentData)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                students[email] = existing;
                continue;
            }

            var user = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                StudentId = studentId,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, DefaultPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "User");
                students[email] = user;
            }
        }

        return students;
    }

    private static async Task<Dictionary<string, Subject>> SeedSubjectsAsync(
        ApplicationDbContext context,
        Dictionary<string, ApplicationUser> professors)
    {
        var subjects = new Dictionary<string, Subject>();

        if (await context.Subjects.AnyAsync(s => s.Code == "INFO101"))
        {
            // Already seeded, load existing
            var existing = await context.Subjects.ToListAsync();
            foreach (var s in existing)
                subjects[s.Code] = s;
            return subjects;
        }

        var subjectData = new[]
        {
            // Year 1
            ("Algoritmica", "INFO101", "Introducere in algoritmi si structuri de date fundamentale.", 1, "alexandru.popescu@unibuc.ro"),
            ("Programare Orientata pe Obiecte", "INFO102", "Concepte OOP: clase, mostenire, polimorfism.", 1, "maria.ionescu@unibuc.ro"),
            ("Logica Matematica", "INFO103", "Logica propozitionala si predicatelor.", 1, "andrei.dumitrescu@unibuc.ro"),
            ("Arhitectura Calculatoarelor", "INFO104", "Structura procesoarelor, memoria, assembly.", 1, "mihai.georgescu@unibuc.ro"),
            // Year 2
            ("Baze de Date", "INFO201", "SQL, normalizare, tranzactii, indexare.", 2, "andrei.dumitrescu@unibuc.ro"),
            ("Retele de Calculatoare", "INFO202", "TCP/IP, routing, securitate retea.", 2, "elena.stanescu@unibuc.ro"),
            ("Sisteme de Operare", "INFO203", "Procese, threading, memoria, filesystem.", 2, "mihai.georgescu@unibuc.ro"),
            ("Structuri de Date", "INFO204", "Arbori, grafuri, heap, hash tables.", 2, "alexandru.popescu@unibuc.ro"),
            // Year 3
            ("Inteligenta Artificiala", "INFO301", "ML, retele neuronale, NLP.", 3, "mihai.georgescu@unibuc.ro"),
            ("Inginerie Software", "INFO302", "Agile, design patterns, CI/CD.", 3, "maria.ionescu@unibuc.ro"),
            ("Securitate Informatica", "INFO303", "Criptografie, vulnerabilitati, pentesting.", 3, "elena.stanescu@unibuc.ro"),
            ("Dezvoltare Web", "INFO304", "React, Node.js, REST APIs.", 3, "andrei.dumitrescu@unibuc.ro")
        };

        foreach (var (name, code, description, year, profEmail) in subjectData)
        {
            if (!professors.TryGetValue(profEmail, out var prof)) continue;

            var subject = new Subject
            {
                Name = name,
                Code = code,
                Description = description,
                Year = year,
                ProfessorId = prof.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            context.Subjects.Add(subject);
            subjects[code] = subject;
        }

        await context.SaveChangesAsync();
        return subjects;
    }

    private static async Task SeedAchievementsAsync(ApplicationDbContext context)
    {
        if (await context.Achievements.AnyAsync()) return;

        var achievements = new[]
        {
            new Achievement { Title = "First Login", Description = "Welcome to CampusConnect!", Icon = "🎉" },
            new Achievement { Title = "Profile Complete", Description = "Completed your profile information.", Icon = "✅" },
            new Achievement { Title = "Event Participant", Description = "Joined your first event.", Icon = "🎪" },
            new Achievement { Title = "High Achiever", Description = "Got a grade of 10.", Icon = "🏆" },
            new Achievement { Title = "Group Leader", Description = "Active in study groups.", Icon = "👥" }
        };

        context.Achievements.AddRange(achievements);
        await context.SaveChangesAsync();
    }

    private static async Task SeedGradesAsync(
        ApplicationDbContext context,
        Dictionary<string, Subject> subjects,
        Dictionary<string, ApplicationUser> students,
        Dictionary<string, ApplicationUser> professors)
    {
        if (await context.Grades.CountAsync() > 20) return;

        var gradeData = new[]
        {
            // Year 1 students - Year 1 subjects
            ("INFO101", "andreea.marin@s.unibuc.ro", 10m, "Excelent!"),
            ("INFO101", "bogdan.radu@s.unibuc.ro", 8.5m, "Bine."),
            ("INFO101", "cristina.vasilescu@s.unibuc.ro", 9m, "Foarte bine."),
            ("INFO101", "daniel.popa@s.unibuc.ro", 7m, "Acceptabil."),
            ("INFO102", "andreea.marin@s.unibuc.ro", 9.5m, "Design OOP foarte bun."),
            ("INFO102", "bogdan.radu@s.unibuc.ro", 9m, "Corect."),
            ("INFO102", "cristina.vasilescu@s.unibuc.ro", 10m, "Exceptional!"),
            ("INFO102", "daniel.popa@s.unibuc.ro", 8m, "Bine."),
            ("INFO103", "andreea.marin@s.unibuc.ro", 10m, "Excelent la demonstratii."),
            ("INFO103", "bogdan.radu@s.unibuc.ro", 7.5m, "Acceptabil."),
            ("INFO103", "cristina.vasilescu@s.unibuc.ro", 9.5m, "Foarte riguroasa."),
            ("INFO103", "daniel.popa@s.unibuc.ro", 6.5m, "Trebuie sa aprofundezi."),
            ("INFO104", "andreea.marin@s.unibuc.ro", 9m, "Foarte bun la assembly."),
            ("INFO104", "bogdan.radu@s.unibuc.ro", 9m, "Foarte bine."),
            ("INFO104", "cristina.vasilescu@s.unibuc.ro", 8.5m, "Bine."),
            ("INFO104", "daniel.popa@s.unibuc.ro", 8m, "Bine la practica."),
            // Year 2 students - Year 2 subjects
            ("INFO201", "elena.constantinescu@s.unibuc.ro", 9m, "SQL optimizat."),
            ("INFO201", "florin.dragomir@s.unibuc.ro", 8.5m, "Bun la modelare."),
            ("INFO201", "gabriela.niculae@s.unibuc.ro", 10m, "Exceptional!"),
            ("INFO201", "horia.moldovan@s.unibuc.ro", 7.5m, "Mai lucreaza la JOIN."),
            ("INFO202", "elena.constantinescu@s.unibuc.ro", 8m, "Buna intelegere."),
            ("INFO202", "florin.dragomir@s.unibuc.ro", 9.5m, "Excelent la config."),
            ("INFO202", "gabriela.niculae@s.unibuc.ro", 9m, "Foarte bine."),
            ("INFO202", "horia.moldovan@s.unibuc.ro", 8.5m, "Bun."),
            ("INFO203", "elena.constantinescu@s.unibuc.ro", 8.5m, "Buna intelegere procese."),
            ("INFO203", "florin.dragomir@s.unibuc.ro", 9m, "Excelent sincronizare."),
            ("INFO203", "gabriela.niculae@s.unibuc.ro", 9.5m, "Foarte bine memoria."),
            ("INFO203", "horia.moldovan@s.unibuc.ro", 7.5m, "Mai exerseaza threading."),
            ("INFO204", "elena.constantinescu@s.unibuc.ro", 9m, "Foarte bine arbori."),
            ("INFO204", "florin.dragomir@s.unibuc.ro", 8m, "Bine grafuri."),
            ("INFO204", "gabriela.niculae@s.unibuc.ro", 10m, "Exceptional!"),
            ("INFO204", "horia.moldovan@s.unibuc.ro", 8m, "Bine hash."),
            // Year 3 students - Year 3 subjects
            ("INFO301", "ioana.petrescu@s.unibuc.ro", 10m, "Proiect ML exceptional!"),
            ("INFO301", "vlad.tanase@s.unibuc.ro", 9m, "Retea neuronala corecta."),
            ("INFO301", "laura.stoica@s.unibuc.ro", 9.5m, "Excelent NLP."),
            ("INFO301", "matei.florescu@s.unibuc.ro", 8m, "Optimizeaza hiperparametri."),
            ("INFO302", "ioana.petrescu@s.unibuc.ro", 9.5m, "Agile bine organizat."),
            ("INFO302", "vlad.tanase@s.unibuc.ro", 10m, "Design patterns exemplar."),
            ("INFO302", "laura.stoica@s.unibuc.ro", 8.5m, "Mai lucreaza la teste."),
            ("INFO302", "matei.florescu@s.unibuc.ro", 9m, "Documentatie completa."),
            ("INFO303", "ioana.petrescu@s.unibuc.ro", 9m, "Foarte bine criptografie."),
            ("INFO303", "vlad.tanase@s.unibuc.ro", 9.5m, "Excelent pentesting."),
            ("INFO303", "laura.stoica@s.unibuc.ro", 8m, "Mai lucreaza XSS."),
            ("INFO303", "matei.florescu@s.unibuc.ro", 8.5m, "Bine SQL injection."),
            ("INFO304", "ioana.petrescu@s.unibuc.ro", 10m, "Proiect web exceptional!"),
            ("INFO304", "vlad.tanase@s.unibuc.ro", 9m, "Foarte bine REST."),
            ("INFO304", "laura.stoica@s.unibuc.ro", 9.5m, "Foarte bun React."),
            ("INFO304", "matei.florescu@s.unibuc.ro", 8m, "Mai lucreaza backend.")
        };

        foreach (var (subjectCode, studentEmail, value, comment) in gradeData)
        {
            if (!subjects.TryGetValue(subjectCode, out var subject)) continue;
            if (!students.TryGetValue(studentEmail, out var student)) continue;

            // Check if grade already exists
            var exists = await context.Grades.AnyAsync(g =>
                g.SubjectId == subject.Id && g.StudentId == student.Id);
            if (exists) continue;

            var grade = new Grade
            {
                SubjectId = subject.Id,
                StudentId = student.Id,
                Value = value,
                Comments = comment,
                CreatedAt = DateTime.UtcNow,
                CreatedByProfessorId = subject.ProfessorId
            };
            context.Grades.Add(grade);
        }

        await context.SaveChangesAsync();
    }

    private static async Task<Dictionary<string, Group>> SeedGroupsAsync(
        ApplicationDbContext context,
        Dictionary<string, ApplicationUser> professors)
    {
        var groups = new Dictionary<string, Group>();

        if (await context.Groups.AnyAsync(g => g.Name.Contains("Grupa 131")))
        {
            var existing = await context.Groups.ToListAsync();
            foreach (var g in existing)
                groups[g.Name] = g;
            return groups;
        }

        var groupData = new[]
        {
            ("Grupa 131 - Algoritmica", "Seminar Algoritmica", "Algoritmica", "alexandru.popescu@unibuc.ro"),
            ("Grupa 132 - POO", "Laborator POO", "POO", "maria.ionescu@unibuc.ro"),
            ("Grupa 231 - Baze de Date", "Laborator BD", "Baze de Date", "andrei.dumitrescu@unibuc.ro"),
            ("Grupa 331 - Inteligenta Artificiala", "Seminar IA", "Inteligenta Artificiala", "mihai.georgescu@unibuc.ro"),
            ("Echipa Proiect IS", "Proiect CampusConnect", "Inginerie Software", "maria.ionescu@unibuc.ro")
        };

        foreach (var (name, desc, subject, profEmail) in groupData)
        {
            if (!professors.TryGetValue(profEmail, out var prof)) continue;

            var group = new Group
            {
                Name = name,
                Description = desc,
                Subject = subject,
                ProfessorId = prof.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            context.Groups.Add(group);
            groups[name] = group;
        }

        await context.SaveChangesAsync();
        return groups;
    }

    private static async Task SeedGroupMembersAsync(
        ApplicationDbContext context,
        Dictionary<string, Group> groups,
        Dictionary<string, ApplicationUser> students)
    {
        if (await context.GroupMembers.CountAsync() > 10) return;

        var membershipData = new[]
        {
            ("Grupa 131 - Algoritmica", new[] { "andreea.marin@s.unibuc.ro", "bogdan.radu@s.unibuc.ro", "cristina.vasilescu@s.unibuc.ro", "daniel.popa@s.unibuc.ro" }),
            ("Grupa 132 - POO", new[] { "andreea.marin@s.unibuc.ro", "bogdan.radu@s.unibuc.ro", "cristina.vasilescu@s.unibuc.ro", "daniel.popa@s.unibuc.ro" }),
            ("Grupa 231 - Baze de Date", new[] { "elena.constantinescu@s.unibuc.ro", "florin.dragomir@s.unibuc.ro", "gabriela.niculae@s.unibuc.ro", "horia.moldovan@s.unibuc.ro" }),
            ("Grupa 331 - Inteligenta Artificiala", new[] { "ioana.petrescu@s.unibuc.ro", "vlad.tanase@s.unibuc.ro", "laura.stoica@s.unibuc.ro", "matei.florescu@s.unibuc.ro" }),
            ("Echipa Proiect IS", new[] { "ioana.petrescu@s.unibuc.ro", "vlad.tanase@s.unibuc.ro", "laura.stoica@s.unibuc.ro", "matei.florescu@s.unibuc.ro" })
        };

        foreach (var (groupName, studentEmails) in membershipData)
        {
            if (!groups.TryGetValue(groupName, out var group)) continue;

            foreach (var email in studentEmails)
            {
                if (!students.TryGetValue(email, out var student)) continue;

                var exists = await context.GroupMembers.AnyAsync(gm =>
                    gm.GroupId == group.Id && gm.UserId == student.Id);
                if (exists) continue;

                context.GroupMembers.Add(new GroupMember
                {
                    GroupId = group.Id,
                    UserId = student.Id,
                    JoinedAt = DateTime.UtcNow
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task<List<GroupTask>> SeedGroupTasksAsync(
        ApplicationDbContext context,
        Dictionary<string, Group> groups,
        Dictionary<string, ApplicationUser> professors)
    {
        var tasks = new List<GroupTask>();

        if (await context.GroupTasks.CountAsync() > 10)
        {
            return await context.GroupTasks.ToListAsync();
        }

        var taskData = new[]
        {
            ("Grupa 131 - Algoritmica", "Tema 1: Sortari", "Implementati QuickSort, MergeSort, HeapSort.", 14),
            ("Grupa 131 - Algoritmica", "Tema 2: Cautare Binara", "Cautare binara iterativa si recursiva.", 7),
            ("Grupa 131 - Algoritmica", "Laborator: Complexitate", "Analizati complexitatea functiilor.", 4),
            ("Grupa 132 - POO", "Proiect: Sistem Biblioteca", "Sistem gestiune biblioteca OOP.", 21),
            ("Grupa 132 - POO", "Tema: Design Patterns", "Singleton, Factory, Observer.", 10),
            ("Grupa 132 - POO", "Quiz: Mostenire", "Pregatiti pentru quiz.", 5),
            ("Grupa 231 - Baze de Date", "Proiect: E-Commerce DB", "Baza date magazin online.", 28),
            ("Grupa 231 - Baze de Date", "Tema: Normalizare", "Normalizare la 3NF.", 10),
            ("Grupa 231 - Baze de Date", "Lab: SQL Avansat", "Subquery, window functions, CTE.", 6),
            ("Grupa 331 - Inteligenta Artificiala", "Proiect: Clasificator CNN", "Model CIFAR-10, acuratete 85%.", 35),
            ("Grupa 331 - Inteligenta Artificiala", "Tema: Regresie Liniara", "Regresie de la zero.", 8),
            ("Grupa 331 - Inteligenta Artificiala", "Lab: PyTorch", "Refaceti cu PyTorch.", 12),
            ("Echipa Proiect IS", "Sprint 1: Setup", "Git, CI/CD, arhitectura.", -16),
            ("Echipa Proiect IS", "Sprint 2: Auth", "Autentificare JWT.", -1),
            ("Echipa Proiect IS", "Sprint 3: Core", "Note, Grupuri, Evenimente.", 14),
            ("Echipa Proiect IS", "Sprint 4: Testing", "Teste si deploy.", 28),
            ("Echipa Proiect IS", "Documentatie", "README, UML, ghid.", 21)
        };

        foreach (var (groupName, title, desc, dueDays) in taskData)
        {
            if (!groups.TryGetValue(groupName, out var group)) continue;

            var task = new GroupTask
            {
                Title = title,
                Description = desc,
                GroupId = group.Id,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(dueDays),
                CreatedByProfessorId = group.ProfessorId
            };
            context.GroupTasks.Add(task);
            tasks.Add(task);
        }

        await context.SaveChangesAsync();
        return tasks;
    }

    private static async Task<List<Event>> SeedEventsAsync(
        ApplicationDbContext context,
        Dictionary<string, ApplicationUser> professors)
    {
        var events = new List<Event>();

        if (await context.Events.CountAsync() > 5)
        {
            return await context.Events.ToListAsync();
        }

        var eventData = new[]
        {
            ("Hackathon FMI 2025", "Hackathon anual. Premii 10.000 EUR.", 45, "Competition", "mihai.georgescu@unibuc.ro"),
            ("Career Day IT", "Amazon, Google, Microsoft, Adobe.", 30, "Career", "maria.ionescu@unibuc.ro"),
            ("Workshop: Git si GitHub", "Git profesionist: branching, PR.", 7, "Workshop", "andrei.dumitrescu@unibuc.ro"),
            ("Conferinta AI in Education", "AI in educatie.", 60, "Conference", "mihai.georgescu@unibuc.ro"),
            ("LAN Party FMI", "CS2, LoL, Valorant. Pizza!", 14, "Social", null),
            ("Sesiune Q&A Master", "Intrebari admitere master.", 21, "Academic", "elena.stanescu@unibuc.ro"),
            ("Docker & Kubernetes", "Workshop containerizare.", 10, "Workshop", "andrei.dumitrescu@unibuc.ro"),
            ("Prezentare Stagii", "Internship vara 2025.", 3, "Career", "maria.ionescu@unibuc.ro")
        };

        foreach (var (title, desc, days, category, profEmail) in eventData)
        {
            int? organizerId = null;
            if (profEmail != null && professors.TryGetValue(profEmail, out var prof))
            {
                organizerId = prof.Id;
            }

            var ev = new Event
            {
                Title = title,
                Description = desc,
                Date = DateTime.Now.AddDays(days),
                DateCreated = DateTime.Now,
                OrganizerId = organizerId,
                Category = category
            };
            context.Events.Add(ev);
            events.Add(ev);
        }

        await context.SaveChangesAsync();
        return events;
    }

    private static async Task SeedEventParticipantsAsync(
        ApplicationDbContext context,
        List<Event> events,
        Dictionary<string, ApplicationUser> students)
    {
        if (await context.EventParticipants.CountAsync() > 20) return;

        var studentList = students.Values.ToList();
        var random = new Random(42); // Fixed seed for reproducibility

        foreach (var ev in events)
        {
            // Add 3-7 random participants per event
            var participantCount = random.Next(3, 8);
            var selectedStudents = studentList.OrderBy(_ => random.Next()).Take(participantCount);

            foreach (var student in selectedStudents)
            {
                var exists = await context.EventParticipants.AnyAsync(ep =>
                    ep.EventId == ev.Id && ep.UserId == student.Id);
                if (exists) continue;

                context.EventParticipants.Add(new EventParticipant
                {
                    EventId = ev.Id,
                    UserId = student.Id,
                    JoinedAt = DateTime.UtcNow.AddDays(-random.Next(1, 10))
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedAnnouncementsAsync(
        ApplicationDbContext context,
        Dictionary<string, ApplicationUser> professors)
    {
        if (await context.Announcements.CountAsync() > 5) return;

        var profList = professors.Values.ToList();
        var announcementData = new[]
        {
            ("Inscrieri Hackathon FMI 2025", "Inscrierile pentru Hackathon sunt deschise!", "Events"),
            ("Program modificat sesiune", "Secretariat: Luni-Vineri, 10:00-14:00.", "Administrative"),
            ("Burse performanta", "Listele au fost publicate.", "Academic"),
            ("Mentenanta servere", "5 februarie, 22:00-06:00.", "Technical"),
            ("Carti noi biblioteca", "50 titluri noi IA si ML.", "Library"),
            ("Workshop interviu tehnic", "Inregistrati-va! Locuri limitate.", "Career"),
            ("Concurs ACM-ICPC", "Selectie lab 309, sambata.", "Competition"),
            ("Deadline licenta prelungit", "Pana pe 15 februarie.", "Academic")
        };

        var i = 0;
        foreach (var (title, content, category) in announcementData)
        {
            var prof = profList[i % profList.Count];
            context.Announcements.Add(new Announcement
            {
                Title = title,
                Content = content,
                Category = category,
                CreatedByUserId = prof.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            });
            i++;
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedLibraryAsync(
        ApplicationDbContext context,
        Dictionary<string, ApplicationUser> professors)
    {
        if (await context.LibraryFolders.CountAsync() > 3) return;

        var profList = professors.Values.ToList();
        var folders = new[]
        {
            "Algoritmica - Cursuri si Materiale",
            "POO - Programare Orientata pe Obiecte",
            "Baze de Date - SQL si Modelare",
            "Inteligenta Artificiala - ML si Deep Learning",
            "Inginerie Software - Best Practices",
            "Resurse Generale - Tutoriale si Ghiduri"
        };

        var createdFolders = new List<LibraryFolder>();
        foreach (var folderName in folders)
        {
            var folder = new LibraryFolder
            {
                Id = Guid.NewGuid(),
                Name = folderName,
                CreatedAtUtc = DateTime.UtcNow
            };
            context.LibraryFolders.Add(folder);
            createdFolders.Add(folder);
        }

        await context.SaveChangesAsync();

        // Add items to folders
        var itemData = new[]
        {
            (0, "Curs 1: Introducere Algoritmica", "https://fmi.unibuc.ro/algo/curs1.pdf"),
            (0, "Video: Sortari vizualizate", "https://youtube.com/watch?v=kPRA0W1kECg"),
            (1, "Tutorial: Design Patterns Java", "https://refactoring.guru/design-patterns"),
            (1, "Carte: Clean Code", "https://amazon.com/Clean-Code"),
            (2, "Tutorial SQL incepatori", "https://w3schools.com/sql/"),
            (2, "Tool: DB Diagram", "https://dbdiagram.io/"),
            (3, "Curs: Intro ML", "https://coursera.org/ml"),
            (3, "Tool: Google Colab", "https://colab.research.google.com/"),
            (4, "Guide: CI/CD GitHub Actions", "https://docs.github.com/actions"),
            (4, "Tutorial: Git Branching", "https://atlassian.com/git/tutorials"),
            (5, "GitHub Student Pack", "https://education.github.com/pack"),
            (5, "LeetCode", "https://leetcode.com/"),
            (5, "Roadmap.sh", "https://roadmap.sh/")
        };

        foreach (var (folderIndex, title, url) in itemData)
        {
            if (folderIndex >= createdFolders.Count) continue;

            context.LibraryItems.Add(new LibraryItem
            {
                Id = Guid.NewGuid(),
                FolderId = createdFolders[folderIndex].Id,
                Title = title,
                Type = LibraryItemType.Link,
                Url = url,
                CreatedByUserId = profList[folderIndex % profList.Count].Id.ToString(),
                CreatedAtUtc = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedCourseMaterialsAsync(
        ApplicationDbContext context,
        Dictionary<string, Group> groups,
        Dictionary<string, ApplicationUser> professors)
    {
        if (await context.CourseMaterials.CountAsync() > 5) return;

        // Course materials with real GeeksforGeeks links
        var materialData = new[]
        {
            ("Grupa 131 - Algoritmica", "Introducere in Algoritmica", "https://www.geeksforgeeks.org/introduction-to-algorithms/"),
            ("Grupa 131 - Algoritmica", "Algoritmi de Sortare", "https://www.geeksforgeeks.org/sorting-algorithms/"),
            ("Grupa 132 - POO", "Concepte OOP in Java", "https://www.geeksforgeeks.org/object-oriented-programming-oops-concept-in-java/"),
            ("Grupa 132 - POO", "Polimorfism in Java", "https://www.geeksforgeeks.org/polymorphism-in-java/"),
            ("Grupa 231 - Baze de Date", "Modelul Relational DBMS", "https://www.geeksforgeeks.org/relational-model-in-dbms/"),
            ("Grupa 231 - Baze de Date", "Tutorial SQL", "https://www.geeksforgeeks.org/sql-tutorial/"),
            ("Grupa 331 - Inteligenta Artificiala", "Machine Learning Basics", "https://www.geeksforgeeks.org/machine-learning/"),
            ("Grupa 331 - Inteligenta Artificiala", "Linear Regression", "https://www.geeksforgeeks.org/linear-regression-in-machine-learning/"),
            ("Echipa Proiect IS", "Git Flow Workflow", "https://www.geeksforgeeks.org/git-flow-workflow/"),
            ("Echipa Proiect IS", "Code Review Best Practices", "https://www.geeksforgeeks.org/code-review-in-software-development/")
        };

        foreach (var (groupName, title, url) in materialData)
        {
            if (!groups.TryGetValue(groupName, out var group)) continue;

            context.CourseMaterials.Add(new CourseMaterial
            {
                Title = title,
                Description = $"Material educativ: {title}",
                FileName = title.Replace(" ", "_") + ".link",
                FileUrl = url,
                FileType = "link",
                FileSize = 0,
                GroupId = group.Id,
                UploadedByProfessorId = group.ProfessorId,
                UploadedAt = DateTime.UtcNow.AddDays(-10)
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedSavedTasksAsync(
        ApplicationDbContext context,
        List<GroupTask> tasks,
        Dictionary<string, ApplicationUser> students)
    {
        if (await context.SavedTasks.CountAsync() > 20) return;

        var random = new Random(42);
        var studentList = students.Values.ToList();

        foreach (var task in tasks.Take(12)) // First 12 tasks
        {
            // 2-4 students save each task
            var savers = studentList.OrderBy(_ => random.Next()).Take(random.Next(2, 5));

            foreach (var student in savers)
            {
                var exists = await context.SavedTasks.AnyAsync(st =>
                    st.TaskId == task.Id && st.UserId == student.Id);
                if (exists) continue;

                var isCompleted = random.Next(100) < 40; // 40% completed
                context.SavedTasks.Add(new SavedTask
                {
                    UserId = student.Id,
                    TaskId = task.Id,
                    SavedAt = DateTime.UtcNow.AddDays(-random.Next(5, 15)),
                    IsCompleted = isCompleted,
                    CompletedAt = isCompleted ? DateTime.UtcNow.AddDays(-random.Next(1, 4)) : null
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedGroupAnnouncementsAsync(
        ApplicationDbContext context,
        Dictionary<string, Group> groups)
    {
        if (await context.GroupAnnouncements.CountAsync() > 5) return;

        // Get existing announcements to forward to groups
        var announcements = await context.Announcements.ToListAsync();
        if (!announcements.Any()) return;

        // Forward relevant announcements to each group
        var forwardData = new[]
        {
            ("Grupa 131 - Algoritmica", new[] { "Inscrieri Hackathon FMI 2025", "Concurs ACM-ICPC" }),
            ("Grupa 132 - POO", new[] { "Inscrieri Hackathon FMI 2025", "Workshop interviu tehnic" }),
            ("Grupa 231 - Baze de Date", new[] { "Inscrieri Hackathon FMI 2025", "Mentenanta servere" }),
            ("Grupa 331 - Inteligenta Artificiala", new[] { "Inscrieri Hackathon FMI 2025", "Carti noi biblioteca" }),
            ("Echipa Proiect IS", new[] { "Inscrieri Hackathon FMI 2025", "Workshop interviu tehnic", "Deadline licenta prelungit" })
        };

        foreach (var (groupName, announcementTitles) in forwardData)
        {
            if (!groups.TryGetValue(groupName, out var group)) continue;

            foreach (var title in announcementTitles)
            {
                var announcement = announcements.FirstOrDefault(a => a.Title == title);
                if (announcement == null) continue;

                var exists = await context.GroupAnnouncements.AnyAsync(ga =>
                    ga.GroupId == group.Id && ga.AnnouncementId == announcement.Id);
                if (exists) continue;

                context.GroupAnnouncements.Add(new GroupAnnouncement
                {
                    GroupId = group.Id,
                    AnnouncementId = announcement.Id,
                    ForwardedByProfessorId = group.ProfessorId,
                    ForwardedAt = DateTime.UtcNow.AddDays(-3)
                });
            }
        }

        await context.SaveChangesAsync();
    }
}