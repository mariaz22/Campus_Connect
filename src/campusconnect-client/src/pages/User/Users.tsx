import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import {
  Users as UsersIcon,
  Search,
  Trash2,
  Shield,
  ShieldAlert,
  ShieldCheck,
  Sparkles,
  GraduationCap
} from 'lucide-react';

// Importăm componentele UI (asigură-te că le ai în proiect)
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Badge } from '../../components/ui/Badge';
import { Avatar, AvatarFallback, AvatarImage } from '../../components/ui/Avatar';

import '../../index.css';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5099/api';

interface UserSummary {
  id: number;
  firstName: string;
  lastName: string;
  profilePictureUrl: string | null;
  studentId: string;
  role: string;
}

const Users: React.FC = () => {
  const [users, setUsers] = useState<UserSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [isCurrentUserAdmin, setIsCurrentUserAdmin] = useState(false);

  const location = useLocation();
  const navigate = useNavigate();
  const searchTerm = new URLSearchParams(location.search).get('search') || '';

  // 1. Verificare Admin Logat
  useEffect(() => {
    const userString = localStorage.getItem('user');
    if (userString) {
      try {
        const userObj = JSON.parse(userString);
        if (userObj.role === 'Admin' || userObj.role === 'admin' || userObj.isAdmin === true) {
          setIsCurrentUserAdmin(true);
        }
      } catch (e) {
        console.error("Error parsing user data", e);
      }
    }
  }, []);

  // 2. Fetch Users
  useEffect(() => {
    const fetchUsers = async () => {
      setLoading(true);
      try {
        const token = localStorage.getItem("token");
        const res = await fetch(`${API_BASE_URL}/user/search?search=${encodeURIComponent(searchTerm)}`, {
          headers: {
            "Authorization": `Bearer ${token}`
          }
        });
        if (res.ok) {
          const data = await res.json();
          setUsers(data);
        }
      } catch (err) {
        console.error("Error searching users:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, [location.search]);

  // Funcții de acțiune (Delete / Toggle)
  const handleDeleteUser = async (userId: number) => {
    if (!window.confirm("Are you sure you want to delete this user?")) return;

    try {
      const token = localStorage.getItem("token");
      const res = await fetch(`${API_BASE_URL}/user/delete/${userId}`, {
        method: 'DELETE',
        headers: { "Authorization": `Bearer ${token}` }
      });

      if (res.ok) {
        setUsers(prevUsers => prevUsers.filter(u => u.id !== userId));
      } else {
        alert("Error deleting user.");
      }
    } catch (err) {
      console.error(err);
    }
  };

  const handleToggleAdmin = async (userId: number, currentName: string) => {
    if (!window.confirm(`Change role for ${currentName}?`)) return;

    try {
      const token = localStorage.getItem("token");
      const res = await fetch(`${API_BASE_URL}/user/${userId}/toggle-admin`, {
        method: 'PATCH',
        headers: { "Authorization": `Bearer ${token}` }
      });

      if (res.ok) {
        const data = await res.json();
        setUsers(prevUsers => prevUsers.map(u => {
            if (u.id === userId) {
                return { ...u, role: data.newRole || (u.role === 'Admin' ? 'User' : 'Admin') };
            }
            return u;
        }));
      } else {
        alert("Error changing role.");
      }
    } catch (err) {
      console.error(err);
    }
  };

  // Helper pentru culorile badge-ului în stil Tailwind
  const getRoleBadgeClass = (role: string) => {
    if (role === 'Admin') return 'bg-red-100 text-red-700 border-red-200 dark:bg-red-900/30 dark:text-red-400 dark:border-red-800';
    if (role === 'Professor') return 'bg-purple-100 text-purple-700 border-purple-200 dark:bg-purple-900/30 dark:text-purple-400 dark:border-purple-800';
    return 'bg-blue-100 text-blue-700 border-blue-200 dark:bg-blue-900/30 dark:text-blue-400 dark:border-blue-800';
  };

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header Section - Stilizat ca la Announcements */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-indigo-600 via-purple-600 to-pink-600 p-8 text-white shadow-2xl"
        >
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>

          <div className="relative z-10 flex items-center justify-between">
            <div>
              <div className="flex items-center gap-3 mb-2">
                <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                  <UsersIcon className="h-8 w-8" />
                </div>
                <div>
                  <h1 className="text-4xl font-bold flex items-center gap-2">
                    User Management
                    <Sparkles className="h-6 w-6 text-yellow-300" />
                  </h1>
                  <p className="text-white/80 mt-1">Manage students, professors, and administrators</p>
                </div>
              </div>
            </div>
          </div>
        </motion.div>

        {/* Search Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.1 }}
        >
          <Card>
            <CardContent className="pt-6">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-muted-foreground" />
                <Input
                  type="text"
                  placeholder="Search users by name or email..."
                  defaultValue={searchTerm}
                  onChange={(e: React.ChangeEvent<HTMLInputElement>) => {
                    // Opțional: poți face debounce aici
                    if(e.target.value === "") navigate("/users");
                  }}
                  onKeyDown={(e: React.KeyboardEvent<HTMLInputElement>) => {
                    if (e.key === 'Enter') {
                      navigate(`/users?search=${encodeURIComponent(e.currentTarget.value)}`);
                    }
                  }}
                  className="pl-10 h-12 text-lg"
                />
              </div>
            </CardContent>
          </Card>
        </motion.div>

        {/* Loading State */}
        {loading ? (
          <div className="flex items-center justify-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
          </div>
        ) : users.length === 0 ? (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            className="text-center py-12"
          >
            <UsersIcon className="h-16 w-16 mx-auto text-muted-foreground mb-4" />
            <h3 className="text-xl font-semibold mb-2">No users found</h3>
            <p className="text-muted-foreground mb-4">Try adjusting your search term</p>
            {searchTerm && (
                <Button onClick={() => navigate("/users")} variant="outline">
                    View All Users
                </Button>
            )}
          </motion.div>
        ) : (
          /* Users Grid */
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            <AnimatePresence mode="popLayout">
              {users.map((user, index) => {
                const role = (user as any).role || (user as any).Role || 'User';
                const firstName = (user as any).firstName || (user as any).FirstName;
                const lastName = (user as any).lastName || (user as any).LastName;
                const isAdminRole = role === 'Admin';

                return (
                  <motion.div
                    key={user.id}
                    initial={{ opacity: 0, scale: 0.9 }}
                    animate={{ opacity: 1, scale: 1 }}
                    exit={{ opacity: 0, scale: 0.9 }}
                    transition={{ delay: index * 0.05 }}
                    layout
                  >
                    <Card className="group relative overflow-hidden border-2 hover:border-primary/50 transition-all h-full hover:shadow-xl flex flex-col">
                      <div className="absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>
                      
                      <CardHeader className="relative flex flex-col items-center pb-2">
                        <Avatar className="h-24 w-24 border-4 border-white shadow-lg mb-4">
                            <AvatarImage src={user.profilePictureUrl || ""} className="object-cover" />
                            <AvatarFallback className="bg-gradient-to-br from-blue-400 to-indigo-500 text-white text-2xl font-bold">
                                {firstName?.[0]}{lastName?.[0]}
                            </AvatarFallback>
                        </Avatar>
                        
                        <div className="text-center space-y-1">
                            <h3 className="text-xl font-bold text-gray-900 dark:text-white group-hover:text-primary transition-colors">
                                {firstName} {lastName}
                            </h3>
                            <div className="flex items-center justify-center gap-1 text-sm text-muted-foreground">
                                <GraduationCap className="h-4 w-4" />
                                <span>{user.studentId || "No ID"}</span>
                            </div>
                        </div>

                        <Badge className={`mt-3 ${getRoleBadgeClass(role)} border shadow-sm`}>
                            {isAdminRole ? <Shield className="h-3 w-3 mr-1" /> : <UsersIcon className="h-3 w-3 mr-1" />}
                            {role}
                        </Badge>
                      </CardHeader>

                      <CardContent className="relative flex-1 flex flex-col justify-end pt-2">
                        {isCurrentUserAdmin && (
                            <div className="grid grid-cols-2 gap-2 mt-4 pt-4 border-t border-gray-100 dark:border-gray-800">
                                <Button 
                                    size="sm" 
                                    variant="default"
                                    className={`w-full ${isAdminRole ? 'bg-orange-500 hover:bg-orange-600 text-white' : 'bg-blue-600 hover:bg-blue-700'}`}
                                    onClick={() => handleToggleAdmin(user.id, firstName)}
                                >
                                    {isAdminRole ? (
                                        <><ShieldAlert className="h-4 w-4 mr-2" /> Revoke</>
                                    ) : (
                                        <><ShieldCheck className="h-4 w-4 mr-2" /> Grant</>
                                    )}
                                </Button>
                                
                                <Button 
                                    size="sm" 
                                    variant="destructive"
                                    className="w-full"
                                    onClick={() => handleDeleteUser(user.id)}
                                >
                                    <Trash2 className="h-4 w-4 mr-2" />
                                    Delete
                                </Button>
                            </div>
                        )}
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

export default Users;