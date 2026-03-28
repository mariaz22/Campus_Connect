import { Link, useNavigate, useLocation } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import {
  Home,
  Megaphone,
  Calendar as CalendarIcon,
  Users,
  CheckSquare,
  User,
  LogOut,
  Menu,
  X,
  MapPin,
  BookOpen,
  GraduationCap,
  FileText
} from 'lucide-react';
import { ThemeToggle } from './ThemeToggle';
import { Button } from './ui/Button';
import { Avatar, AvatarFallback } from './ui/Avatar';
import { useState, useEffect } from 'react';

interface LayoutProps {
  children: React.ReactNode;
}

export function Layout({ children }: LayoutProps) {
  const navigate = useNavigate();
  const location = useLocation();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);
  const [scrolled, setScrolled] = useState(false);

  const token = localStorage.getItem('token');
  const user = JSON.parse(localStorage.getItem('user') || '{}');

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 10);
    };
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  const handleLogout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    navigate('/login');
  };

  const navItems = [
    { name: 'Dashboard', path: '/dashboard', icon: Home },
    { name: 'Announcements', path: '/announcements', icon: Megaphone },
    { name: 'Events', path: '/events', icon: CalendarIcon },
    { name: 'Groups', path: '/groups', icon: Users },
    { name: 'My Tasks', path: '/my-tasks', icon: CheckSquare },
    { name: 'Campus Map', path: '/campus-map', icon: MapPin },
    { name: 'Library', path: '/library', icon: BookOpen },
  ];

  // Add role-specific navigation items
  const roleSpecificItems = [];
  
  if (user.role === 'Professor' || user.role === 'Admin') {
    roleSpecificItems.push(
      { name: 'Subjects', path: '/subjects', icon: GraduationCap },
      { name: 'Manage Grades', path: '/manage-grades', icon: FileText }
    );
  } else if (user.role === 'User' || !user.role) {
    // Student role
    roleSpecificItems.push(
      { name: 'My Grades', path: '/my-grades', icon: GraduationCap }
    );
  }

  const allNavItems = [...navItems, ...roleSpecificItems];

  const isActive = (path: string) => location.pathname === path;

  return (
    <div className="min-h-screen bg-gradient-to-br from-background via-background to-primary/5">
      {/* Navigation Bar */}
      <motion.nav
        initial={{ y: -100 }}
        animate={{ y: 0 }}
        className={`sticky top-0 z-50 transition-all duration-300 ${
          scrolled
            ? 'bg-background/80 backdrop-blur-xl border-b shadow-lg'
            : 'bg-background/60 backdrop-blur-md border-b border-border/40'
        }`}
      >
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between h-16">
            {/* Logo */}
            <Link to="/dashboard" className="flex items-center space-x-2 group flex-shrink-0">
              <motion.div
                whileHover={{ rotate: 360, scale: 1.1 }}
                transition={{ duration: 0.6 }}
                className="w-9 h-9 flex items-center justify-center"
              >
                <svg viewBox="0 0 200 200" className="w-full h-full">
                  <circle cx="100" cy="100" r="90" fill="url(#gradNav)" />
                  <circle cx="70" cy="70" r="25" fill="white" opacity="0.9" />
                  <circle cx="70" cy="130" r="20" fill="white" opacity="0.8" />
                  <circle cx="160" cy="140" r="20" fill="white" opacity="0.8" />
                  <circle cx="100" cy="100" r="30" fill="white" opacity="0.9" />
                  <path d="M 50 80 Q 100 50 150 80" stroke="white" strokeWidth="8" fill="none" opacity="0.7" />
                  <path d="M 150 80 Q 180 110 160 140" stroke="white" strokeWidth="8" fill="none" opacity="0.7" />
                  <path d="M 50 80 Q 40 105 70 130" stroke="white" strokeWidth="8" fill="none" opacity="0.7" />
                  <defs>
               allN    <linearGradient id="gradNav" x1="0%" y1="0%" x2="100%" y2="100%">
                      <stop offset="0%" style={{stopColor: '#667eea', stopOpacity: 1}} />
                      <stop offset="100%" style={{stopColor: '#764ba2', stopOpacity: 1}} />
                    </linearGradient>
                  </defs>
                </svg>
              </motion.div>
              <div className="hidden sm:block">
                <span className="text-lg font-bold bg-gradient-to-r from-primary via-purple-600 to-pink-600 bg-clip-text text-transparent">
                  CampusConnect
                </span>
                <p className="text-[9px] text-muted-foreground -mt-1">Student Portal</p>
              </div>
            </Link>

            {/* Desktop Navigation */}
            <div className="hidden lg:flex items-center space-x-1">
              {navItems.map((item) => {
                const Icon = item.icon;
                const active = isActive(item.path);
                return (
                  <Link key={item.path} to={item.path}>
                    <motion.div
                      whileHover={{ scale: 1.05 }}
                      whileTap={{ scale: 0.95 }}
                    >
                      <Button
                        variant={active ? "default" : "ghost"}
                        className="relative space-x-2"
                        size="sm"
                      >
                        <Icon className="h-4 w-4" />
                        <span>{item.name}</span>
                        {active && (
                          <motion.div
                            layoutId="activeTab"
                            className="absolute inset-0 bg-primary rounded-md -z-10"
                            transition={{ type: "spring", bounce: 0.2, duration: 0.6 }}
                          />
                        )}
                      </Button>
                    </motion.div>
                  </Link>
                );
              })}
            </div>

            {/* Right Side Actions */}
            <div className="flex items-center space-x-1 flex-shrink-0">
              <ThemeToggle />

              <div className="hidden lg:flex items-center space-x-2">
                <Link to="/profile">
                  <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                    <Button variant="ghost" className="space-x-2 px-2" size="sm">
                      <Avatar className="h-6 w-6">
                        <AvatarFallback className="bg-gradient-to-br from-primary to-purple-600 text-white text-xs">
                          {user.firstName?.[0]}{user.lastName?.[0]}
                        </AvatarFallback>
                      </Avatar>
                      <span className="text-sm font-medium">{user.firstName}</span>
                    </Button>
                  </motion.div>
                </Link>

                {token && (
                  <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={handleLogout}
                      className="rounded-full h-8 w-8"
                    >
                      <LogOut className="h-4 w-4" />
                    </Button>
                  </motion.div>
                )}
              </div>

              {/* Mobile menu button */}
              <Button
                variant="ghost"
                size="icon"
                className="lg:hidden"
                onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
              >
                <AnimatePresence mode="wait">
                  {mobileMenuOpen ? (
                    <motion.div
                      key="close"
                      initial={{ rotate: -90, opacity: 0 }}
                      animate={{ rotate: 0, opacity: 1 }}
                      exit={{ rotate: 90, opacity: 0 }}
                      transition={{ duration: 0.2 }}
                    >
                      <X className="h-6 w-6" />
                    </motion.div>
                  ) : (
                    <motion.div
                      key="menu"
                      initial={{ rotate: 90, opacity: 0 }}
                      animate={{ rotate: 0, opacity: 1 }}
                      exit={{ rotate: -90, opacity: 0 }}
                      transition={{ duration: 0.2 }}
                    >
                      <Menu className="h-6 w-6" />
                    </motion.div>
                  )}
                </AnimatePresence>
              </Button>
            </div>
          </div>
        </div>

        {/* Mobile Navigation */}
        <AnimatePresence>
          {mobileMenuOpen && (
            <motion.div
              initial={{ opacity: 0, height: 0 }}
              animate={{ opacity: 1, height: 'auto' }}
              exit={{ opacity: 0, height: 0 }}
              transition={{ duration: 0.3 }}
              className="lg:hidden border-t bg-background/95 backdrop-blur-lg overflow-hidden"
            >
              <div className="px-4 py-4 space-y-2">
                {allNavItems.map((item, index) => {
                  const Icon = item.icon;
                  const active = isActive(item.path);
                  return (
                    <motion.div
                      key={item.path}
                      initial={{ opacity: 0, x: -20 }}
                      animate={{ opacity: 1, x: 0 }}
                      transition={{ delay: index * 0.1 }}
                    >
                      <Link
                        to={item.path}
                        onClick={() => setMobileMenuOpen(false)}
                      >
                        <Button
                          variant={active ? "default" : "ghost"}
                          className="w-full justify-start space-x-2"
                        >
                          <Icon className="h-4 w-4" />
                          <span>{item.name}</span>
                        </Button>
                      </Link>
                    </motion.div>
                  );
                })}

                <div className="border-t pt-2 mt-2 space-y-2">
                  <Link to="/profile" onClick={() => setMobileMenuOpen(false)}>
                    <Button variant="ghost" className="w-full justify-start space-x-2">
                      <User className="h-4 w-4" />
                      <span>Profile</span>
                    </Button>
                  </Link>

                  {token && (
                    <Button
                      variant="ghost"
                      onClick={() => {
                        handleLogout();
                        setMobileMenuOpen(false);
                      }}
                      className="w-full justify-start space-x-2 text-red-500 hover:text-red-600 hover:bg-red-50 dark:hover:bg-red-950/20"
                    >
                      <LogOut className="h-4 w-4" />
                      <span>Logout</span>
                    </Button>
                  )}
                </div>
              </div>
            </motion.div>
          )}
        </AnimatePresence>
      </motion.nav>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <AnimatePresence mode="wait">
          <motion.div
            key={location.pathname}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -20 }}
            transition={{ duration: 0.3 }}
          >
            {children}
          </motion.div>
        </AnimatePresence>
      </main>

      {/* Footer */}
      <motion.footer
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        transition={{ delay: 0.5 }}
        className="border-t bg-background/50 backdrop-blur-sm mt-16"
      >
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="flex flex-col md:flex-row items-center justify-between gap-4">
            <div className="flex items-center space-x-2">
              <div className="w-8 h-8 flex items-center justify-center">
                <svg viewBox="0 0 200 200" className="w-full h-full">
                  <circle cx="100" cy="100" r="90" fill="url(#gradFooter)" />
                  <circle cx="70" cy="70" r="25" fill="white" opacity="0.9" />
                  <circle cx="70" cy="130" r="20" fill="white" opacity="0.8" />
                  <circle cx="160" cy="140" r="20" fill="white" opacity="0.8" />
                  <circle cx="100" cy="100" r="30" fill="white" opacity="0.9" />
                  <path d="M 50 80 Q 100 50 150 80" stroke="white" strokeWidth="8" fill="none" opacity="0.7" />
                  <path d="M 150 80 Q 180 110 160 140" stroke="white" strokeWidth="8" fill="none" opacity="0.7" />
                  <path d="M 50 80 Q 40 105 70 130" stroke="white" strokeWidth="8" fill="none" opacity="0.7" />
                  <defs>
                    <linearGradient id="gradFooter" x1="0%" y1="0%" x2="100%" y2="100%">
                      <stop offset="0%" style={{stopColor: '#667eea', stopOpacity: 1}} />
                      <stop offset="100%" style={{stopColor: '#764ba2', stopOpacity: 1}} />
                    </linearGradient>
                  </defs>
                </svg>
              </div>
              <span className="text-sm text-muted-foreground">
                © 2025 CampusConnect. All rights reserved.
              </span>
            </div>
            <div className="flex items-center gap-6 text-sm text-muted-foreground">
              <a href="#" className="hover:text-primary transition-colors">Privacy</a>
              <a href="#" className="hover:text-primary transition-colors">Terms</a>
              <a href="#" className="hover:text-primary transition-colors">Support</a>
            </div>
          </div>
        </div>
      </motion.footer>
    </div>
  );
}
