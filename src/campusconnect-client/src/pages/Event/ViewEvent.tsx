import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import {
  Calendar,
  ArrowLeft,
  Sparkles,
  MapPin,
  Clock,
  Users,
  Edit,
  Trash2,
  UserPlus,
  UserMinus,
  BookmarkPlus,
  BookmarkCheck,
  Tag,
  Shield,
} from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Skeleton } from '../../components/ui/Skeleton'; 

const API_BASE_URL = 'http://localhost:5099/api';

const categories = [
  { value: 'Academic', label: 'Academic', color: 'bg-blue-500' },
  { value: 'Sports', label: 'Sports', color: 'bg-green-500' },
  { value: 'Cultural', label: 'Cultural', color: 'bg-purple-500' },
  { value: 'Social', label: 'Social', color: 'bg-pink-500' },
  { value: 'Workshop', label: 'Workshop', color: 'bg-orange-500' },
];

interface CurrentUser {
  id?: number;
  userId?: number;
  firstName: string;
}

interface EventData {
  id: number;
  title: string;
  description: string;
  category: string;
  date: string;
  location: string;
  organizerId: number;
  participants: any[];
}

function ViewEvent() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [event, setEvent] = useState<EventData | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);
  const [isJoined, setIsJoined] = useState(false);
  const [isSaved, setIsSaved] = useState(false);

  useEffect(() => {
    const userString = localStorage.getItem('user');
    if (userString) {
      const userObj = JSON.parse(userString);
      setCurrentUser(userObj);
    }
    fetchEvent();
  }, [id]);

  const fetchEvent = async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/event/${id}`);
      if (!response.ok) throw new Error('Event not found');
      
      const data = await response.json();
      setEvent(data);

      if (localStorage.getItem('user')) {
         const userObj = JSON.parse(localStorage.getItem('user') || '{}');
         const myId = userObj.userId || userObj.id;
         const joined = data.participants?.some((p: any) => p.userId === myId);
         setIsJoined(!!joined);
      }

      // Check if event is saved
      await checkIfSaved();
    } catch (err) {
      setError('Could not load the event.');
    } finally {
      setLoading(false);
    }
  };

  const checkIfSaved = async () => {
    const token = localStorage.getItem('token');
    if (!token) return;

    try {
      const response = await fetch(`${API_BASE_URL}/event/saved`, {
        headers: { 'Authorization': `Bearer ${token}` }
      });
      
      if (response.ok) {
        const savedEvents = await response.json();
        const saved = savedEvents.some((e: any) => e.id === Number(id));
        setIsSaved(saved);
      }
    } catch (error) {
      console.error('Error checking saved status:', error);
    }
  };

  const handleJoinToggle = async () => {
    const token = localStorage.getItem('token');
    if (!token) {
      alert("You must be authenticated.");
      return;
    }
    const endpoint = isJoined ? `${id}/leave` : `${id}/join`;
    const method = isJoined ? 'DELETE' : 'POST';

    try {
      const response = await fetch(`${API_BASE_URL}/event/${endpoint}`, {
        method: method,
        headers: { 'Authorization': `Bearer ${token}` }
      });

      if (response.ok) {
        setIsJoined(!isJoined);
        navigate('/events');
      } else {
        alert("Error processing request.");
      }
    } catch (error) {
      console.error(error);
    }
  };

  const handleDelete = async () => {
    if (!window.confirm("Are you sure you want to delete this event?")) return;
    const token = localStorage.getItem('token');
    try {
      const response = await fetch(`${API_BASE_URL}/event/${id}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
      });
      if (response.ok) {
        navigate('/events'); 
      }
    } catch (error) {
      alert("Error deleting event");
    }
  };

  const handleSaveToggle = async () => {
    const token = localStorage.getItem('token');
    if (!token) {
      alert("You must be authenticated.");
      return;
    }

    const endpoint = isSaved ? `${id}/unsave` : `${id}/save`;
    const method = isSaved ? 'DELETE' : 'POST';

    try {
      const response = await fetch(`${API_BASE_URL}/event/${endpoint}`, {
        method: method,
        headers: { 'Authorization': `Bearer ${token}` }
      });

      if (response.ok) {
        setIsSaved(!isSaved);
        alert(isSaved ? 'Event removed from saved!' : 'Event saved successfully!');
      } else {
        const errorData = await response.json();
        alert(errorData.message || "Error processing request.");
      }
    } catch (error) {
      console.error(error);
      alert("Error processing request.");
    }
  };
  const checkAdmin = () => {
    const userString = localStorage.getItem('user');
    if (!userString) return false;
    try {
      const user = JSON.parse(userString);
      return user.role?.toLowerCase() === 'admin' || user.isAdmin === true;
    } catch (e) {
      return false;
    }
  };
  const currentUserId = currentUser?.userId || currentUser?.id;
  const isOrganizer = currentUserId && String(currentUserId) === String(event?.organizerId);
  const isAdmin = checkAdmin();
  const categoryColor = categories.find((c) => c.value === event?.category)?.color || 'bg-slate-500';

  if (loading) {
    return (
      <Layout>
        <div className="space-y-6">
          <Skeleton className="h-48 w-full rounded-2xl" />
          <Card>
            <CardHeader>
              <Skeleton className="h-8 w-3/4" />
            </CardHeader>
            <CardContent className="space-y-4">
              <Skeleton className="h-6 w-1/2" />
              <Skeleton className="h-32 w-full" />
            </CardContent>
          </Card>
        </div>
      </Layout>
    );
  }

  if (error || !event) {
    return (
      <Layout>
        <div className="text-center py-12">
          <Shield className="h-16 w-16 mx-auto text-red-500 mb-4" />
          <h3 className="text-xl font-semibold mb-2 text-red-500">Error</h3>
          <p className="text-muted-foreground mb-4">{error || 'Event not found'}</p>
          <Button onClick={() => navigate('/events')}>
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back to Events
          </Button>
        </div>
      </Layout>
    );
  }

  const eventDate = new Date(event.date);

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-purple-500 via-pink-500 to-rose-500 p-8 text-white shadow-2xl"
        >
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>

          <div className="relative z-10 flex items-center justify-between">
            <div>
              <div className="flex items-center gap-3 mb-4">
                <Badge className={`${categoryColor} text-white border-0`}>{event.category}</Badge>
              </div>
              <div className="flex items-center gap-3 mb-2">
                <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                  <Calendar className="h-8 w-8" />
                </div>
                <div>
                  <h1 className="text-4xl font-bold flex items-center gap-2">
                    {event.title}
                    <Sparkles className="h-6 w-6 text-yellow-300" />
                  </h1>
                  <p className="text-white/80 mt-1">Event Details</p>
                </div>
              </div>
            </div>

            <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
              <Button
                onClick={() => navigate('/events')}
                className="bg-white text-purple-600 hover:bg-white/90 shadow-lg"
              >
                <ArrowLeft className="h-5 w-5 mr-2" />
                Back
              </Button>
            </motion.div>
          </div>
        </motion.div>

        {/* Event Info Cards */}
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
                    <Clock className="h-6 w-6 text-blue-600" />
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Date & Time</p>
                    <p className="font-semibold">
                      {eventDate.toLocaleDateString('en-US', {
                        month: 'short',
                        day: 'numeric',
                        year: 'numeric',
                      })}
                    </p>
                    <p className="text-sm">
                      {eventDate.toLocaleTimeString('en-US', {
                        hour: '2-digit',
                        minute: '2-digit',
                      })}
                    </p>
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
                  <div className="p-3 rounded-xl bg-green-100 dark:bg-green-900/20">
                    <MapPin className="h-6 w-6 text-green-600" />
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Location</p>
                    <p className="font-semibold">{event.location}</p>
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
                  <div className="p-3 rounded-xl bg-purple-100 dark:bg-purple-900/20">
                    <Users className="h-6 w-6 text-purple-600" />
                  </div>
                  <div>
                    <p className="text-sm text-muted-foreground">Participants</p>
                    <p className="text-2xl font-bold">{event.participants?.length || 0}</p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </motion.div>
        </div>

        {/* Description */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.4 }}
        >
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Tag className="h-5 w-5" />
                Description
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-muted-foreground whitespace-pre-wrap">{event.description}</p>
            </CardContent>
          </Card>
        </motion.div>

        {/* Actions */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.5 }}
        >
          <Card>
            <CardHeader>
              <CardTitle>Actions</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex flex-wrap gap-3">
                {/* Join/Leave Button */}
                <Button
                  onClick={handleJoinToggle}
                  variant={isJoined ? 'outline' : 'default'}
                  className={isJoined ? 'hover:bg-red-50 hover:text-red-600 hover:border-red-200' : ''}
                >
                  {isJoined ? (
                    <>
                      <UserMinus className="h-4 w-4 mr-2" />
                      Leave Event
                    </>
                  ) : (
                    <>
                      <UserPlus className="h-4 w-4 mr-2" />
                      Join Event
                    </>
                  )}
                </Button>

                {/* Save/Unsave Button */}
                <Button
                  onClick={handleSaveToggle}
                  variant="outline"
                  className={
                    isSaved
                      ? 'bg-yellow-50 text-yellow-700 border-yellow-200 hover:bg-yellow-100 dark:bg-yellow-900/20 dark:text-yellow-400 dark:border-yellow-900'
                      : ''
                  }
                >
                  {isSaved ? (
                    <>
                      <BookmarkCheck className="h-4 w-4 mr-2" />
                      Saved
                    </>
                  ) : (
                    <>
                      <BookmarkPlus className="h-4 w-4 mr-2" />
                      Save Event
                    </>
                  )}
                </Button>

                {/* Organizer Actions */}
                {(isOrganizer || isAdmin) && (
                  <>
                    <Button onClick={() => navigate(`/edit-event/${event.id}`)} variant="outline">
                      <Edit className="h-4 w-4 mr-2" />
                      Edit
                    </Button>

                    <Button
                      onClick={handleDelete}
                      variant="outline"
                      className="hover:bg-red-50 hover:text-red-600 hover:border-red-200 dark:hover:bg-red-950/20"
                    >
                      <Trash2 className="h-4 w-4 mr-2" />
                      Delete
                    </Button>
                  </>
                )}
              </div>

              {isOrganizer && (
                <div className="mt-4 p-3 rounded-lg bg-orange-50 dark:bg-orange-900/20 border border-orange-200 dark:border-orange-900">
                  <p className="text-sm text-orange-800 dark:text-orange-400 flex items-center gap-2">
                    <Shield className="h-4 w-4" />
                    You are the organizer of this event
                  </p>
                </div>
              )}
            </CardContent>
          </Card>
        </motion.div>
      </div>
    </Layout>
  );
}

export default ViewEvent;