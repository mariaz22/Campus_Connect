import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { QRCodeSVG } from 'qrcode.react';
import { motion } from 'framer-motion';
import { ArrowLeft, RotateCcw, GraduationCap } from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Button } from '../../components/ui/Button';
import { Avatar, AvatarFallback } from '../../components/ui/Avatar';

const API_BASE_URL = 'http://localhost:5099/api';

interface UserDetails {
  id: number;
  firstName: string;
  lastName: string;
  studentId: string;
  profilePictureUrl?: string;
  dateofBirth?: string;
}

function StudentCard() {
  const navigate = useNavigate();
  const [isFlipped, setIsFlipped] = useState(false);
  const [userDetails, setUserDetails] = useState<UserDetails | null>(null);
  const [loading, setLoading] = useState(true);

  const token = localStorage.getItem('token');

  useEffect(() => {
    const fetchUserDetails = async () => {
      if (!token) {
        navigate('/login');
        return;
      }

      try {
        const res = await fetch(`${API_BASE_URL}/user/details`, {
          headers: { Authorization: `Bearer ${token}` }
        });

        if (res.ok) {
          const data = await res.json();
          setUserDetails(data);
        }
      } catch (error) {
        console.error('Error fetching user details:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchUserDetails();
  }, [token, navigate]);

  const handleFlip = () => {
    setIsFlipped(!isFlipped);
  };

  // Generate verification URL for QR code
  const verificationUrl = userDetails
    ? `${window.location.origin}/verify-student/${userDetails.id}/${btoa(userDetails.studentId || '')}`
    : '';

  if (loading) {
    return (
      <Layout>
        <div className="flex items-center justify-center min-h-[60vh]">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-600"></div>
        </div>
      </Layout>
    );
  }

  if (!userDetails || !userDetails.studentId) {
    return (
      <Layout>
        <div className="flex flex-col items-center justify-center min-h-[60vh] gap-4">
          <p className="text-lg text-muted-foreground">Nu ai un card de student disponibil.</p>
          <Button onClick={() => navigate('/profile')}>
            <ArrowLeft className="h-4 w-4 mr-2" /> Inapoi la profil
          </Button>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="flex flex-col items-center justify-center min-h-[70vh] px-4">
        {/* Back button */}
        <div className="w-full max-w-md mb-6">
          <Button variant="ghost" onClick={() => navigate('/profile')}>
            <ArrowLeft className="h-4 w-4 mr-2" /> Inapoi la profil
          </Button>
        </div>

        {/* Card Container */}
        <div
          className="student-card-container cursor-pointer"
          onClick={handleFlip}
          style={{ perspective: '1000px' }}
        >
          <motion.div
            className="student-card-inner"
            animate={{ rotateY: isFlipped ? 180 : 0 }}
            transition={{ duration: 0.6, type: 'spring', stiffness: 100 }}
            style={{
              transformStyle: 'preserve-3d',
              width: '340px',
              height: '540px',
              position: 'relative'
            }}
          >
            {/* Front Side */}
            <div
              className="student-card-face student-card-front absolute w-full h-full rounded-2xl shadow-2xl overflow-hidden"
              style={{
                backfaceVisibility: 'hidden',
                background: 'linear-gradient(135deg, #1e3a5f 0%, #2c5282 50%, #1e3a5f 100%)'
              }}
            >
              {/* Header with Logo */}
              <div className="bg-white/10 backdrop-blur-sm p-4 flex items-center gap-3 border-b border-white/20">
                <img
                  src="/unibuc-logo.png"
                  alt="UniBuc Logo"
                  className="h-14 w-14 object-contain bg-white rounded-lg p-1"
                  onError={(e) => {
                    // Fallback to icon if logo not found
                    e.currentTarget.style.display = 'none';
                    e.currentTarget.nextElementSibling?.classList.remove('hidden');
                  }}
                />
                <div className="h-14 w-14 bg-white rounded-lg p-2 hidden items-center justify-center">
                  <GraduationCap className="h-10 w-10 text-blue-900" />
                </div>
                <div className="text-white">
                  <h2 className="font-bold text-lg leading-tight">Universitatea din</h2>
                  <h2 className="font-bold text-lg leading-tight">Bucuresti</h2>
                </div>
              </div>

              {/* Card Type */}
              <div className="text-center py-2 bg-amber-500/90">
                <span className="text-white font-bold tracking-widest text-sm">LEGITIMATIE STUDENT</span>
              </div>

              {/* Student Info */}
              <div className="p-6 flex flex-col items-center text-white">
                {/* Photo */}
                <div className="mb-4">
                  <Avatar className="h-28 w-28 border-4 border-white/30 shadow-lg">
                    {userDetails.profilePictureUrl ? (
                      <img
                        src={userDetails.profilePictureUrl}
                        alt="Profile"
                        className="h-full w-full object-cover"
                      />
                    ) : (
                      <AvatarFallback className="text-3xl font-bold bg-white/20 text-white">
                        {userDetails.firstName?.[0]}{userDetails.lastName?.[0]}
                      </AvatarFallback>
                    )}
                  </Avatar>
                </div>

                {/* Name */}
                <h3 className="text-2xl font-bold text-center mb-1">
                  {userDetails.firstName}
                </h3>
                <h3 className="text-2xl font-bold text-center mb-4">
                  {userDetails.lastName}
                </h3>

                {/* Student ID */}
                <div className="bg-white/10 rounded-lg px-6 py-3 mb-4">
                  <p className="text-xs text-white/70 text-center mb-1">Nr. Matricol</p>
                  <p className="text-xl font-mono font-bold tracking-wider text-center">
                    {userDetails.studentId}
                  </p>
                </div>

                {/* Validity */}
                <div className="text-xs text-white/60 text-center">
                  <p>An universitar 2025-2026</p>
                </div>
              </div>

              {/* Tap hint */}
              <div className="absolute bottom-4 left-0 right-0 text-center">
                <p className="text-white/50 text-xs flex items-center justify-center gap-1">
                  <RotateCcw className="h-3 w-3" /> Apasa pentru QR
                </p>
              </div>
            </div>

            {/* Back Side */}
            <div
              className="student-card-face student-card-back absolute w-full h-full rounded-2xl shadow-2xl overflow-hidden"
              style={{
                backfaceVisibility: 'hidden',
                transform: 'rotateY(180deg)',
                background: 'linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%)'
              }}
            >
              {/* Header */}
              <div className="bg-gradient-to-r from-blue-900 to-blue-700 p-4 text-center">
                <h2 className="text-white font-bold text-lg">VERIFICARE STUDENT</h2>
              </div>

              {/* QR Code */}
              <div className="flex flex-col items-center justify-center p-6">
                {/* Clickable QR for testing */}
                <a
                  href={verificationUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  onClick={(e) => e.stopPropagation()}
                  className="bg-white p-4 rounded-2xl shadow-lg mb-4 hover:shadow-xl transition-shadow cursor-pointer"
                  title="Click pentru a testa verificarea"
                >
                  <QRCodeSVG
                    value={verificationUrl}
                    size={160}
                    level="H"
                    includeMargin={true}
                  />
                </a>

                <p className="text-gray-600 text-sm text-center mb-4">
                  Scaneaza sau click pe QR pentru verificare
                </p>

                {/* Student Info Summary - More prominent */}
                <div className="bg-blue-50 border border-blue-200 rounded-xl p-4 w-full">
                  <div className="text-center mb-3">
                    <p className="text-xl font-bold text-blue-900">{userDetails.firstName} {userDetails.lastName}</p>
                  </div>
                  <div className="flex justify-center">
                    <div className="bg-white px-4 py-2 rounded-lg shadow-sm">
                      <p className="text-xs text-gray-500 text-center">Nr. Matricol</p>
                      <p className="text-lg font-mono font-bold text-blue-800 text-center">{userDetails.studentId}</p>
                    </div>
                  </div>
                </div>
              </div>

              {/* Tap hint */}
              <div className="absolute bottom-4 left-0 right-0 text-center">
                <p className="text-gray-400 text-xs flex items-center justify-center gap-1">
                  <RotateCcw className="h-3 w-3" /> Apasa pentru fata
                </p>
              </div>
            </div>
          </motion.div>
        </div>

        {/* Instructions */}
        <div className="mt-8 text-center max-w-md">
          <p className="text-sm text-muted-foreground">
            Apasa pe card pentru a-l intoarce. Codul QR poate fi scanat pentru verificarea statusului de student.
          </p>
        </div>
      </div>
    </Layout>
  );
}

export default StudentCard;