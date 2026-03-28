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

// Types
export interface ChatMessage {
  id: string;
  content: string;
  role: 'user' | 'assistant';
  timestamp: Date;
  suggestedActions?: SuggestedAction[];
}

export interface SuggestedAction {
  label: string;
  actionType: 'navigate' | 'link' | 'query';
  payload?: string;
}

export interface ChatRequest {
  message: string;
  sessionId?: string;
}

export interface ChatResponse {
  message: string;
  sessionId: string;
  timestamp: string;
  suggestedActions?: SuggestedAction[];
}

export interface UserContext {
  userId: number;
  firstName: string;
  lastName: string;
  email: string;
  role: string;
  groupNames: string[];
  subjectNames: string[];
  totalBuildings: number;
  totalRooms: number;
}

// Assistant API
export const assistantApi = {
  // Send a chat message
  sendMessage: async (request: ChatRequest): Promise<ChatResponse> => {
    const response = await fetch(`${API_BASE_URL}/assistant/chat`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(request)
    });
    if (!response.ok) {
      const error = await response.json().catch(() => ({ message: 'Failed to send message' }));
      throw new Error(error.message || 'Failed to send message');
    }
    return response.json();
  },

  // Get user context
  getUserContext: async (): Promise<UserContext> => {
    const response = await fetch(`${API_BASE_URL}/assistant/context`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to get user context');
    return response.json();
  },

  // Get suggested questions
  getSuggestions: async (): Promise<string[]> => {
    const response = await fetch(`${API_BASE_URL}/assistant/suggestions`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to get suggestions');
    return response.json();
  }
};
