import { useNavigate } from 'react-router-dom';
import { Button } from '@/components/Button';
import { Card } from '@/components/Card';
import { useAuth } from '@/context/AuthContext';
import { spacing } from '@/styles/tokens';

export default function AdminPage() {
  const navigate = useNavigate();
  const { user, logout, isLoading } = useAuth();

  const handleLogout = async () => {
    await logout();
    navigate('/');
  };

  return (
    <div style={{ display: 'grid', gap: spacing[4], maxWidth: '720px', margin: '0 auto' }}>
      <Card title="Admin Dashboard">
        <p>Welcome, {user?.fullName ?? 'Admin'}.</p>
        <p>Email: {user?.email}</p>
        <p>Role: {user?.role}</p>
      </Card>

      <Button label={isLoading ? 'Logging out...' : 'Logout'} onClick={handleLogout} disabled={isLoading} variant="secondary" />
    </div>
  );
}
