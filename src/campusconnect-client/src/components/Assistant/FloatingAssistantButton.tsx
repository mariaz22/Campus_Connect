import { motion, AnimatePresence } from 'framer-motion';
import { MessageCircle, X } from 'lucide-react';
import { useAssistant } from '../../contexts/AssistantContext';
import { cn } from '../../lib/utils';

export function FloatingAssistantButton() {
  const { isOpen, toggleAssistant } = useAssistant();

  return (
    <motion.button
      onClick={toggleAssistant}
      className={cn(
        'fixed bottom-6 right-6 z-50 w-14 h-14 rounded-full',
        'bg-gradient-to-br from-primary via-purple-600 to-pink-600',
        'text-white shadow-lg hover:shadow-xl',
        'flex items-center justify-center',
        'transition-all duration-300'
      )}
      whileHover={{ scale: 1.1 }}
      whileTap={{ scale: 0.95 }}
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
    >
      <AnimatePresence mode="wait">
        {isOpen ? (
          <motion.div
            key="close"
            initial={{ rotate: -90, opacity: 0 }}
            animate={{ rotate: 0, opacity: 1 }}
            exit={{ rotate: 90, opacity: 0 }}
          >
            <X className="h-6 w-6" />
          </motion.div>
        ) : (
          <motion.div
            key="open"
            initial={{ rotate: 90, opacity: 0 }}
            animate={{ rotate: 0, opacity: 1 }}
            exit={{ rotate: -90, opacity: 0 }}
          >
            <MessageCircle className="h-6 w-6" />
          </motion.div>
        )}
      </AnimatePresence>

      {/* Pulse animation when closed */}
      {!isOpen && (
        <span className="absolute inset-0 rounded-full bg-primary/30 animate-ping" />
      )}
    </motion.button>
  );
}
