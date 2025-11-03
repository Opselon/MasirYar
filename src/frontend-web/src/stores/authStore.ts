// src/frontend-web/src/stores/authStore.ts
import { create } from 'zustand';
import { immer } from 'zustand/middleware/immer';

interface AuthState {
  token: string | null;
  setToken: (token: string | null) => void;
}

export const useAuthStore = create<AuthState>()(
  immer((set) => ({
    token: null,
    setToken: (token) => {
      set((state) => {
        state.token = token;
      });
    },
  }))
);
