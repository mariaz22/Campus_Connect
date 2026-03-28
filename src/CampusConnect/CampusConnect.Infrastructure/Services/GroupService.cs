using CampusConnect.Application.DTOs.Groups;
using CampusConnect.Application.Interfaces;
using CampusConnect.Domain.Entities;
using CampusConnect.Domain.Services;
using CampusConnect.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampusConnect.Infrastructure.Services;

public class GroupService : IGroupService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public GroupService(
        ApplicationDbContext context, 
        ICurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    public async Task<GroupResponse> CreateGroupAsync(CreateGroupRequest request)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var user = await _userManager.FindByIdAsync(userId.ToString()!);
        var roles = await _userManager.GetRolesAsync(user!);
        
        if (!roles.Contains("Professor") && !roles.Contains("Admin"))
            throw new UnauthorizedAccessException("Only professors and admins can create groups");

        var group = new Group
        {
            Name = request.Name,
            Description = request.Description,
            Subject = request.Subject,
            ProfessorId = userId.Value,
            CreatedAt = DateTime.UtcNow
        };

        _context.Groups.Add(group);
        await _context.SaveChangesAsync();

        return await MapToGroupResponse(group, userId.Value);
    }

    public async Task<IEnumerable<GroupResponse>> GetAllGroupsAsync()
    {
        var userId = _currentUserService.GetCurrentUserId() ?? 0;
        var groups = await _context.Groups
            .Where(g => g.IsActive)
            .Include(g => g.Professor)
            .Include(g => g.Members)
            .Include(g => g.Tasks)
            .ToListAsync();

        return groups.Select(g => MapToGroupResponse(g, userId).Result);
    }

    public async Task<GroupResponse> GetGroupByIdAsync(int id)
{
    var group = await _context.Groups
        .Include(g => g.Professor)
        .Include(g => g.Members) // <--- CRUCIAL pentru butonul de Join/Leave
        .FirstOrDefaultAsync(g => g.Id == id);

    if (group == null) return null;

    return new GroupResponse
    {
        Id = group.Id,
        Name = group.Name,
        Subject = group.Subject,
        Description = group.Description,
        ProfessorId = group.ProfessorId,
        MembersCount = group.Members.Count,
        ProfessorName = $"{group.Professor.FirstName} {group.Professor.LastName}",
        CreatedAt = group.CreatedAt,
        IsActive = group.IsActive,
        TasksCount = await _context.GroupTasks.CountAsync(t => t.GroupId == group.Id),
        IsUserMember = group.Members.Any(m => m.UserId == _currentUserService.GetCurrentUserId() ? true : false)

    };

}

    public async Task<IEnumerable<GroupResponse>> GetMyGroupsAsync()
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var groups = await _context.GroupMembers
            .Where(gm => gm.UserId == userId.Value)
            .Include(gm => gm.Group)
                .ThenInclude(g => g.Professor)
            .Include(gm => gm.Group)
                .ThenInclude(g => g.Members)
            .Include(gm => gm.Group)
                .ThenInclude(g => g.Tasks)
            .Select(gm => gm.Group)
            .ToListAsync();

        return groups.Select(g => MapToGroupResponse(g, userId.Value).Result);
    }

    public async Task<IEnumerable<GroupResponse>> GetGroupsCreatedByMeAsync()
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var groups = await _context.Groups
            .Where(g => g.ProfessorId == userId.Value && g.IsActive)
            .Include(g => g.Professor)
            .Include(g => g.Members)
            .Include(g => g.Tasks)
            .ToListAsync();

        return groups.Select(g => MapToGroupResponse(g, userId.Value).Result);
    }

    public async Task<bool> DeleteGroupAsync(int groupId)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var group = await _context.Groups.FindAsync(groupId);
        if (group == null)
            return false;

        if (group.ProfessorId != userId.Value)
            throw new UnauthorizedAccessException("Only the group creator can delete it");

        group.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> JoinGroupAsync(int groupId)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        // Verifică dacă user-ul este profesor (adminii pot join pentru verificare)
        var user = await _userManager.FindByIdAsync(userId.ToString()!);
        var roles = await _userManager.GetRolesAsync(user!);
        
        if (roles.Contains("Professor") && !roles.Contains("Admin"))
            throw new UnauthorizedAccessException("Professors cannot join groups. They can only create them.");

        var group = await _context.Groups.FindAsync(groupId);
        if (group == null || !group.IsActive)
            return false;

        var existingMember = await _context.GroupMembers
            .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId.Value);

        if (existingMember != null)
            return false; // Already a member

        var groupMember = new GroupMember
        {
            GroupId = groupId,
            UserId = userId.Value,
            JoinedAt = DateTime.UtcNow
        };

        _context.GroupMembers.Add(groupMember);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LeaveGroupAsync(int groupId)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var groupMember = await _context.GroupMembers
            .FirstOrDefaultAsync(gm => gm.GroupId == groupId && gm.UserId == userId.Value);

        if (groupMember == null)
            return false;

        _context.GroupMembers.Remove(groupMember);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<GroupResponse>> GetAvailableGroupsAsync()
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var joinedGroupIds = await _context.GroupMembers
            .Where(gm => gm.UserId == userId.Value)
            .Select(gm => gm.GroupId)
            .ToListAsync();

        var groups = await _context.Groups
            .Where(g => g.IsActive && !joinedGroupIds.Contains(g.Id))
            .Include(g => g.Professor)
            .Include(g => g.Members)
            .Include(g => g.Tasks)
            .ToListAsync();

        return groups.Select(g => MapToGroupResponse(g, userId.Value).Result);
    }

    public async Task<GroupTaskResponse> CreateTaskAsync(int groupId, CreateTaskRequest request)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var group = await _context.Groups
            .Include(g => g.Professor)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
            throw new KeyNotFoundException("Group not found");

        var user = await _userManager.FindByIdAsync(userId.ToString()!);
        var roles = await _userManager.GetRolesAsync(user!);
        var isAdmin = roles.Contains("Admin");

        if (group.ProfessorId != userId.Value && !isAdmin)
            throw new UnauthorizedAccessException("Only the group professor or admins can create tasks");

        var task = new GroupTask
        {
            Title = request.Title,
            Description = request.Description,
            GroupId = groupId,
            DueDate = request.DueDate,
            CreatedByProfessorId = userId.Value,
            CreatedAt = DateTime.UtcNow
        };

        _context.GroupTasks.Add(task);
        await _context.SaveChangesAsync();

        return await MapToTaskResponse(task, userId.Value);
    }

    public async Task<IEnumerable<GroupTaskResponse>> GetGroupTasksAsync(int groupId)
    {
        var userId = _currentUserService.GetCurrentUserId() ?? 0;
        var tasks = await _context.GroupTasks
            .Where(t => t.GroupId == groupId)
            .Include(t => t.Group)
            .Include(t => t.CreatedByProfessor)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(t => MapToTaskResponse(t, userId).Result);
    }
public async Task<bool> DeleteTaskAsync(int taskId, int userId)
{
    var task = await _context.GroupTasks
        .Include(t => t.Group) 
        .FirstOrDefaultAsync(t => t.Id == taskId);

    if (task == null) return false;

    if (task.Group.ProfessorId != userId) 
    {
        throw new UnauthorizedAccessException("Nu aveți permisiunea să ștergeți acest task.");
    }
    var savedTasks = _context.SavedTasks.Where(st => st.TaskId == taskId);
    _context.SavedTasks.RemoveRange(savedTasks);

    _context.GroupTasks.Remove(task);
    await _context.SaveChangesAsync();

    return true;
}

    public async Task<bool> SaveTaskAsync(int taskId)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var task = await _context.GroupTasks.FindAsync(taskId);
        if (task == null)
            return false;

        var existingSaved = await _context.SavedTasks
            .FirstOrDefaultAsync(st => st.TaskId == taskId && st.UserId == userId.Value);

        if (existingSaved != null)
            return false; // Already saved

        var savedTask = new SavedTask
        {
            TaskId = taskId,
            UserId = userId.Value,
            SavedAt = DateTime.UtcNow
        };

        _context.SavedTasks.Add(savedTask);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnsaveTaskAsync(int taskId)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var savedTask = await _context.SavedTasks
            .FirstOrDefaultAsync(st => st.TaskId == taskId && st.UserId == userId.Value);

        if (savedTask == null)
            return false;

        _context.SavedTasks.Remove(savedTask);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkTaskAsCompletedAsync(int taskId)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var savedTask = await _context.SavedTasks
            .FirstOrDefaultAsync(st => st.TaskId == taskId && st.UserId == userId.Value);

        if (savedTask == null)
            return false;

        savedTask.IsCompleted = true;
        savedTask.CompletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkTaskAsIncompletedAsync(int taskId)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var savedTask = await _context.SavedTasks
            .FirstOrDefaultAsync(st => st.TaskId == taskId && st.UserId == userId.Value);

        if (savedTask == null)
            return false;

        savedTask.IsCompleted = false;
        savedTask.CompletedAt = null;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SavedTaskResponse>> GetMySavedTasksAsync()
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var savedTasks = await _context.SavedTasks
            .Where(st => st.UserId == userId.Value)
            .Include(st => st.Task)
                .ThenInclude(t => t.Group)
            .OrderByDescending(st => st.SavedAt)
            .ToListAsync();

        return savedTasks.Select(st => new SavedTaskResponse
        {
            Id = st.Id,
            TaskId = st.TaskId,
            TaskTitle = st.Task.Title,
            TaskDescription = st.Task.Description,
            GroupName = st.Task.Group.Name,
            Subject = st.Task.Group.Subject,
            SavedAt = st.SavedAt,
            DueDate = st.Task.DueDate,
            IsCompleted = st.IsCompleted,
            CompletedAt = st.CompletedAt
        });
    }

    private async Task<GroupResponse> MapToGroupResponse(Group group, int currentUserId)
    {
        var isMember = await _context.GroupMembers
            .AnyAsync(gm => gm.GroupId == group.Id && gm.UserId == currentUserId);

        return new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            Subject = group.Subject,
            ProfessorId = group.ProfessorId,
            ProfessorName = $"{group.Professor.FirstName} {group.Professor.LastName}",
            CreatedAt = group.CreatedAt,
            IsActive = group.IsActive,
            MembersCount = group.Members.Count,
            TasksCount = group.Tasks.Count,
            IsUserMember = isMember
        };
    }

    private async Task<GroupTaskResponse> MapToTaskResponse(GroupTask task, int currentUserId)
    {
        var isSaved = await _context.SavedTasks
            .AnyAsync(st => st.TaskId == task.Id && st.UserId == currentUserId);

        var savedTask = await _context.SavedTasks
            .FirstOrDefaultAsync(st => st.TaskId == task.Id && st.UserId == currentUserId);

        return new GroupTaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            GroupId = task.GroupId,
            GroupName = task.Group.Name,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            CreatedByProfessorName = $"{task.CreatedByProfessor.FirstName} {task.CreatedByProfessor.LastName}",
            IsSavedByUser = isSaved,
            IsCompleted = savedTask?.IsCompleted ?? false
        };
    }

    // ============= Course Materials Methods =============
    
    public async Task<CourseMaterialDto> UploadCourseMaterialAsync(CreateCourseMaterialRequest request)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var user = await _userManager.FindByIdAsync(userId.ToString()!);
        var roles = await _userManager.GetRolesAsync(user!);
        
        if (!roles.Contains("Professor") && !roles.Contains("Admin"))
            throw new UnauthorizedAccessException("Only professors can upload course materials");

        var group = await _context.Groups.FindAsync(request.GroupId);
        if (group == null)
            throw new InvalidOperationException("Group not found");

        if (group.ProfessorId != userId.Value)
            throw new UnauthorizedAccessException("Only the professor who created the group can upload materials");

        // Validare fișier
        if (request.File == null || request.File.Length == 0)
            throw new InvalidOperationException("File is required");

        // Salvare fișier (în producție ar fi într-un cloud storage)
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "course-materials");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{request.File.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }

        var fileUrl = $"/uploads/course-materials/{uniqueFileName}";
        var fileExtension = Path.GetExtension(request.File.FileName).TrimStart('.');

        var material = new CourseMaterial
        {
            Title = request.Title,
            Description = request.Description,
            FileName = request.File.FileName,
            FileUrl = fileUrl,
            FileType = fileExtension,
            FileSize = request.File.Length,
            GroupId = request.GroupId,
            UploadedByProfessorId = userId.Value,
            UploadedAt = DateTime.UtcNow
        };

        _context.CourseMaterials.Add(material);
        await _context.SaveChangesAsync();

        var professor = await _context.Users.FindAsync(userId.Value);
        
        return new CourseMaterialDto
        {
            Id = material.Id,
            Title = material.Title,
            Description = material.Description,
            FileName = material.FileName,
            FileUrl = material.FileUrl,
            FileType = material.FileType,
            FileSize = material.FileSize,
            GroupId = material.GroupId,
            UploadedByProfessorId = material.UploadedByProfessorId,
            UploadedByProfessorName = $"{professor!.FirstName} {professor.LastName}",
            UploadedAt = material.UploadedAt
        };
    }

    public async Task<IEnumerable<CourseMaterialDto>> GetGroupMaterialsAsync(int groupId)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var group = await _context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
            throw new InvalidOperationException("Group not found");

        // Verifică dacă userul este membru sau profesor al grupului
        var isMember = group.Members.Any(m => m.UserId == userId.Value);
        var isProfessor = group.ProfessorId == userId.Value;

        if (!isMember && !isProfessor)
            throw new UnauthorizedAccessException("You must be a member of this group to view materials");

        var materials = await _context.CourseMaterials
            .Include(cm => cm.UploadedByProfessor)
            .Where(cm => cm.GroupId == groupId)
            .OrderByDescending(cm => cm.UploadedAt)
            .ToListAsync();

        return materials.Select(m => new CourseMaterialDto
        {
            Id = m.Id,
            Title = m.Title,
            Description = m.Description,
            FileName = m.FileName,
            FileUrl = m.FileUrl,
            FileType = m.FileType,
            FileSize = m.FileSize,
            GroupId = m.GroupId,
            UploadedByProfessorId = m.UploadedByProfessorId,
            UploadedByProfessorName = $"{m.UploadedByProfessor.FirstName} {m.UploadedByProfessor.LastName}",
            UploadedAt = m.UploadedAt
        });
    }

    public async Task<bool> DeleteCourseMaterialAsync(int materialId)
    {
        var userId = _currentUserService.GetCurrentUserId();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");

        var material = await _context.CourseMaterials
            .Include(cm => cm.Group)
            .FirstOrDefaultAsync(cm => cm.Id == materialId);

        if (material == null)
            throw new InvalidOperationException("Course material not found");

        if (material.UploadedByProfessorId != userId.Value)
            throw new UnauthorizedAccessException("Only the professor who uploaded this material can delete it");

        // Șterge fișierul fizic
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", material.FileUrl.TrimStart('/'));
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        _context.CourseMaterials.Remove(material);
        await _context.SaveChangesAsync();

        return true;
    }}