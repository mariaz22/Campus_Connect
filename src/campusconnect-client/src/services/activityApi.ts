import axios from 'axios';

const API_URL = 'http://localhost:5099/api';

const getAuthHeader = () => {
  const token = localStorage.getItem('token');
  return {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  };
};

export interface UserActivity {
  id: number;
  userId: number;
  activityType: string;
  entityType: string;
  entityId?: number;
  entityName?: string;
  description?: string;
  createdAt: string;
}

export const activityApi = {
  async getRecentActivities(): Promise<UserActivity[]> {
    const response = await axios.get(`${API_URL}/Activity/recent`, getAuthHeader());
    return response.data;
  },

  async getAllActivities(): Promise<UserActivity[]> {
    const response = await axios.get(`${API_URL}/Activity/all`, getAuthHeader());
    return response.data;
  },

  async getActivities(limit?: number): Promise<UserActivity[]> {
    const params = limit ? { limit } : {};
    const response = await axios.get(`${API_URL}/Activity`, {
      ...getAuthHeader(),
      params,
    });
    return response.data;
  },
};

export default activityApi;
