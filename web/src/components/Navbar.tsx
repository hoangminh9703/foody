import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Button } from './Button';
import { useAuth } from '@/context/AuthContext';
import { colors, spacing } from '@/styles/tokens';

export const Navbar: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/');
  };

  return (
    <nav
      style={{
        backgroundColor: colors.black,
        color: colors.white,
        borderBottom: `2px solid ${colors.yellow}`,
      }}
    >
      <div
        style={{
          maxWidth: '1200px',
          margin: '0 auto',
          padding: `${spacing[4]} ${spacing[6]}`,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'space-between',
          gap: spacing[4],
        }}
      >
        <Link to="/" style={{ color: colors.white, fontSize: '1.25rem', fontWeight: 700, textDecoration: 'none' }}>
          Medicare
        </Link>

        <div style={{ display: 'flex', alignItems: 'center', gap: spacing[4], flexWrap: 'wrap', justifyContent: 'flex-end' }}>
          <Link to="/" style={{ color: colors.white, textDecoration: 'none' }}>
            Home
          </Link>
          {isAuthenticated ? (
            <>
              <Link to="/admin" style={{ color: colors.white, textDecoration: 'none' }}>
                Dashboard
              </Link>
              <span style={{ color: colors.yellow, fontSize: '0.875rem' }}>{user?.fullName}</span>
              <Button variant="secondary" size="sm" onClick={handleLogout} label="Logout" />
            </>
          ) : (
            <Button variant="primary" size="sm" onClick={() => navigate('/login')} label="Login" />
          )}
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
