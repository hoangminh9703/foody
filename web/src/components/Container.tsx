import { spacing } from '@/styles/tokens';

interface ContainerProps {
  children: React.ReactNode;
  maxWidth?: string;
}

const getContainerStyles = (maxWidth?: string): React.CSSProperties => ({
  maxWidth: maxWidth || '1200px',
  margin: '0 auto',
  padding: `0 ${spacing[4]}`,
  width: '100%',
});

export function Container({ children, maxWidth }: ContainerProps) {
  return <div style={getContainerStyles(maxWidth)}>{children}</div>;
}
