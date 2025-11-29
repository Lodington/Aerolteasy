import { defineConfig } from 'vite'
import { svelte } from '@sveltejs/vite-plugin-svelte'
import tailwindcss from "@tailwindcss/vite";

export default defineConfig({
  plugins: [svelte(), tailwindcss()],
  root: './',
  server: {
    port: 5173,
    host: true,
    strictPort: true
  },
  build: {
    outDir: 'dist',
    assetsDir: 'assets',
    target: 'esnext',
    minify: !process.env.TAURI_DEBUG ? 'esbuild' : false,
    sourcemap: !!process.env.TAURI_DEBUG,
    rollupOptions: {
      input: './index.html'
    }
  },
  clearScreen: false,
  envPrefix: ['VITE_', 'TAURI_']
})