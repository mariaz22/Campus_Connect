import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // Proxy API requests to the backend during development to avoid CORS.
    // Backend (API) launchSettings shows http://localhost:5099 and https://localhost:7112.
    // Use http://localhost:5099 as the proxy target (or https://localhost:7112 if you prefer HTTPS
    // — if using the HTTPS target with a dev self-signed cert, set secure: false).
    proxy: {
      '/api': {
        target: 'http://localhost:5099',
        changeOrigin: true,
        secure: false
      }
      // If you want to proxy to HTTPS backend instead, use this config:
      // '/api': {
      //   target: 'https://localhost:7112',
      //   changeOrigin: true,
      //   secure: false
      // }
    }
  }
})
