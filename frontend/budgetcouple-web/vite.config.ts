import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'
import { VitePWA } from 'vite-plugin-pwa'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    VitePWA({
      registerType: 'autoUpdate',
      includeAssets: ['favicon.ico'],
      manifest: {
        name: 'BudgetCouple',
        short_name: 'BudgetCouple',
        description: 'App de Gestão de Despesas Pessoais Compartilhadas',
        theme_color: '#0f172a',
        background_color: '#ffffff',
        display: 'standalone',
        start_url: '/',
        icons: []
      }
    })
  ],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  },
  server: {
    port: 5173
  }
})
