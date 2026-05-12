import { colors, typography, spacing, borderRadius } from './tokens';

const globalStyles = `
  * {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
  }

  html, body, #root {
    width: 100%;
    height: 100%;
  }

  body {
    font-family: ${typography.fontFamily.base};
    font-size: ${typography.fontSize.base};
    line-height: ${typography.lineHeight.normal};
    color: ${colors.textPrimary};
    background-color: ${colors.bgPrimary};
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
  }

  a {
    color: ${colors.yellow};
    text-decoration: none;
    transition: opacity 0.2s ease;

    &:hover {
      opacity: 0.8;
    }
  }

  button {
    font-family: ${typography.fontFamily.base};
    cursor: pointer;
    border: none;
    border-radius: ${borderRadius.md};
    transition: all 0.2s ease;
  }

  input, textarea, select {
    font-family: ${typography.fontFamily.base};
    border: 1px solid ${colors.gray300};
    border-radius: ${borderRadius.md};
    padding: ${spacing[2]} ${spacing[3]};

    &:focus {
      outline: none;
      border-color: ${colors.yellow};
      box-shadow: 0 0 0 3px rgba(255, 193, 7, 0.1);
    }
  }

  h1, h2, h3, h4, h5, h6 {
    font-weight: ${typography.fontWeight.bold};
    line-height: ${typography.lineHeight.tight};
  }

  h1 { font-size: ${typography.fontSize['4xl']}; }
  h2 { font-size: ${typography.fontSize['3xl']}; }
  h3 { font-size: ${typography.fontSize['2xl']}; }
  h4 { font-size: ${typography.fontSize.xl}; }
  h5 { font-size: ${typography.fontSize.lg}; }
  h6 { font-size: ${typography.fontSize.base}; }
`;

export default globalStyles;
