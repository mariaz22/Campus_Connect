const API_BASE_URL = 'http://localhost:5099/api';

// Helper pentru a obține token-ul
const getAuthToken = () => {
  return localStorage.getItem('token');
};

// Helper pentru headere cu autentificare
const getAuthHeaders = () => {
  const token = getAuthToken();
  return {
    'Content-Type': 'application/json',
    ...(token && { 'Authorization': `Bearer ${token}` })
  };
};

export interface Group {
  id: number;
  name: string;
  description?: string;
  subject: string;
  professorId: number;
  professorName: string;
  createdAt: string;
  isActive: boolean;
  membersCount: number;
  tasksCount: number;
  isUserMember: boolean;
}

export interface GroupTask {
  id: number;
  title: string;
  description?: string;
  groupId: number;
  groupName: string;
  createdAt: string;
  dueDate?: string;
  createdByProfessorName: string;
  isSavedByUser: boolean;
  isCompleted: boolean;
}

export interface SavedTask {
  id: number;
  taskId: number;
  taskTitle: string;
  taskDescription?: string;
  groupName: string;
  subject: string;
  savedAt: string;
  dueDate?: string;
  isCompleted: boolean;
  completedAt?: string;
}

export interface CreateGroupRequest {
  name: string;
  description?: string;
  subject: string;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  dueDate?: string;
}

// Group API
export const groupApi = {
  // Get all groups
  getAllGroups: async (): Promise<Group[]> => {
    const response = await fetch(`${API_BASE_URL}/group`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch groups');
    return response.json();
  },

  // Get group by ID
  getGroupById: async (id: number): Promise<Group> => {
    const response = await fetch(`${API_BASE_URL}/group/${id}`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch group');
    return response.json();
  },

  // Get my groups (where I'm a member)
  getMyGroups: async (): Promise<Group[]> => {
    const response = await fetch(`${API_BASE_URL}/group/my-groups`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch my groups');
    return response.json();
  },

  // Get groups created by me (professor)
  getGroupsCreatedByMe: async (): Promise<Group[]> => {
    const response = await fetch(`${API_BASE_URL}/group/created-by-me`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch created groups');
    return response.json();
  },

  // Get available groups (not a member)
  getAvailableGroups: async (): Promise<Group[]> => {
    const response = await fetch(`${API_BASE_URL}/group/available`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch available groups');
    return response.json();
  },

  // Create group (professor only)
  createGroup: async (data: CreateGroupRequest): Promise<Group> => {
    const response = await fetch(`${API_BASE_URL}/group`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(data)
    });
    if (!response.ok) throw new Error('Failed to create group');
    return response.json();
  },

  // Delete group (professor only)
  deleteGroup: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/group/${id}`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to delete group');
  },

  // Join group
  joinGroup: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/group/${id}/join`, {
      method: 'POST',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to join group');
  },

  // Leave group
  leaveGroup: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/group/${id}/leave`, {
      method: 'POST',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to leave group');
  },

  // Get group tasks
  getGroupTasks: async (groupId: number): Promise<GroupTask[]> => {
    const response = await fetch(`${API_BASE_URL}/group/${groupId}/tasks`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch tasks');
    return response.json();
  },

  // Create task (professor only)
  createTask: async (groupId: number, data: CreateTaskRequest): Promise<GroupTask> => {
    const response = await fetch(`${API_BASE_URL}/group/${groupId}/tasks`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(data)
    });
    if (!response.ok) throw new Error('Failed to create task');
    return response.json();
  },

  // Delete task (professor only)
  deleteTask: async (taskId: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/group/tasks/${taskId}`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to delete task');
  },

  // Save task
  saveTask: async (taskId: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/group/tasks/${taskId}/save`, {
      method: 'POST',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to save task');
  },

  // Unsave task
  unsaveTask: async (taskId: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/group/tasks/${taskId}/unsave`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to unsave task');
  },

  // Mark task as completed
  markTaskAsCompleted: async (taskId: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/group/tasks/${taskId}/complete`, {
      method: 'PATCH',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to mark task as completed');
  },

  // Mark task as incomplete
  markTaskAsIncomplete: async (taskId: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/group/tasks/${taskId}/incomplete`, {
      method: 'PATCH',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to mark task as incomplete');
  },

  // Get my saved tasks
  getMySavedTasks: async (): Promise<SavedTask[]> => {
    const response = await fetch(`${API_BASE_URL}/group/my-saved-tasks`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch saved tasks');
    return response.json();
  }
};
