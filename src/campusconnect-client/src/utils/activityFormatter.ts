import type { UserActivity } from '../services/activityApi';

export interface FormattedActivity {
  title: string;
  time: string;
  type: 'task' | 'event' | 'group' | 'announcement' | 'profile' | 'achievement';
  entityId?: number;
}

export const formatActivity = (activity: UserActivity): FormattedActivity => {
  const actionMap: Record<string, string> = {
    'Create': 'created',
    'Edit': 'updated',
    'Delete': 'deleted',
    'Save': 'saved',
    'Join': 'joined',
    'Leave': 'left',
    'Complete': 'completed',
  };

  const action = actionMap[activity.activityType] || activity.activityType.toLowerCase();
  const entityType = activity.entityType.toLowerCase();

  let title = '';
  if (activity.entityName) {
    title = `${action.charAt(0).toUpperCase() + action.slice(1)} ${entityType}: ${activity.entityName}`;
  } else {
    title = `${action.charAt(0).toUpperCase() + action.slice(1)} ${entityType}`;
  }

  if (activity.description) {
    title = activity.description;
  }

  let dateString = activity.createdAt;
  if (typeof dateString === 'string' && !dateString.endsWith('Z') && !dateString.includes('+')) {
    dateString += 'Z';
  }
  
  const time = formatTimeAgo(new Date(dateString));

  const typeMap: Record<string, FormattedActivity['type']> = {
    'task': 'task',
    'grouptask': 'task',
    'event': 'event',
    'group': 'group',
    'announcement': 'announcement',
    'profile': 'profile',
    'achievement': 'achievement',
  };

  const type = typeMap[entityType] || 'profile';

  return {
    title,
    time,
    type,
    entityId: activity.entityId,
  };
};

export const formatTimeAgo = (date: Date): string => {
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffSec = Math.floor(diffMs / 1000);
  const diffMin = Math.floor(diffSec / 60);
  const diffHour = Math.floor(diffMin / 60);
  const diffDay = Math.floor(diffHour / 24);

  if (diffSec < 60) {
    return 'just now';
  } else if (diffMin < 60) {
    return `${diffMin} minute${diffMin !== 1 ? 's' : ''} ago`;
  } else if (diffHour < 24) {
    return `${diffHour} hour${diffHour !== 1 ? 's' : ''} ago`;
  } else if (diffDay < 7) {
    return `${diffDay} day${diffDay !== 1 ? 's' : ''} ago`;
  } else if (diffDay < 30) {
    const weeks = Math.floor(diffDay / 7);
    return `${weeks} week${weeks !== 1 ? 's' : ''} ago`;
  } else {
    return date.toLocaleDateString();
  }
};