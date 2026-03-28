import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Calendar, ArrowLeft, Sparkles, Save, FileText, Tag, MapPin, Clock } from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
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

interface EventFormData {
  title: string;
  description: string;
  category: string;
  date: string;
  location: string;
}

function EditEvent() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [formData, setFormData] = useState<EventFormData>({
    title: '',
    description: '',
    category: 'Academic',
    date: '',
    location: ''
  });

  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  useEffect(() => {
    const fetchEvent = async () => {
      try {
        const response = await fetch(`${API_BASE_URL}/event/${id}`);
        if (!response.ok) throw new Error('Could not find event');

        const data = await response.json();
        const formattedDate = data.date ? new Date(data.date).toISOString().slice(0, 16) : '';

        setFormData({
          title: data.title,
          description: data.description,
          category: data.category || 'Academic',
          date: formattedDate,
          location: data.location
        });
      } catch (err) {
        setMessage({ type: 'error', text: 'Error loading event data.' });
      } finally {
        setLoading(false);
      }
    };

    fetchEvent();
  }, [id]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleCategoryChange = (category: string) => {
    setFormData({
      ...formData,
      category
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setMessage(null);

    const token = localStorage.getItem('token');
    if (!token) {
      setMessage({ type: 'error', text: 'You are not authenticated!' });
      setSubmitting(false);
      return;
    }

    try {
      const response = await fetch(`${API_BASE_URL}/event/${id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify({ id: Number(id), ...formData }),
      });

      if (response.ok) {
        setMessage({ type: 'success', text: 'Event updated successfully!' });
        setTimeout(() => {
          navigate(`/event/${id}`);
        }, 1500);
      } else {
        const errData = await response.json().catch(() => null);
        setMessage({
          type: 'error',
          text: errData?.title || 'Update failed. Verify you are the organizer.'
        });
      }
    } catch (error) {
      console.error(error);
      setMessage({ type: 'error', text: 'Connection error. Please try again.' });
    } finally {
      setSubmitting(false);
    }
  };

  const selectedCategory = categories.find((c) => c.value === formData.category);

  if (loading) {
    return (
      <Layout>
        <div className="space-y-6">
          <Skeleton className="h-48 w-full rounded-2xl" />
          <Card>
            <CardHeader>
              <Skeleton className="h-6 w-48" />
            </CardHeader>
            <CardContent className="space-y-6">
              <Skeleton className="h-10 w-full" />
              <Skeleton className="h-10 w-full" />
              <Skeleton className="h-32 w-full" />
            </CardContent>
          </Card>
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
                    Edit Event
                    <Sparkles className="h-6 w-6 text-yellow-300" />
                  </h1>
                  <p className="text-white/80 mt-1">Update event information</p>
                </div>
              </div>
            </div>

            <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
              <Button
                onClick={() => navigate(`/event/${id}`)}
                className="bg-white text-purple-600 hover:bg-white/90 shadow-lg"
              >
                <ArrowLeft className="h-5 w-5 mr-2" />
                Back
              </Button>
            </motion.div>
          </div>
        </motion.div>

        {/* Form Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
        >
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <FileText className="h-5 w-5" />
                Event Details
              </CardTitle>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleSubmit} className="space-y-6">
                {/* Message */}
                {message && (
                  <motion.div
                    initial={{ opacity: 0, y: -10 }}
                    animate={{ opacity: 1, y: 0 }}
                    className={`p-4 rounded-lg border ${
                      message.type === 'success'
                        ? 'bg-green-50 dark:bg-green-950/20 text-green-800 dark:text-green-400 border-green-200 dark:border-green-900'
                        : 'bg-red-50 dark:bg-red-950/20 text-red-800 dark:text-red-400 border-red-200 dark:border-red-900'
                    }`}
                  >
                    {message.text}
                  </motion.div>
                )}

                {/* Title */}
                <div>
                  <label className="block text-sm font-medium mb-2">
                    Event Title <span className="text-red-500">*</span>
                  </label>
                  <Input
                    type="text"
                    name="title"
                    placeholder="Enter event title..."
                    value={formData.title}
                    onChange={handleChange}
                    required
                    className="w-full"
                  />
                </div>

                {/* Category */}
                <div>
                  <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                    <Tag className="h-4 w-4" />
                    Category <span className="text-red-500">*</span>
                  </label>
                  <div className="flex flex-wrap gap-2">
                    {categories.map((cat) => (
                      <motion.div key={cat.value} whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                        <Badge
                          onClick={() => handleCategoryChange(cat.value)}
                          className={`cursor-pointer transition-all ${
                            formData.category === cat.value
                              ? `${cat.color} text-white`
                              : 'bg-secondary text-secondary-foreground hover:bg-secondary/80'
                          }`}
                        >
                          {cat.label}
                        </Badge>
                      </motion.div>
                    ))}
                  </div>
                  {selectedCategory && (
                    <p className="text-sm text-muted-foreground mt-2">
                      Selected: <span className="font-medium">{selectedCategory.label}</span>
                    </p>
                  )}
                </div>

                {/* Date and Time */}
                <div>
                  <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                    <Clock className="h-4 w-4" />
                    Date and Time <span className="text-red-500">*</span>
                  </label>
                  <Input
                    type="datetime-local"
                    name="date"
                    value={formData.date}
                    onChange={handleChange}
                    required
                    className="w-full"
                  />
                </div>

                {/* Location */}
                <div>
                  <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                    <MapPin className="h-4 w-4" />
                    Location <span className="text-red-500">*</span>
                  </label>
                  <Input
                    type="text"
                    name="location"
                    placeholder="Enter event location..."
                    value={formData.location}
                    onChange={handleChange}
                    required
                    className="w-full"
                  />
                </div>

                {/* Description */}
                <div>
                  <label className="block text-sm font-medium mb-2">
                    Description <span className="text-red-500">*</span>
                  </label>
                  <textarea
                    name="description"
                    placeholder="Write event description here..."
                    value={formData.description}
                    onChange={handleChange}
                    required
                    rows={8}
                    className="w-full px-3 py-2 border border-input rounded-md bg-background focus:outline-none focus:ring-2 focus:ring-ring focus:border-transparent resize-y"
                  />
                  <p className="text-sm text-muted-foreground mt-1">
                    {formData.description.length} characters
                  </p>
                </div>

                {/* Submit Buttons */}
                <div className="flex gap-4">
                  <Button
                    type="submit"
                    disabled={submitting}
                    className="flex-1"
                  >
                    {submitting ? (
                      <>
                        <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                        Saving...
                      </>
                    ) : (
                      <>
                        <Save className="h-4 w-4 mr-2" />
                        Save Changes
                      </>
                    )}
                  </Button>
                  <Button
                    type="button"
                    variant="outline"
                    onClick={() => navigate(`/event/${id}`)}
                    disabled={submitting}
                  >
                    Cancel
                  </Button>
                </div>
              </form>
            </CardContent>
          </Card>
        </motion.div>
      </div>
    </Layout>
  );
}

export default EditEvent;