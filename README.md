# Campus Connect - Product Vision

## Vision Statement
Campus Connect aims to revolutionize the way students, professors, and university staff interact with campus services and academic information. Our goal is to provide a **centralized, intuitive, and secure platform** that streamlines communication, access to academic resources, and administrative tasks, all through a single interface.

---

## Key Features

### Academic Management
- **Grades & Transcripts** - Students can view their grades in real-time; professors can manage and publish grades
- **Subjects & Courses** - Complete course catalog with materials, schedules, and enrollment information
- **Schedule Management** - Interactive timetables for students and professors
- **Digital Documents** - Access and download official academic documents

### AI-Powered Assistant
- **Smart Campus Assistant** - AI chatbot that helps users navigate the platform, answers questions about schedules, grades, campus facilities, and provides personalized recommendations

### Social & Community
- **Groups** - Create and join study groups, project teams, or interest-based communities with task management
- **Events** - Discover, create, and participate in campus events (academic, social, cultural)
- **Announcements** - University-wide and targeted announcements with category subscriptions
- **Notifications** - Real-time notifications for grades, events, group activities, and announcements

### Gamification & Engagement
- **Achievements & Badges** - Earn achievements for academic performance, participation, and campus engagement
- **Activity Tracking** - Track personal activity history and progress
- **Leaderboards** - Compare achievements with peers

### Campus Facilities
- **Interactive Campus Map** - Real interactive map powered by Leaflet showing all university faculty buildings with precise GPS coordinates
- **2D Architectural Floor Plans** - Each building includes detailed 2D floor plans showing room layouts
- **Room Booking System** - Browse available classrooms, check real-time availability, and submit booking requests
- **Building Directory** - Information about campus buildings, rooms, and their availability

### Digital Library
- **Course Materials** - Access lecture notes, presentations, PDFs, and study resources uploaded by professors
- **Organized Folders** - Materials organized by subject and category for easy navigation
- **Download** - Download documents for offline study

### Virtual Student Card
- **Digital ID Card** - Virtual student card with photo, name, email, and student ID number
- **QR Code Integration** - Unique QR code for identity verification, event check-ins, and campus facility access
- **Mobile-First Design** - Access your student card anytime from your phone

---

## Target Users

### Students
- Access academic information (grades, schedules, transcripts)
- Use AI assistant for quick answers and guidance
- Join groups and participate in events
- Earn achievements and track progress
- Book study rooms and navigate campus
- Receive personalized notifications

### Professors
- Publish and manage course materials and grades
- Create announcements for students
- Organize groups and academic events
- Track student participation
- Manage room reservations

### Administrators
- Oversee platform usage and analytics
- Manage users, roles, and permissions
- Configure achievements and gamification elements
- Manage campus facilities and bookings
- Ensure content quality and platform compliance

---

## Technology Stack

| Layer | Technologies |
|-------|-------------|
| **Frontend** | React 19, TypeScript, Vite, TailwindCSS |
| **Backend** | .NET 9, ASP.NET Core, Entity Framework Core |
| **Database** | SQL Server |
| **Authentication** | ASP.NET Identity, JWT Tokens |
| **Architecture** | Clean Architecture, CQRS (MediatR) |

---

## Getting Started

### Prerequisites
- .NET 9 SDK
- Node.js 20+
- SQL Server

### Running the Application

**Backend:**
```bash
cd src/CampusConnect/CampusConnect.Api
dotnet run
```
API runs on: http://localhost:5099

**Frontend:**
```bash
cd src/campusconnect-client
npm install
npm run dev
```
Client runs on: http://localhost:5173
