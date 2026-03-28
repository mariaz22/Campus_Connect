import { motion } from 'framer-motion';
import type { UserAchievement, Achievement } from '../services/achievementApi';

interface AchievementCardProps {
  achievement: UserAchievement | Achievement;
  unlocked?: boolean;
  unlockedAt?: string;
}

export function AchievementCard({ achievement, unlocked = true, unlockedAt }: AchievementCardProps) {
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('ro-RO', {
      day: 'numeric',
      month: 'long',
      year: 'numeric',
    });
  };

  return (
    <motion.div
      initial={{ opacity: 0, scale: 0.9 }}
      animate={{ opacity: 1, scale: 1 }}
      transition={{ duration: 0.3 }}
      className={`
        relative overflow-hidden rounded-lg border p-6 shadow-sm transition-all hover:shadow-md
        ${unlocked
          ? 'bg-gradient-to-br from-purple-50 to-blue-50 dark:from-purple-950/20 dark:to-blue-950/20 border-purple-200 dark:border-purple-800'
          : 'bg-gray-100 dark:bg-gray-800 border-gray-300 dark:border-gray-700 opacity-60'
        }
      `}
    >
      {unlocked && (
        <div className="absolute top-0 right-0 w-20 h-20 bg-gradient-to-br from-yellow-400/20 to-orange-400/20 rounded-bl-full" />
      )}

      <div className="relative flex items-start gap-4">
        <div className={`
          text-5xl flex-shrink-0
          ${unlocked ? 'filter-none' : 'grayscale opacity-50'}
        `}>
          {achievement.icon}
        </div>

        <div className="flex-1 min-w-0">
          <h3 className={`
            text-lg font-semibold mb-1
            ${unlocked
              ? 'text-gray-900 dark:text-white'
              : 'text-gray-500 dark:text-gray-400'
            }
          `}>
            {achievement.title}
          </h3>

          <p className={`
            text-sm mb-2
            ${unlocked
              ? 'text-gray-700 dark:text-gray-300'
              : 'text-gray-500 dark:text-gray-500'
            }
          `}>
            {achievement.description}
          </p>

          {unlocked && unlockedAt && (
            <div className="flex items-center gap-2 text-xs text-purple-700 dark:text-purple-400">
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <span>Unlocked on {formatDate(unlockedAt)}</span>
            </div>
          )}

          {!unlocked && (
            <div className="flex items-center gap-2 text-xs text-gray-500 dark:text-gray-500">
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
              </svg>
              <span>Locked</span>
            </div>
          )}
        </div>
      </div>
    </motion.div>
  );
}
