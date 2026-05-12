import { BrowserRouter as Router, Navigate, Route, Routes, useNavigate } from 'react-router-dom';
import Layout from './Layout';
import { AuthProvider } from '@/context/AuthContext';
import { Button } from '@/components/Button';
import LoginPage from '@/pages/LoginPage';
import AdminPage from '@/pages/AdminPage';
import { ProtectedRoute } from '@/components/ProtectedRoute';
import { colors, spacing } from '@/styles/tokens';

const homePageStyles: React.CSSProperties = {
  textAlign: 'center',
};

const ctaButtonStyles: React.CSSProperties = {
  backgroundColor: colors.yellow,
  color: colors.black,
  padding: `${spacing[3]} ${spacing[6]}`,
  fontSize: '1.125rem',
  fontWeight: 600,
  border: 'none',
  borderRadius: '0.5rem',
  cursor: 'pointer',
  marginTop: spacing[4],
  transition: 'opacity 0.2s ease',
};

function HomePage() {
  const navigate = useNavigate();

  return (
    <div style={homePageStyles}>
      <h1>Chào mừng đến với Medicare</h1>
      <p>Đặt cơm ngon mỗi ngày - Giao hàng nhanh - Menu thay đổi hàng ngày</p>
      <Button label="Đặt cơm ngay" onClick={() => navigate('/login')} style={ctaButtonStyles} />
    </div>
  );
}

function App() {
  return (
    <AuthProvider>
      <Router>
        <Layout>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route
              path="/admin"
              element={
                <ProtectedRoute>
                  <AdminPage />
                </ProtectedRoute>
              }
            />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </Layout>
      </Router>
    </AuthProvider>
  );
}

export default App;
