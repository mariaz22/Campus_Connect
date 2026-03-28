import { useEffect, useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import {
  Calendar,
  Plus,
  MapPin,
  Users,
  Clock,
  Sparkles,
  Filter,
  CalendarDays,
  Tag,
} from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Input } from '../../components/ui/Input';

const API_BASE_URL = 'http://localhost:5099/api';

interface EventSummary {
  id: number;
  title: string;
  description?: string;
  category: string;
  date: string;
  location: string;
  participants: any[];
}

const categories = [
  { value: '', label: 'All Events', color: 'bg-slate-500' },
  { value: 'Academic', label: 'Academic', color: 'bg-blue-500' },
  { value: 'Sports', label: 'Sports', color: 'bg-green-500' },
  { value: 'Cultural', label: 'Cultural', color: 'bg-purple-500' },
  { value: 'Social', label: 'Social', color: 'bg-pink-500' },
  { value: 'Workshop', label: 'Workshop', color: 'bg-orange-500' },
];

function UpcomingEvents() {
  const navigate = useNavigate();
  const location = useLocation();

  // Extragem termenul de căutare din URL (?search=...)
  const queryParams = new URLSearchParams(location.search);
  const searchTerm = queryParams.get('search') || '';

  const [events, setEvents] = useState<EventSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('');
  const [localSearchTerm, setLocalSearchTerm] = useState(searchTerm);

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
  const checkProfessor = () => {
    const userString = localStorage.getItem('user');
    if (!userString) return false;
    try {
      const user = JSON.parse(userString);
      return user.role?.toLowerCase() === 'professor';
    } catch (e) {
      return false;
    }
  };

  const isAdmin = checkAdmin();
  const isProfessor = checkProfessor();

  // Funcția de fetch primește acum searchTerm ca parametru
  const fetchEvents = async (search: string) => {
    setLoading(true);
    setError(''); // Resetăm eroarea la fiecare căutare nouă
    try {
      // Dacă search există, adăugăm parametrul în URL, altfel chemăm endpoint-ul simplu
      const url = search 
        ? `${API_BASE_URL}/event/upcoming?search=${encodeURIComponent(search)}`
        : `${API_BASE_URL}/event/upcoming`;

      const response = await fetch(url);
      if (!response.ok) throw new Error('Nu s-au putut incarca evenimentele.');
      
      const data = await response.json();
      setEvents(data);
    } catch (err) {
      console.error(err);
      setError('Error connecting to server.');
    } finally {
      setLoading(false);
    }
  };

  // Acest useEffect "ascultă" de schimbările din URL
  // Când utilizatorul scrie în bara de search și apasă Enter, searchTerm se schimbă și declanșează fetch-ul
  useEffect(() => {
    fetchEvents(searchTerm);
  }, [searchTerm]);

  const filteredEvents = events.filter((event) => {
    const matchesSearch =
      event.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
      event.location.toLowerCase().includes(searchTerm.toLowerCase());
    const matchesCategory = categoryFilter === '' || event.category === categoryFilter;
    return matchesSearch && matchesCategory;
  });

  const getCategoryColor = (cat: string) => {
    const found = categories.find((c) => c.value === cat);
    return found?.color || 'bg-slate-500';
  };

  const getUpcomingEventsStats = () => {
    const now = new Date();
    const today = events.filter((e) => {
      const eventDate = new Date(e.date);
      return eventDate.toDateString() === now.toDateString();
    }).length;

    const thisWeek = events.filter((e) => {
      const eventDate = new Date(e.date);
      const weekFromNow = new Date(now);
      weekFromNow.setDate(weekFromNow.getDate() + 7);
      return eventDate >= now && eventDate <= weekFromNow;
    }).length;

    return { today, thisWeek };
  };

  const stats = getUpcomingEventsStats();

  if (error) {
    return (
      <Layout>
        <div className="text-center py-12">
          <Calendar className="h-16 w-16 mx-auto text-red-500 mb-4" />
          <h3 className="text-xl font-semibold mb-2 text-red-500">{error}</h3>
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
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-purple-500 via-pink-500 to-rose-500 p-8 text-white shadow-2xl"
        >
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>

          <div className="relative z-10 flex items-center justify-between">
            <div>
              <div className="flex items-center gap-3 mb-2">
                <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                  <Calendar className="h-8 w-8" />
                </div>
                <div>
                  <h1 className="text-4xl font-bold flex items-center gap-2">
                    Upcoming Events
                    <Sparkles className="h-6 w-6 text-yellow-300" />
                  </h1>
                  <p className="text-white/80 mt-1">Discover and join exciting campus events</p>
                </div>
              </div>
            </div>

            {(isAdmin || isProfessor) && (
              <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
              <Button
                onClick={() => navigate('/create-event')}
                className="bg-white text-purple-600 hover:bg-white/90 shadow-lg"
              >
                <Plus className="h-5 w-5 mr-2" />
                Create Event
              </Button>
              </motion.div>
            )}
          </div>
        </motion.div>

        {/* Stats & Filters */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
        >
          <Card>
            <CardContent className="pt-6">
              {/* Search and Category Filter */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
                <div className="relative">
                  <Input
                    type="text"
                    placeholder="Search events..."
                    value={localSearchTerm}
                    onChange={(e) => {
                      setLocalSearchTerm(e.target.value);
                      navigate(`?search=${encodeURIComponent(e.target.value)}`);
                    }}
                    className="pl-10"
                  />
                </div>

                <div className="flex items-center gap-2 overflow-x-auto pb-2">
                  <Filter className="h-5 w-5 text-muted-foreground flex-shrink-0" />
                  <div className="flex gap-2">
                    {categories.map((cat) => (
                      <motion.div key={cat.value} whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                        <Button
                          variant={categoryFilter === cat.value ? 'default' : 'outline'}
                          size="sm"
                          onClick={() => setCategoryFilter(cat.value)}
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
              <div className="flex items-center gap-6 text-sm text-muted-foreground">
                <div className="flex items-center gap-1">
                  <CalendarDays className="h-4 w-4" />
                  <span>{filteredEvents.length} events</span>
                </div>
                <div className="flex items-center gap-1">
                  <Clock className="h-4 w-4" />
                  <span>{stats.today} today</span>
                </div>
                <div className="flex items-center gap-1">
                  <Tag className="h-4 w-4" />
                  <span>{stats.thisWeek} this week</span>
                </div>
              </div>
            </CardContent>
          </Card>
        </motion.div>

        {/* Events Grid */}
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
          </div>
        ) : filteredEvents.length === 0 ? (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            className="text-center py-12"
          >
            <Calendar className="h-16 w-16 mx-auto text-muted-foreground mb-4" />
            <h3 className="text-xl font-semibold mb-2">No events found</h3>
            <p className="text-muted-foreground mb-4">
              {searchTerm || categoryFilter
                ? 'Try adjusting your filters or search term'
                : 'No upcoming events scheduled'}
            </p>
            {isAdmin && (
              <Button onClick={() => navigate('/create-event')}>
                <Plus className="h-5 w-5 mr-2" />
                Create First Event
              </Button>
            )}
          </motion.div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <AnimatePresence mode="popLayout">
              {filteredEvents.map((event, index) => {
                const eventDate = new Date(event.date);
                const categoryColor = getCategoryColor(event.category);
                const participantCount = event.participants?.length || 0;

                return (
                  <motion.div
                    key={event.id}
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    exit={{ opacity: 0, scale: 0.9 }}
                    transition={{ delay: index * 0.05 }}
                    layout
                  >
                    <Card
                      className="group relative overflow-hidden border-2 hover:border-primary/50 transition-all h-full cursor-pointer hover:shadow-xl"
                      onClick={() => navigate(`/event/${event.id}`)}
                    >
                      <div className="absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>

                      <CardHeader className="relative">
                        <div className="flex items-start justify-between mb-3">
                          <Badge className={`${categoryColor} text-white border-0`}>
                            {event.category}
                          </Badge>

                          <div className="flex items-center gap-1 text-sm text-muted-foreground">
                            <Users className="h-4 w-4" />
                            <span>{participantCount}</span>
                          </div>
                        </div>

                        <CardTitle className="text-xl line-clamp-2 group-hover:text-primary transition-colors">
                          {event.title}
                        </CardTitle>
                      </CardHeader>

                      <CardContent>
                        <div className="space-y-3">
                          <div className="flex items-center gap-2 text-sm text-muted-foreground">
                            <Calendar className="h-4 w-4 flex-shrink-0" />
                            <span>
                              {eventDate.toLocaleDateString('en-US', {
                                month: 'short',
                                day: 'numeric',
                                year: 'numeric',
                              })}
                            </span>
                          </div>

                          <div className="flex items-center gap-2 text-sm text-muted-foreground">
                            <Clock className="h-4 w-4 flex-shrink-0" />
                            <span>
                              {eventDate.toLocaleTimeString('en-US', {
                                hour: '2-digit',
                                minute: '2-digit',
                              })}
                            </span>
                          </div>

                          {event.location && (
                            <div className="flex items-center gap-2 text-sm text-muted-foreground">
                              <MapPin className="h-4 w-4 flex-shrink-0" />
                              <span className="line-clamp-1">{event.location}</span>
                            </div>
                          )}
                        </div>

                        <Button
                          className="w-full mt-4 group-hover:bg-primary group-hover:text-primary-foreground transition-colors"
                          variant="outline"
                        >
                          View Details
                        </Button>
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
}

export default UpcomingEvents;
