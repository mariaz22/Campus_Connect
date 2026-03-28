const API_URL = 'http://localhost:5099/api';

export interface Achievement {
  id: number;
  title: string;
  description: string;
  icon: string;
  createdAt: string;
  isActive: boolean;
}

export interface UserAchievement {
  id: number;
  achievementId: number;
  title: string;
  description: string;
  icon: string;
  unlockedAt: string;
}

export interface CreateAchievementRequest {
  title: string;
  description: string;
  icon: string;
}

export interface UpdateAchievementRequest {
  title: string;
  description: string;
  icon: string;
  isActive: boolean;
}

const achievementApi = {
  // Get all achievements
  getAllAchievements: async (): Promise<Achievement[]> => {
    const response = await fetch(`${API_URL}/achievement`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error('Failed to fetch achievements');
    }

    return response.json();
  },

  // Get achievement by ID
  getAchievementById: async (id: number): Promise<Achievement> => {
    const response = await fetch(`${API_URL}/achievement/${id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error('Failed to fetch achievement');
    }

    return response.json();
  },

  // Get user achievements
  getUserAchievements: async (userId: number): Promise<UserAchievement[]> => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/achievement/user/${userId}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to fetch user achievements');
    }

    return response.json();
  },

  // Get my achievements
  getMyAchievements: async (): Promise<UserAchievement[]> => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/achievement/my`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to fetch my achievements');
    }

    return response.json();
  },

  // Create achievement (Admin only)
  createAchievement: async (request: CreateAchievementRequest): Promise<Achievement> => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/achievement`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      throw new Error('Failed to create achievement');
    }

    return response.json();
  },

  // Update achievement (Admin only)
  updateAchievement: async (id: number, request: UpdateAchievementRequest): Promise<Achievement> => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/achievement/${id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
      body: JSON.stringify(request),
    });

    if (!response.ok) {
      throw new Error('Failed to update achievement');
    }

    return response.json();
  },

  // Delete achievement (Admin only)
  deleteAchievement: async (id: number): Promise<void> => {
    const token = localStorage.getItem('token');
    const response = await fetch(`${API_URL}/achievement/${id}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    });

    if (!response.ok) {
      throw new Error('Failed to delete achievement');
    }
  },
};

export default achievementApi;
