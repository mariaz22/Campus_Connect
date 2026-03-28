import { BrowserRouter, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import { ThemeProvider } from './contexts/ThemeContext';
import Login from './pages/Login';
import Register from './pages/Register';
import ConfirmEmail from './pages/ConfirmEmail';
import Home from './pages/Home';
import ViewProfile from './pages/User/ViewProfile';
import EditProfile from './pages/User/EditProfile';
import StudentCard from './pages/User/StudentCard';
import VerifyStudent from './pages/User/VerifyStudent';
import Announcements from './pages/Announcements';
import CreateAnnouncement from './pages/CreateAnnouncement';
import CreateEvent from './pages/Event/CreateEvent';
import EditEvent from './pages/Event/UpdateEvent';
import ViewEvent from './pages/Event/ViewEvent';
import Users from './pages/User/Users';
import './App.css';
import UpcomingEvents from './pages/Event/UpcomingEvent';
import Groups from './pages/Groups/Groups';
import GroupDetails from './pages/Groups/GroupDetails';
import MyTasks from './pages/Groups/MyTasks';
import CampusMap from './pages/CampusMap/CampusMap';
import AllAchievements from './pages/Achievements/AllAchievements';
import ManageAchievements from './pages/Achievements/ManageAchievements';
import ActivityHistory from './pages/ActivityHistory';
import BookingRequests from './pages/BookingRequests/BookingRequests';
import MyBookingRequests from './pages/MyBookingRequests/MyBookingRequests';
import Notifications from './pages/Notifications';
import Library from './pages/Library/Library';
import SubjectsManagement from './pages/SubjectsManagement';
import GradesManagement from './pages/GradesManagement';
import StudentGrades from './pages/StudentGrades';
import SubjectDetails from './pages/Subjects/SubjectDetails';
import Documents from './pages/Documents/Documents';
import { AiChatWidget } from './components/AiAssistant/AiChatWidget';

function AiChatWrapper() {
  const location = useLocation();
  const publicPaths = ['/', '/login', '/register', '/confirm-email'];
  const isPublicPath = publicPaths.includes(location.pathname) || location.pathname.startsWith('/verify-student');

  if (isPublicPath) return null;
  return <AiChatWidget />;
}

function App() {
  return (
    <ThemeProvider>
      <BrowserRouter>
        <AiChatWrapper />
        <Routes>
          <Route path="/" element={<Navigate to="/login" replace />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/confirm-email" element={<ConfirmEmail />} />
          <Route path="/dashboard" element={<Home />} />
          <Route path="/profile" element={<ViewProfile />} />
          <Route path="/profile/:id" element={<ViewProfile />} />
          <Route path="/edit-profile" element={<EditProfile />} />
          <Route path="/announcements" element={<Announcements />} />
          <Route path="/create-announcement" element={<CreateAnnouncement />} />
          <Route path="/events" element={<UpcomingEvents />} />
          <Route path="/create-event" element={<CreateEvent />} />
          <Route path="/edit-event/:id" element={<EditEvent />} />
          <Route path="/event/:id" element={<ViewEvent />} />
          <Route path="/groups" element={<Groups />} />
          <Route path='/users' element={<Users />} />
          <Route path="/groups/:id" element={<GroupDetails />} />
          <Route path="/my-tasks" element={<MyTasks />} />
          <Route path="/campus-map" element={<CampusMap />} />
          <Route path="/achievements" element={<AllAchievements />} />
          <Route path="/manage-achievements" element={<ManageAchievements />} />
          <Route path="/activity-history" element={<ActivityHistory />} />
          <Route path="/booking-requests" element={<BookingRequests />} />
          <Route path="/my-booking-requests" element={<MyBookingRequests />} />
          <Route path="/notifications" element={<Notifications />} />
          <Route path="/library" element={<Library />} />
          <Route path="/subjects" element={<SubjectsManagement />} />
          <Route path="/subjects/:id" element={<SubjectDetails />} />
          <Route path="/manage-grades" element={<GradesManagement />} />
          <Route path="/my-grades" element={<StudentGrades />} />
          <Route path="/documents" element={<Documents />} />
          <Route path="/student-card" element={<StudentCard />} />
          <Route path="/verify-student/:id/:token" element={<VerifyStudent />} />
        </Routes>
      </BrowserRouter>
    </ThemeProvider>
  );
}
export default App;