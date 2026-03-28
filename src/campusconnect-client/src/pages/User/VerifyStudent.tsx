import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { CheckCircle, XCircle, GraduationCap, ArrowLeft } from 'lucide-react';
import { Button } from '../../components/ui/Button';

const API_BASE_URL = 'http://localhost:5099/api';

interface VerificationResult {
  valid: boolean;
  firstName?: string;
  lastName?: string;
  studentId?: string;
}

function VerifyStudent() {
  const { id, token } = useParams<{ id: string; token: string }>();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [result, setResult] = useState<VerificationResult | null>(null);

  useEffect(() => {
    const verifyStudent = async () => {
      if (!id || !token) {
        setResult({ valid: false });
        setLoading(false);
        return;
      }

      try {
        // Decode the token to get studentId
        const decodedStudentId = atob(token);
        console.log('Decoded studentId from URL:', decodedStudentId);
        console.log('User ID from URL:', id);

        // Fetch public user details to verify
        const res = await fetch(`${API_BASE_URL}/user/public-details/${id}`);
        console.log('API Response status:', res.status);

        if (res.ok) {
          const userData = await res.json();
          console.log('API returned userData:', userData);
          console.log('Comparing:', userData.studentId, '===', decodedStudentId);

          // Verify that the studentId matches
          if (userData.studentId === decodedStudentId) {
            setResult({
              valid: true,
              firstName: userData.firstName,
              lastName: userData.lastName,
              studentId: userData.studentId
            });
          } else {
            setResult({ valid: false });
          }
        } else {
          setResult({ valid: false });
        }
      } catch (error) {
        console.error('Verification error:', error);
        setResult({ valid: false });
      } finally {
        setLoading(false);
      }
    };

    verifyStudent();
  }, [id, token]);

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
        <div className="text-center">
          <div className="animate-spin rounded-full h-16 w-16 border-b-4 border-blue-600 mx-auto mb-4"></div>
          <p className="text-lg text-gray-600">Se verifica statusul de student...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
      <motion.div
        initial={{ opacity: 0, scale: 0.9 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 0.5 }}
        className="max-w-md w-full"
      >
        {result?.valid ? (
          // Valid Student
          <div className="bg-white rounded-2xl shadow-2xl overflow-hidden">
            {/* Header */}
            <div className="bg-gradient-to-r from-green-500 to-emerald-600 p-6 text-center">
              <motion.div
                initial={{ scale: 0 }}
                animate={{ scale: 1 }}
                transition={{ delay: 0.2, type: 'spring' }}
              >
                <CheckCircle className="h-20 w-20 text-white mx-auto mb-3" />
              </motion.div>
              <h1 className="text-2xl font-bold text-white">Student Verificat</h1>
            </div>

            {/* Content */}
            <div className="p-6">
              {/* University Badge */}
              <div className="flex items-center gap-3 mb-6 p-4 bg-blue-50 rounded-xl">
                <img
                  src="/unibuc-logo.png"
                  alt="UniBuc"
                  className="h-12 w-12 object-contain"
                  onError={(e) => {
                    e.currentTarget.style.display = 'none';
                    e.currentTarget.nextElementSibling?.classList.remove('hidden');
                  }}
                />
                <div className="hidden h-12 w-12 bg-blue-100 rounded-lg items-center justify-center">
                  <GraduationCap className="h-8 w-8 text-blue-600" />
                </div>
                <div>
                  <p className="font-semibold text-blue-900">Universitatea din Bucuresti</p>
                  <p className="text-sm text-blue-600">Student activ</p>
                </div>
              </div>

              {/* Student Info */}
              <div className="space-y-4">
                <div className="flex justify-between items-center py-3 border-b">
                  <span className="text-gray-500">Nume complet</span>
                  <span className="font-bold text-xl text-gray-900">{result.firstName} {result.lastName}</span>
                </div>
                <div className="flex justify-between items-center py-3 border-b">
                  <span className="text-gray-500">Nr. Matricol</span>
                  <span className="font-mono font-bold text-xl text-gray-900">{result.studentId}</span>
                </div>
                <div className="flex justify-between items-center py-3">
                  <span className="text-gray-500">Status</span>
                  <span className="px-3 py-1 bg-green-100 text-green-700 rounded-full text-sm font-medium">
                    Activ
                  </span>
                </div>
              </div>

              {/* Footer note */}
              <div className="mt-6 p-4 bg-gray-50 rounded-xl text-center">
                <p className="text-sm text-gray-500">
                  Verificare efectuata la {new Date().toLocaleString('ro-RO')}
                </p>
              </div>
            </div>
          </div>
        ) : (
          // Invalid
          <div className="bg-white rounded-2xl shadow-2xl overflow-hidden">
            {/* Header */}
            <div className="bg-gradient-to-r from-red-500 to-rose-600 p-6 text-center">
              <motion.div
                initial={{ scale: 0 }}
                animate={{ scale: 1 }}
                transition={{ delay: 0.2, type: 'spring' }}
              >
                <XCircle className="h-20 w-20 text-white mx-auto mb-3" />
              </motion.div>
              <h1 className="text-2xl font-bold text-white">Verificare Esuata</h1>
            </div>

            {/* Content */}
            <div className="p-6 text-center">
              <p className="text-gray-600 mb-6">
                Nu am putut verifica acest card de student. Codul QR poate fi invalid sau expirat.
              </p>

              <div className="p-4 bg-red-50 rounded-xl mb-6">
                <p className="text-sm text-red-600">
                  Daca credeti ca aceasta este o eroare, rugam studentul sa isi regenereze cardul digital.
                </p>
              </div>

              <Button
                onClick={() => navigate('/login')}
                className="w-full"
              >
                <ArrowLeft className="h-4 w-4 mr-2" />
                Inapoi la aplicatie
              </Button>
            </div>
          </div>
        )}

        {/* Powered by */}
        <p className="text-center text-gray-400 text-sm mt-6">
          Verificat prin CampusConnect
        </p>
      </motion.div>
    </div>
  );
}

export default VerifyStudent;