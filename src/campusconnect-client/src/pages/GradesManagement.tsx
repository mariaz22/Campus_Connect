import React, { useState, useEffect } from 'react';
import { subjectApi, gradeApi } from '../services/gradesApi';
import type {
  Subject,
  Grade,
  CreateGradeRequest,
  UpdateGradeRequest,
} from '../services/gradesApi';
import { Search, Plus, Pencil, Trash2, X } from 'lucide-react';
import axios from 'axios';

interface UserSummary {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  studentId?: string;
}

const GradesManagement: React.FC = () => {
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [selectedSubject, setSelectedSubject] = useState<number | null>(null);
  const [grades, setGrades] = useState<Grade[]>([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<UserSummary[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [showAddModal, setShowAddModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [selectedStudent, setSelectedStudent] = useState<UserSummary | null>(null);
  const [editingGrade, setEditingGrade] = useState<Grade | null>(null);
  const [gradeFormData, setGradeFormData] = useState({
    value: '',
    comments: '',
  });

  useEffect(() => {
    loadSubjects();
  }, []);

  useEffect(() => {
    if (selectedSubject) {
      loadGrades();
    }
  }, [selectedSubject]);

  const loadSubjects = async () => {
    try {
      const data = await subjectApi.getMySubjects();
      setSubjects(data);
      if (data.length > 0 && !selectedSubject) {
        setSelectedSubject(data[0].id);
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error loading subjects');
    }
  };

  const loadGrades = async () => {
    if (!selectedSubject) return;
    
    try {
      setLoading(true);
      const data = await gradeApi.getGradesBySubject(selectedSubject);
      setGrades(data);
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error loading grades');
    } finally {
      setLoading(false);
    }
  };

  const searchUsers = async () => {
    if (!searchQuery.trim()) {
      setSearchResults([]);
      return;
    }

    try {
      const token = localStorage.getItem('token');
      const response = await axios.get(
        `http://localhost:5099/api/user/search?search=${encodeURIComponent(searchQuery)}`,
        {
          headers: { Authorization: `Bearer ${token}` },
        }
      );
      setSearchResults(response.data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error searching users');
    }
  };

  const handleAddGrade = (student: UserSummary) => {
    setSelectedStudent(student);
    setGradeFormData({ value: '', comments: '' });
    setShowAddModal(true);
    setSearchQuery('');
    setSearchResults([]);
  };

  const handleEditGrade = (grade: Grade) => {
    setEditingGrade(grade);
    setGradeFormData({
      value: grade.value.toString(),
      comments: grade.comments || '',
    });
    setShowEditModal(true);
  };

  const handleDeleteGrade = async (id: number, studentName: string) => {
    if (!confirm(`Are you sure you want to delete the grade for ${studentName}?`)) return;

    try {
      await gradeApi.deleteGrade(id);
      await loadGrades();
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error deleting grade');
    }
  };

  const handleSubmitAddGrade = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedStudent || !selectedSubject) return;

    const value = parseFloat(gradeFormData.value);
    if (isNaN(value) || value < 1 || value > 10) {
      setError('Grade must be between 1 and 10');
      return;
    }

    try {
      const createData: CreateGradeRequest = {
        subjectId: selectedSubject,
        studentId: selectedStudent.id,
        value: value,
        comments: gradeFormData.comments || undefined,
      };
      await gradeApi.createGrade(createData);
      setShowAddModal(false);
      await loadGrades();
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error adding grade');
    }
  };

  const handleSubmitEditGrade = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingGrade) return;

    const value = parseFloat(gradeFormData.value);
    if (isNaN(value) || value < 1 || value > 10) {
      setError('Grade must be between 1 and 10');
      return;
    }

    try {
      const updateData: UpdateGradeRequest = {
        value: value,
        comments: gradeFormData.comments || undefined,
      };
      await gradeApi.updateGrade(editingGrade.id, updateData);
      setShowEditModal(false);
      await loadGrades();
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error updating grade');
    }
  };

  const currentSubject = subjects.find((s) => s.id === selectedSubject);

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-800 mb-6">Manage Grades</h1>

      {error && (
        <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
          {error}
        </div>
      )}

      {subjects.length === 0 ? (
        <div className="text-center py-12 bg-gray-50 rounded-lg">
          <p className="text-gray-500 text-lg">You haven't created any subjects yet.</p>
          <a href="/subjects" className="mt-4 text-blue-500 hover:text-blue-600 font-medium inline-block">
            Create your first subject
          </a>
        </div>
      ) : (
        <>
          {/* Subject Selector */}
          <div className="bg-white rounded-lg shadow-md p-6 mb-6">
            <label className="block text-gray-700 font-medium mb-2">Select Subject</label>
            <select
              value={selectedSubject || ''}
              onChange={(e) => setSelectedSubject(Number(e.target.value))}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            >
              {subjects.map((subject) => (
                <option key={subject.id} value={subject.id}>
                  {subject.name} ({subject.code})
                </option>
              ))}
            </select>
          </div>

          {currentSubject && (
            <>
              {/* Student Search */}
              <div className="bg-white rounded-lg shadow-md p-6 mb-6">
                <h2 className="text-xl font-semibold text-gray-800 mb-4">
                  Add Grade for {currentSubject.name}
                </h2>
                <div className="flex gap-2 mb-4">
                  <div className="flex-1 relative">
                    <input
                      type="text"
                      value={searchQuery}
                      onChange={(e) => setSearchQuery(e.target.value)}
                      onKeyDown={(e) => e.key === 'Enter' && searchUsers()}
                      placeholder="Search student by name or email..."
                      className="w-full px-4 py-2 pr-10 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                    />
                    <Search className="absolute right-3 top-2.5 text-gray-400" size={20} />
                  </div>
                  <button
                    onClick={searchUsers}
                    className="px-6 py-2 bg-blue-500 hover:bg-blue-600 text-white rounded-lg transition"
                  >
                    Search
                  </button>
                </div>

                {searchResults.length > 0 && (
                  <div className="border border-gray-200 rounded-lg divide-y">
                    {searchResults.map((user) => (
                      <div key={user.id} className="p-3 flex justify-between items-center hover:bg-gray-50">
                        <div>
                          <p className="font-medium text-gray-800">
                            {user.firstName} {user.lastName}
                          </p>
                          <p className="text-sm text-gray-500">{user.email}</p>
                          {user.studentId && (
                            <p className="text-xs text-gray-400">ID: {user.studentId}</p>
                          )}
                        </div>
                        <button
                          onClick={() => handleAddGrade(user)}
                          className="flex items-center gap-2 px-4 py-2 bg-green-500 hover:bg-green-600 text-white rounded-lg transition"
                        >
                          <Plus size={16} />
                          Add Grade
                        </button>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              {/* Grades List */}
              <div className="bg-white rounded-lg shadow-md p-6">
                <h2 className="text-xl font-semibold text-gray-800 mb-4">
                  Existing Grades ({grades.length})
                </h2>

                {loading ? (
                  <div className="flex justify-center py-8">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
                  </div>
                ) : grades.length === 0 ? (
                  <p className="text-gray-500 text-center py-8">No grades for this subject.</p>
                ) : (
                  <div className="overflow-x-auto">
                    <table className="w-full">
                      <thead className="bg-gray-50 border-b">
                        <tr>
                          <th className="px-4 py-3 text-left text-sm font-semibold text-gray-700">Student</th>
                          <th className="px-4 py-3 text-left text-sm font-semibold text-gray-700">Email</th>
                          <th className="px-4 py-3 text-center text-sm font-semibold text-gray-700">Grade</th>
                          <th className="px-4 py-3 text-left text-sm font-semibold text-gray-700">Comments</th>
                          <th className="px-4 py-3 text-left text-sm font-semibold text-gray-700">Date</th>
                          <th className="px-4 py-3 text-center text-sm font-semibold text-gray-700">Actions</th>
                        </tr>
                      </thead>
                      <tbody className="divide-y">
                        {grades.map((grade) => (
                          <tr key={grade.id} className="hover:bg-gray-50">
                            <td className="px-4 py-3 text-sm text-gray-800">{grade.studentName}</td>
                            <td className="px-4 py-3 text-sm text-gray-600">{grade.studentEmail}</td>
                            <td className="px-4 py-3 text-center">
                              <span
                                className={`inline-block px-3 py-1 rounded-full font-semibold ${
                                  grade.value >= 5
                                    ? 'bg-green-100 text-green-800'
                                    : 'bg-red-100 text-red-800'
                                }`}
                              >
                                {grade.value.toFixed(2)}
                              </span>
                            </td>
                            <td className="px-4 py-3 text-sm text-gray-600">
                              {grade.comments || '-'}
                            </td>
                            <td className="px-4 py-3 text-sm text-gray-500">
                              {new Date(grade.createdAt).toLocaleDateString('en-US')}
                            </td>
                            <td className="px-4 py-3">
                              <div className="flex justify-center gap-2">
                                <button
                                  onClick={() => handleEditGrade(grade)}
                                  className="text-blue-500 hover:text-blue-700 p-2 rounded hover:bg-blue-50 transition"
                                  title="Edit"
                                >
                                  <Pencil size={16} />
                                </button>
                                <button
                                  onClick={() => handleDeleteGrade(grade.id, grade.studentName)}
                                  className="text-red-500 hover:text-red-700 p-2 rounded hover:bg-red-50 transition"
                                  title="Delete"
                                >
                                  <Trash2 size={16} />
                                </button>
                              </div>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                )}
              </div>
            </>
          )}
        </>
      )}

      {/* Add Grade Modal */}
      {showAddModal && selectedStudent && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full p-6">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-2xl font-bold text-gray-800">Add Grade</h2>
              <button
                onClick={() => setShowAddModal(false)}
                className="text-gray-400 hover:text-gray-600 transition"
              >
                <X size={24} />
              </button>
            </div>

            <div className="mb-4 p-3 bg-gray-50 rounded">
              <p className="text-sm text-gray-600">Student:</p>
              <p className="font-semibold text-gray-800">
                {selectedStudent.firstName} {selectedStudent.lastName}
              </p>
              <p className="text-sm text-gray-500">{selectedStudent.email}</p>
            </div>

            <form onSubmit={handleSubmitAddGrade}>
              <div className="mb-4">
                <label className="block text-gray-700 font-medium mb-2">Grade (1-10) *</label>
                <input
                  type="number"
                  step="0.01"
                  min="1"
                  max="10"
                  value={gradeFormData.value}
                  onChange={(e) => setGradeFormData({ ...gradeFormData, value: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                />
              </div>

              <div className="mb-6">
                <label className="block text-gray-700 font-medium mb-2">Comments</label>
                <textarea
                  value={gradeFormData.comments}
                  onChange={(e) => setGradeFormData({ ...gradeFormData, comments: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  rows={3}
                  placeholder="Add optional comments..."
                />
              </div>

              <div className="flex gap-3">
                <button
                  type="button"
                  onClick={() => setShowAddModal(false)}
                  className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="flex-1 px-4 py-2 bg-blue-500 hover:bg-blue-600 text-white rounded-lg transition"
                >
                  Add Grade
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Edit Grade Modal */}
      {showEditModal && editingGrade && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full p-6">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-2xl font-bold text-gray-800">Edit Grade</h2>
              <button
                onClick={() => setShowEditModal(false)}
                className="text-gray-400 hover:text-gray-600 transition"
              >
                <X size={24} />
              </button>
            </div>

            <div className="mb-4 p-3 bg-gray-50 rounded">
              <p className="text-sm text-gray-600">Student:</p>
              <p className="font-semibold text-gray-800">{editingGrade.studentName}</p>
              <p className="text-sm text-gray-500">{editingGrade.studentEmail}</p>
            </div>

            <form onSubmit={handleSubmitEditGrade}>
              <div className="mb-4">
                <label className="block text-gray-700 font-medium mb-2">Grade (1-10) *</label>
                <input
                  type="number"
                  step="0.01"
                  min="1"
                  max="10"
                  value={gradeFormData.value}
                  onChange={(e) => setGradeFormData({ ...gradeFormData, value: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  required
                />
              </div>

              <div className="mb-6">
                <label className="block text-gray-700 font-medium mb-2">Comments</label>
                <textarea
                  value={gradeFormData.comments}
                  onChange={(e) => setGradeFormData({ ...gradeFormData, comments: e.target.value })}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                  rows={3}
                  placeholder="Add optional comments..."
                />
              </div>

              <div className="flex gap-3">
                <button
                  type="button"
                  onClick={() => setShowEditModal(false)}
                  className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="flex-1 px-4 py-2 bg-blue-500 hover:bg-blue-600 text-white rounded-lg transition"
                >
                  Save
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default GradesManagement;
