import { FormEvent, useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '@/components/Button';
import { Card } from '@/components/Card';
import { Input } from '@/components/Input';
import { useAuth } from '@/context/AuthContext';
import { colors, spacing } from '@/styles/tokens';

export default function LoginPage() {
  const navigate = useNavigate();
  const { login, isAuthenticated, isLoading, error } = useAuth();
  const [email, setEmail] = useState('admin@medicare.local');
  const [password, setPassword] = useState('Admin123!');
  const [localError, setLocalError] = useState<string | null>(null);

  useEffect(() => {
    if (isAuthenticated) {
      navigate('/admin', { replace: true });
    }
  }, [isAuthenticated, navigate]);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setLocalError(null);

    if (!email.trim() || !password.trim()) {
      setLocalError('Email and password are required.');
      return;
    }

    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
      setLocalError('Please enter a valid email address.');
      return;
    }

    try {
      await login(email, password);
      navigate('/admin', { replace: true });
    } catch {
      // AuthContext stores the error message.
    }
  };

  return (
    <div style={{ display: 'flex', justifyContent: 'center', padding: `${spacing[8]} ${spacing[4]}` }}>
      <Card title="Admin Login" style={{ width: '100%', maxWidth: '480px' }}>
        <form onSubmit={handleSubmit} style={{ display: 'grid', gap: spacing[4] }}>
          <Input
            label="Email"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            placeholder=""
          />
          <Input
            label="Password"
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            placeholder="••••••••"
          />

          {(localError || error) && (
            <div style={{ color: colors.error, fontSize: '0.95rem' }}>
              {localError || error}
            </div>
          )}

          <Button type="submit" label={isLoading ? 'Signing in...' : 'Sign in'} disabled={isLoading} />
        </form>
      </Card>
    </div>
  );
}
