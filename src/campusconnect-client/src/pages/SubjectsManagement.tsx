import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import {
  BookOpen,
  Plus,
  Pencil,
  Trash2,
  X,
  GraduationCap,
  Calendar,
  ArrowRight,
  Sparkles,
} from 'lucide-react';
import { Layout } from '../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { Input } from '../components/ui/Input';
import { subjectApi } from '../services/gradesApi';
import type { Subject, CreateSubjectRequest, UpdateSubjectRequest } from '../services/gradesApi';

const SubjectsManagement = () => {
  const navigate = useNavigate();
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingSubject, setEditingSubject] = useState<Subject | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    code: '',
    description: '',
    year: 1,
  });

  useEffect(() => {
    loadSubjects();
  }, []);

  const loadSubjects = async () => {
    try {
      setLoading(true);
      const data = await subjectApi.getMySubjects();
      setSubjects(data);
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error loading subjects');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = () => {
    setEditingSubject(null);
    setFormData({ name: '', code: '', description: '', year: 1 });
    setShowModal(true);
  };

  const handleEdit = (subject: Subject) => {
    setEditingSubject(subject);
    setFormData({
      name: subject.name,
      code: subject.code,
      description: subject.description || '',
      year: subject.year,
    });
    setShowModal(true);
  };

  const handleDelete = async (id: number, name: string) => {
    if (!confirm(`Are you sure you want to delete the subject "${name}"?`)) return;

    try {
      await subjectApi.deleteSubject(id);
      await loadSubjects();
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error deleting subject');
    }
  };

  const generateSubjectCode = (name: string): string => {
    const prefix = name.substring(0, 3).toUpperCase().replace(/[^A-Z]/g, 'X');
    const randomNum = Math.floor(100 + Math.random() * 900);
    return `${prefix}${randomNum}`;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      if (editingSubject) {
        const updateData: UpdateSubjectRequest = {
          name: formData.name,
          description: formData.description || undefined,
          year: formData.year,
        };
        await subjectApi.updateSubject(editingSubject.id, updateData);
      } else {
        const generatedCode = generateSubjectCode(formData.name);
        const createData: CreateSubjectRequest = {
          name: formData.name,
          code: generatedCode,
          description: formData.description || undefined,
          year: formData.year,
        };
        await subjectApi.createSubject(createData);
      }

      setShowModal(false);
      await loadSubjects();
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error saving subject');
    }
  };

  const closeModal = () => {
    setShowModal(false);
    setEditingSubject(null);
    setFormData({ name: '', code: '', description: '', year: 1 });
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

  if (loading) {
    return (
      <Layout>
        <div className="flex items-center justify-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
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

          <div className="relative z-10 flex items-center justify-between">
            <div>
              <div className="flex items-center gap-3 mb-2">
                <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                  <BookOpen className="h-8 w-8" />
                </div>
                <div>
                  <h1 className="text-4xl font-bold flex items-center gap-2">
                    My Subjects
                    <Sparkles className="h-6 w-6 text-yellow-300" />
                  </h1>
                  <p className="text-white/80 mt-1">Manage your courses and grades</p>
                </div>
              </div>
            </div>

            <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
              <Button
                onClick={handleCreate}
                className="bg-white text-purple-600 hover:bg-white/90 shadow-lg"
              >
                <Plus className="h-5 w-5 mr-2" />
                Add Subject
              </Button>
            </motion.div>
          </div>
        </motion.div>

        {/* Error Message */}
        {error && (
          <motion.div
            initial={{ opacity: 0, y: -10 }}
            animate={{ opacity: 1, y: 0 }}
          >
            <Card className="border-red-200 bg-red-50 dark:bg-red-900/20 dark:border-red-800">
              <CardContent className="pt-6">
                <p className="text-red-600 dark:text-red-400 font-medium">{error}</p>
              </CardContent>
            </Card>
          </motion.div>
        )}

        {/* Empty State */}
        {subjects.length === 0 ? (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="text-center py-12">
            <BookOpen className="h-16 w-16 mx-auto text-muted-foreground mb-4" />
            <h3 className="text-xl font-semibold mb-2">No subjects yet</h3>
            <p className="text-muted-foreground mb-6">Create your first subject to start adding grades</p>
            <Button onClick={handleCreate}>
              <Plus className="h-5 w-5 mr-2" />
              Create First Subject
            </Button>
          </motion.div>
        ) : (
          /* Subjects grouped by year */
          <div className="space-y-8">
            {[1, 2, 3].map((year) => {
              const yearSubjects = subjects.filter((s) => s.year === year);
              if (yearSubjects.length === 0) return null;

              return (
                <motion.div
                  key={year}
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: year * 0.1 }}
                >
                  <div className="flex items-center gap-3 mb-4">
                    <Badge className={`${getYearColor(year)} px-4 py-2 text-base`}>
                      <GraduationCap className="h-4 w-4 mr-2" />
                      Year {year}
                    </Badge>
                    <div className="h-px flex-1 bg-border"></div>
                    <span className="text-sm text-muted-foreground">{yearSubjects.length} subjects</span>
                  </div>

                  <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    <AnimatePresence mode="popLayout">
                      {yearSubjects.map((subject, index) => (
                        <motion.div
                          key={subject.id}
                          initial={{ opacity: 0, scale: 0.9 }}
                          animate={{ opacity: 1, scale: 1 }}
                          exit={{ opacity: 0, scale: 0.9 }}
                          transition={{ delay: index * 0.05 }}
                          layout
                        >
                          <Card className="group relative overflow-hidden border-2 hover:border-primary/50 transition-all h-full hover:shadow-xl cursor-pointer"
                            onClick={() => navigate(`/subjects/${subject.id}`)}
                          >
                            <div className="pointer-events-none absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>

                            <CardHeader className="relative">
                              <div className="flex items-start justify-between mb-2">
                                <Badge className={getYearColor(subject.year)}>Year {subject.year}</Badge>
                                <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity"
                                  onClick={(e) => e.stopPropagation()}
                                >
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    onClick={(e) => {
                                      e.stopPropagation();
                                      handleEdit(subject);
                                    }}
                                    className="h-8 w-8 text-muted-foreground hover:text-primary"
                                  >
                                    <Pencil className="h-4 w-4" />
                                  </Button>
                                  <Button
                                    variant="ghost"
                                    size="icon"
                                    onClick={(e) => {
                                      e.stopPropagation();
                                      handleDelete(subject.id, subject.name);
                                    }}
                                    className="h-8 w-8 text-muted-foreground hover:text-red-500"
                                  >
                                    <Trash2 className="h-4 w-4" />
                                  </Button>
                                </div>
                              </div>

                              <CardTitle className="text-xl line-clamp-2 group-hover:text-primary transition-colors">
                                {subject.name}
                              </CardTitle>
                              <span className="inline-flex items-center px-2 py-1 rounded-md border border-input bg-background font-mono text-xs mt-2">
                                {subject.code}
                              </span>
                            </CardHeader>

                            <CardContent>
                              {subject.description && (
                                <p className="text-muted-foreground text-sm line-clamp-2 mb-4">
                                  {subject.description}
                                </p>
                              )}

                              <div className="flex items-center justify-between pt-4 border-t">
                                <div className="flex items-center gap-2 text-xs text-muted-foreground">
                                  <Calendar className="h-4 w-4" />
                                  {new Date(subject.createdAt).toLocaleDateString('en-US')}
                                </div>
                                <div className="flex items-center gap-1 text-primary font-medium text-sm">
                                  <span>Manage Grades</span>
                                  <ArrowRight className="h-4 w-4 group-hover:translate-x-1 transition-transform" />
                                </div>
                              </div>
                            </CardContent>
                          </Card>
                        </motion.div>
                      ))}
                    </AnimatePresence>
                  </div>
                </motion.div>
              );
            })}
          </div>
        )}

        {/* Create/Edit Modal */}
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
                          {editingSubject ? <Pencil className="h-5 w-5" /> : <Plus className="h-5 w-5" />}
                        </div>
                        <CardTitle className="text-white">
                          {editingSubject ? 'Edit Subject' : 'Add New Subject'}
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
                      <div>
                        <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                          <BookOpen className="h-4 w-4 text-primary" />
                          Subject Name *
                        </label>
                        <Input
                          type="text"
                          value={formData.name}
                          onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                          required
                          placeholder="e.g.: Mathematics"
                        />
                      </div>

                      <div>
                        <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                          <GraduationCap className="h-4 w-4 text-primary" />
                          Academic Year *
                        </label>
                        <select
                          value={formData.year}
                          onChange={(e) => setFormData({ ...formData, year: parseInt(e.target.value) })}
                          className="w-full px-3 py-2 border border-input rounded-md bg-background"
                          required
                        >
                          <option value={1}>Year 1</option>
                          <option value={2}>Year 2</option>
                          <option value={3}>Year 3</option>
                        </select>
                      </div>

                      <div>
                        <label className="block text-sm font-medium mb-2">
                          Description (optional)
                        </label>
                        <textarea
                          value={formData.description}
                          onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                          rows={3}
                          className="w-full px-3 py-2 border border-input rounded-md bg-background resize-none"
                          placeholder="Subject description..."
                        />
                      </div>

                      <div className="flex gap-3 pt-4">
                        <Button type="button" variant="outline" onClick={closeModal} className="flex-1">
                          Cancel
                        </Button>
                        <Button type="submit" className="flex-1">
                          {editingSubject ? 'Save Changes' : 'Create Subject'}
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

export default SubjectsManagement;