import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Trophy, Plus, Edit, Trash2, Save, X } from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import achievementApi, { type Achievement, type CreateAchievementRequest, type UpdateAchievementRequest } from '../../services/achievementApi';

function ManageAchievements() {
  const [achievements, setAchievements] = useState<Achievement[]>([]);
  const [loading, setLoading] = useState(true);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [isCreating, setIsCreating] = useState(false);

  const [formData, setFormData] = useState({
    title: '',
    description: '',
    icon: '',
    isActive: true,
  });

  useEffect(() => {
    fetchAchievements();
  }, []);

  const fetchAchievements = async () => {
    setLoading(true);
    try {
      const data = await achievementApi.getAllAchievements();
      setAchievements(data);
    } catch (error) {
      console.error('Failed to fetch achievements', error);
      alert('Failed to load achievements');
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async () => {
    if (!formData.title || !formData.description || !formData.icon) {
      alert('Please fill all fields');
      return;
    }

    try {
      const request: CreateAchievementRequest = {
        title: formData.title,
        description: formData.description,
        icon: formData.icon,
      };
      await achievementApi.createAchievement(request);
      setIsCreating(false);
      setFormData({ title: '', description: '', icon: '', isActive: true });
      fetchAchievements();
    } catch (error) {
      console.error('Failed to create achievement', error);
      alert('Failed to create achievement');
    }
  };

  const handleUpdate = async (id: number) => {
    if (!formData.title || !formData.description || !formData.icon) {
      alert('Please fill all fields');
      return;
    }

    try {
      const request: UpdateAchievementRequest = {
        title: formData.title,
        description: formData.description,
        icon: formData.icon,
        isActive: formData.isActive,
      };
      await achievementApi.updateAchievement(id, request);
      setEditingId(null);
      setFormData({ title: '', description: '', icon: '', isActive: true });
      fetchAchievements();
    } catch (error) {
      console.error('Failed to update achievement', error);
      alert('Failed to update achievement');
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this achievement?')) return;

    try {
      await achievementApi.deleteAchievement(id);
      fetchAchievements();
    } catch (error) {
      console.error('Failed to delete achievement', error);
      alert('Failed to delete achievement');
    }
  };

  const startEdit = (achievement: Achievement) => {
    setEditingId(achievement.id);
    setFormData({
      title: achievement.title,
      description: achievement.description,
      icon: achievement.icon,
      isActive: achievement.isActive,
    });
    setIsCreating(false);
  };

  const cancelEdit = () => {
    setEditingId(null);
    setIsCreating(false);
    setFormData({ title: '', description: '', icon: '', isActive: true });
  };

  if (loading) {
    return (
      <Layout>
        <div className="flex items-center justify-center min-h-[60vh]">
          <p className="text-muted-foreground">Loading...</p>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="space-y-6">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="flex items-center justify-between"
        >
          <div>
            <h1 className="text-3xl font-bold flex items-center gap-2">
              <Trophy className="h-8 w-8 text-yellow-500" />
              Manage Achievements
            </h1>
            <p className="text-muted-foreground mt-1">Create, edit, and delete achievements</p>
          </div>
          <Button
            onClick={() => {
              setIsCreating(true);
              setEditingId(null);
              setFormData({ title: '', description: '', icon: '', isActive: true });
            }}
            className="bg-yellow-500 hover:bg-yellow-600"
          >
            <Plus className="h-4 w-4 mr-2" />
            New Achievement
          </Button>
        </motion.div>

        {/* Create Form */}
        {isCreating && (
          <motion.div
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
          >
            <Card className="border-yellow-200 dark:border-yellow-800">
              <CardHeader>
                <CardTitle>Create New Achievement</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  <div>
                    <label className="text-sm font-medium">Title</label>
                    <Input
                      value={formData.title}
                      onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                      placeholder="Achievement title"
                    />
                  </div>
                  <div>
                    <label className="text-sm font-medium">Description</label>
                    <Input
                      value={formData.description}
                      onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                      placeholder="Achievement description"
                    />
                  </div>
                  <div>
                    <label className="text-sm font-medium">Icon (emoji)</label>
                    <Input
                      value={formData.icon}
                      onChange={(e) => setFormData({ ...formData, icon: e.target.value })}
                      placeholder="🏆"
                      maxLength={10}
                    />
                  </div>
                  <div className="flex gap-2">
                    <Button onClick={handleCreate} className="bg-green-600 hover:bg-green-700">
                      <Save className="h-4 w-4 mr-2" />
                      Create
                    </Button>
                    <Button onClick={cancelEdit} variant="outline">
                      <X className="h-4 w-4 mr-2" />
                      Cancel
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          </motion.div>
        )}

        {/* Achievements List */}
        <div className="grid grid-cols-1 gap-4">
          {achievements.map((achievement, index) => (
            <motion.div
              key={achievement.id}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: index * 0.05 }}
            >
              {editingId === achievement.id ? (
                <Card className="border-blue-200 dark:border-blue-800">
                  <CardContent className="pt-6">
                    <div className="space-y-4">
                      <div>
                        <label className="text-sm font-medium">Title</label>
                        <Input
                          value={formData.title}
                          onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                        />
                      </div>
                      <div>
                        <label className="text-sm font-medium">Description</label>
                        <Input
                          value={formData.description}
                          onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                        />
                      </div>
                      <div>
                        <label className="text-sm font-medium">Icon</label>
                        <Input
                          value={formData.icon}
                          onChange={(e) => setFormData({ ...formData, icon: e.target.value })}
                          maxLength={10}
                        />
                      </div>
                      <div className="flex items-center gap-2">
                        <input
                          type="checkbox"
                          checked={formData.isActive}
                          onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
                          className="h-4 w-4"
                        />
                        <label className="text-sm font-medium">Active</label>
                      </div>
                      <div className="flex gap-2">
                        <Button onClick={() => handleUpdate(achievement.id)} className="bg-blue-600 hover:bg-blue-700">
                          <Save className="h-4 w-4 mr-2" />
                          Save
                        </Button>
                        <Button onClick={cancelEdit} variant="outline">
                          <X className="h-4 w-4 mr-2" />
                          Cancel
                        </Button>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ) : (
                <Card>
                  <CardContent className="pt-6">
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-4">
                        <div className="text-4xl">{achievement.icon}</div>
                        <div>
                          <h3 className="font-semibold text-lg">{achievement.title}</h3>
                          <p className="text-sm text-muted-foreground">{achievement.description}</p>
                          <p className="text-xs text-muted-foreground mt-1">
                            Status: {achievement.isActive ? 'Active' : 'Inactive'}
                          </p>
                        </div>
                      </div>
                      <div className="flex gap-2">
                        <Button
                          onClick={() => startEdit(achievement)}
                          variant="outline"
                          size="sm"
                        >
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button
                          onClick={() => handleDelete(achievement.id)}
                          variant="outline"
                          size="sm"
                          className="text-red-600 hover:text-red-700"
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              )}
            </motion.div>
          ))}
        </div>

        {achievements.length === 0 && !isCreating && (
          <div className="text-center py-12">
            <Trophy className="h-16 w-16 mx-auto mb-4 text-gray-300 dark:text-gray-700" />
            <p className="text-muted-foreground">No achievements yet. Create your first one!</p>
          </div>
        )}
      </div>
    </Layout>
  );
}

export default ManageAchievements;
