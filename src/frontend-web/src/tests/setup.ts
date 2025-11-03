// src/frontend-web/src/tests/setup.ts
import '@testing-library/jest-dom';
import { vi } from 'vitest';

// Mock useRouter
vi.mock('next/navigation', () => ({
  useRouter: () => ({
    push: vi.fn(),
  }),
}));
