import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { BookOpen, GraduationCap, TrendingUp, Award, Sparkles } from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardContent } from '../../components/ui/Card';
import * as gradesApi from '../../services/gradesApi';
import type { StudentGradesResponse } from '../../services/gradesApi';

const MyGrades = () => {
  const [gradesData, setGradesData] = useState<StudentGradesResponse | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadGrades();
  }, []);

  const loadGrades = async () => {
    setLoading(true);
    try {
      const data = await gradesApi.gradeApi.getMyGrades();
      setGradesData(data);
    } catch (error) {
      console.error('Error loading grades:', error);
    } finally {
      setLoading(false);
    }
  };

  const getGradeColor = (value: number) => {
    if (value >= 9) return 'text-green-600 bg-green-50 border-green-200';
    if (value >= 7) return 'text-blue-600 bg-blue-50 border-blue-200';
    if (value >= 5) return 'text-yellow-600 bg-yellow-50 border-yellow-200';
    return 'text-red-600 bg-red-50 border-red-200';
  };

  const calculateAverage = () => {
    if (!gradesData || gradesData.subjectGrades.length === 0) return 0;
    const allGrades = gradesData.subjectGrades.flatMap(sg => sg.grades);
    if (allGrades.length === 0) return 0;
    const sum = allGrades.reduce((acc, grade) => acc + grade.value, 0);
    return (sum / allGrades.length).toFixed(2);
  };
  const average = calculateAverage();

  return (
    <Layout>
      <div className="space-y-8">
        {/* Hero Header */}
        <motion.div
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-teal-500 via-cyan-600 to-blue-600 p-8 text-white shadow-2xl"
        >
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>
          
          <div className="relative z-10">
            <div className="flex items-center gap-4">
              <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                <GraduationCap className="h-8 w-8" />
              </div>
              <div>
                <h1 className="text-4xl font-bold flex items-center gap-2">
                  My Grades
                  <Sparkles className="h-6 w-6 text-yellow-300" />
                </h1>
                <p className="text-white/80 mt-1">View your academic progress</p>
              </div>
            </div>
          </div>
        </motion.div>

        <div className="container mx-auto px-4 max-w-7xl">
          {/* Statistics Cards */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.1 }}
          >
            <Card className="bg-gradient-to-br from-primary/10 to-primary/5 border-primary/20">
              <CardContent className="pt-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-primary font-medium mb-1">Overall Average</p>
                    <p className="text-3xl font-bold text-primary">{average}</p>
                  </div>
                  <TrendingUp className="w-12 h-12 text-primary opacity-50" />
                </div>
              </CardContent>
            </Card>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.2 }}
          >
            <Card className="bg-gradient-to-br from-green-500/10 to-green-500/5 border-green-500/20">
              <CardContent className="pt-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-green-600 font-medium mb-1">Total Grades</p>
                    <p className="text-3xl font-bold text-green-700">
                      {gradesData?.subjectGrades.reduce((acc, sg) => acc + sg.grades.length, 0) || 0}
                    </p>
                  </div>
                  <Award className="w-12 h-12 text-green-600 opacity-50" />
                </div>
              </CardContent>
            </Card>
          </motion.div>

          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.3 }}
          >
            <Card className="bg-gradient-to-br from-purple-500/10 to-purple-500/5 border-purple-500/20">
              <CardContent className="pt-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-purple-600 font-medium mb-1">Subjects</p>
                    <p className="text-3xl font-bold text-purple-700">
                      {gradesData?.subjectGrades.length || 0}
                    </p>
                  </div>
                  <BookOpen className="w-12 h-12 text-purple-600 opacity-50" />
                </div>
              </CardContent>
            </Card>
          </motion.div>
        </div>

        <div className="container mx-auto px-4 max-w-7xl">
          {/* Grades List */}
          {loading ? (
          <div className="flex justify-center items-center h-64">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        ) : !gradesData || gradesData.subjectGrades.length === 0 ? (
          <Card>
            <CardContent className="py-12 text-center">
              <GraduationCap className="w-16 h-16 text-muted-foreground mx-auto mb-4" />
              <p className="text-xl mb-2">You don't have any grades yet</p>
              <p className="text-muted-foreground">Your grades will appear here after your professors assign them</p>
            </CardContent>
          </Card>
        ) : (
          <div className="space-y-12">
            {[1, 2, 3].map(year => {
              const yearSubjects = gradesData.subjectGrades.filter(sg => sg.year === year);
              if (yearSubjects.length === 0) return null;

              return (
                <motion.div 
                  key={year}
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  className="space-y-6"
                >
                  {/* Year Header - Ultra Minimalist */}
                  <div className="flex items-center gap-3 mb-6">
                    <GraduationCap className="text-indigo-600" size={32} />
                    <h2 className="text-3xl font-bold text-gray-900">Year {year}</h2>
                  </div>

                  {/* Subjects List - No Containers */}
                  <div className="space-y-8 pl-6">
                    {yearSubjects.map((subjectGrade, index) => (
                      <motion.div
                        key={subjectGrade.subjectId}
                        initial={{ opacity: 0, x: -10 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: 0.05 * index }}
                        className="space-y-3"
                      >
                        {/* Subject Name with Average */}
                        <div className="flex items-baseline gap-3">
                          <h3 className="text-xl font-semibold text-gray-800">
                            {subjectGrade.subjectName}
                          </h3>
                          <span className="text-sm text-indigo-600 font-medium">
                            (Average: {subjectGrade.averageGrade?.toFixed(2) || 'N/A'})
                          </span>
                        </div>

                        {/* Grades - Simple Inline List */}
                        <div className="flex items-center gap-2 flex-wrap pl-4">
                          <span className="text-gray-600 font-medium">Grades:</span>
                          {subjectGrade.grades.map((grade, idx) => (
                            <span key={grade.id} className="inline-flex items-center">
                              <span
                                className={`${getGradeColor(grade.value)} px-3 py-1 rounded-md font-bold text-base cursor-help transition-all hover:scale-105`}
                                title={`${new Date(grade.createdAt).toLocaleDateString('en-US', { day: '2-digit', month: '2-digit', year: 'numeric' })}${grade.comments ? '\n' + grade.comments : ''}`}
                              >
                                {grade.value.toFixed(2)}
                              </span>
                              {idx < subjectGrade.grades.length - 1 && (
                                <span className="text-gray-400 mx-1">•</span>
                              )}
                            </span>
                          ))}
                        </div>
                      </motion.div>
                    ))}
                  </div>
                </motion.div>
              );
            })}
          </div>
        )}
        </div>
      </div>
      </div>
    </Layout>
  );
}

export default MyGrades;
