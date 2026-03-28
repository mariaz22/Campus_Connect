import { useEffect, useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Link } from 'react-router-dom';
import {
  Megaphone,
  Plus,
  Star,
  Trash2,
  Filter,
  Search,
  Calendar,
  Tag,
  BookmarkPlus,
  BookmarkCheck,
  Sparkles,
  Forward,
  X,
} from 'lucide-react';
import { Layout } from '../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import { Input } from '../components/ui/Input';
import { useLocation } from 'react-router-dom';
import '../index.css';

const API_BASE_URL = 'http://localhost:5099/api';

interface CurrentUser {
  id?: number;
  userId?: number;
  firstName: string;
  role?: string;
  isAdmin?: boolean;
}

interface Announcement {
  id: number;
  title: string;
  content: string;
  category: string;
  createdAt: string;
  createdByUserId: number;
}

const categories = [
  { value: '', label: 'All Categories', color: 'bg-slate-500' },
  { value: 'Academic', label: 'Academic', color: 'bg-blue-500' },
  { value: 'Sports', label: 'Sports', color: 'bg-green-500' },
  { value: 'Events', label: 'Events', color: 'bg-purple-500' },
  { value: 'General', label: 'General', color: 'bg-orange-500' },
];

const Announcements = () => {
  const [announcements, setAnnouncements] = useState<Announcement[]>([]);
  const [loading, setLoading] = useState(true);
  const [savedIds, setSavedIds] = useState<number[]>([]);
  const [category, setCategory] = useState<string>('');
  const [searchTerm, setSearchTerm] = useState('');
  const [forwardModalOpen, setForwardModalOpen] = useState(false);
  const [selectedAnnouncementId, setSelectedAnnouncementId] = useState<number | null>(null);
  const [professorGroups, setProfessorGroups] = useState<any[]>([]);
  const [forwardingToGroup, setForwardingToGroup] = useState<number | null>(null);
  
  const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);

  const location = useLocation();

  useEffect(() => {
      const userString = localStorage.getItem('user');
      if (userString) {
        try {
            const userObj = JSON.parse(userString);
            setCurrentUser(userObj);
        } catch (e) {
            console.error("Error parsing user from localstorage");
        }
      }
  }, []);

  useEffect(() => {
    const fetchAnnouncements = async () => {
      setLoading(true);
      try {
        const params = new URLSearchParams();
        if (category) params.append('category', category);
        if (searchTerm) params.append('search', searchTerm);

        const res = await fetch(`${API_BASE_URL}/announcements?${params.toString()}`);
        const data = await res.json();
        setAnnouncements(data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchAnnouncements();
  }, [category, location.search, searchTerm]); // Added searchTerm to dependency

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) return;
    const fetchSaved = async () => {
      try {
        const res = await fetch(`${API_BASE_URL}/announcements/saved`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        if (res.ok) {
          const data = await res.json();
          setSavedIds(data.map((a: any) => a.id));
        }
      } catch (err) {
        console.error('Error fetching saved announcements', err);
      }
    };
    fetchSaved();
  }, []);

  const handleToggleBookmark = async (id: number) => {
    const token = localStorage.getItem('token');
    if (!token) {
      alert('You need to be logged in to save announcements.');
      return;
    }

    const isSaved = savedIds.includes(id);
    try {
      const res = await fetch(`${API_BASE_URL}/announcements/${id}/bookmark`, {
        method: isSaved ? 'DELETE' : 'POST',
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (res.ok || res.status === 204) {
        setSavedIds((prev) =>
          isSaved ? prev.filter((x) => x !== id) : [...prev, id]
        );
      }
    } catch (err) {
      console.error('Bookmark error:', err);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this announcement?')) return;

    const token = localStorage.getItem('token');

    if (!token) {
        alert("You have to be logged in to delete announcements.");
        return;
    }

    try {
      const response = await fetch(`${API_BASE_URL}/announcements/${id}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${token}`
        }
      });

      if (response.ok) {
        setAnnouncements((prev) => prev.filter((a) => a.id !== id));
      } else {
        if (response.status === 403) {
            alert("You do not have permission to delete this announcement (you can only delete announcements you created).");
        } else {
            alert("A apărut o eroare la ștergere.");
        }
      }
    } catch (err) {
      console.error('Delete error:', err);
    }
  };

  const handleOpenForwardModal = async (announcementId: number) => {
    setSelectedAnnouncementId(announcementId);
    
    // Fetch professor's groups (groups created by the professor)
    const token = localStorage.getItem('token');
    try {
      const response = await fetch(`${API_BASE_URL}/group/created-by-me`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (response.ok) {
        const groups = await response.json();
        setProfessorGroups(groups);
        setForwardModalOpen(true);
      } else {
        console.error('Failed to fetch groups:', response.status);
        setForwardModalOpen(true); // Still open modal to show empty state
      }
    } catch (error) {
      console.error('Error fetching groups:', error);
      setForwardModalOpen(true); // Still open modal to show empty state
    }
  };

  const handleForwardToGroup = async (groupId: number) => {
    if (!selectedAnnouncementId) return;
    
    setForwardingToGroup(groupId);
    const token = localStorage.getItem('token');
    
    try {
      const response = await fetch(`${API_BASE_URL}/group/${groupId}/forward-announcement/${selectedAnnouncementId}`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (response.ok) {
        alert('Announcement forwarded successfully!');
        setForwardModalOpen(false);
        setSelectedAnnouncementId(null);
      } else {
        alert('Failed to forward announcement');
      }
    } catch (error) {
      console.error('Error forwarding announcement:', error);
      alert('Error forwarding announcement');
    } finally {
      setForwardingToGroup(null);
    }
  };

  const filteredAnnouncements = announcements.filter((a) =>
    a.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    a.content.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const getCategoryColor = (cat: string) => {
    const found = categories.find((c) => c.value === cat);
    return found?.color || 'bg-slate-500';
  };

  // 2. Logic to determine permissions
  const currentUserId = currentUser?.userId || currentUser?.id;
  const isAdmin = currentUser?.role === 'Admin' || currentUser?.role === 'admin' || currentUser?.isAdmin === true;
  const isProfessor = currentUser?.role === 'Professor' || currentUser?.role === 'professor';
  return (
    <Layout>
      <div className="space-y-6">
        {/* Header Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-blue-500 via-cyan-500 to-teal-500 p-8 text-white shadow-2xl"
        >
           {/* ... Header Content ... */}
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>

          <div className="relative z-10 flex items-center justify-between">
            <div>
              <div className="flex items-center gap-3 mb-2">
                <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                  <Megaphone className="h-8 w-8" />
                </div>
                <div>
                  <h1 className="text-4xl font-bold flex items-center gap-2">
                    Announcements
                    <Sparkles className="h-6 w-6 text-yellow-300" />
                  </h1>
                  <p className="text-white/80 mt-1">Stay updated with campus news and events</p>
                </div>
              </div>
            </div>
            { (isAdmin || isProfessor) && (
              <Link to="/create-announcement">
                <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                  <Button className="bg-white text-blue-600 hover:bg-white/90 shadow-lg">
                    <Plus className="h-5 w-5 mr-2" />
                    Create Announcement
                  </Button>
                </motion.div>
              </Link>
            )}
          </div>
        </motion.div>
        

        {/* Filters Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
        >
          <Card>
            <CardContent className="pt-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {/* Search */}
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-muted-foreground" />
                  <Input
                    type="text"
                    placeholder="Search announcements..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="pl-10"
                  />
                </div>

                {/* Category Filter */}
                <div className="flex items-center gap-2 overflow-x-auto pb-2">
                  <Filter className="h-5 w-5 text-muted-foreground flex-shrink-0" />
                  <div className="flex gap-2">
                    {categories.map((cat) => (
                      <motion.div key={cat.value} whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                        <Button
                          variant={category === cat.value ? 'default' : 'outline'}
                          size="sm"
                          onClick={() => setCategory(cat.value)}
                          className="whitespace-nowrap"
                        >
                          {cat.label}
                        </Button>
                      </motion.div>
                    ))}
                  </div>
                </div>
              </div>

              {/* Stats */}
              <div className="mt-4 flex items-center gap-4 text-sm text-muted-foreground">
                <div className="flex items-center gap-1">
                  <Tag className="h-4 w-4" />
                  <span>{filteredAnnouncements.length} announcements</span>
                </div>
                <div className="flex items-center gap-1">
                  <Star className="h-4 w-4" />
                  <span>{savedIds.length} saved</span>
                </div>
              </div>
            </CardContent>
          </Card>
        </motion.div>

        {/* Announcements Grid */}
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
          </div>
        ) : filteredAnnouncements.length === 0 ? (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            className="text-center py-12"
          >
            <Megaphone className="h-16 w-16 mx-auto text-muted-foreground mb-4" />
            <h3 className="text-xl font-semibold mb-2">No announcements found</h3>
            <p className="text-muted-foreground">Try adjusting your filters or search term</p>
          </motion.div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <AnimatePresence mode="popLayout">
              {filteredAnnouncements.map((announcement, index) => {
                const isSaved = savedIds.includes(announcement.id);
                const categoryColor = getCategoryColor(announcement.category);
                
                const isCreator = currentUserId && Number(currentUserId) === announcement.createdByUserId;

                return (
                  <motion.div
                    key={announcement.id}
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    exit={{ opacity: 0, scale: 0.9 }}
                    transition={{ delay: index * 0.05 }}
                    layout
                  >
                    <Card className="group relative overflow-hidden border-2 hover:border-primary/50 transition-all h-full hover:shadow-xl">
                      <div className="absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>

                      <CardHeader className="relative">
                        <div className="flex items-start justify-between mb-3">
                          <Badge className={`${categoryColor} text-white border-0`}>
                            {announcement.category}
                          </Badge>

                          <div className="flex items-center gap-2">
                            {isProfessor && (
                              <motion.button
                                whileHover={{ scale: 1.1 }}
                                whileTap={{ scale: 0.9 }}
                                onClick={() => handleOpenForwardModal(announcement.id)}
                                className="p-2 rounded-lg hover:bg-blue-50 dark:hover:bg-blue-950/20 transition-colors"
                                title="Forward to group"
                              >
                                <Forward className="h-5 w-5 text-blue-500" />
                              </motion.button>
                            )}
                            
                            <motion.button
                              whileHover={{ scale: 1.1 }}
                              whileTap={{ scale: 0.9 }}
                              onClick={() => handleToggleBookmark(announcement.id)}
                              className="p-2 rounded-lg hover:bg-secondary transition-colors"
                            >
                              {isSaved ? (
                                <BookmarkCheck className="h-5 w-5 text-yellow-500 fill-yellow-500" />
                              ) : (
                                <BookmarkPlus className="h-5 w-5 text-muted-foreground" />
                              )}
                            </motion.button>
                            
                            {(isAdmin || isCreator) && (
                              <motion.button
                                whileHover={{ scale: 1.1 }}
                                whileTap={{ scale: 0.9 }}
                                onClick={() => handleDelete(announcement.id)}
                                className="p-2 rounded-lg hover:bg-red-50 dark:hover:bg-red-950/20 transition-colors"
                              >
                                <Trash2 className="h-5 w-5 text-red-500" />
                              </motion.button>
                            )}
                          </div>
                        </div>

                        <CardTitle className="text-xl line-clamp-2 group-hover:text-primary transition-colors">
                          {announcement.title}
                        </CardTitle>
                      </CardHeader>

                      <CardContent>
                        <p className="text-muted-foreground line-clamp-3 mb-4">
                          {announcement.content}
                        </p>

                        <div className="flex items-center gap-2 text-sm text-muted-foreground">
                          <Calendar className="h-4 w-4" />
                          <span>
                            {new Date(announcement.createdAt).toLocaleDateString('en-US', {
                              month: 'short',
                              day: 'numeric',
                              year: 'numeric',
                              hour: '2-digit',
                              minute: '2-digit',
                            })}
                          </span>
                        </div>
                      </CardContent>
                    </Card>
                  </motion.div>
                );
              })}
            </AnimatePresence>
          </div>
        )}

        {/* Forward Modal */}
        <AnimatePresence>
          {forwardModalOpen && (
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4"
              onClick={() => setForwardModalOpen(false)}
            >
              <motion.div
                initial={{ scale: 0.9, opacity: 0 }}
                animate={{ scale: 1, opacity: 1 }}
                exit={{ scale: 0.9, opacity: 0 }}
                onClick={(e) => e.stopPropagation()}
                className="bg-background rounded-xl shadow-2xl max-w-md w-full max-h-[80vh] overflow-hidden"
              >
                <div className="p-6 border-b flex items-center justify-between">
                  <h2 className="text-2xl font-bold">Forward to Group</h2>
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setForwardModalOpen(false)}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </div>
                
                <div className="p-6 overflow-y-auto max-h-[60vh]">
                  {professorGroups.length === 0 ? (
                    <p className="text-center text-muted-foreground py-8">
                      You don't have any groups yet
                    </p>
                  ) : (
                    <div className="space-y-3">
                      {professorGroups.map((group) => (
                        <motion.div
                          key={group.id}
                          whileHover={{ scale: 1.02 }}
                          whileTap={{ scale: 0.98 }}
                        >
                          <Card 
                            className="cursor-pointer hover:border-primary transition-colors"
                            onClick={() => handleForwardToGroup(group.id)}
                          >
                            <CardContent className="pt-6">
                              <div className="flex items-center justify-between">
                                <div>
                                  <h3 className="font-semibold">{group.name}</h3>
                                  <p className="text-sm text-muted-foreground">{group.subject}</p>
                                </div>
                                {forwardingToGroup === group.id ? (
                                  <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-primary"></div>
                                ) : (
                                  <Forward className="h-5 w-5 text-primary" />
                                )}
                              </div>
                            </CardContent>
                          </Card>
                        </motion.div>
                      ))}
                    </div>
                  )}
                </div>
              </motion.div>
            </motion.div>
          )}
        </AnimatePresence>
      </div>
    </Layout>
  );
};

export default Announcements;