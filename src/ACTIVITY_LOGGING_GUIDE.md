# Activity Logging System - Guide de utilizare

## Prezentare generală

Acest sistem de audit păstrează toate acțiunile utilizatorilor și le afișează în Dashboard și în pagina de istoric complet.

## Structura sistemului

### Backend (C# / .NET)

1. **Entitate**: `UserActivity` (`CampusConnect.Domain/Entities/UserActivity.cs`)
   - `Id`: ID-ul activității
   - `UserId`: ID-ul utilizatorului
   - `ActivityType`: Tipul de acțiune (Create, Edit, Delete, Save, Join, Leave, Complete)
   - `EntityType`: Tipul entității (Profile, Event, Announcement, Group, Task, Achievement)
   - `EntityId`: ID-ul entității (opțional)
   - `EntityName`: Numele entității (opțional)
   - `Description`: Descriere custom (opțional)
   - `CreatedAt`: Data și ora activității

2. **Service**: `IActivityLoggerService` (`CampusConnect.Infrastructure/Services/ActivityLoggerService.cs`)
   - `LogActivityAsync()`: Înregistrează o activitate
   - `GetUserActivitiesAsync()`: Obține toate activitățile unui user
   - `GetRecentActivitiesAsync()`: Obține ultimele N activități

3. **Controller**: `ActivityController` (`CampusConnect.Api/Controllers/ActivityController.cs`)
   - `GET /api/Activity/recent`: Ultimele 3 activități
   - `GET /api/Activity/all`: Toate activitățile
   - `GET /api/Activity?limit=X`: Primele X activități

### Frontend (React / TypeScript)

1. **Service**: `activityApi` (`services/activityApi.ts`)
2. **Formatter**: `formatActivity()` (`utils/activityFormatter.ts`)
3. **Componente**:
   - Dashboard: Afișează ultimele 3 activități în `Home.tsx`
   - Activity History: Pagină completă cu toate activitățile în `ActivityHistory.tsx`

## Cum să adaugi logging pentru o acțiune

### Backend - Exemplu pentru a loga o activitate

În controller sau service, injectează `IActivityLoggerService`:

```csharp
private readonly IActivityLoggerService _activityLogger;

public YourController(IActivityLoggerService activityLogger)
{
    _activityLogger = activityLogger;
}
```

Apoi, după ce execuți o acțiune, loghează-o:

```csharp
// Exemplu: Creare Event
await _activityLogger.LogActivityAsync(
    userId: currentUserId,
    activityType: "Create",
    entityType: "Event",
    entityId: newEvent.Id,
    entityName: newEvent.Title,
    description: $"Created event '{newEvent.Title}'"
);

// Exemplu: Edit Profile
await _activityLogger.LogActivityAsync(
    userId: currentUserId,
    activityType: "Edit",
    entityType: "Profile",
    description: "Updated profile information"
);

// Exemplu: Join Group
await _activityLogger.LogActivityAsync(
    userId: currentUserId,
    activityType: "Join",
    entityType: "Group",
    entityId: groupId,
    entityName: group.Name,
    description: $"Joined group '{group.Name}'"
);

// Exemplu: Complete Task
await _activityLogger.LogActivityAsync(
    userId: currentUserId,
    activityType: "Complete",
    entityType: "Task",
    entityId: taskId,
    entityName: task.Title,
    description: $"Completed task '{task.Title}'"
);
```

## Tipuri de activități suportate

### Activity Types
- `Create`: Creare entitate nouă
- `Edit`: Editare entitate existentă
- `Delete`: Ștergere entitate
- `Save`: Salvare entitate (pentru saved announcements, events, tasks)
- `Join`: Alăturare (pentru grupuri, evenimente)
- `Leave`: Părăsire (pentru grupuri)
- `Complete`: Completare (pentru task-uri)

### Entity Types
- `Profile`: Profil utilizator
- `Event`: Evenimente
- `Announcement`: Anunțuri
- `Group`: Grupuri de studiu
- `Task` sau `GroupTask`: Task-uri
- `Achievement`: Achievement-uri

## Locuri unde trebuie adăugat logging

Următoarele acțiuni ar trebui să logăm:

### Profile
- ✅ Edit profile → `Edit`, `Profile`

### Events
- ✅ Create event → `Create`, `Event`
- ✅ Edit event → `Edit`, `Event`
- ✅ Delete event → `Delete`, `Event`
- ✅ Save event → `Save`, `Event`
- ✅ Join event → `Join`, `Event`

### Announcements
- ✅ Create announcement → `Create`, `Announcement`
- ✅ Edit announcement → `Edit`, `Announcement`
- ✅ Delete announcement → `Delete`, `Announcement`
- ✅ Save announcement → `Save`, `Announcement`

### Groups
- ✅ Create group → `Create`, `Group`
- ✅ Edit group → `Edit`, `Group`
- ✅ Delete group → `Delete`, `Group`
- ✅ Join group → `Join`, `Group`
- ✅ Leave group → `Leave`, `Group`

### Tasks
- ✅ Create task → `Create`, `Task`
- ✅ Edit task → `Edit`, `Task`
- ✅ Delete task → `Delete`, `Task`
- ✅ Save task → `Save`, `Task`
- ✅ Complete task → `Complete`, `Task`

### Achievements
- ✅ Create achievement → `Create`, `Achievement` (doar admin)
- ✅ Edit achievement → `Edit`, `Achievement` (doar admin)
- ✅ Delete achievement → `Delete`, `Achievement` (doar admin)

## Exemplu complet

```csharp
// În AnnouncementController.cs
[HttpPost]
public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementDto dto)
{
    var userId = GetCurrentUserId();

    var announcement = new Announcement
    {
        Title = dto.Title,
        Content = dto.Content,
        // ... alte proprietăți
    };

    _context.Announcements.Add(announcement);
    await _context.SaveChangesAsync();

    // LOG ACTIVITATE
    await _activityLogger.LogActivityAsync(
        userId: userId,
        activityType: "Create",
        entityType: "Announcement",
        entityId: announcement.Id,
        entityName: announcement.Title
    );

    return Ok(announcement);
}
```

## Note importante

- Serviciul este deja înregistrat în `Program.cs`
- Migrația pentru tabelul `UserActivities` a fost aplicată
- Activitățile sunt sortate descrescător după dată (cele mai recente primele)
- Pe Dashboard se afișează ultimele 3 activități
- Pagina Activity History oferă filtrare pe tip de activitate
- Sistemul este gata să fie folosit - trebuie doar adăugat logging-ul în controller-ele existente!
