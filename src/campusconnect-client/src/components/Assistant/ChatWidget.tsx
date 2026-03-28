import { useState, useRef, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { Send, Loader2, Bot, User, Trash2, Sparkles } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { useAssistant } from '../../contexts/AssistantContext';
import { Card, CardHeader, CardTitle, CardContent } from '../ui/Card';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { Badge } from '../ui/Badge';
import { cn } from '../../lib/utils';
import type { ChatMessage, SuggestedAction } from '../../services/assistantApi';

export function ChatWidget() {
  const { isOpen, messages, isLoading, sendMessage, clearChat } = useAssistant();
  const [input, setInput] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const navigate = useNavigate();

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  useEffect(() => {
    if (isOpen) {
      inputRef.current?.focus();
    }
  }, [isOpen]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!input.trim() || isLoading) return;

    const message = input.trim();
    setInput('');
    await sendMessage(message);
  };

  const handleActionClick = (action: SuggestedAction) => {
    if (action.actionType === 'navigate' && action.payload) {
      navigate(action.payload);
    } else if (action.actionType === 'link' && action.payload) {
      window.open(action.payload, '_blank');
    } else if (action.actionType === 'query' && action.payload) {
      sendMessage(action.payload);
    }
  };

  const handleSuggestedQuestion = (question: string) => {
    sendMessage(question);
  };

  const suggestedQuestions = [
    "How do I join a group?",
    "Where is the campus map?",
    "Show my tasks"
  ];

  return (
    <AnimatePresence>
      {isOpen && (
        <motion.div
          initial={{ opacity: 0, y: 20, scale: 0.95 }}
          animate={{ opacity: 1, y: 0, scale: 1 }}
          exit={{ opacity: 0, y: 20, scale: 0.95 }}
          className="fixed bottom-24 right-6 z-50 w-96 max-w-[calc(100vw-3rem)]"
        >
          <Card className="shadow-2xl border-2 overflow-hidden">
            {/* Header */}
            <CardHeader className="bg-gradient-to-r from-primary via-purple-600 to-pink-600 text-white p-4">
              <div className="flex items-center justify-between">
                <CardTitle className="flex items-center gap-2 text-lg text-white">
                  <Bot className="h-5 w-5" />
                  Campus Assistant
                  <Sparkles className="h-4 w-4 text-yellow-300" />
                </CardTitle>
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={clearChat}
                  className="h-8 w-8 text-white/80 hover:text-white hover:bg-white/20"
                >
                  <Trash2 className="h-4 w-4" />
                </Button>
              </div>
            </CardHeader>

            {/* Messages */}
            <CardContent className="p-0">
              <div className="h-80 overflow-y-auto p-4 space-y-4 bg-gradient-to-b from-background to-secondary/20">
                {messages.length === 0 && (
                  <motion.div
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    className="text-center py-8"
                  >
                    <Bot className="h-12 w-12 mx-auto text-muted-foreground/50 mb-3" />
                    <p className="text-sm text-muted-foreground mb-4">
                      Hi! I'm your campus assistant. How can I help you today?
                    </p>
                    <div className="space-y-2">
                      {suggestedQuestions.map((question, index) => (
                        <motion.button
                          key={question}
                          initial={{ opacity: 0, x: -10 }}
                          animate={{ opacity: 1, x: 0 }}
                          transition={{ delay: index * 0.1 }}
                          onClick={() => handleSuggestedQuestion(question)}
                          className="block w-full text-left px-3 py-2 text-sm rounded-lg bg-secondary hover:bg-secondary/80 transition-colors"
                        >
                          {question}
                        </motion.button>
                      ))}
                    </div>
                  </motion.div>
                )}

                {messages.map((message, index) => (
                  <MessageBubble
                    key={message.id}
                    message={message}
                    onActionClick={handleActionClick}
                    index={index}
                  />
                ))}

                {isLoading && (
                  <motion.div
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    className="flex items-center gap-2 text-muted-foreground"
                  >
                    <div className="p-2 rounded-full bg-primary/10">
                      <Bot className="h-4 w-4 text-primary" />
                    </div>
                    <div className="flex items-center gap-1">
                      <Loader2 className="h-4 w-4 animate-spin" />
                      <span className="text-sm">Thinking...</span>
                    </div>
                  </motion.div>
                )}

                <div ref={messagesEndRef} />
              </div>

              {/* Input */}
              <form onSubmit={handleSubmit} className="p-4 border-t bg-background">
                <div className="flex gap-2">
                  <Input
                    ref={inputRef}
                    value={input}
                    onChange={(e) => setInput(e.target.value)}
                    placeholder="Ask me anything..."
                    disabled={isLoading}
                    className="flex-1"
                  />
                  <Button
                    type="submit"
                    disabled={!input.trim() || isLoading}
                    size="icon"
                  >
                    <Send className="h-4 w-4" />
                  </Button>
                </div>
              </form>
            </CardContent>
          </Card>
        </motion.div>
      )}
    </AnimatePresence>
  );
}

function MessageBubble({
  message,
  onActionClick,
  index
}: {
  message: ChatMessage;
  onActionClick: (action: SuggestedAction) => void;
  index: number;
}) {
  const isUser = message.role === 'user';

  return (
    <motion.div
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: index * 0.05 }}
      className={cn('flex gap-2', isUser && 'flex-row-reverse')}
    >
      <div className={cn(
        'p-2 rounded-full shrink-0',
        isUser ? 'bg-primary' : 'bg-primary/10'
      )}>
        {isUser ? (
          <User className="h-4 w-4 text-white" />
        ) : (
          <Bot className="h-4 w-4 text-primary" />
        )}
      </div>

      <div className={cn(
        'max-w-[80%] rounded-2xl px-4 py-2',
        isUser
          ? 'bg-primary text-primary-foreground rounded-br-sm'
          : 'bg-secondary rounded-bl-sm'
      )}>
        <p className="text-sm whitespace-pre-wrap">{message.content}</p>

        {message.suggestedActions && message.suggestedActions.length > 0 && (
          <div className="mt-2 flex flex-wrap gap-1">
            {message.suggestedActions.map((action, idx) => (
              <Badge
                key={idx}
                variant="secondary"
                className="cursor-pointer hover:bg-primary hover:text-primary-foreground transition-colors"
                onClick={() => onActionClick(action)}
              >
                {action.label}
              </Badge>
            ))}
          </div>
        )}
      </div>
    </motion.div>
  );
}
