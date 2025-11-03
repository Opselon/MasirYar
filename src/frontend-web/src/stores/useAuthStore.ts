// src/frontend-web/src/stores/useAuthStore.ts
import { create } from 'zustand';
import { immer } from 'zustand/middleware/immer';

// Define the types for our state
interface User {
  id: string;
  username: string;
  email: string;
}

interface AuthState {
  user: User | null;
  token: string | null;
}

interface AuthActions {
  login: (user: User, token: string) => void;
  logout: () => void;
  setUser: (user: User) => void;
}

// Create the store
export const useAuthStore = create<AuthState & { actions: AuthActions }>()(
  immer((set) => ({
    user: null,
    token: null,
    actions: {
      login: (user, token) =>
        set((state) => {
          state.user = user;
          state.token = token;
          // In a real app, you'd also persist the token (e.g., in localStorage)
        }),
      logout: () =>
        set((state) => {
          state.user = null;
          state.token = null;
          // And remove the token from persistence
        }),
      setUser: (user) =>
        set((state) => {
          state.user = user;
        }),
    },
  }))
);
