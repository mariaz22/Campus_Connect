import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import { BookOpen, Plus, Trash2, Sparkles, ArrowLeft } from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Input } from '../../components/ui/Input';
import * as gradesApi from '../../services/gradesApi';
import type { Subject, CreateSubjectRequest } from '../../services/gradesApi';

const MySubjects = () => {
  const navigate = useNavigate();
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [newSubject, setNewSubject] = useState<CreateSubjectRequest>({ name: '', code: '', description: '', year: 1 });

  useEffect(() => {
    loadSubjects();
  }, []);

  const loadSubjects = async () => {
    setLoading(true);
    try {
      const data = await gradesApi.subjectApi.getMySubjects();
      setSubjects(data);
    } catch (error) {
      console.error('Error loading subjects:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateSubject = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await gradesApi.subjectApi.createSubject(newSubject);
      setNewSubject({ name: '', code: '', description: '', year: 1 });
      setShowCreateForm(false);
      loadSubjects();
    } catch (error) {
      console.error('Error creating subject:', error);
    }
  };

  const handleDeleteSubject = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this subject? All associated grades will be deleted!')) return;
    try {
      await gradesApi.subjectApi.deleteSubject(id);
      loadSubjects();
    } catch (error) {
      console.error('Error deleting subject:', error);
    }
  };

  const navigateToSubject = (id: number) => {
    navigate(`/subjects/${id}`);
  };

  return (
    <Layout>
      <div className="min-h-screen bg-gradient-to-br from-primary/5 via-purple-600/5 to-pink-600/5">
      <div className="space-y-8">
        {/* Hero Header */}
        <motion.div
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-primary via-purple-600 to-pink-600 p-8 text-white shadow-2xl"
        >
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>
          
          <div className="relative z-10 flex items-center justify-between">
            <div className="flex items-center gap-4">
              <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                <Button
                  onClick={() => navigate(-1)}
                  className="bg-white/20 hover:bg-white/30 backdrop-blur-sm text-white border-white/30 shadow-lg"
                >
                  <ArrowLeft className="w-5 h-5 mr-2" />
                  Back
                </Button>
              </motion.div>
              <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                <BookOpen className="h-8 w-8" />
              </div>
              <div>
                <h1 className="text-4xl font-bold flex items-center gap-2">
                  My Subjects
                  <Sparkles className="h-6 w-6 text-yellow-300" />
                </h1>
                <p className="text-white/80 mt-1">Manage your subjects and grade students</p>
              </div>
            </div>
            <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
              <Button
                onClick={() => setShowCreateForm(!showCreateForm)}
                className="bg-white text-purple-600 hover:bg-white/90 shadow-lg"
              >
                <Plus className="w-5 h-5 mr-2" />
                New Subject
              </Button>
            </motion.div>
          </div>
        </motion.div>

        <div className="container mx-auto px-4 max-w-7xl">

        {/* Create Subject Form */}
        <AnimatePresence>
          {showCreateForm && (
            <motion.div
              initial={{ opacity: 0, height: 0 }}
              animate={{ opacity: 1, height: 'auto' }}
              exit={{ opacity: 0, height: 0 }}
              className="mb-6"
            >
              <Card>
                <CardHeader>
                  <CardTitle>Create New Subject</CardTitle>
                </CardHeader>
                <CardContent>
                  <form onSubmit={handleCreateSubject} className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium mb-2">
                        Subject Name *
                      </label>
                      <Input
                        type="text"
                        placeholder="e.g: Mathematics, Physics, etc."
                        value={newSubject.name}
                        onChange={(e) => setNewSubject({ ...newSubject, name: e.target.value })}
                        required
                        className="w-full"
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-2">
                        Description (optional)
                      </label>
                      <textarea
                        placeholder="Subject description..."
                        value={newSubject.description || ''}
                        onChange={(e) => setNewSubject({ ...newSubject, description: e.target.value })}
                        className="w-full px-3 py-2 border border-input rounded-md bg-background"
                        rows={3}
                      />
                    </div>
                    <div className="flex gap-3">
                      <Button type="submit">
                        Create Subject
                      </Button>
                      <Button
                        type="button"
                        onClick={() => setShowCreateForm(false)}
                        variant="outline"
                      >
                        Cancel
                      </Button>
                    </div>
                  </form>
                </CardContent>
              </Card>
            </motion.div>
          )}
        </AnimatePresence>

        {/* Subjects List */}
        {loading ? (
          <div className="flex justify-center items-center h-64">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        ) : subjects.length === 0 ? (
          <Card>
            <CardContent className="py-12 text-center">
              <BookOpen className="w-16 h-16 text-muted-foreground mx-auto mb-4" />
              <p className="text-xl mb-2">You don't have any subjects yet</p>
              <p className="text-muted-foreground mb-4">Create your first subject to get started</p>
              <Button
                onClick={() => setShowCreateForm(true)}
              >
                <Plus className="w-5 h-5 mr-2" />
                Create Subject
              </Button>
            </CardContent>
          </Card>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {subjects.map((subject) => (
              <motion.div
                key={subject.id}
                initial={{ opacity: 0, scale: 0.9 }}
                animate={{ opacity: 1, scale: 1 }}
                whileHover={{ scale: 1.02 }}
                transition={{ duration: 0.2 }}
              >
                <Card 
                  className="cursor-pointer hover:shadow-lg transition-all border-2 hover:border-primary/50 h-full"
                  onClick={() => navigateToSubject(subject.id)}
                >
                  <CardHeader>
                    <div className="flex justify-between items-start">
                      <div className="flex-1">
                        <CardTitle className="text-xl mb-2">{subject.name}</CardTitle>
                        <Badge className="text-xs border border-gray-300">
                          <BookOpen className="w-3 h-3 mr-1" />
                          {subject.code}
                        </Badge>
                      </div>
                      <BookOpen className="w-8 h-8 text-blue-600" />
                    </div>
                  </CardHeader>
                  <CardContent>
                    {subject.description && (
                      <p className="text-muted-foreground text-sm mb-4 line-clamp-2">
                        {subject.description}
                      </p>
                    )}
                    <div className="flex gap-2 mt-4">
                      <Button
                        onClick={(e) => {
                          e.stopPropagation();
                          navigateToSubject(subject.id);
                        }}
                        className="flex-1"
                        size="sm"
                      >
                        View Details
                      </Button>
                      <Button
                        onClick={(e) => {
                          e.stopPropagation();
                          handleDeleteSubject(subject.id);
                        }}
                        variant="destructive"
                        size="sm"
                      >
                        <Trash2 className="w-4 h-4" />
                      </Button>
                    </div>
                  </CardContent>
                </Card>
              </motion.div>
            ))}
          </div>
        )}
        </div>
      </div>
      </div>
    </Layout>
  );
};

export default MySubjects;
