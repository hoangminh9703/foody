import React from 'react';
import { borderRadius, colors, spacing } from '@/styles/tokens';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
  label?: string;
  error?: string | null;
}

export const Input: React.FC<InputProps> = ({ label, error = null, style, ...rest }) => {
  return (
    <div style={{ display: 'flex', flexDirection: 'column' }}>
      {label && (
        <label style={{ marginBottom: spacing[1], fontSize: '0.875rem', fontWeight: 600, color: colors.textPrimary }}>
          {label}
        </label>
      )}
      <input
        {...rest}
        style={{
          padding: `${spacing[2]} ${spacing[3]}`,
          borderRadius: borderRadius.md,
          border: `1px solid ${error ? colors.error : colors.gray300}`,
          fontSize: '1rem',
          outline: 'none',
          ...style,
        }}
      />
      {error && <p style={{ marginTop: spacing[1], fontSize: '0.875rem', color: colors.error }}>{error}</p>}
    </div>
  );
};

export default Input;
