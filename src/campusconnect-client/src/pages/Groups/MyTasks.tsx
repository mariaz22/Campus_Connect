import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import {
  CheckSquare,
  Circle,
  CheckCircle2,
  Trash2,
  Clock,
  Calendar,
  BookOpen,
  Sparkles,
  ListTodo,
  Target,
  TrendingUp,
  AlertCircle,
} from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { groupApi } from '../../services/groupApi';
import type { SavedTask } from '../../services/groupApi';

const MyTasks = () => {
  const [tasks, setTasks] = useState<SavedTask[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<'all' | 'pending' | 'completed'>('all');

  useEffect(() => {
    loadTasks();
  }, []);

  const loadTasks = async () => {
    setLoading(true);
    try {
      const tasksData = await groupApi.getMySavedTasks();
      setTasks(tasksData);
    } catch (error) {
      console.error('Error loading tasks:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleToggleComplete = async (taskId: number, isCompleted: boolean) => {
    try {
      if (isCompleted) {
        await groupApi.markTaskAsIncomplete(taskId);
      } else {
        await groupApi.markTaskAsCompleted(taskId);
      }
      loadTasks();
    } catch (error) {
      console.error('Error toggling task completion:', error);
    }
  };

  const handleRemoveTask = async (taskId: number) => {
    if (!window.confirm('Are you sure you want to remove this task?')) return;
    try {
      await groupApi.unsaveTask(taskId);
      loadTasks();
    } catch (error) {
      console.error('Error removing task:', error);
    }
  };

  const formatDate = (date?: string) => {
    if (!date) return null;
    return new Date(date).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const isOverdue = (dueDate?: string) => {
    if (!dueDate) return false;
    return new Date(dueDate) < new Date();
  };

  const getFilteredTasks = () => {
    switch (filter) {
      case 'pending':
        return tasks.filter((t) => !t.isCompleted);
      case 'completed':
        return tasks.filter((t) => t.isCompleted);
      default:
        return tasks;
    }
  };

  const filteredTasks = getFilteredTasks();
  const pendingCount = tasks.filter((t) => !t.isCompleted).length;
  const completedCount = tasks.filter((t) => t.isCompleted).length;
  const completionRate = tasks.length > 0 ? Math.round((completedCount / tasks.length) * 100) : 0;
  const overdueCount = tasks.filter((t) => !t.isCompleted && isOverdue(t.dueDate)).length;

  const tabs = [
    { id: 'all' as const, label: 'All Tasks', count: tasks.length },
    { id: 'pending' as const, label: 'Pending', count: pendingCount },
    { id: 'completed' as const, label: 'Completed', count: completedCount },
  ];

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-orange-500 via-red-500 to-pink-500 p-8 text-white shadow-2xl"
        >
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>

          <div className="relative z-10">
            <div className="flex items-center gap-3 mb-4">
              <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                <CheckSquare className="h-8 w-8" />
              </div>
              <div>
                <h1 className="text-4xl font-bold flex items-center gap-2">
                  My Tasks
                  <Sparkles className="h-6 w-6 text-yellow-300" />
                </h1>
                <p className="text-white/80 mt-1">Track your assignments and deadlines</p>
              </div>
            </div>

            {/* Stats */}
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mt-6">
              <div className="bg-white/10 backdrop-blur-sm rounded-xl p-4">
                <div className="flex items-center gap-2 mb-2">
                  <ListTodo className="h-5 w-5" />
                  <span className="text-sm opacity-80">Total</span>
                </div>
                <div className="text-3xl font-bold">{tasks.length}</div>
              </div>

              <div className="bg-white/10 backdrop-blur-sm rounded-xl p-4">
                <div className="flex items-center gap-2 mb-2">
                  <Clock className="h-5 w-5" />
                  <span className="text-sm opacity-80">Pending</span>
                </div>
                <div className="text-3xl font-bold text-yellow-300">{pendingCount}</div>
              </div>

              <div className="bg-white/10 backdrop-blur-sm rounded-xl p-4">
                <div className="flex items-center gap-2 mb-2">
                  <Target className="h-5 w-5" />
                  <span className="text-sm opacity-80">Completed</span>
                </div>
                <div className="text-3xl font-bold text-green-300">{completedCount}</div>
              </div>

              <div className="bg-white/10 backdrop-blur-sm rounded-xl p-4">
                <div className="flex items-center gap-2 mb-2">
                  <TrendingUp className="h-5 w-5" />
                  <span className="text-sm opacity-80">Progress</span>
                </div>
                <div className="text-3xl font-bold">{completionRate}%</div>
              </div>
            </div>
          </div>
        </motion.div>

        {/* Tabs */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
        >
          <Card>
            <CardContent className="pt-6">
              <div className="flex gap-2 overflow-x-auto pb-2">
                {tabs.map((tab) => (
                  <motion.div key={tab.id} whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                    <Button
                      variant={filter === tab.id ? 'default' : 'outline'}
                      onClick={() => setFilter(tab.id)}
                      className="whitespace-nowrap"
                    >
                      {tab.label}
                      <Badge className="ml-2 bg-primary-foreground/20">{tab.count}</Badge>
                    </Button>
                  </motion.div>
                ))}
              </div>

              {overdueCount > 0 && filter !== 'completed' && (
                <div className="mt-4 flex items-center gap-2 text-sm text-red-500">
                  <AlertCircle className="h-4 w-4" />
                  <span>{overdueCount} overdue tasks</span>
                </div>
              )}
            </CardContent>
          </Card>
        </motion.div>

        {/* Tasks List */}
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
          </div>
        ) : filteredTasks.length === 0 ? (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="text-center py-12">
            <CheckSquare className="h-16 w-16 mx-auto text-muted-foreground mb-4" />
            <h3 className="text-xl font-semibold mb-2">No tasks found</h3>
            <p className="text-muted-foreground">
              {filter === 'all' && 'Join a group and save tasks to see them here'}
              {filter === 'pending' && 'No pending tasks'}
              {filter === 'completed' && 'No completed tasks yet'}
            </p>
          </motion.div>
        ) : (
          <div className="space-y-4">
            <AnimatePresence mode="popLayout">
              {filteredTasks.map((task, index) => {
                const overdue = !task.isCompleted && isOverdue(task.dueDate);

                return (
                  <motion.div
                    key={task.id}
                    initial={{ opacity: 0, x: -20 }}
                    animate={{ opacity: 1, x: 0 }}
                    exit={{ opacity: 0, x: 20 }}
                    transition={{ delay: index * 0.05 }}
                    layout
                  >
                    <Card
                      className={`group relative overflow-hidden border-2 transition-all hover:shadow-lg ${
                        task.isCompleted
                          ? 'bg-green-50/50 dark:bg-green-950/10 border-green-200 dark:border-green-900'
                          : overdue
                          ? 'bg-red-50/50 dark:bg-red-950/10 border-red-200 dark:border-red-900'
                          : 'hover:border-primary/50'
                      }`}
                    >
                      <CardContent className="pt-6">
                        <div className="flex items-start gap-4">
                          {/* Checkbox */}
                          <motion.button
                            whileHover={{ scale: 1.1 }}
                            whileTap={{ scale: 0.9 }}
                            onClick={() => handleToggleComplete(task.taskId, task.isCompleted)}
                            className="mt-1 flex-shrink-0"
                          >
                            {task.isCompleted ? (
                              <CheckCircle2 className="h-6 w-6 text-green-500 fill-green-500" />
                            ) : (
                              <Circle className="h-6 w-6 text-muted-foreground hover:text-primary" />
                            )}
                          </motion.button>

                          {/* Content */}
                          <div className="flex-1 min-w-0">
                            <h4
                              className={`text-lg font-semibold mb-2 ${
                                task.isCompleted ? 'line-through text-muted-foreground' : ''
                              }`}
                            >
                              {task.taskTitle}
                            </h4>

                            {task.taskDescription && (
                              <p className="text-sm text-muted-foreground mb-3 line-clamp-2">
                                {task.taskDescription}
                              </p>
                            )}

                            <div className="flex flex-wrap gap-3 text-sm">
                              <div className="flex items-center gap-1 text-muted-foreground">
                                <BookOpen className="h-4 w-4" />
                                <span className="font-medium">{task.groupName}</span>
                                <span>· {task.subject}</span>
                              </div>

                              {task.dueDate && (
                                <div
                                  className={`flex items-center gap-1 ${
                                    overdue ? 'text-red-500 font-semibold' : 'text-muted-foreground'
                                  }`}
                                >
                                  <Calendar className="h-4 w-4" />
                                  <span>{formatDate(task.dueDate)}</span>
                                  {overdue && (
                                    <Badge className="ml-1 bg-red-500 text-white">Overdue</Badge>
                                  )}
                                </div>
                              )}

                              {task.isCompleted && task.completedAt && (
                                <div className="flex items-center gap-1 text-green-600">
                                  <CheckCircle2 className="h-4 w-4" />
                                  <span>Completed {formatDate(task.completedAt)}</span>
                                </div>
                              )}
                            </div>
                          </div>

                          {/* Delete Button */}
                          <motion.button
                            whileHover={{ scale: 1.1 }}
                            whileTap={{ scale: 0.9 }}
                            onClick={() => handleRemoveTask(task.taskId)}
                            className="p-2 rounded-lg hover:bg-red-50 dark:hover:bg-red-950/20 transition-colors flex-shrink-0"
                          >
                            <Trash2 className="h-5 w-5 text-red-500" />
                          </motion.button>
                        </div>
                      </CardContent>
                    </Card>
                  </motion.div>
                );
              })}
            </AnimatePresence>
          </div>
        )}
      </div>
    </Layout>
  );
};

export default MyTasks;
