import axios from 'axios';

const API_URL = 'http://localhost:5099/api';

export interface Subject {
  id: number;
  name: string;
  code: string;
  description?: string;
  year: number; // Anul de studiu: 1, 2, 3
  professorId: number;
  professorName: string;
  createdAt: string;
  isActive: boolean;
}

export interface CreateSubjectRequest {
  name: string;
  code: string;
  description?: string;
  year: number; // Anul de studiu: 1, 2, 3
}

export interface UpdateSubjectRequest {
  name: string;
  description?: string;
  year: number; // Anul de studiu: 1, 2, 3
}

export interface Grade {
  id: number;
  subjectId: number;
  subjectName: string;
  subjectCode: string;
  studentId: number;
  studentName: string;
  studentEmail: string;
  value: number;
  comments?: string;
  createdAt: string;
  updatedAt?: string;
  createdByProfessorId: number;
  createdByProfessorName: string;
}

export interface CreateGradeRequest {
  subjectId: number;
  studentId: number;
  value: number;
  comments?: string;
}

export interface UpdateGradeRequest {
  value: number;
  comments?: string;
}

export interface StudentGradesResponse {
  studentId: number;
  studentName: string;
  studentEmail: string;
  subjectGrades: SubjectGrades[];
}

export interface SubjectGrades {
  subjectId: number;
  subjectName: string;
  subjectCode: string;
  year: number; // Anul de studiu
  professorName: string;
  grades: Grade[];
  averageGrade?: number;
}

const getAuthHeader = () => {
  const token = localStorage.getItem('token');
  return token ? { Authorization: `Bearer ${token}` } : {};
};

// Subject API
export const subjectApi = {
  createSubject: async (data: CreateSubjectRequest): Promise<Subject> => {
    const response = await axios.post(`${API_URL}/subject`, data, {
      headers: getAuthHeader(),
    });
    return response.data;
  },

  updateSubject: async (id: number, data: UpdateSubjectRequest): Promise<Subject> => {
    const response = await axios.put(`${API_URL}/subject/${id}`, data, {
      headers: getAuthHeader(),
    });
    return response.data;
  },

  deleteSubject: async (id: number): Promise<void> => {
    await axios.delete(`${API_URL}/subject/${id}`, {
      headers: getAuthHeader(),
    });
  },

  getSubject: async (id: number): Promise<Subject> => {
    const response = await axios.get(`${API_URL}/subject/${id}`, {
      headers: getAuthHeader(),
    });
    return response.data;
  },

  getMySubjects: async (): Promise<Subject[]> => {
    const response = await axios.get(`${API_URL}/subject/my-subjects`, {
      headers: getAuthHeader(),
    });
    return response.data;
  },

  getAllSubjects: async (): Promise<Subject[]> => {
    const response = await axios.get(`${API_URL}/subject`, {
      headers: getAuthHeader(),
    });
    return response.data;
  },
};

// Grade API
export const gradeApi = {
  createGrade: async (data: CreateGradeRequest): Promise<Grade> => {
    const response = await axios.post(`${API_URL}/grade`, data, {
      headers: getAuthHeader(),
    });
    return response.data;
  },

  updateGrade: async (id: number, data: UpdateGradeRequest): Promise<Grade> => {
    const response = await axios.put(`${API_URL}/grade/${id}`, data, {
      headers: getAuthHeader(),
    });
    return response.data;
  },

  deleteGrade: async (id: number): Promise<void> => {
    await axios.delete(`${API_URL}/grade/${id}`, {
      headers: getAuthHeader(),
    });
  },

  getGrade: async (id: number): Promise<Grade> => {
    const response = await axios.get(`${API_URL}/grade/${id}`, {
      headers: getAuthHeader(),
    });
    return response.data;
  },

  getGradesBySubject: async (subjectId: number): Promise<Grade[]> => {
    const response = await axios.get(`${API_URL}/grade/subject/${subjectId}`, {
      headers: getAuthHeader(),
    });
    return response.data;
  },

  getGradesByStudent: async (studentId: number): Promise<Grade[]> => {
    const response = await axios.get(`${API_URL}/grade/student/${studentId}`, {
      headers: getAuthHeader(),
    });
    return response.data;
  },

  getStudentGradesGrouped: async (studentId: number): Promise<StudentGradesResponse> => {
    const response = await axios.get(`${API_URL}/grade/student/${studentId}/grouped`, {
      headers: getAuthHeader(),
    });
    return response.data;
  },

  getMyGrades: async (): Promise<StudentGradesResponse> => {
    const response = await axios.get(`${API_URL}/grade/my-grades`, {
      headers: getAuthHeader(),
    });
    return response.data;
  },

  getGradesBySubjectAndStudent: async (
    subjectId: number,
    studentId: number
  ): Promise<Grade[]> => {
    const response = await axios.get(
      `${API_URL}/grade/subject/${subjectId}/student/${studentId}`,
      {
        headers: getAuthHeader(),
      }
    );
    return response.data;
  },
};
