import { useEffect, useState } from "react";
import { useSearchParams, useNavigate } from "react-router-dom";
import { motion } from "framer-motion";
import { CheckCircle2, XCircle, Loader2, ArrowRight } from "lucide-react";
import { Button } from "../components/ui/Button";
import {
  Card,
  CardHeader,
  CardTitle,
  CardDescription,
  CardContent,
} from "../components/ui/Card";
import { ThemeToggle } from "../components/ThemeToggle";

const API_BASE_URL = "http://localhost:5099/api";

export default function ConfirmEmail() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState<"loading" | "success" | "error">(
    "loading",
  );
  const [message, setMessage] = useState("");

  useEffect(() => {
    let mounted = true;

    const confirmEmail = async () => {
      const userId = searchParams.get("userId");
      const token = searchParams.get("token");

      if (!userId || !token) {
        if (mounted) {
          setStatus("error");
          setMessage("Invalid link - missing parameters");
        }
        return;
      }

      try {
        const response = await fetch(
          `${API_BASE_URL}/auth/confirm-email?userId=${userId}&token=${encodeURIComponent(token)}`,
        );

        const data = await response.json();

        if (!mounted) return; // Prevent state update if component unmounted

        if (response.ok && data.success) {
          setStatus("success");
          setMessage(data.message || "Email confirmed successfully!");
        } else {
          setStatus("error");
          setMessage(data.message || "Confirmation error");
        }
      } catch (error) {
        if (mounted) {
          setStatus("error");
          setMessage("Server connection error");
        }
      }
    };

    confirmEmail();

    return () => {
      mounted = false;
    };
  }, [searchParams]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-primary/10 via-background to-purple-500/10 p-4">
      <div className="absolute top-4 right-4">
        <ThemeToggle />
      </div>

      <motion.div
        initial={{ opacity: 0, scale: 0.95 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 0.3 }}
        className="w-full max-w-md"
      >
        <Card className="shadow-2xl">
          <CardHeader className="space-y-3">
            <div className="flex items-center justify-center space-x-2 mb-2">
              <motion.div
                initial={{ rotate: 0 }}
                animate={{ rotate: status === "loading" ? 360 : 0 }}
                transition={{
                  duration: 0.5,
                  repeat: status === "loading" ? Infinity : 0,
                }}
                className="w-12 h-12 flex items-center justify-center"
              >
                <svg viewBox="0 0 200 200" className="w-full h-full">
                  <circle cx="100" cy="100" r="90" fill="url(#grad3)" />
                  <circle cx="70" cy="70" r="25" fill="white" opacity="0.9" />
                  <circle cx="70" cy="130" r="20" fill="white" opacity="0.8" />
                  <circle cx="160" cy="140" r="20" fill="white" opacity="0.8" />
                  <circle cx="100" cy="100" r="30" fill="white" opacity="0.9" />
                  <path
                    d="M 50 80 Q 100 50 150 80"
                    stroke="white"
                    strokeWidth="8"
                    fill="none"
                    opacity="0.7"
                  />
                  <path
                    d="M 150 80 Q 180 110 160 140"
                    stroke="white"
                    strokeWidth="8"
                    fill="none"
                    opacity="0.7"
                  />
                  <path
                    d="M 50 80 Q 40 105 70 130"
                    stroke="white"
                    strokeWidth="8"
                    fill="none"
                    opacity="0.7"
                  />
                  <defs>
                    <linearGradient
                      id="grad3"
                      x1="0%"
                      y1="0%"
                      x2="100%"
                      y2="100%"
                    >
                      <stop
                        offset="0%"
                        style={{ stopColor: "#667eea", stopOpacity: 1 }}
                      />
                      <stop
                        offset="100%"
                        style={{ stopColor: "#764ba2", stopOpacity: 1 }}
                      />
                    </linearGradient>
                  </defs>
                </svg>
              </motion.div>
              <span className="text-2xl font-bold bg-gradient-to-r from-primary to-purple-600 bg-clip-text text-transparent">
                CampusConnect
              </span>
            </div>
            <CardTitle className="text-center">Email Confirmation</CardTitle>
            <CardDescription className="text-center">
              {status === "loading" && "Verifying your email..."}
              {status === "success" && "Your email has been verified!"}
              {status === "error" && "Verification failed"}
            </CardDescription>
          </CardHeader>

          <CardContent className="flex flex-col items-center space-y-6">
            {status === "loading" && (
              <motion.div
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                className="flex flex-col items-center space-y-4"
              >
                <Loader2 className="h-16 w-16 text-primary animate-spin" />
                <p className="text-sm text-muted-foreground text-center">
                  Please wait while we verify your email address...
                </p>
              </motion.div>
            )}

            {status === "success" && (
              <motion.div
                initial={{ scale: 0 }}
                animate={{ scale: 1 }}
                transition={{ type: "spring", stiffness: 200, damping: 15 }}
                className="flex flex-col items-center space-y-4"
              >
                <div className="relative">
                  <motion.div
                    initial={{ scale: 0 }}
                    animate={{ scale: 1 }}
                    transition={{ delay: 0.2 }}
                    className="absolute inset-0 bg-green-500/20 rounded-full blur-xl"
                  />
                  <div className="relative bg-green-100 dark:bg-green-900/20 p-4 rounded-full">
                    <CheckCircle2 className="h-16 w-16 text-green-600 dark:text-green-400" />
                  </div>
                </div>
                <div className="text-center space-y-2">
                  <p className="text-lg font-semibold text-green-700 dark:text-green-400">
                    Success!
                  </p>
                  <p className="text-sm text-muted-foreground max-w-sm">
                    {message}
                  </p>
                </div>
                <Button
                  onClick={() => navigate("/login")}
                  className="w-full mt-4"
                >
                  Go to Login
                  <ArrowRight className="ml-2 h-4 w-4" />
                </Button>
              </motion.div>
            )}

            {status === "error" && (
              <motion.div
                initial={{ scale: 0 }}
                animate={{ scale: 1 }}
                transition={{ type: "spring", stiffness: 200, damping: 15 }}
                className="flex flex-col items-center space-y-4"
              >
                <div className="relative">
                  <motion.div
                    initial={{ scale: 0 }}
                    animate={{ scale: 1 }}
                    transition={{ delay: 0.2 }}
                    className="absolute inset-0 bg-red-500/20 rounded-full blur-xl"
                  />
                  <div className="relative bg-red-100 dark:bg-red-900/20 p-4 rounded-full">
                    <XCircle className="h-16 w-16 text-red-600 dark:text-red-400" />
                  </div>
                </div>
                <div className="text-center space-y-2">
                  <p className="text-lg font-semibold text-red-700 dark:text-red-400">
                    Verification Failed
                  </p>
                  <p className="text-sm text-muted-foreground max-w-sm">
                    {message}
                  </p>
                </div>
                <div className="flex flex-col sm:flex-row gap-3 w-full mt-4">
                  <Button
                    variant="outline"
                    onClick={() => navigate("/register")}
                    className="flex-1"
                  >
                    Try Again
                  </Button>
                  <Button onClick={() => navigate("/login")} className="flex-1">
                    Go to Login
                  </Button>
                </div>
              </motion.div>
            )}
          </CardContent>
        </Card>
      </motion.div>
    </div>
  );
}
