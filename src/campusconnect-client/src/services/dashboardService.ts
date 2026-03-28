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

export interface DashboardStats {
  activeGroups: number;
  tasksCompleted: number;
  totalTasks: number;
  eventsJoined: number;
  totalAnnouncements: number;
  newAnnouncementsToday: number;
}

export const dashboardService = {
  async getDashboardStats(): Promise<DashboardStats> {
    try {
      const [groupsRes, tasksRes, eventsRes, announcementsRes] = await Promise.all([
        axios.get(`${API_URL}/Group/my-groups`, getAuthHeader()),
        axios.get(`${API_URL}/Group/my-saved-tasks`, getAuthHeader()),
        axios.get(`${API_URL}/Event/my-events`, getAuthHeader()),
        axios.get(`${API_URL}/Announcements`, getAuthHeader()),
      ]);

      const groups = groupsRes.data;
      const tasks = tasksRes.data;
      const events = eventsRes.data;
      const announcements = announcementsRes.data;

      const completedTasks = tasks.filter((task: any) => task.isCompleted).length;
      const totalTasks = tasks.length;

      const today = new Date();
      today.setHours(0, 0, 0, 0);
      const newAnnouncementsToday = announcements.filter((ann: any) => {
        const createdAt = new Date(ann.createdAt);
        createdAt.setHours(0, 0, 0, 0);
        return createdAt.getTime() === today.getTime();
      }).length;

      return {
        activeGroups: groups.length,
        tasksCompleted: completedTasks,
        totalTasks: totalTasks,
        eventsJoined: events.length,
        totalAnnouncements: announcements.length,
        newAnnouncementsToday: newAnnouncementsToday,
      };
    } catch (error) {
      console.error('Error fetching dashboard stats:', error);
      return {
        activeGroups: 0,
        tasksCompleted: 0,
        totalTasks: 0,
        eventsJoined: 0,
        totalAnnouncements: 0,
        newAnnouncementsToday: 0,
      };
    }
  },
};
