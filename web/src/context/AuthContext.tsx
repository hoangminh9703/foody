import React, { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { authService, type AuthUser } from '@/services/authService';

interface AuthContextValue {
  isAuthenticated: boolean;
  user: AuthUser | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  isLoading: boolean;
  error: string | null;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);
const STORAGE_KEY = 'medicare.auth.user';

function loadCachedUser(): AuthUser | null {
  const rawValue = sessionStorage.getItem(STORAGE_KEY);

  if (!rawValue) {
    return null;
  }

  try {
    return JSON.parse(rawValue) as AuthUser;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(() => {
    if (typeof window === 'undefined') {
      return null;
    }

    return loadCachedUser();
  });
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const persistUser = useCallback((nextUser: AuthUser | null) => {
    if (nextUser) {
      sessionStorage.setItem(STORAGE_KEY, JSON.stringify(nextUser));
    } else {
      sessionStorage.removeItem(STORAGE_KEY);
    }
  }, []);

  const refreshUser = useCallback(async () => {
    try {
      const currentUser = await authService.getCurrentUser();
      setUser(currentUser);
      persistUser(currentUser);
    } catch (refreshError) {
      console.error('Failed to load current user', refreshError);
      setUser(null);
      persistUser(null);
    } finally {
      setIsLoading(false);
    }
  }, [persistUser]);

  useEffect(() => {
    void refreshUser();
  }, [refreshUser]);

  const login = useCallback(async (email: string, password: string) => {
    setIsLoading(true);
    setError(null);

    try {
      const response = await authService.login(email, password);
      const nextUser: AuthUser = {
        userId: response.userId,
        email: response.email,
        fullName: response.fullName,
        role: response.role,
        lastLoginAt: response.lastLoginAt,
        expiresAt: response.expiresAt,
      };

      setUser(nextUser);
      persistUser(nextUser);
    } catch (loginError) {
      const message = loginError instanceof Error ? loginError.message : 'Unable to log in';
      setError(message);
      throw loginError;
    } finally {
      setIsLoading(false);
    }
  }, [persistUser]);

  const logout = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      await authService.logout();
    } finally {
      setUser(null);
      persistUser(null);
      setIsLoading(false);
    }
  }, [persistUser]);

  const value = useMemo<AuthContextValue>(() => ({
    isAuthenticated: Boolean(user),
    user,
    login,
    logout,
    isLoading,
    error,
  }), [error, isLoading, login, logout, user]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }

  return context;
}
