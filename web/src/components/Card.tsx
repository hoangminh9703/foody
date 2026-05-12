import React from 'react';
import { borderRadius, colors, spacing, shadows } from '@/styles/tokens';

interface CardProps extends React.HTMLAttributes<HTMLDivElement> {
  title?: string;
  children: React.ReactNode;
}

export function Card({ title, children, style, ...rest }: CardProps) {
  return (
    <div
      {...rest}
      style={{
        backgroundColor: colors.white,
        border: `1px solid ${colors.gray200}`,
        borderRadius: borderRadius.lg,
        padding: spacing[4],
        boxShadow: shadows.base,
        marginBottom: spacing[4],
        ...style,
      }}
    >
      {title && <h3 style={{ fontSize: '1.25rem', fontWeight: 600, marginBottom: spacing[3], color: colors.textPrimary }}>{title}</h3>}
      {children}
    </div>
  );
}

export default Card;
