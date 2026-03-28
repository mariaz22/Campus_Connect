import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { gradeApi } from '../services/gradesApi';
import type { StudentGradesResponse } from '../services/gradesApi';
import {
  BookOpen,
  GraduationCap,
  Trophy,
  Sparkles,
  TrendingUp,
  Calendar,
} from 'lucide-react';
import { Layout } from '../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../components/ui/Card';
import { Badge } from '../components/ui/Badge';

const StudentGrades = () => {
  const [gradesData, setGradesData] = useState<StudentGradesResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadGrades();
  }, []);

  const loadGrades = async () => {
    try {
      setLoading(true);
      const data = await gradeApi.getMyGrades();
      setGradesData(data);
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Error loading grades');
    } finally {
      setLoading(false);
    }
  };

  const getGradeColor = (grade: number) => {
    if (grade >= 9) return 'bg-green-500 text-white';
    if (grade >= 7) return 'bg-blue-500 text-white';
    if (grade >= 5) return 'bg-yellow-500 text-white';
    return 'bg-red-500 text-white';
  };

  const getGradeBorderColor = (grade: number) => {
    if (grade >= 9) return 'border-green-500';
    if (grade >= 7) return 'border-blue-500';
    if (grade >= 5) return 'border-yellow-500';
    return 'border-red-500';
  };

  const getYearColor = (year: number) => {
    switch (year) {
      case 1:
        return 'bg-blue-500 text-white';
      case 2:
        return 'bg-purple-500 text-white';
      case 3:
        return 'bg-orange-500 text-white';
      default:
        return 'bg-gray-500 text-white';
    }
  };

  if (loading) {
    return (
      <Layout>
        <div className="flex items-center justify-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
        </div>
      </Layout>
    );
  }

  if (error) {
    return (
      <Layout>
        <div className="space-y-6">
          <Card className="border-red-200 bg-red-50 dark:bg-red-900/20 dark:border-red-800">
            <CardContent className="pt-6">
              <p className="text-red-600 dark:text-red-400 font-medium">{error}</p>
            </CardContent>
          </Card>
        </div>
      </Layout>
    );
  }

  if (!gradesData || gradesData.subjectGrades.length === 0) {
    return (
      <Layout>
        <div className="space-y-6">
          {/* Header */}
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-emerald-500 via-teal-500 to-cyan-500 p-8 text-white shadow-2xl"
          >
            <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>

            <div className="relative z-10">
              <div className="flex items-center gap-3 mb-2">
                <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                  <GraduationCap className="h-8 w-8" />
                </div>
                <div>
                  <h1 className="text-4xl font-bold flex items-center gap-2">
                    My Grades
                    <Sparkles className="h-6 w-6 text-yellow-300" />
                  </h1>
                  <p className="text-white/80 mt-1">Track your academic performance</p>
                </div>
              </div>
            </div>
          </motion.div>

          {/* Empty State */}
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="text-center py-12">
            <BookOpen className="h-16 w-16 mx-auto text-muted-foreground mb-4" />
            <h3 className="text-xl font-semibold mb-2">No grades yet</h3>
            <p className="text-muted-foreground">Your grades will appear here when professors add them.</p>
          </motion.div>
        </div>
      </Layout>
    );
  }

  // Calculate overall average
  const allGrades = gradesData.subjectGrades.flatMap((sg) => sg.grades);
  const overallAverage =
    allGrades.length > 0
      ? allGrades.reduce((sum, g) => sum + g.value, 0) / allGrades.length
      : 0;

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-emerald-500 via-teal-500 to-cyan-500 p-8 text-white shadow-2xl"
        >
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxwYXRoIGQ9Ik0zNiAxOGMzLjMxIDAgNiAyLjY5IDYgNnMtMi42OSA2LTYgNi02LTIuNjktNi02IDIuNjktNiA2LTZ6TTI0IDBoNnY2aC02VjB6TTAgMjRoNnY2SDB2LTZ6bTAgMGg2djZIMHYtNnoiIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iLjA1Ii8+PC9nPjwvc3ZnPg==')] opacity-30"></div>

          <div className="relative z-10">
            <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-6">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                  <GraduationCap className="h-8 w-8" />
                </div>
                <div>
                  <h1 className="text-4xl font-bold flex items-center gap-2">
                    My Grades
                    <Sparkles className="h-6 w-6 text-yellow-300" />
                  </h1>
                  <p className="text-white/80 mt-1">{gradesData.studentName}</p>
                </div>
              </div>

              {/* Stats Cards */}
              <div className="flex gap-4">
                <div className="bg-white/20 backdrop-blur-sm rounded-xl p-4 min-w-[120px]">
                  <div className="flex items-center gap-2 mb-1">
                    <Trophy className="h-4 w-4" />
                    <span className="text-sm opacity-90">Average</span>
                  </div>
                  <div className="text-3xl font-bold">{overallAverage.toFixed(2)}</div>
                </div>
                <div className="bg-white/20 backdrop-blur-sm rounded-xl p-4 min-w-[120px]">
                  <div className="flex items-center gap-2 mb-1">
                    <BookOpen className="h-4 w-4" />
                    <span className="text-sm opacity-90">Subjects</span>
                  </div>
                  <div className="text-3xl font-bold">{gradesData.subjectGrades.length}</div>
                </div>
                <div className="bg-white/20 backdrop-blur-sm rounded-xl p-4 min-w-[120px]">
                  <div className="flex items-center gap-2 mb-1">
                    <TrendingUp className="h-4 w-4" />
                    <span className="text-sm opacity-90">Total Grades</span>
                  </div>
                  <div className="text-3xl font-bold">{allGrades.length}</div>
                </div>
              </div>
            </div>
          </div>
        </motion.div>

        {/* Grades by Year */}
        <div className="space-y-8">
          {[1, 2, 3].map((year) => {
            const yearSubjects = gradesData.subjectGrades.filter((sg) => sg.year === year);
            if (yearSubjects.length === 0) return null;

            // Calculate year average
            const yearGrades = yearSubjects.flatMap((sg) => sg.grades);
            const yearAverage =
              yearGrades.length > 0
                ? yearGrades.reduce((sum, g) => sum + g.value, 0) / yearGrades.length
                : 0;

            return (
              <motion.div
                key={year}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: year * 0.1 }}
              >
                <div className="flex items-center gap-3 mb-4">
                  <Badge className={`${getYearColor(year)} px-4 py-2 text-base`}>
                    <GraduationCap className="h-4 w-4 mr-2" />
                    Year {year}
                  </Badge>
                  <div className="h-px flex-1 bg-border"></div>
                  <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <span>Year Average:</span>
                    <Badge className={getGradeColor(yearAverage)}>{yearAverage.toFixed(2)}</Badge>
                  </div>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  <AnimatePresence mode="popLayout">
                    {yearSubjects.map((subjectGrade, index) => (
                      <motion.div
                        key={subjectGrade.subjectId}
                        initial={{ opacity: 0, scale: 0.9 }}
                        animate={{ opacity: 1, scale: 1 }}
                        exit={{ opacity: 0, scale: 0.9 }}
                        transition={{ delay: index * 0.05 }}
                        layout
                      >
                        <Card className="h-full hover:shadow-lg transition-shadow">
                          <CardHeader>
                            <div className="flex items-start justify-between">
                              <div>
                                <CardTitle className="text-lg">{subjectGrade.subjectName}</CardTitle>
                                <div className="flex items-center gap-2 mt-1">
                                  <span className="text-xs font-mono text-muted-foreground px-2 py-0.5 bg-muted rounded">
                                    {subjectGrade.subjectCode}
                                  </span>
                                  <span className="text-xs text-muted-foreground">
                                    Prof. {subjectGrade.professorName}
                                  </span>
                                </div>
                              </div>
                              {subjectGrade.averageGrade && (
                                <Badge className={`${getGradeColor(subjectGrade.averageGrade)} text-lg px-3 py-1`}>
                                  {subjectGrade.averageGrade.toFixed(2)}
                                </Badge>
                              )}
                            </div>
                          </CardHeader>
                          <CardContent>
                            <div className="space-y-3">
                              <div className="flex items-center gap-2 text-sm text-muted-foreground">
                                <span className="font-medium">Grades:</span>
                              </div>
                              <div className="flex flex-wrap gap-2">
                                {subjectGrade.grades.map((grade) => (
                                  <div
                                    key={grade.id}
                                    className={`relative group cursor-help px-3 py-2 rounded-lg border-2 ${getGradeBorderColor(grade.value)} bg-background`}
                                  >
                                    <span className="font-bold text-lg">{grade.value.toFixed(2)}</span>

                                    {/* Tooltip */}
                                    <div className="absolute bottom-full left-1/2 -translate-x-1/2 mb-2 hidden group-hover:block z-10">
                                      <div className="bg-popover text-popover-foreground text-xs rounded-lg shadow-lg p-3 whitespace-nowrap border">
                                        <div className="flex items-center gap-2 mb-1">
                                          <Calendar className="h-3 w-3" />
                                          {new Date(grade.createdAt).toLocaleDateString('en-US', {
                                            day: '2-digit',
                                            month: 'short',
                                            year: 'numeric',
                                          })}
                                        </div>
                                        {grade.comments && (
                                          <div className="text-muted-foreground mt-1 max-w-[200px] truncate">
                                            {grade.comments}
                                          </div>
                                        )}
                                      </div>
                                    </div>
                                  </div>
                                ))}
                              </div>
                            </div>
                          </CardContent>
                        </Card>
                      </motion.div>
                    ))}
                  </AnimatePresence>
                </div>
              </motion.div>
            );
          })}
        </div>
      </div>
    </Layout>
  );
};

export default StudentGrades;