// src/frontend-web/vitest.config.ts
import { defineConfig } from 'vitest/config';
import react from '@vitejs/plugin-react';
import tsconfigPaths from 'vite-tsconfig-paths';

export default defineConfig({
  plugins: [react(), tsconfigPaths()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/tests/setup.ts', // I'll create this file next
    css: true,
    exclude: ['**/playwright/**', '**/node_modules/**', '**/dist/**'],
  },
});
