import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { FileText, Download, ArrowLeft, GraduationCap, ClipboardList, Loader2 } from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';

const API_BASE_URL = 'http://localhost:5099/api';

interface DocumentType {
  id: string;
  title: string;
  description: string;
  icon: typeof FileText;
  endpoint: string;
  filename: string;
  gradient: string;
}

const documentTypes: DocumentType[] = [
  {
    id: 'enrollment',
    title: 'Adeverinta de Student',
    description: 'Document oficial care atesta calitatea de student inscris la Universitatea din Bucuresti.',
    icon: GraduationCap,
    endpoint: '/document/enrollment-certificate',
    filename: 'Adeverinta_Student',
    gradient: 'from-blue-500 to-cyan-500'
  },
  {
    id: 'transcript',
    title: 'Situatie Scolara',
    description: 'Document cu toate notele obtinute, grupate pe ani de studiu si discipline.',
    icon: ClipboardList,
    endpoint: '/document/transcript',
    filename: 'Situatie_Scolara',
    gradient: 'from-purple-500 to-pink-500'
  }
];

function Documents() {
  const navigate = useNavigate();
  const [loadingDoc, setLoadingDoc] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const token = localStorage.getItem('token');

  const handleDownload = async (doc: DocumentType) => {
    if (!token) {
      navigate('/login');
      return;
    }

    setLoadingDoc(doc.id);
    setError(null);

    try {
      const response = await fetch(`${API_BASE_URL}${doc.endpoint}`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.message || 'Eroare la generarea documentului');
      }

      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${doc.filename}_${new Date().toISOString().split('T')[0]}.pdf`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } catch (err) {
      console.error('Download error:', err);
      setError(err instanceof Error ? err.message : 'Eroare la descarcarea documentului');
    } finally {
      setLoadingDoc(null);
    }
  };

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header */}
        <div className="flex items-center gap-4">
          <Button variant="ghost" onClick={() => navigate('/dashboard')}>
            <ArrowLeft className="h-4 w-4 mr-2" /> Inapoi
          </Button>
        </div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="text-center mb-8"
        >
          <h1 className="text-3xl font-bold mb-2">Documente Oficiale</h1>
          <p className="text-muted-foreground">
            Genereaza si descarca documente oficiale in format PDF
          </p>
        </motion.div>

        {error && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg text-center"
          >
            {error}
          </motion.div>
        )}

        {/* Documents Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 max-w-4xl mx-auto">
          {documentTypes.map((doc, index) => {
            const Icon = doc.icon;
            const isLoading = loadingDoc === doc.id;

            return (
              <motion.div
                key={doc.id}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: index * 0.1 }}
              >
                <Card className="h-full hover:shadow-lg transition-shadow">
                  <CardHeader>
                    <div className={`w-16 h-16 rounded-2xl bg-gradient-to-br ${doc.gradient} flex items-center justify-center mb-4 shadow-lg`}>
                      <Icon className="h-8 w-8 text-white" />
                    </div>
                    <CardTitle className="text-xl">{doc.title}</CardTitle>
                    <CardDescription>{doc.description}</CardDescription>
                  </CardHeader>
                  <CardContent>
                    <Button
                      onClick={() => handleDownload(doc)}
                      disabled={isLoading}
                      className="w-full"
                    >
                      {isLoading ? (
                        <>
                          <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                          Se genereaza...
                        </>
                      ) : (
                        <>
                          <Download className="h-4 w-4 mr-2" />
                          Descarca PDF
                        </>
                      )}
                    </Button>
                  </CardContent>
                </Card>
              </motion.div>
            );
          })}
        </div>

        {/* Info Section */}
        <motion.div
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ delay: 0.3 }}
          className="max-w-2xl mx-auto mt-8"
        >
          <Card className="bg-blue-50 dark:bg-blue-950/20 border-blue-200 dark:border-blue-800">
            <CardContent className="pt-6">
              <div className="flex items-start gap-4">
                <div className="p-3 rounded-full bg-blue-100 dark:bg-blue-900">
                  <FileText className="h-6 w-6 text-blue-600 dark:text-blue-400" />
                </div>
                <div>
                  <h3 className="font-semibold text-blue-900 dark:text-blue-100 mb-2">
                    Despre documente
                  </h3>
                  <ul className="text-sm text-blue-700 dark:text-blue-300 space-y-1">
                    <li>• Documentele sunt generate automat cu datele tale din sistem</li>
                    <li>• Fiecare document primeste un numar unic de inregistrare</li>
                    <li>• Documentele sunt valabile conform Legii nr. 455/2001</li>
                    <li>• Pentru documente cu semnatura si stampila, contacteaza secretariatul</li>
                  </ul>
                </div>
              </div>
            </CardContent>
          </Card>
        </motion.div>
      </div>
    </Layout>
  );
}

export default Documents;