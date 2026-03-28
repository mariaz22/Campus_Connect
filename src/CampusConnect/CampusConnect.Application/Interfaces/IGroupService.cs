using CampusConnect.Application.DTOs.Groups;

namespace CampusConnect.Application.Interfaces;

public interface IGroupService
{
    // Group Management (doar profesori pot crea)
    Task<GroupResponse> CreateGroupAsync(CreateGroupRequest request);
    Task<IEnumerable<GroupResponse>> GetAllGroupsAsync();
    Task<GroupResponse?> GetGroupByIdAsync(int groupId);
    Task<IEnumerable<GroupResponse>> GetMyGroupsAsync(); // Grupurile unde userul este membru
    Task<IEnumerable<GroupResponse>> GetGroupsCreatedByMeAsync(); // Grupurile create de profesor
    Task<bool> DeleteGroupAsync(int groupId);
    
    // Group Membership (studenții pot face join)
    Task<bool> JoinGroupAsync(int groupId);
    Task<bool> LeaveGroupAsync(int groupId);
    Task<IEnumerable<GroupResponse>> GetAvailableGroupsAsync(); // Grupuri la care nu este membru
    
    // Task Management (doar profesorul care a creat grupul poate posta taskuri)
    Task<GroupTaskResponse> CreateTaskAsync(int groupId, CreateTaskRequest request);
    Task<IEnumerable<GroupTaskResponse>> GetGroupTasksAsync(int groupId);
    Task<bool> DeleteTaskAsync(int taskId, int userId);
    
    // Saved Tasks (studenții pot salva taskuri)
    Task<bool> SaveTaskAsync(int taskId);
    Task<bool> UnsaveTaskAsync(int taskId);
    Task<bool> MarkTaskAsCompletedAsync(int taskId);
    Task<bool> MarkTaskAsIncompletedAsync(int taskId);
    Task<IEnumerable<SavedTaskResponse>> GetMySavedTasksAsync();
    
    // Course Materials (doar profesorul care a creat grupul poate urca materiale)
    Task<CourseMaterialDto> UploadCourseMaterialAsync(CreateCourseMaterialRequest request);
    Task<IEnumerable<CourseMaterialDto>> GetGroupMaterialsAsync(int groupId);
    Task<bool> DeleteCourseMaterialAsync(int materialId);
}
