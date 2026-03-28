import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import {
  BookOpen,
  Plus,
  Pencil,
  Trash2,
  X,
  GraduationCap,
  ArrowLeft,
  Users,
  Sparkles,
  Search,
} from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Input } from '../../components/ui/Input';
import { subjectApi, gradeApi } from '../../services/gradesApi';
import type { Subject, Grade, CreateGradeRequest, UpdateGradeRequest } from '../../services/gradesApi';
import axios from 'axios';

interface Student {
  id: number;
  fullName: string;
  email: string;
  studentId: string;
}

const SubjectDetails = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [subject, setSubject] = useState<Subject | null>(null);
  const [grades, setGrades] = useState<Grade[]>([]);
  const [students, setStudents] = useState<Student[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingGrade, setEditingGrade] = useState<Grade | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [studentSearchTerm, setStudentSearchTerm] = useState('');
  const [showStudentDropdown, setShowStudentDropdown] = useState(false);
  const [formData, setFormData] = useState({
    studentId: 0,
    value: 10,
    comments: '',
  });

  useEffect(() => {
    if (id) {
      loadData();
    }
  }, [id]);

  const loadData = async () => {
    try {
      setLoading(true);
      setError('');

      // Load subject first - this is required
      const subjectData = await subjectApi.getSubject(Number(id));
      setSubject(subjectData);

      // Load grades and students separately (can fail without breaking page)
      try {
        const gradesData = await gradeApi.getGradesBySubject(Number(id));
        setGrades(gradesData);
      } catch (err) {
        console.error('Error loading grades:', err);
        setGrades([]);
      }

      try {
        const usersData = await axios.get('http://localhost:5099/api/user/search', {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
        });
        // Filter to only get users with role "User" (students)
        const studentsList = usersData.data
          .filter((u: any) => u.role === 'User')
          .map((u: any) => ({
            id: u.id,
            fullName: `${u.firstName} ${u.lastName}`,
            email: u.email || '',
            studentId: u.studentId || '',
          }));
        setStudents(studentsList);
      } catch (err) {
        console.error('Error loading students:', err);
        setStudents([]);
      }
    } catch (err: any) {
      console.error('Error loading subject:', err);
      setError(err.response?.data?.message || 'Error loading subject');
      setSubject(null);
    } finally {
      setLoading(false);
    }
  };

  const handleAddGrade = () => {
    setEditingGrade(null);
    setFormData({ studentId: 0, value: 10, comments: '' });
    setStudentSearchTerm('');
    setShowStudentDropdown(false);
    setShowModal(true);
  };

  const handleEditGrade = (grade: Grade) => {
    setEditingGrade(grade);
    setFormData({
      studentId: grade.studentId,
      value: grade.value,
      comments: grade.comments || '',
    });
    setShowModal(true);
  };

  const handleDeleteGrade = async (gradeId: number) => {
    if (!confirm('Are you sure you want to delete this grade?')) return;

    try {
      await gradeApi.deleteGrade(gradeId);
      await loadData();
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error deleting grade');
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      if (editingGrade) {
        const updateData: UpdateGradeRequest = {
          value: formData.value,
          comments: formData.comments || undefined,
        };
        await gradeApi.updateGrade(editingGrade.id, updateData);
      } else {
        const createData: CreateGradeRequest = {
          subjectId: Number(id),
          studentId: formData.studentId,
          value: formData.value,
          comments: formData.comments || undefined,
        };
        await gradeApi.createGrade(createData);
      }

      setShowModal(false);
      await loadData();
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error saving grade');
    }
  };

  const closeModal = () => {
    setShowModal(false);
    setEditingGrade(null);
    setFormData({ studentId: 0, value: 10, comments: '' });
    setStudentSearchTerm('');
    setShowStudentDropdown(false);
  };

  const getGradeColor = (grade: number) => {
    if (grade >= 9) return 'bg-green-500 text-white';
    if (grade >= 7) return 'bg-blue-500 text-white';
    if (grade >= 5) return 'bg-yellow-500 text-white';
    return 'bg-red-500 text-white';
  };

  const getYearColor = (year: number) => {
    switch (year) {
      case 1:
        return 'bg-blue-500 text-white';
      case 2:
        return 'bg-purple-500 text-white';
      case 3:
        return 'bg-orange-500 text-white';
      default:
        return 'bg-gray-500 text-white';
    }
  };

  // Group grades by student
  const gradesByStudent = grades.reduce((acc, grade) => {
    if (!acc[grade.studentId]) {
      acc[grade.studentId] = {
        studentName: grade.studentName,
        studentEmail: grade.studentEmail,
        grades: [],
      };
    }
    acc[grade.studentId].grades.push(grade);
    return acc;
  }, {} as Record<number, { studentName: string; studentEmail: string; grades: Grade[] }>);

  // Filter by search term
  const filteredStudents = Object.entries(gradesByStudent).filter(
    ([_, data]) =>
      data.studentName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      data.studentEmail.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) {
    return (
      <Layout>
        <div className="flex items-center justify-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
        </div>
      </Layout>
    );
  }

  if (!subject) {
    return (
      <Layout>
        <div className="space-y-6">
          <Card className="border-red-200 bg-red-50 dark:bg-red-900/20 dark:border-red-800">
            <CardContent className="pt-6">
              <p className="text-red-600 dark:text-red-400 font-medium">Subject not found</p>
            </CardContent>
          </Card>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-indigo-500 via-purple-500 to-pink-500 p-8 text-white shadow-2xl"
        >
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>

          <div className="relative z-10">
            <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
              <div>
                <Button
                  onClick={() => navigate('/subjects')}
                  variant="ghost"
                  className="text-white/80 hover:text-white hover:bg-white/20 mb-4"
                >
                  <ArrowLeft className="h-4 w-4 mr-2" />
                  Back to Subjects
                </Button>
                <div className="flex items-center gap-3">
                  <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                    <BookOpen className="h-8 w-8" />
                  </div>
                  <div>
                    <h1 className="text-4xl font-bold flex items-center gap-2">
                      {subject.name}
                      <Sparkles className="h-6 w-6 text-yellow-300" />
                    </h1>
                    <div className="flex items-center gap-3 mt-2">
                      <span className="text-white/80 font-mono bg-white/20 px-2 py-1 rounded">
                        {subject.code}
                      </span>
                      <Badge className={getYearColor(subject.year)}>Year {subject.year}</Badge>
                    </div>
                  </div>
                </div>
              </div>

              {/* Stats */}
              <div className="flex gap-4">
                <div className="bg-white/20 backdrop-blur-sm rounded-xl p-4 min-w-[120px]">
                  <div className="flex items-center gap-2 mb-1">
                    <Users className="h-4 w-4" />
                    <span className="text-sm opacity-90">Students</span>
                  </div>
                  <div className="text-3xl font-bold">{Object.keys(gradesByStudent).length}</div>
                </div>
                <div className="bg-white/20 backdrop-blur-sm rounded-xl p-4 min-w-[120px]">
                  <div className="flex items-center gap-2 mb-1">
                    <GraduationCap className="h-4 w-4" />
                    <span className="text-sm opacity-90">Grades</span>
                  </div>
                  <div className="text-3xl font-bold">{grades.length}</div>
                </div>
              </div>
            </div>
          </div>
        </motion.div>

        {/* Error Message */}
        {error && (
          <motion.div initial={{ opacity: 0, y: -10 }} animate={{ opacity: 1, y: 0 }}>
            <Card className="border-red-200 bg-red-50 dark:bg-red-900/20 dark:border-red-800">
              <CardContent className="pt-6">
                <p className="text-red-600 dark:text-red-400 font-medium">{error}</p>
              </CardContent>
            </Card>
          </motion.div>
        )}

        {/* Actions Bar */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
        >
          <Card>
            <CardContent className="pt-6">
              <div className="flex flex-col md:flex-row gap-4 items-center justify-between">
                <div className="relative flex-1 max-w-md">
                  <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                  <Input
                    type="text"
                    placeholder="Search students..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10"
                  />
                </div>
                <Button onClick={handleAddGrade}>
                  <Plus className="h-5 w-5 mr-2" />
                  Add Grade
                </Button>
              </div>
            </CardContent>
          </Card>
        </motion.div>

        {/* Grades List */}
        {grades.length === 0 ? (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="text-center py-12">
            <GraduationCap className="h-16 w-16 mx-auto text-muted-foreground mb-4" />
            <h3 className="text-xl font-semibold mb-2">No grades yet</h3>
            <p className="text-muted-foreground mb-6">Add grades for students in this subject</p>
            <Button onClick={handleAddGrade}>
              <Plus className="h-5 w-5 mr-2" />
              Add First Grade
            </Button>
          </motion.div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <AnimatePresence mode="popLayout">
              {filteredStudents.map(([studentId, data], index) => {
                const avgGrade =
                  data.grades.reduce((sum, g) => sum + g.value, 0) / data.grades.length;

                return (
                  <motion.div
                    key={studentId}
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    exit={{ opacity: 0, scale: 0.9 }}
                    transition={{ delay: index * 0.05 }}
                    layout
                  >
                    <Card className="h-full hover:shadow-lg transition-shadow">
                      <CardHeader>
                        <div className="flex items-start justify-between">
                          <div>
                            <CardTitle className="text-lg">{data.studentName}</CardTitle>
                            <p className="text-sm text-muted-foreground">{data.studentEmail}</p>
                          </div>
                          <Badge className={`${getGradeColor(avgGrade)} text-lg px-3 py-1`}>
                            Avg: {avgGrade.toFixed(2)}
                          </Badge>
                        </div>
                      </CardHeader>
                      <CardContent>
                        <div className="space-y-3">
                          <div className="flex items-center gap-2 text-sm text-muted-foreground">
                            <span className="font-medium">Grades:</span>
                          </div>
                          <div className="flex flex-wrap gap-2">
                            {data.grades.map((grade) => (
                              <div
                                key={grade.id}
                                className="group relative flex items-center gap-1"
                              >
                                <Badge className={`${getGradeColor(grade.value)} px-3 py-1.5 text-base`}>
                                  {grade.value.toFixed(2)}
                                </Badge>
                                <div className="flex gap-0.5 opacity-0 group-hover:opacity-100 transition-opacity">
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    className="h-6 w-6"
                                    onClick={() => handleEditGrade(grade)}
                                  >
                                    <Pencil className="h-3 w-3" />
                                  </Button>
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    className="h-6 w-6 text-red-500 hover:text-red-600"
                                    onClick={() => handleDeleteGrade(grade.id)}
                                  >
                                    <Trash2 className="h-3 w-3" />
                                  </Button>
                                </div>
                              </div>
                            ))}
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  </motion.div>
                );
              })}
            </AnimatePresence>
          </div>
        )}

        {/* Add/Edit Grade Modal */}
        <AnimatePresence>
          {showModal && (
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              className="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center p-4 z-50"
              onClick={closeModal}
            >
              <motion.div
                initial={{ opacity: 0, scale: 0.95, y: 20 }}
                animate={{ opacity: 1, scale: 1, y: 0 }}
                exit={{ opacity: 0, scale: 0.95, y: 20 }}
                onClick={(e) => e.stopPropagation()}
                className="w-full max-w-md"
              >
                <Card>
                  <CardHeader className="bg-gradient-to-r from-indigo-500 to-purple-500 text-white rounded-t-lg">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-3">
                        <div className="p-2 bg-white/20 rounded-lg">
                          {editingGrade ? <Pencil className="h-5 w-5" /> : <Plus className="h-5 w-5" />}
                        </div>
                        <CardTitle className="text-white">
                          {editingGrade ? 'Edit Grade' : 'Add New Grade'}
                        </CardTitle>
                      </div>
                      <Button
                        variant="ghost"
                        size="icon"
                        onClick={closeModal}
                        className="text-white/80 hover:text-white hover:bg-white/20"
                      >
                        <X className="h-5 w-5" />
                      </Button>
                    </div>
                  </CardHeader>
                  <CardContent className="pt-6">
                    <form onSubmit={handleSubmit} className="space-y-4">
                      {!editingGrade && (
                        <div>
                          <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                            <Users className="h-4 w-4 text-primary" />
                            Student *
                          </label>
                          <div className="relative">
                            <div className="relative">
                              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                              <Input
                                type="text"
                                placeholder="Search by name, email or ID..."
                                value={studentSearchTerm}
                                onChange={(e) => {
                                  setStudentSearchTerm(e.target.value);
                                  setShowStudentDropdown(true);
                                }}
                                onFocus={() => setShowStudentDropdown(true)}
                                className="pl-10"
                              />
                            </div>
                            {showStudentDropdown && (
                              <div className="absolute z-10 w-full mt-1 max-h-48 overflow-y-auto bg-background border border-input rounded-md shadow-lg">
                                {students
                                  .filter(
                                    (student) =>
                                      student.fullName.toLowerCase().includes(studentSearchTerm.toLowerCase()) ||
                                      student.email.toLowerCase().includes(studentSearchTerm.toLowerCase()) ||
                                      student.studentId.toLowerCase().includes(studentSearchTerm.toLowerCase()) ||
                                      student.id.toString().includes(studentSearchTerm)
                                  )
                                  .map((student) => (
                                    <div
                                      key={student.id}
                                      className={`px-3 py-2 cursor-pointer hover:bg-muted transition-colors ${
                                        formData.studentId === student.id ? 'bg-primary/10 text-primary' : ''
                                      }`}
                                      onClick={() => {
                                        setFormData({ ...formData, studentId: student.id });
                                        setStudentSearchTerm(student.fullName);
                                        setShowStudentDropdown(false);
                                      }}
                                    >
                                      <div className="font-medium">{student.fullName}</div>
                                      <div className="text-xs text-muted-foreground">
                                        {student.email} {student.studentId && `| Nr. matricol: ${student.studentId}`}
                                      </div>
                                    </div>
                                  ))}
                                {students.filter(
                                  (student) =>
                                    student.fullName.toLowerCase().includes(studentSearchTerm.toLowerCase()) ||
                                    student.email.toLowerCase().includes(studentSearchTerm.toLowerCase()) ||
                                    student.studentId.toLowerCase().includes(studentSearchTerm.toLowerCase()) ||
                                    student.id.toString().includes(studentSearchTerm)
                                ).length === 0 && (
                                  <div className="px-3 py-2 text-muted-foreground text-sm">
                                    No students found
                                  </div>
                                )}
                              </div>
                            )}
                          </div>
                          {formData.studentId > 0 && (
                            <div className="mt-2 text-sm text-green-600 dark:text-green-400">
                              Selected: {students.find(s => s.id === formData.studentId)?.fullName} ({students.find(s => s.id === formData.studentId)?.email})
                            </div>
                          )}
                        </div>
                      )}

                      {editingGrade && (
                        <div className="bg-muted p-3 rounded-lg">
                          <p className="text-sm text-muted-foreground">Student</p>
                          <p className="font-medium">{editingGrade.studentName}</p>
                        </div>
                      )}

                      <div>
                        <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                          <GraduationCap className="h-4 w-4 text-primary" />
                          Grade Value (1-10) *
                        </label>
                        <Input
                          type="number"
                          min="1"
                          max="10"
                          step="0.01"
                          value={formData.value}
                          onChange={(e) =>
                            setFormData({ ...formData, value: parseFloat(e.target.value) })
                          }
                          required
                        />
                      </div>

                      <div>
                        <label className="block text-sm font-medium mb-2">Comments (optional)</label>
                        <textarea
                          value={formData.comments}
                          onChange={(e) => setFormData({ ...formData, comments: e.target.value })}
                          rows={3}
                          className="w-full px-3 py-2 border border-input rounded-md bg-background resize-none"
                          placeholder="Add a comment about this grade..."
                        />
                      </div>

                      <div className="flex gap-3 pt-4">
                        <Button type="button" variant="outline" onClick={closeModal} className="flex-1">
                          Cancel
                        </Button>
                        <Button type="submit" className="flex-1">
                          {editingGrade ? 'Save Changes' : 'Add Grade'}
                        </Button>
                      </div>
                    </form>
                  </CardContent>
                </Card>
              </motion.div>
            </motion.div>
          )}
        </AnimatePresence>
      </div>
    </Layout>
  );
};

export default SubjectDetails;
