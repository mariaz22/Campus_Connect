import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import {
  MessageSquare,
  Calendar,
  User,
  Megaphone,
  Loader2,
  Trash2,
} from 'lucide-react';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Badge } from '../../components/ui/Badge';
import { Button } from '../../components/ui/Button';

interface GroupAnnouncement {
  id: number;
  announcementId: number;
  title: string;
  content: string;
  category: string;
  forwardedAt: string;
  forwardedByProfessorName: string;
  forwardedByProfessorId?: number;
}

interface GroupChatProps {
  groupId: number;
}

const GroupChat = ({ groupId }: GroupChatProps) => {
  const [announcements, setAnnouncements] = useState<GroupAnnouncement[]>([]);
  const [loading, setLoading] = useState(true);
  const [deletingId, setDeletingId] = useState<number | null>(null);

  const token = localStorage.getItem('token');
  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const isProfessor = user.role === 'Professor';

  useEffect(() => {
    loadGroupAnnouncements();
  }, [groupId]);

  const loadGroupAnnouncements = async () => {
    setLoading(true);
    try {
      const response = await fetch(`http://localhost:5099/api/group/${groupId}/announcements`, {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (response.ok) {
        const data = await response.json();
        setAnnouncements(data);
      }
    } catch (error) {
      console.error('Error loading group announcements:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteAnnouncement = async (groupAnnouncementId: number) => {
    if (!confirm('Are you sure you want to remove this announcement from the group?')) {
      return;
    }

    setDeletingId(groupAnnouncementId);
    try {
      const response = await fetch(
        `http://localhost:5099/api/group/${groupId}/announcements/${groupAnnouncementId}`,
        {
          method: 'DELETE',
          headers: {
            'Authorization': `Bearer ${token}`,
          },
        }
      );

      if (response.ok) {
        setAnnouncements(prev => prev.filter(a => a.id !== groupAnnouncementId));
      } else {
        const error = await response.json();
        alert(error.message || 'Failed to delete announcement');
      }
    } catch (error) {
      console.error('Error deleting announcement:', error);
      alert('An error occurred while deleting the announcement');
    } finally {
      setDeletingId(null);
    }
  };

  const getCategoryColor = (category: string) => {
    switch (category) {
      case 'Academic':
        return 'bg-blue-500';
      case 'Sports':
        return 'bg-green-500';
      case 'Events':
        return 'bg-purple-500';
      case 'General':
        return 'bg-orange-500';
      default:
        return 'bg-slate-500';
    }
  };

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: 0.7 }}
    >
      <Card>
        <CardHeader className="flex-shrink-0">
          <CardTitle className="flex items-center gap-2">
            <MessageSquare className="h-5 w-5" />
            Group Chat
            {announcements.length > 0 && (
              <Badge variant="secondary" className="ml-auto">
                {announcements.length}
              </Badge>
            )}
          </CardTitle>
        </CardHeader>
        
        <CardContent>
          {loading ? (
            <div className="flex items-center justify-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
            </div>
          ) : announcements.length === 0 ? (
            <div className="text-center py-12">
              <Megaphone className="h-12 w-12 mx-auto text-muted-foreground/50 mb-3" />
              <p className="text-muted-foreground text-sm">No announcements yet</p>
              <p className="text-xs text-muted-foreground mt-1">
                Professor announcements will appear here
              </p>
            </div>
          ) : (
            <div className="space-y-4">
              <AnimatePresence>
                {announcements.map((announcement, index) => (
                  <motion.div
                    key={announcement.id}
                    initial={{ opacity: 0, x: -20 }}
                    animate={{ opacity: 1, x: 0 }}
                    exit={{ opacity: 0, x: 20 }}
                    transition={{ delay: index * 0.05 }}
                  >
                    <Card className="border-l-4 border-l-primary hover:shadow-md transition-shadow">
                      <CardContent className="pt-4">
                        <div className="flex items-start justify-between mb-2">
                          <Badge className={`${getCategoryColor(announcement.category)} text-white text-xs`}>
                            {announcement.category}
                          </Badge>
                          <div className="flex items-center gap-2">
                            <span className="text-xs text-muted-foreground flex items-center gap-1">
                              <Calendar className="h-3 w-3" />
                              {formatDate(announcement.forwardedAt)}
                            </span>
                            {isProfessor && (
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => handleDeleteAnnouncement(announcement.id)}
                                disabled={deletingId === announcement.id}
                                className="h-6 w-6 p-0 hover:bg-red-100 hover:text-red-600"
                              >
                                {deletingId === announcement.id ? (
                                  <Loader2 className="h-3 w-3 animate-spin" />
                                ) : (
                                  <Trash2 className="h-3 w-3" />
                                )}
                              </Button>
                            )}
                          </div>
                        </div>
                        
                        <h4 className="font-semibold text-sm mb-2 line-clamp-2">
                          {announcement.title}
                        </h4>
                        
                        <p className="text-xs text-muted-foreground mb-3 line-clamp-3">
                          {announcement.content}
                        </p>
                        
                        <div className="flex items-center gap-1.5 text-xs text-muted-foreground">
                          <User className="h-3 w-3" />
                          <span>{announcement.forwardedByProfessorName}</span>
                        </div>
                      </CardContent>
                    </Card>
                  </motion.div>
                ))}
              </AnimatePresence>
            </div>
          )}
        </CardContent>
      </Card>
    </motion.div>
  );
};

export default GroupChat;
