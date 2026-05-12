export interface AuthUser {
  userId: number;
  email: string;
  fullName: string;
  role: string;
  lastLoginAt?: string | null;
  expiresAt?: string;
}

export interface AuthResponse extends AuthUser {
  success: boolean;
  message: string;
}

const jsonHeaders = {
  'Content-Type': 'application/json',
};

async function readErrorMessage(response: Response): Promise<string> {
  try {
    const payload = await response.json();
    return payload?.message || payload?.error?.message || 'Request failed';
  } catch {
    return 'Request failed';
  }
}

async function request<T>(url: string, options: RequestInit = {}): Promise<T> {
  const response = await fetch(url, {
    credentials: 'include',
    ...options,
    headers: {
      ...jsonHeaders,
      ...(options.headers || {}),
    },
  });

  if (!response.ok) {
    throw new Error(await readErrorMessage(response));
  }

  return response.json() as Promise<T>;
}

export const authService = {
  async login(email: string, password: string): Promise<AuthResponse> {
    return request<AuthResponse>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
  },

  async logout(): Promise<void> {
    await request<{ success: boolean }>('/api/auth/logout', {
      method: 'POST',
    });
  },

  async getCurrentUser(): Promise<AuthUser | null> {
    const response = await fetch('/api/auth/me', { credentials: 'include' });

    if (response.status === 401) {
      return null;
    }

    if (!response.ok) {
      throw new Error(await readErrorMessage(response));
    }

    const data = (await response.json()) as AuthResponse;
    return data;
  },

  isAuthenticated(): boolean {
    return Boolean(sessionStorage.getItem('medicare.auth.user'));
  },
};
