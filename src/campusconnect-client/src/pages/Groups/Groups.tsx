import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import {
  Users,
  Plus,
  BookOpen,
  UserCheck,
  UserPlus,
  Trash2,
  Sparkles,
  CheckSquare,
  GraduationCap,
  X,
} from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Input } from '../../components/ui/Input';
import { groupApi } from '../../services/groupApi';
import type { Group } from '../../services/groupApi';

const Groups = () => {
  const navigate = useNavigate();
  const [groups, setGroups] = useState<Group[]>([]);
  const [myGroups, setMyGroups] = useState<Group[]>([]);
  const [createdGroups, setCreatedGroups] = useState<Group[]>([]);
  const [availableGroups, setAvailableGroups] = useState<Group[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'all' | 'my' | 'available' | 'created'>('all');
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [newGroup, setNewGroup] = useState({ name: '', description: '', subject: '' });

  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const isProfessor = user.role === 'Professor';
  const isAdmin = user.role === 'Admin';
  const canCreateGroups = isProfessor || isAdmin;

  useEffect(() => {
    loadGroups();
  }, [activeTab]);

  const loadGroups = async () => {
    setLoading(true);
    try {
      switch (activeTab) {
        case 'all':
          const allGroups = await groupApi.getAllGroups();
          setGroups(allGroups);
          break;
        case 'my':
          const myGrps = await groupApi.getMyGroups();
          setMyGroups(myGrps);
          break;
        case 'available':
          const availGrps = await groupApi.getAvailableGroups();
          setAvailableGroups(availGrps);
          break;
        case 'created':
          if (canCreateGroups) {
            const createdGrps = await groupApi.getGroupsCreatedByMe();
            setCreatedGroups(createdGrps);
          }
          break;
      }
    } catch (error) {
      console.error('Error loading groups:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleJoinGroup = async (id: number) => {
    try {
      await groupApi.joinGroup(id);
      loadGroups();
    } catch (error) {
      console.error('Error joining group:', error);
    }
  };

  const handleLeaveGroup = async (id: number) => {
    try {
      await groupApi.leaveGroup(id);
      loadGroups();
    } catch (error) {
      console.error('Error leaving group:', error);
    }
  };

  const handleDeleteGroup = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this group?')) return;
    try {
      await groupApi.deleteGroup(id);
      loadGroups();
    } catch (error) {
      console.error('Error deleting group:', error);
    }
  };

  const handleCreateGroup = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await groupApi.createGroup(newGroup);
      setShowCreateForm(false);
      setNewGroup({ name: '', description: '', subject: '' });
      loadGroups();
    } catch (error) {
      console.error('Error creating group:', error);
    }
  };

  const getCurrentGroups = () => {
    switch (activeTab) {
      case 'all':
        return groups;
      case 'my':
        return myGroups;
      case 'available':
        return availableGroups;
      case 'created':
        return createdGroups;
      default:
        return [];
    }
  };

  const tabs = [
    { id: 'all' as const, label: 'All Groups', show: true },
    { id: 'my' as const, label: 'My Groups', show: !isProfessor || isAdmin },
    { id: 'available' as const, label: 'Available', show: !isProfessor || isAdmin },
    { id: 'created' as const, label: 'Created by Me', show: canCreateGroups },
  ];

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-emerald-500 via-green-500 to-teal-500 p-8 text-white shadow-2xl"
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
                    Study Groups
                    <Sparkles className="h-6 w-6 text-yellow-300" />
                  </h1>
                  <p className="text-white/80 mt-1">Collaborate and learn together</p>
                </div>
              </div>
            </div>

            {canCreateGroups && (
              <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                <Button
                  onClick={() => setShowCreateForm(!showCreateForm)}
                  className="bg-white text-green-600 hover:bg-white/90 shadow-lg"
                >
                  <Plus className="h-5 w-5 mr-2" />
                  Create Group
                </Button>
              </motion.div>
            )}
          </div>
        </motion.div>

        {/* Create Group Form */}
        <AnimatePresence>
          {showCreateForm && (
            <motion.div
              initial={{ opacity: 0, height: 0 }}
              animate={{ opacity: 1, height: 'auto' }}
              exit={{ opacity: 0, height: 0 }}
            >
              <Card>
                <CardHeader>
                  <div className="flex items-center justify-between">
                    <CardTitle>Create New Group</CardTitle>
                    <Button variant="ghost" size="icon" onClick={() => setShowCreateForm(false)}>
                      <X className="h-5 w-5" />
                    </Button>
                  </div>
                </CardHeader>
                <CardContent>
                  <form onSubmit={handleCreateGroup} className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium mb-2">Group Name *</label>
                      <Input
                        type="text"
                        value={newGroup.name}
                        onChange={(e) => setNewGroup({ ...newGroup, name: e.target.value })}
                        required
                        placeholder="e.g., Math Study Group"
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-2">Subject *</label>
                      <Input
                        type="text"
                        value={newGroup.subject}
                        onChange={(e) => setNewGroup({ ...newGroup, subject: e.target.value })}
                        required
                        placeholder="e.g., Mathematics"
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-2">Description</label>
                      <textarea
                        value={newGroup.description}
                        onChange={(e) => setNewGroup({ ...newGroup, description: e.target.value })}
                        rows={3}
                        className="w-full px-3 py-2 border border-input rounded-md bg-background"
                        placeholder="Brief description of the group..."
                      />
                    </div>
                    <Button type="submit" className="w-full">
                      <Plus className="h-5 w-5 mr-2" />
                      Create Group
                    </Button>
                  </form>
                </CardContent>
              </Card>
            </motion.div>
          )}
        </AnimatePresence>

        {/* Tabs */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
        >
          <Card>
            <CardContent className="pt-6">
              <div className="flex gap-2 overflow-x-auto pb-2">
                {tabs
                  .filter((tab) => tab.show)
                  .map((tab) => (
                    <motion.div key={tab.id} whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                      <Button
                        variant={activeTab === tab.id ? 'default' : 'outline'}
                        onClick={() => setActiveTab(tab.id)}
                        className="whitespace-nowrap"
                      >
                        {tab.label}
                      </Button>
                    </motion.div>
                  ))}
              </div>

              <div className="mt-4 text-sm text-muted-foreground">
                {getCurrentGroups().length} groups
              </div>
            </CardContent>
          </Card>
        </motion.div>

        {/* Groups Grid */}
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
          </div>
        ) : getCurrentGroups().length === 0 ? (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="text-center py-12">
            <Users className="h-16 w-16 mx-auto text-muted-foreground mb-4" />
            <h3 className="text-xl font-semibold mb-2">No groups found</h3>
            <p className="text-muted-foreground">
              {activeTab === 'available' && 'All groups have been joined'}
              {activeTab === 'my' && 'You haven\'t joined any groups yet'}
              {activeTab === 'created' && 'You haven\'t created any groups yet'}
              {activeTab === 'all' && 'No groups available'}
            </p>
          </motion.div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <AnimatePresence mode="popLayout">
              {getCurrentGroups().map((group, index) => {
                const isOwner = group.professorId === user.id;

                return (
                  <motion.div
                    key={group.id}
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    exit={{ opacity: 0, scale: 0.9 }}
                    transition={{ delay: index * 0.05 }}
                    layout
                  >
                    <Card className="group relative overflow-hidden border-2 hover:border-primary/50 transition-all h-full hover:shadow-xl">
                      <div className="pointer-events-none absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>

                      <CardHeader className="relative">
                        <div className="flex items-start justify-between mb-3">
                          <Badge className="bg-emerald-500 text-white border-0">{group.subject}</Badge>

                          {isOwner && (
                            <Badge className="bg-orange-100 text-orange-700 dark:bg-orange-900/30 dark:text-orange-400 border border-orange-500">
                              <GraduationCap className="h-3 w-3 mr-1" />
                              Owner
                            </Badge>
                          )}
                        </div>

                        <CardTitle className="text-xl line-clamp-2 group-hover:text-primary transition-colors">
                          {group.name}
                        </CardTitle>
                      </CardHeader>

                      <CardContent>
                        {group.description && (
                          <p className="text-muted-foreground text-sm line-clamp-2 mb-4">{group.description}</p>
                        )}

                        <div className="flex items-center gap-4 mb-4 text-sm text-muted-foreground">
                          <div className="flex items-center gap-1">
                            <Users className="h-4 w-4" />
                            <span>{group.membersCount} members</span>
                          </div>
                          <div className="flex items-center gap-1">
                            <CheckSquare className="h-4 w-4" />
                            <span>{group.tasksCount} tasks</span>
                          </div>
                        </div>

                        {group.professorName && (
                          <div className="flex items-center gap-2 mb-4 text-sm">
                            <GraduationCap className="h-4 w-4 text-muted-foreground" />
                            <span className="text-muted-foreground">{group.professorName}</span>
                          </div>
                        )}

                        <div className="flex flex-col gap-2">
                          <Button
                            onClick={() => navigate(`/groups/${group.id}`)}
                            className="w-full"
                            variant="outline"
                          >
                            <BookOpen className="h-4 w-4 mr-2" />
                            View Details
                          </Button>

                          {activeTab === 'available' && (!isProfessor || isAdmin) && (
                            <Button onClick={() => handleJoinGroup(group.id)} className="w-full">
                              <UserPlus className="h-4 w-4 mr-2" />
                              Join Group
                            </Button>
                          )}

                          {activeTab === 'my' && !isOwner && (
                            <Button
                              onClick={() => handleLeaveGroup(group.id)}
                              variant="outline"
                              className="w-full hover:bg-red-50 hover:text-red-600 hover:border-red-200"
                            >
                              <UserCheck className="h-4 w-4 mr-2" />
                              Leave Group
                            </Button>
                          )}

                          {activeTab === 'created' && isOwner && (
                            <Button
                              onClick={() => handleDeleteGroup(group.id)}
                              variant="outline"
                              className="w-full hover:bg-red-50 hover:text-red-600 hover:border-red-200"
                            >
                              <Trash2 className="h-4 w-4 mr-2" />
                              Delete Group
                            </Button>
                          )}
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

export default Groups;
