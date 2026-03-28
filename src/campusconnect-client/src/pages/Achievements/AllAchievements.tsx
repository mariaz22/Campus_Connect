import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Trophy, Lock } from 'lucide-react';
import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { AchievementCard } from '../../components/AchievementCard';
import achievementApi, { type Achievement, type UserAchievement } from '../../services/achievementApi';

function AllAchievements() {
  const [allAchievements, setAllAchievements] = useState<Achievement[]>([]);
  const [myAchievements, setMyAchievements] = useState<UserAchievement[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchAchievements = async () => {
      setLoading(true);
      try {
        const [all, my] = await Promise.all([
          achievementApi.getAllAchievements(),
          achievementApi.getMyAchievements(),
        ]);
        setAllAchievements(all);
        setMyAchievements(my);
      } catch (error) {
        console.error('Failed to fetch achievements', error);
      } finally {
        setLoading(false);
      }
    };

    fetchAchievements();
  }, []);

  const unlockedIds = myAchievements.map(a => a.achievementId);

  const unlockedAchievements = allAchievements.filter(a => unlockedIds.includes(a.id));
  const lockedAchievements = allAchievements.filter(a => !unlockedIds.includes(a.id));

  const getUnlockedDate = (achievementId: number) => {
    const userAchievement = myAchievements.find(a => a.achievementId === achievementId);
    return userAchievement?.unlockedAt;
  };

  if (loading) {
    return (
      <Layout>
        <div className="flex items-center justify-center min-h-[60vh]">
          <div className="text-center">
            <Trophy className="h-12 w-12 mx-auto mb-4 text-yellow-500 animate-bounce" />
            <p className="text-muted-foreground">Loading achievements...</p>
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="space-y-8">
        {/* Hero Section */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-yellow-500 via-orange-500 to-red-500 p-8 text-white shadow-2xl"
        >
          <div className="relative z-10">
            <div className="flex items-center gap-4 mb-4">
              <div className="p-4 rounded-2xl bg-white/20 backdrop-blur-sm">
                <Trophy className="h-12 w-12" />
              </div>
              <div>
                <h1 className="text-4xl font-bold">Achievements</h1>
                <p className="text-white/90 text-lg mt-1">
                  You've unlocked {myAchievements.length} of {allAchievements.length} achievements
                </p>
              </div>
            </div>

            {/* Progress Bar */}
            <div className="mt-6 bg-white/20 rounded-full h-3 overflow-hidden">
              <motion.div
                initial={{ width: 0 }}
                animate={{ width: `${(myAchievements.length / allAchievements.length) * 100}%` }}
                transition={{ duration: 1, delay: 0.5 }}
                className="h-full bg-white rounded-full"
              />
            </div>
            <p className="text-white/80 text-sm mt-2 text-right">
              {Math.round((myAchievements.length / allAchievements.length) * 100)}% Complete
            </p>
          </div>
        </motion.div>

        {/* Unlocked Achievements */}
        {unlockedAchievements.length > 0 && (
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.2 }}
          >
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-yellow-600 dark:text-yellow-400">
                  <Trophy className="h-6 w-6" />
                  Unlocked Achievements ({unlockedAchievements.length})
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  {unlockedAchievements.map((achievement) => (
                    <AchievementCard
                      key={achievement.id}
                      achievement={achievement}
                      unlocked={true}
                      unlockedAt={getUnlockedDate(achievement.id)}
                    />
                  ))}
                </div>
              </CardContent>
            </Card>
          </motion.div>
        )}

        {/* Locked Achievements */}
        {lockedAchievements.length > 0 && (
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.3 }}
          >
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-gray-500 dark:text-gray-400">
                  <Lock className="h-6 w-6" />
                  Locked Achievements ({lockedAchievements.length})
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  {lockedAchievements.map((achievement) => (
                    <AchievementCard
                      key={achievement.id}
                      achievement={achievement}
                      unlocked={false}
                    />
                  ))}
                </div>
              </CardContent>
            </Card>
          </motion.div>
        )}

        {allAchievements.length === 0 && (
          <div className="text-center py-12">
            <Trophy className="h-16 w-16 mx-auto mb-4 text-gray-300 dark:text-gray-700" />
            <p className="text-muted-foreground">No achievements available yet.</p>
          </div>
        )}
      </div>
    </Layout>
  );
}

export default AllAchievements;
