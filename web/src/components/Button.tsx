import React from 'react';
import { borderRadius, colors, spacing } from '@/styles/tokens';

type Variant = 'primary' | 'secondary' | 'danger';
type Size = 'sm' | 'md' | 'lg';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: Variant;
  size?: Size;
  label?: string;
}

const getVariantStyles = (variant: Variant): React.CSSProperties => {
  switch (variant) {
    case 'secondary':
      return {
        backgroundColor: colors.gray200,
        color: colors.black,
      };
    case 'danger':
      return {
        backgroundColor: colors.error,
        color: colors.white,
      };
    case 'primary':
    default:
      return {
        backgroundColor: colors.yellow,
        color: colors.black,
      };
  }
};

const getSizeStyles = (size: Size): React.CSSProperties => {
  switch (size) {
    case 'sm':
      return { padding: `${spacing[1]} ${spacing[3]}`, fontSize: '0.875rem' };
    case 'lg':
      return { padding: `${spacing[3]} ${spacing[6]}`, fontSize: '1.125rem' };
    case 'md':
    default:
      return { padding: `${spacing[2]} ${spacing[4]}`, fontSize: '1rem' };
  }
};

export function Button({
  variant = 'primary',
  size = 'md',
  label,
  children,
  disabled = false,
  style,
  ...rest
}: ButtonProps) {
  return (
    <button
      {...rest}
      disabled={disabled}
      style={{
        ...getVariantStyles(variant),
        ...getSizeStyles(size),
        border: 'none',
        borderRadius: borderRadius.md,
        cursor: disabled ? 'not-allowed' : 'pointer',
        opacity: disabled ? 0.6 : 1,
        fontWeight: 600,
        transition: 'opacity 0.2s ease',
        ...style,
      }}
    >
      {children ?? label}
    </button>
  );
}

export default Button;
