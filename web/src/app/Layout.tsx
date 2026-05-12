import { ReactNode } from 'react';
import Navbar from '@/components/Navbar';
import { colors, spacing } from '@/styles/tokens';

interface LayoutProps {
  children: ReactNode;
}

const layoutStyles: React.CSSProperties = {
  display: 'flex',
  flexDirection: 'column',
  minHeight: '100vh',
  backgroundColor: colors.bgPrimary,
};

const mainStyles: React.CSSProperties = {
  flex: 1,
  padding: `${spacing[6]} ${spacing[6]}`,
};

const footerStyles: React.CSSProperties = {
  backgroundColor: colors.gray900,
  color: colors.white,
  padding: `${spacing[6]} ${spacing[6]}`,
  textAlign: 'center',
  borderTop: `1px solid ${colors.gray800}`,
};

export default function Layout({ children }: LayoutProps) {
  return (
    <div style={layoutStyles}>
      <header>
        <Navbar />
      </header>
      <main style={mainStyles}>{children}</main>
      <footer style={footerStyles}>
        <p>&copy; 2026 Medicare. All rights reserved.</p>
      </footer>
    </div>
  );
}
