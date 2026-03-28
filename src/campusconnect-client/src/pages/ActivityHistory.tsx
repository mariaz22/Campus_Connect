import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import {
  Clock,
  Calendar,
  Users,
  CheckSquare,
  Megaphone,
  UserCog,
  Trophy,
  ArrowLeft,
  Filter,
} from 'lucide-react';
import { Layout } from '../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Button } from '../components/ui/Button';
import { Badge } from '../components/ui/Badge';
import activityApi from '../services/activityApi';
import { formatActivity, type FormattedActivity } from '../utils/activityFormatter';

function ActivityHistory() {
  const navigate = useNavigate();
  const [activities, setActivities] = useState<FormattedActivity[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<string>('all');

  useEffect(() => {
    const fetchActivities = async () => {
      setLoading(true);
      try {
        const data = await activityApi.getAllActivities();
        setActivities(data.map(formatActivity));
      } catch (error) {
        console.error('Failed to fetch activities', error);
      } finally {
        setLoading(false);
      }
    };
    fetchActivities();
  }, []);

  const filteredActivities = filter === 'all'
    ? activities
    : activities.filter(activity => activity.type === filter);

  const getIconForType = (type: FormattedActivity['type']) => {
    switch (type) {
      case 'task':
        return <CheckSquare className="h-5 w-5" />;
      case 'event':
        return <Calendar className="h-5 w-5" />;
      case 'group':
        return <Users className="h-5 w-5" />;
      case 'announcement':
        return <Megaphone className="h-5 w-5" />;
      case 'profile':
        return <UserCog className="h-5 w-5" />;
      case 'achievement':
        return <Trophy className="h-5 w-5" />;
      default:
        return <Clock className="h-5 w-5" />;
    }
  };

  const filterOptions = [
    { value: 'all', label: 'All Activities' },
    { value: 'task', label: 'Tasks' },
    { value: 'event', label: 'Events' },
    { value: 'group', label: 'Groups' },
    { value: 'announcement', label: 'Announcements' },
    { value: 'profile', label: 'Profile' },
    { value: 'achievement', label: 'Achievements' },
  ];

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.5 }}
        >
          <div className="flex items-center gap-4 mb-6">
            <Button
              variant="ghost"
              size="sm"
              onClick={() => navigate('/')}
            >
              <ArrowLeft className="h-4 w-4 mr-2" />
              Back to Dashboard
            </Button>
          </div>

          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-4xl font-bold flex items-center gap-3">
                <Clock className="h-10 w-10 text-primary" />
                Activity History
              </h1>
              <p className="text-muted-foreground mt-2">
                View all your recent activities and actions
              </p>
            </div>
          </div>
        </motion.div>

        {/* Filter Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
        >
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Filter className="h-5 w-5" />
                Filter Activities
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="flex flex-wrap gap-2">
                {filterOptions.map((option) => (
                  <Badge
                    key={option.value}
                    variant={filter === option.value ? 'default' : 'secondary'}
                    className="cursor-pointer px-4 py-2"
                    onClick={() => setFilter(option.value)}
                  >
                    {option.label}
                  </Badge>
                ))}
              </div>
            </CardContent>
          </Card>
        </motion.div>

        {/* Activities List */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.2 }}
        >
          <Card>
            <CardHeader>
              <CardTitle>
                {filter === 'all' ? 'All Activities' : `${filterOptions.find(o => o.value === filter)?.label}`}
                <span className="ml-2 text-sm font-normal text-muted-foreground">
                  ({filteredActivities.length} {filteredActivities.length === 1 ? 'activity' : 'activities'})
                </span>
              </CardTitle>
            </CardHeader>
            <CardContent>
              {loading ? (
                <div className="text-center py-12">
                  <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto"></div>
                  <p className="text-muted-foreground mt-4">Loading activities...</p>
                </div>
              ) : filteredActivities.length === 0 ? (
                <div className="text-center py-12 text-muted-foreground">
                  <Clock className="h-16 w-16 mx-auto mb-4 opacity-30" />
                  <p className="text-lg font-medium">No activities found</p>
                  <p className="text-sm mt-2">
                    {filter === 'all'
                      ? 'Your activities will appear here as you interact with the platform'
                      : `No ${filterOptions.find(o => o.value === filter)?.label.toLowerCase()} activities yet`}
                  </p>
                </div>
              ) : (
                <div className="space-y-3">
                  {filteredActivities.map((activity, index) => (
                    <motion.div
                      key={index}
                      initial={{ opacity: 0, x: -20 }}
                      animate={{ opacity: 1, x: 0 }}
                      transition={{ delay: 0.05 * index }}
                      className="flex items-start gap-4 p-4 rounded-lg border border-border hover:bg-secondary/50 transition-colors cursor-pointer"
                      onClick={() => {
                        if (activity.type === 'group' && activity.entityId) {
                          navigate(`/groups/${activity.entityId}`);
                        } else if (activity.type === 'event' && activity.entityId) {
                          navigate(`/events/${activity.entityId}`);
                        } else if (activity.type === 'announcement') {
                          navigate(`/announcements`);
                        } else if (activity.type === 'task') {
                          navigate(`/my-tasks`);
                        }
                      }}
                    >
                      <div className="p-3 rounded-lg bg-primary/10 text-primary">
                        {getIconForType(activity.type)}
                      </div>
                      <div className="flex-1">
                        <p className="font-medium text-lg">{activity.title}</p>
                        <p className="text-sm text-muted-foreground mt-1">{activity.time}</p>
                      </div>
                      <Badge variant="secondary" className="capitalize">
                        {activity.type}
                      </Badge>
                    </motion.div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>
        </motion.div>
      </div>
    </Layout>
  );
}

export default ActivityHistory;
