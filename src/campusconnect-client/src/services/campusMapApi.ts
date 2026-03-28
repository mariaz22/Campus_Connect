const API_BASE_URL = 'http://localhost:5099/api';

const getAuthHeaders = () => {
  const token = localStorage.getItem('token');
  return {
    'Content-Type': 'application/json',
    ...(token && { 'Authorization': `Bearer ${token}` })
  };
};

export interface Building {
  id: number;
  name: string;
  description?: string;
  address: string;
  latitude: number;
  longitude: number;
  geoJsonPolygon?: string;
  roomsCount: number;
}

export interface Room {
  id: number;
  name: string;
  capacity?: number;
  floor?: string;
  equipment?: string;
  buildingId: number;
  buildingName: string;
  currentStatus: 'Free' | 'Occupied' | 'OccupiedSoon' | 'Unknown';
  occupiedUntil?: string;
  nextOccupiedAt?: string;
}

export interface RoomDetails extends Room {
  todaySchedules: Schedule[];
}

export interface Schedule {
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
  professorName: string;
  isCurrentlyActive: boolean;
}

export interface CreateScheduleRequest {
  title: string;
  description?: string;
  roomId: number;
  startTime: string;
  endTime: string;
  recurrencePattern?: string;
  recurrenceEndDate?: string;
}

export interface UpdateScheduleRequest {
  title?: string;
  description?: string;
  startTime?: string;
  endTime?: string;
  recurrencePattern?: string;
  recurrenceEndDate?: string;
}

export interface RoomReservation {
  id: number;
  title: string;
  description?: string;
  roomId: number;
  roomName: string;
  buildingId: number;
  buildingName: string;
  startTime: string;
  endTime: string;
  requestedByUserId: number;
  requestedByUserName: string;
  status: 'Pending' | 'Approved' | 'Rejected';
  processedByAdminId?: number;
  processedByAdminName?: string;
  rejectionReason?: string;
  createdAt: string;
  processedAt?: string;
}

export interface CreateReservationRequest {
  title: string;
  description?: string;
  roomId: number;
  startTime: string;
  endTime: string;
}

export interface ProcessReservationRequest {
  approve: boolean;
  rejectionReason?: string;
}

export interface AvailabilityCheckResult {
  available: boolean;
  reason?: string;
}

export const campusMapApi = {
  // Buildings
  getAllBuildings: async (): Promise<Building[]> => {
    const response = await fetch(`${API_BASE_URL}/building`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch buildings');
    return response.json();
  },

  getBuildingById: async (id: number): Promise<Building> => {
    const response = await fetch(`${API_BASE_URL}/building/${id}`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch building');
    return response.json();
  },

  // Rooms
  getRoomsByBuilding: async (buildingId: number): Promise<Room[]> => {
    const response = await fetch(`${API_BASE_URL}/room/building/${buildingId}`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch rooms');
    return response.json();
  },

  getRoomDetails: async (roomId: number): Promise<RoomDetails> => {
    const response = await fetch(`${API_BASE_URL}/room/${roomId}`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch room details');
    return response.json();
  },

  getAvailableRoomsNow: async (): Promise<Room[]> => {
    const response = await fetch(`${API_BASE_URL}/room/available`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch available rooms');
    return response.json();
  },

  // Schedules
  getRoomSchedules: async (roomId: number, date?: Date | string): Promise<Schedule[]> => {
    // Accept either Date object or string in YYYY-MM-DD format
    let dateParam = '';
    if (date) {
      if (typeof date === 'string') {
        // Already in YYYY-MM-DD format
        dateParam = `?date=${date}`;
      } else {
        // Format date as YYYY-MM-DD using local date components to avoid timezone issues
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        dateParam = `?date=${year}-${month}-${day}`;
      }
    }
    const response = await fetch(`${API_BASE_URL}/schedule/room/${roomId}${dateParam}`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch schedules');
    return response.json();
  },

  getRoomSchedulesToday: async (roomId: number): Promise<Schedule[]> => {
    const response = await fetch(`${API_BASE_URL}/schedule/room/${roomId}/today`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch today\'s schedules');
    return response.json();
  },

  createSchedule: async (request: CreateScheduleRequest): Promise<Schedule> => {
    const response = await fetch(`${API_BASE_URL}/schedule`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(request)
    });
    if (!response.ok) throw new Error('Failed to create schedule');
    return response.json();
  },

  updateSchedule: async (id: number, request: UpdateScheduleRequest): Promise<Schedule> => {
    const response = await fetch(`${API_BASE_URL}/schedule/${id}`, {
      method: 'PUT',
      headers: getAuthHeaders(),
      body: JSON.stringify(request)
    });
    if (!response.ok) throw new Error('Failed to update schedule');
    return response.json();
  },

  deleteSchedule: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/schedule/${id}`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to delete schedule');
  },

  // Room Reservations
  createReservation: async (request: CreateReservationRequest): Promise<RoomReservation> => {
    const response = await fetch(`${API_BASE_URL}/roomreservation`, {
      method: 'POST',
      headers: getAuthHeaders(),
      body: JSON.stringify(request)
    });
    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || 'Failed to create reservation');
    }
    return response.json();
  },

  getPendingReservations: async (): Promise<RoomReservation[]> => {
    const response = await fetch(`${API_BASE_URL}/roomreservation/pending`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch pending reservations');
    return response.json();
  },

  getMyReservations: async (): Promise<RoomReservation[]> => {
    const response = await fetch(`${API_BASE_URL}/roomreservation/my-reservations`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to fetch my reservations');
    return response.json();
  },

  processReservation: async (id: number, request: ProcessReservationRequest): Promise<RoomReservation> => {
    const response = await fetch(`${API_BASE_URL}/roomreservation/${id}/process`, {
      method: 'PUT',
      headers: getAuthHeaders(),
      body: JSON.stringify(request)
    });
    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || 'Failed to process reservation');
    }
    return response.json();
  },

  cancelReservation: async (id: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/roomreservation/${id}`, {
      method: 'DELETE',
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to cancel reservation');
  },

  checkAvailability: async (roomId: number, startTime: string, endTime: string): Promise<AvailabilityCheckResult> => {
    const params = new URLSearchParams({
      roomId: roomId.toString(),
      startTime,
      endTime
    });
    const response = await fetch(`${API_BASE_URL}/roomreservation/check-availability?${params}`, {
      headers: getAuthHeaders()
    });
    if (!response.ok) throw new Error('Failed to check availability');
    return response.json();
  }
};
