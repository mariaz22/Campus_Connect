import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { User, ArrowLeft, Sparkles, Save, Calendar, IdCard, Image } from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Skeleton } from '../../components/ui/Skeleton';

interface UserProfile {
  firstName: string;
  lastName: string;
  profilePictureUrl?: string;
  dateOfBirth: string;
  studentId?: string;
}

interface AuthData {
  token: string;
}

const API_BASE_URL = 'http://localhost:5099/api';
const GET_PROFILE_URL = `${API_BASE_URL}/user/details`;
const PUT_PROFILE_URL = `${API_BASE_URL}/user/update`;

const EditProfile = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<UserProfile>({
    firstName: '',
    lastName: '',
    profilePictureUrl: '',
    dateOfBirth: '',
    studentId: ''
  });
  const [loading, setLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);
  const [authData, setAuthData] = useState<AuthData | null>(null);

  const loadAuthData = (): AuthData | null => {
    if (typeof window === 'undefined') return null;
    try {
      const userJson = localStorage.getItem('user');
      const token = localStorage.getItem('token');

      if (userJson && token) {
        return { token: token };
      }
    } catch (e) {
      console.error('Error reading authentication data from localStorage:', e);
    }
    return null;
  };

  const formatIsoDateToInput = (isoDate: string | undefined | null): string => {
    if (!isoDate) return '';
    const datePart = isoDate.substring(0, 10);
    if (datePart.match(/^\d{4}-\d{2}-\d{2}$/)) {
      return datePart;
    }
    return '';
  };

  useEffect(() => {
    const data = loadAuthData();
    if (!data) {
      setLoading(false);
      setMessage({ type: 'error', text: 'You are not authenticated. Please log in.' });
      setTimeout(() => navigate('/login'), 2000);
      return;
    }
    setAuthData(data);

    const fetchCurrentProfile = async () => {
      try {
        const response = await fetch(GET_PROFILE_URL, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${data.token}`
          },
        });

        if (!response.ok) {
          if (response.status === 404) {
            setMessage({
              type: 'error',
              text: 'Error 404: GET endpoint not found.'
            });
          } else {
            const errorData = await response.json().catch(() => ({ message: `HTTP error ${response.status}` }));
            setMessage({
              type: 'error',
              text: errorData.message || 'Error loading current data.'
            });
          }
          setLoading(false);
          return;
        }

        const profileData = await response.json();

        if (profileData) {
          setFormData({
            firstName: profileData.firstName || '',
            lastName: profileData.lastName || '',
            profilePictureUrl: profileData.profilePictureUrl || '',
            dateOfBirth: formatIsoDateToInput(profileData.dateOfBirth),
            studentId: String(profileData.studentId || '')
          });
        }
      } catch (error) {
        setMessage({
          type: 'error',
          text: 'Server connection error at GET.'
        });
      } finally {
        setLoading(false);
      }
    };

    fetchCurrentProfile();
  }, [navigate]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!authData) {
      setMessage({ type: 'error', text: 'Session expired. Please log in again.' });
      return;
    }

    setIsSubmitting(true);
    setMessage(null);

    const payload = {
      ...formData,
      dateOfBirth: formData.dateOfBirth === '' ? null : formData.dateOfBirth,
      studentId: formData.studentId === '' ? null : formData.studentId,
    };

    try {
      const response = await fetch(PUT_PROFILE_URL, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${authData.token}`
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        if (response.status === 404) {
          setMessage({
            type: 'error',
            text: 'Error 404: PUT endpoint not found.'
          });
        } else {
          let errorData;
          try {
            errorData = await response.json();
          } catch (e) {
            errorData = { message: `HTTP error ${response.status}. Non-JSON response.` };
          }
          const modelStateErrors = errorData?.errors
            && typeof errorData.errors === 'object'
            && !Array.isArray(errorData.errors)
            ? Object.values(errorData.errors).flat().join(', ')
            : null;
          const arrayErrors = Array.isArray(errorData?.errors)
            ? errorData.errors.join(', ')
            : null;

          setMessage({
            type: 'error',
            text: modelStateErrors || arrayErrors || errorData.message || 'Unknown error editing profile.'
          });
        }
        return;
      }

      const data = await response.json();

      if (data.message) {
        setMessage({
          type: 'success',
          text: data.message || 'Profile successfully updated. Redirecting...'
        });

        const localUserData = localStorage.getItem('user');
        if (localUserData) {
          const updatedUser = { ...JSON.parse(localUserData), ...formData };
          localStorage.setItem('user', JSON.stringify(updatedUser));
        }

        setTimeout(() => {
          navigate('/profile');
        }, 1500);
      } else {
        setMessage({
          type: 'error',
          text: data.message || data.errors?.join(', ') || 'Error editing profile.'
        });
      }
    } catch (error) {
      const errMsg = error instanceof Error ? error.message : String(error);
      setMessage({
        type: 'error',
        text: `Server connection error: ${errMsg}`
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

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
              <Skeleton className="h-10 w-full" />
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
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-indigo-500 via-purple-500 to-pink-500 p-8 text-white shadow-2xl"
        >
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>

          <div className="relative z-10 flex items-center justify-between">
            <div>
              <div className="flex items-center gap-3 mb-2">
                <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                  <User className="h-8 w-8" />
                </div>
                <div>
                  <h1 className="text-4xl font-bold flex items-center gap-2">
                    Edit Profile
                    <Sparkles className="h-6 w-6 text-yellow-300" />
                  </h1>
                  <p className="text-white/80 mt-1">Update your personal information</p>
                </div>
              </div>
            </div>

            <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
              <Button
                onClick={() => navigate('/profile')}
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
              <CardTitle>Profile Information</CardTitle>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleSubmit} className="space-y-6">
                {/* Message */}
                {message && (
                  <motion.div
                    initial={{ opacity: 0, y: -10 }}
                    animate={{ opacity: 1, y: 0 }}
                    className={`p-4 rounded-lg border ${message.type === 'success'
                        ? 'bg-green-50 dark:bg-green-950/20 text-green-800 dark:text-green-400 border-green-200 dark:border-green-900'
                        : 'bg-red-50 dark:bg-red-950/20 text-red-800 dark:text-red-400 border-red-200 dark:border-red-900'
                      }`}
                  >
                    {message.text}
                  </motion.div>
                )}

                {/* First Name */}
                <div>
                  <label className="block text-sm font-medium mb-2">
                    First Name <span className="text-red-500">*</span>
                  </label>
                  <Input
                    type="text"
                    name="firstName"
                    placeholder="Enter your first name..."
                    value={formData.firstName}
                    onChange={handleChange}
                    required
                    className="w-full"
                  />
                </div>

                {/* Last Name */}
                <div>
                  <label className="block text-sm font-medium mb-2">
                    Last Name <span className="text-red-500">*</span>
                  </label>
                  <Input
                    type="text"
                    name="lastName"
                    placeholder="Enter your last name..."
                    value={formData.lastName}
                    onChange={handleChange}
                    required
                    className="w-full"
                  />
                </div>

                {/* Date of Birth */}
                <div>
                  <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                    <Calendar className="h-4 w-4" />
                    Date of Birth
                  </label>
                  <Input
                    type="date"
                    name="dateOfBirth"
                    value={formData.dateOfBirth || ''}
                    onChange={handleChange}
                    className="w-full"
                  />
                </div>

                {/* Student ID */}
                <div>
                  <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                    <IdCard className="h-4 w-4" />
                    Student ID
                  </label>
                  <Input
                    type="text"
                    name="studentId"
                    placeholder="Enter your student ID..."
                    value={formData.studentId || ''}
                    onChange={handleChange}
                    className="w-full"
                  />
                </div>

                {/* Profile Picture URL */}
                <div>
                  <label className="block text-sm font-medium mb-2 flex items-center gap-2">
                    <Image className="h-4 w-4" />
                    Profile Picture URL
                  </label>
                  <Input
                    type="text"
                    name="profilePictureUrl"
                    placeholder="Enter profile picture URL..."
                    value={formData.profilePictureUrl || ''}
                    onChange={handleChange}
                    className="w-full"
                  />
                </div>

                {/* Submit Buttons */}
                <div className="flex gap-4">
                  <Button
                    type="submit"
                    disabled={isSubmitting}
                    className="flex-1"
                  >
                    {isSubmitting ? (
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
                    onClick={() => navigate('/profile')}
                    disabled={isSubmitting}
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
};

export default EditProfile;
