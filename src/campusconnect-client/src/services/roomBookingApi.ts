const API_BASE_URL = 'http://localhost:5099/api';

const getAuthHeaders = () => {
  const token = localStorage.getItem('token');
  return {
    'Content-Type': 'application/json',
    ...(token && { 'Authorization': `Bearer ${token}` })
  };
};

export interface RoomBookingRequest {
  id: number;
  title: string;
  description?: string;
  roomId: number;
  roomName: string;
  buildingId: number;
  buildingName: string;
  startTime: string;
  endTime: string;
  recurrencePattern?: string;
  recurrenceEndDate?: string;
  requestedByUserId: number;
  requestedByUserName: string;
  requestedByUserEmail: string;
  status: 'Pending' | 'Approved' | 'Rejected';
  reviewedByAdminId?: number;
  reviewedByAdminName?: string;
  reviewedAt?: string;
  rejectionReason?: string;
  createdAt: string;
}

export interface CreateRoomBookingRequest {
  title: string;
  description?: string;
  roomId: number;
  startTime: string;
  endTime: string;
  recurrencePattern?: string;
  recurrenceEndDate?: string;
}

export interface ReviewBookingRequest {
  approve: boolean;
  rejectionReason?: string;
}

export const roomBookingApi = {
  // Create a new booking request (User only)
  createBookingRequest: async (request: CreateRoomBookingRequest): Promise<RoomBookingRequest> => {
    const response = await fetch(`${API_BASE_URL}/roombooking`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(request)
    });
    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Failed to create booking request');
    }
    return response.json();
  },

  // Get all pending requests (Admin only)
  getPendingRequests: async (): Promise<RoomBookingRequest[]> => {
    const response = await fetch(`${API_BASE_URL}/roombooking/pending`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch pending requests');
    return response.json();
  },

  // Get my booking requests
  getMyRequests: async (): Promise<RoomBookingRequest[]> => {
    const response = await fetch(`${API_BASE_URL}/roombooking/my-requests`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch my requests');
    return response.json();
  },

  // Get a specific request by ID
  getRequestById: async (id: number): Promise<RoomBookingRequest> => {
    const response = await fetch(`${API_BASE_URL}/roombooking/${id}`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch request');
    return response.json();
  },

  // Approve a booking request (Admin only)
  approveRequest: async (id: number): Promise<RoomBookingRequest> => {
    const response = await fetch(`${API_BASE_URL}/roombooking/${id}/approve`, {
      method: 'POST',
      headers: getAuthHeaders()
    });
    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Failed to approve request');
    }
    return response.json();
  },

  // Reject a booking request (Admin only)
  rejectRequest: async (id: number, rejectionReason?: string): Promise<RoomBookingRequest> => {
    const response = await fetch(`${API_BASE_URL}/roombooking/${id}/reject`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify({ approve: false, rejectionReason })
    });
    if (!response.ok) {
      const error = await response.text();
      throw new Error(error || 'Failed to reject request');
    }
    return response.json();
  },

  // Delete a booking request
  deleteRequest: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/roombooking/${id}`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to delete request');
  }
};
