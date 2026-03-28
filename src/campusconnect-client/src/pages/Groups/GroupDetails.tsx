import { useState, useEffect, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import {
  Users,
  ArrowLeft,
  Sparkles,
  BookOpen,
  GraduationCap,
  FileText,
  Plus,
  Save,
  Trash2,
  CheckCircle2,
  Circle,
  Calendar,
  ClipboardList,
  Shield,
  X,
  UserPlus,
  LogOut,
} from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Badge } from '../../components/ui/Badge';
import { Skeleton } from '../../components/ui/Skeleton';
import { groupApi } from '../../services/groupApi';
import type { Group, GroupTask } from '../../services/groupApi';
import CourseMaterials from './CourseMaterials';
import GroupChat from './GroupChat';

interface TaskCardProps {
  task: GroupTask;
  onSave?: (id: number) => void;
  onUnsave?: (id: number) => void;
  onComplete?: (id: number) => void;
  onIncomplete?: (id: number) => void;
  onDelete?: (id: number) => void;
  isProfessor: boolean;
  isGroupOwner: boolean;
}

const TaskCard = ({ task, onSave, onUnsave, onComplete, onIncomplete, onDelete, isProfessor, isGroupOwner }: TaskCardProps) => {
  const formatDate = (date?: string) => {
    if (!date) return null;
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };

  const isOverdue = task.dueDate && new Date(task.dueDate) < new Date() && !task.isCompleted;
 
  return (
    <motion.div
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: -10 }}
      transition={{ duration: 0.2 }}
    >
      <Card className={`${task.isCompleted ? 'bg-green-50 dark:bg-green-950/20 border-green-200 dark:border-green-900' : ''} ${isOverdue ? 'border-red-200 dark:border-red-900' : ''}`}>
        <CardContent className="pt-6">
          <div className="flex items-start justify-between gap-4">
            <div className="flex-1">
              <div className="flex items-center gap-2 mb-2">
                {task.isCompleted ? (
                  <CheckCircle2 className="h-5 w-5 text-green-600 flex-shrink-0" />
                ) : (
                  <Circle className="h-5 w-5 text-gray-400 flex-shrink-0" />
                )}
                <h4 className="font-semibold text-lg">
                  {task.title}
                </h4>
              </div>

              {task.description && (
                <p className="text-muted-foreground text-sm ml-7 mb-3">
                  {task.description}
                </p>
              )}

              <div className="flex flex-wrap gap-3 ml-7 text-sm">
                <div className="flex items-center gap-1.5 text-muted-foreground">
                  <GraduationCap className="h-4 w-4" />
                  <span>{task.createdByProfessorName}</span>
                </div>

                {task.dueDate && (
                  <div className={`flex items-center gap-1.5 ${isOverdue ? 'text-red-600 dark:text-red-400 font-medium' : 'text-muted-foreground'}`}>
                    <Calendar className="h-4 w-4" />
                    <span>{formatDate(task.dueDate)}</span>
                    {isOverdue && <Badge variant="danger" className="ml-1 text-xs">Overdue</Badge>}
                  </div>
                )}
              </div>
            </div>
          </div>

          <div className="flex flex-wrap gap-2 mt-4">
            {!isProfessor && (
              <>
                {task.isSavedByUser ? (
                  <>
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => onUnsave && onUnsave(task.id)}
                      className="text-xs"
                    >
                      <X className="h-3 w-3 mr-1" />
                      Remove from Saved
                    </Button>
                    {task.isCompleted ? (
                      <Button
                        size="sm"
                        variant="outline"
                        onClick={() => onIncomplete && onIncomplete(task.id)}
                        className="text-xs hover:bg-yellow-50 hover:border-yellow-200"
                      >
                        <Circle className="h-3 w-3 mr-1" />
                        Mark Incomplete
                      </Button>
                    ) : (
                      <Button
                        size="sm"
                        variant="default"
                        onClick={() => onComplete && onComplete(task.id)}
                        className="text-xs bg-green-600 hover:bg-green-700"
                      >
                        <CheckCircle2 className="h-3 w-3 mr-1" />
                        Mark Complete
                      </Button>
                    )}
                  </>
                ) : (
                  <Button
                    size="sm"
                    variant="default"
                    onClick={() => onSave && onSave(task.id)}
                    className="text-xs"
                  >
                    <Save className="h-3 w-3 mr-1" />
                    Save Task
                  </Button>
                )}
              </>
            )}

            {isGroupOwner && onDelete && (
              <Button
                size="sm"
                variant="outline"
                onClick={() => {
                  if (window.confirm('Are you sure you want to delete this task?')) {
                    onDelete(task.id);
                  }
                }}
                className="text-xs hover:bg-red-50 hover:text-red-600 hover:border-red-200 dark:hover:bg-red-950/20"
              >
                <Trash2 className="h-3 w-3 mr-1" />
                Delete
              </Button>
            )}
          </div>
        </CardContent>
      </Card>
    </motion.div>
  );
};

const GroupDetails = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [group, setGroup] = useState<Group | null>(null);
  const [tasks, setTasks] = useState<GroupTask[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [newTask, setNewTask] = useState({ title: '', description: '', dueDate: '' });
  const [isSubmitting, setIsSubmitting] = useState(false);

  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const currentUserId = user.id || user.userId || localStorage.getItem('userId');
  const token = localStorage.getItem('token');
  const isProfessor = user.role === 'Professor';
  const isAdmin = user.role === 'Admin';

  const isGroupOwner = useMemo(() => {
    if (!group || !currentUserId) return false;
    return Number(group.professorId) === Number(currentUserId);
  }, [group, currentUserId]);

  const isMember = useMemo(() => {
    if (!group || !currentUserId) return false;

    const membersList = (group as any).members || (group as any).Members || [];

    return membersList.some((m: any) => {
        const memberId = m.studentId || m.StudentId || m.userId || m.UserId || m.id;
        
        return Number(memberId) === Number(currentUserId);
    });
  }, [group, currentUserId]);

  const canManageTasks = useMemo(() => {
    return isGroupOwner || isAdmin;
  }, [isGroupOwner, isAdmin]);

  useEffect(() => {
    loadGroupDetails();
  }, [id]);

  const loadGroupDetails = async () => {
    setLoading(true);
    try {
      const groupData = await groupApi.getGroupById(Number(id));
      setGroup(groupData);
      const tasksData = await groupApi.getGroupTasks(Number(id));
      setTasks(tasksData);
    } catch (error) {
      console.error('Error loading group details:', error);
      setGroup(null);
    } finally {
      setLoading(false);
    }
  };

  const handleJoinGroup = async () => {
    try {
      const res = await fetch(`http://localhost:5099/api/group/${id}/join`, {
        method: 'POST',
        headers: { 
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` 
        }
      });

      const data = await res.json();

      if (res.ok) {
        alert("Succes: " + data.message);
        await loadGroupDetails(); 
      } else {
        alert("Eroare: " + data.message);
      }
    } catch (err) {
      console.error("Network error:", err);
      alert("An error occurred while trying to join the group.");
    }
  };

  const handleLeaveGroup = async () => {
    if(!window.confirm("Are you sure you want to exit the group?")) return;
    
    try {
      const res = await fetch(`http://localhost:5099/api/group/${id}/leave`, {
        method: 'POST', 
        headers: { 
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}` 
        }
      });
      
      const data = await res.json();

      if (res.ok) {
        alert("Succes: " + data.message);
        await loadGroupDetails(); 
      } else {
        alert("Eroare: " + data.message);
      }
    } catch (err) {
      console.error(err);
    }
  };

  const handleCreateTask = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      await groupApi.createTask(Number(id), {
        title: newTask.title,
        description: newTask.description || undefined,
        dueDate: newTask.dueDate || undefined
      });
      setShowCreateForm(false);
      setNewTask({ title: '', description: '', dueDate: '' });
      await loadGroupDetails();
    } catch (error) {
      console.error('Error creating task:', error);
      alert('Error creating task');
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleSaveTask = async (taskId: number) => {
    try {
      await groupApi.saveTask(taskId);
      await loadGroupDetails();
    } catch (error) {
      console.error('Error saving task:', error);
      alert('Error saving task');
    }
  };

  const handleUnsaveTask = async (taskId: number) => {
    try {
      await groupApi.unsaveTask(taskId);
      await loadGroupDetails();
    } catch (error) {
      console.error('Error unsaving task:', error);
      alert('Error removing task from saved');
    }
  };

  const handleCompleteTask = async (taskId: number) => {
    try {
      await groupApi.markTaskAsCompleted(taskId);
      await loadGroupDetails();
    } catch (error) {
      console.error('Error completing task:', error);
      alert('Error marking task as complete');
    }
  };

  const handleIncompleteTask = async (taskId: number) => {
    try {
      await groupApi.markTaskAsIncomplete(taskId);
      await loadGroupDetails();
    } catch (error) {
      console.error('Error marking task as incomplete:', error);
      alert('Error marking task as incomplete');
    }
  };

  const handleDeleteTask = async (taskId: number) => {
    try {
      await groupApi.deleteTask(taskId);
      await loadGroupDetails();
    } catch (error) {
      console.error('Error deleting task:', error);
      alert('Error deleting task');
    }
  };

  if (loading) {
    return (
      <Layout>
        <div className="space-y-6">
          <Skeleton className="h-48 w-full rounded-2xl" />
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <Skeleton className="h-32 w-full rounded-xl" />
            <Skeleton className="h-32 w-full rounded-xl" />
            <Skeleton className="h-32 w-full rounded-xl" />
          </div>
          <Card>
            <CardHeader>
              <Skeleton className="h-6 w-32" />
            </CardHeader>
            <CardContent className="space-y-4">
              <Skeleton className="h-24 w-full" />
              <Skeleton className="h-24 w-full" />
            </CardContent>
          </Card>
        </div>
      </Layout>
    );
  }

  if (!group) {
    return (
      <Layout>
        <div className="text-center py-12">
          <Shield className="h-16 w-16 mx-auto text-red-500 mb-4" />
          <h3 className="text-xl font-semibold mb-2 text-red-500">Error</h3>
          <p className="text-muted-foreground mb-4">Group not found</p>
          <Button onClick={() => navigate('/groups')}>
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back to Groups
          </Button>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-emerald-500 via-teal-500 to-cyan-500 p-8 text-white shadow-2xl"
        >
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>

          <div className="relative z-10 flex items-center justify-between">
            <div>
              <div className="flex items-center gap-3 mb-2">
                <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                  <Users className="h-8 w-8" />
                </div>
                <div>
                  <h1 className="text-4xl font-bold flex items-center gap-2">
                    {group.name}
                    <Sparkles className="h-6 w-6 text-yellow-300" />
                  </h1>
                  <p className="text-white/80 mt-1">Group Details & Tasks</p>
                </div>
              </div>
            </div>

            <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
              <Button
                onClick={() => navigate('/groups')}
                className="bg-white text-emerald-600 hover:bg-white/90 shadow-lg"
              >
                <ArrowLeft className="h-5 w-5 mr-2" />
                Back
              </Button>
            </motion.div>
          </div>
        </motion.div>

        {/* Group Info Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.1 }}
          >
            <Card>
              <CardContent className="pt-6">
                <div className="flex items-center gap-4">
                  <div className="p-3 rounded-xl bg-blue-100 dark:bg-blue-900/20">
                    <BookOpen className="h-6 w-6 text-blue-600" />
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Subject</p>
                    <p className="font-semibold">{group.subject}</p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.2 }}
          >
            <Card>
              <CardContent className="pt-6">
                <div className="flex items-center gap-4">
                  <div className="p-3 rounded-xl bg-purple-100 dark:bg-purple-900/20">
                    <GraduationCap className="h-6 w-6 text-purple-600" />
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Professor</p>
                    <p className="font-semibold">{group.professorName}</p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.3 }}
          >
            <Card>
              <CardContent className="pt-6">
                <div className="flex items-center gap-4">
                  <div className="p-3 rounded-xl bg-green-100 dark:bg-green-900/20">
                    <Users className="h-6 w-6 text-green-600" />
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Members</p>
                    <p className="text-2xl font-bold">{group.membersCount}</p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </motion.div>
        </div>

        {/* Description */}
        {group.description && (
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.4 }}
          >
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <FileText className="h-5 w-5" />
                  Description
                </CardTitle>
              </CardHeader>
              <CardContent>
                <p className="text-muted-foreground">{group.description}</p>
              </CardContent>
            </Card>
          </motion.div>
        )}

        <div className="flex gap-2">
            {!isGroupOwner && !isMember && (
                <Button onClick={handleJoinGroup} className="bg-white text-emerald-600 hover:bg-gray-100 border border-emerald-600">
                <UserPlus className="h-4 w-4 mr-2" />
                Participate
                </Button>
            )}

            {!isGroupOwner && isMember && (
                <Button onClick={handleLeaveGroup} variant="destructive">
                <LogOut className="h-4 w-4 mr-2" />
                Leave Group
                </Button>
            )}

            {isGroupOwner && (
                <Badge className="bg-yellow-400 text-black px-4 py-2 text-sm h-10 flex items-center">
                Owner
                </Badge>
            )}
        </div>

        {/* Main Content Grid - Left: Tasks + Materials, Right: Chat */}
        {(isMember || isGroupOwner) && (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            {/* Left Column - Tasks and Materials */}
            <div className="lg:col-span-2 space-y-6">
              {/* Tasks Section */}
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.5 }}
              >
                <Card>
                <CardHeader>
                  <div className="flex items-center justify-between">
                    <CardTitle className="flex items-center gap-2">
                      <ClipboardList className="h-5 w-5" />
                      Tasks ({tasks.length})
                    </CardTitle>
                    {canManageTasks && (
                      <Button
                        onClick={() => setShowCreateForm(!showCreateForm)}
                        variant={showCreateForm ? 'outline' : 'default'}
                      >
                        {showCreateForm ? (
                          <>
                            <X className="h-4 w-4 mr-2" />
                            Cancel
                          </>
                        ) : (
                          <>
                            <Plus className="h-4 w-4 mr-2" />
                            Add Task
                          </>
                        )}
                      </Button>
                    )}
                  </div>
                </CardHeader>
                <CardContent>
                  <AnimatePresence>
                    {showCreateForm && (
                      <motion.div
                        initial={{ opacity: 0, height: 0 }}
                        animate={{ opacity: 1, height: 'auto' }}
                        exit={{ opacity: 0, height: 0 }}
                        className="mb-6"
                      >
                        <Card className="bg-secondary/50">
                          <CardHeader>
                            <CardTitle className="text-lg">Create New Task</CardTitle>
                          </CardHeader>
                          <CardContent>
                            <form onSubmit={handleCreateTask} className="space-y-4">
                              <div>
                                <label className="block text-sm font-medium mb-2">
                                  Title <span className="text-red-500">*</span>
                                </label>
                                <Input
                                  type="text"
                                  placeholder="Enter task title..."
                                  value={newTask.title}
                                  onChange={(e) => setNewTask({ ...newTask, title: e.target.value })}
                                  required
                                />
                              </div>

                              <div>
                                <label className="block text-sm font-medium mb-2">
                                  Description
                                </label>
                                <textarea
                                  placeholder="Enter task description..."
                                  value={newTask.description}
                                  onChange={(e) => setNewTask({ ...newTask, description: e.target.value })}
                                  rows={3}
                                  className="w-full px-3 py-2 border border-input rounded-md bg-background focus:outline-none focus:ring-2 focus:ring-ring focus:border-transparent resize-y"
                                />
                              </div>

                              <div>
                                <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                                  <Calendar className="h-4 w-4" />
                                  Due Date
                                </label>
                                <Input
                                  type="date"
                                  value={newTask.dueDate}
                                  onChange={(e) => setNewTask({ ...newTask, dueDate: e.target.value })}
                                />
                              </div>

                              <Button
                                type="submit"
                                disabled={isSubmitting}
                                className="w-full"
                              >
                                {isSubmitting ? (
                                  <>
                                    <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                                    Creating...
                                  </>
                                ) : (
                                  <>
                                    <Plus className="h-4 w-4 mr-2" />
                                    Create Task
                                  </>
                                )}
                              </Button>
                            </form>
                          </CardContent>
                        </Card>
                      </motion.div>
                    )}
                  </AnimatePresence>

                  {tasks.length === 0 ? (
                    <div className="text-center py-12">
                      <ClipboardList className="h-16 w-16 mx-auto text-muted-foreground/50 mb-4" />
                      <p className="text-muted-foreground">No tasks in this group yet</p>
                      {canManageTasks && (
                        <p className="text-sm text-muted-foreground mt-2">
                          Click "Add Task" to create the first task
                        </p>
                      )}
                    </div>
                  ) : (
                    <div className="space-y-3">
                      <AnimatePresence>
                        {tasks.map((task) => (
                          <TaskCard
                            key={task.id}
                            task={task}
                            onSave={handleSaveTask}
                            onUnsave={handleUnsaveTask}
                            onComplete={handleCompleteTask}
                            onIncomplete={handleIncompleteTask}
                            onDelete={canManageTasks ? handleDeleteTask : undefined}
                            isProfessor={isProfessor}
                            isGroupOwner={canManageTasks}
                          />
                        ))}
                      </AnimatePresence>
                    </div>
                  )}
                </CardContent>
              </Card>
            </motion.div>

            {/* Course Materials Section */}
            <CourseMaterials 
              groupId={Number(id)} 
              isGroupOwner={isGroupOwner}
            />

            {/* Owner Badge */}
            {isGroupOwner && (
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.8 }}
              >
                <Card className="bg-orange-50 dark:bg-orange-900/20 border-orange-200 dark:border-orange-900">
                  <CardContent className="pt-6">
                    <p className="text-sm text-orange-800 dark:text-orange-400 flex items-center gap-2">
                      <Shield className="h-4 w-4" />
                      You are the owner of this group
                    </p>
                  </CardContent>
                </Card>
              </motion.div>
            )}
          </div>

          {/* Right Column - Group Chat */}
          <div className="lg:col-span-1">
            <GroupChat groupId={Number(id)} />
          </div>
        </div>
        )}
      </div>
    </Layout>
  );
};

export default GroupDetails;