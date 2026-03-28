import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { useNavigate } from 'react-router-dom';
import { 
  Bell, 
  Check, 
  CheckCheck, 
  Clock, 
  ArrowLeft, 
  Inbox,
  CheckCircle, 
  XCircle      
} from 'lucide-react';
import { Layout } from '../components/Layout';
import { Card, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';

import { roomBookingApi } from '../services/roomBookingApi'; 

const API_BASE_URL = 'http://localhost:5099/api';

interface Notification {
  id: number;
  message: string;
  relatedEntityId: number;   
  relatedEntityType: string; 
  createdAt: string;
  isRead: boolean;
}

const Notifications = () => {
  const navigate = useNavigate();
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<'all' | 'unread'>('all');
  const [processingId, setProcessingId] = useState<number | null>(null);

  const token = localStorage.getItem('token');

  const fetchNotifications = async () => {
    setLoading(true);
    try {
      const res = await fetch(`${API_BASE_URL}/notification?onlyUnread=false`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (res.ok) {
        const data = await res.json();
        setNotifications(data);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (token) fetchNotifications();
  }, [token]);

  const handleMarkAsRead = async (id: number) => {
    setNotifications(prev => prev.map(n => n.id === id ? { ...n, isRead: true } : n));
    try {
      await fetch(`${API_BASE_URL}/notification/${id}/read`, {
        method: 'PUT',
        headers: { Authorization: `Bearer ${token}` }
      });
    } catch (error) {
      console.error(error);
    }
  };

  const handleMarkAllRead = async () => {
    setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
    try {
      await fetch(`${API_BASE_URL}/notification/read-all`, {
        method: 'PUT',
        headers: { Authorization: `Bearer ${token}` }
      });
    } catch (error) {
      console.error(error);
    }
  };

  const handleBookingAction = async (notification: Notification, action: 'approve' | 'reject') => {
    if (!notification.relatedEntityId) return;

    setProcessingId(notification.id); 

    try {
      const bookingId = notification.relatedEntityId;

      if (action === 'approve') {
        await roomBookingApi.approveRequest(bookingId);
      } else {
        const reason = window.prompt("The reason for rejection (optional):");
        
        if (reason === null) {
            setProcessingId(null);
            return; 
        }

        await roomBookingApi.rejectRequest(bookingId, reason);
      }

      await handleMarkAsRead(notification.id);

      setNotifications(prev => prev.map(n => 
        n.id === notification.id 
          ? { ...n, message: `Request ${action === 'approve' ? 'approved' : 'rejected'} successfully.` } 
          : n
      ));

    } catch (err) {
      alert(`Error: ${err instanceof Error ? err.message : 'Action failed'}`);
    } finally {
      setProcessingId(null); 
    }
  };

  const displayedNotifications = filter === 'all' 
    ? notifications 
    : notifications.filter(n => !n.isRead);

  const unreadCount = notifications.filter(n => !n.isRead).length;

  return (
    <Layout>
      <div className="max-w-4xl mx-auto space-y-6">
        
        <div className="flex items-center gap-4 mb-8">
          <Button variant="ghost" onClick={() => navigate(-1)} className="p-2">
            <ArrowLeft className="h-6 w-6" />
          </Button>
          <div>
            <h1 className="text-3xl font-bold flex items-center gap-3">
              Notifications
              <Badge className="bg-blue-600 text-white text-lg px-3 py-1">{unreadCount}</Badge>
            </h1>
            <p className="text-muted-foreground">Manage your notifications and requests</p>
          </div>
        </div>

        <Card>
          <CardContent className="p-4 flex flex-col md:flex-row justify-between items-center gap-4">
            <div className="flex bg-muted p-1 rounded-lg">
              <button
                onClick={() => setFilter('all')}
                className={`px-4 py-2 rounded-md text-sm font-medium transition-all ${filter === 'all' ? 'bg-white dark:bg-zinc-800 shadow-sm' : 'text-muted-foreground hover:text-foreground'}`}
              >
                All
              </button>
              <button
                onClick={() => setFilter('unread')}
                className={`px-4 py-2 rounded-md text-sm font-medium transition-all ${filter === 'unread' ? 'bg-white dark:bg-zinc-800 shadow-sm' : 'text-muted-foreground hover:text-foreground'}`}
              >
                Unread
              </button>
            </div>

            {unreadCount > 1 && (
              <Button onClick={handleMarkAllRead} variant="outline" className="text-blue-600 border-blue-200 hover:bg-blue-50">
                <CheckCheck className="mr-2 h-4 w-4" /> Mark all as read
              </Button>
            )}
          </CardContent>
        </Card>

        <div className="space-y-3">
          {loading ? (
             <div className="text-center py-10">
               <div className="animate-spin h-8 w-8 border-2 border-blue-500 border-t-transparent rounded-full mx-auto"></div>
             </div>
          ) : displayedNotifications.length === 0 ? (
            <div className="text-center py-16 text-muted-foreground bg-muted/30 rounded-2xl border-2 border-dashed">
              <Inbox className="h-12 w-12 mx-auto mb-3 opacity-20" />
              <p>There are no notifications here.</p>
            </div>
          ) : (
            <AnimatePresence>
              {displayedNotifications.map((notification) => (
                <motion.div
                  key={notification.id}
                  initial={{ opacity: 0, y: 10 }}
                  animate={{ opacity: 1, y: 0 }}
                  exit={{ opacity: 0, height: 0 }}
                  layout
                >
                  <div 
                    className={`
                      relative group p-4 rounded-xl border transition-all duration-200
                      ${!notification.isRead 
                        ? 'bg-white border-blue-200 shadow-md dark:bg-zinc-900 dark:border-blue-900' 
                        : 'bg-gray-50 border-gray-100 text-gray-500 dark:bg-zinc-950 dark:border-zinc-800'
                      }
                    `}
                  >
                    <div className="flex gap-4 items-start">
                      <div className={`
                        mt-1 p-2 rounded-full flex-shrink-0
                        ${!notification.isRead ? 'bg-blue-100 text-blue-600' : 'bg-gray-200 text-gray-400'}
                      `}>
                        <Bell className="h-5 w-5" />
                      </div>
                      
                      <div className="flex-1">
                        <p className={`text-base ${!notification.isRead ? 'font-semibold text-foreground' : 'font-medium'}`}>
                          {notification.message}
                        </p>
                        <div className="flex items-center gap-2 mt-1 text-xs text-muted-foreground">
                          <Clock className="h-3 w-3" />
                          {new Date(notification.createdAt).toLocaleString('en-US')}
                        </div>

                        {notification.relatedEntityType === 'RoomBookingRequest' && !notification.isRead && (
                          <div className="mt-4 flex flex-wrap gap-3">
                            <Button 
                              size="sm" 
                              className="bg-green-600 hover:bg-green-700 text-white gap-2"
                              onClick={() => handleBookingAction(notification, 'approve')}
                              disabled={processingId === notification.id}
                            >
                              {processingId === notification.id ? (
                                <span className="animate-spin h-4 w-4 border-2 border-white border-t-transparent rounded-full"/>
                              ) : (
                                <CheckCircle className="h-4 w-4" />
                              )}
                              Accept
                            </Button>
                            
                            <Button 
                              size="sm" 
                              variant="destructive" 
                              className="gap-2"
                              onClick={() => handleBookingAction(notification, 'reject')}
                              disabled={processingId === notification.id}
                            >
                              <XCircle className="h-4 w-4" />
                              Decline
                            </Button>
                          </div>
                        )}
                      </div>

                      {!notification.isRead && (
                        <div className="flex flex-col gap-2 opacity-100 md:opacity-0 group-hover:opacity-100 transition-opacity">
                          <Button 
                            size="sm" 
                            variant="ghost" 
                            title="Mark as read"
                            className="h-8 w-8 p-0 hover:bg-blue-100 hover:text-blue-600"
                            onClick={() => handleMarkAsRead(notification.id)}
                          >
                            <Check className="h-4 w-4" />
                          </Button>
                        </div>
                      )}
                    </div>
                  </div>
                </motion.div>
              ))}
            </AnimatePresence>
          )}
        </div>
      </div>
    </Layout>
  );
};

export default Notifications;