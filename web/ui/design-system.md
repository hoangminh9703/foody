# UI Design System

This document defines the project's design tokens, components, and layout rules. Implementation uses TailwindCSS utility classes — components are provided as reusable React + TypeScript components under `web/src/components`.

## Color Palette

- **Primary**: Yellow — `#FFC107` (use for primary buttons, CTAs)
- **Primary-700**: `#FFB300` (hover)
- **Secondary**: Black — `#000000` (accent, headers)
- **Background**: White — `#FFFFFF`
- **Surface**: Gray-50 — `#F9FAFB` (cards, panels)
- **Text Primary**: Gray-900 — `#111827`
- **Text Secondary**: Gray-600 — `#6B7280`
- **Success**: Green — `#10B981`
- **Warning**: Amber — `#F59E0B`
- **Danger**: Red — `#EF4444`

Tailwind tokens (example mapping in `tailwind.config.cjs`):

```js
module.exports = {
  theme: {
    extend: {
      colors: {
        primary: '#FFC107',
        'primary-700': '#FFB300',
        secondary: '#000000',
        surface: '#F9FAFB',
        'text-primary': '#111827',
        'text-secondary': '#6B7280'
      }
    }
  }
}
```

## Typography

- **Font family**: system stack — `ui-sans-serif, system-ui, -apple-system, 'Segoe UI', Roboto, 'Helvetica Neue', Arial`.
- **Sizes** (tailwind classes):
  - xs: `text-xs` (0.75rem)
  - sm: `text-sm` (0.875rem)
  - base: `text-base` (1rem)
  - lg: `text-lg` (1.125rem)
  - xl: `text-xl` (1.25rem)
  - 2xl: `text-2xl` (1.5rem)
  - 3xl: `text-3xl` (1.875rem)

- **Weights**: 400 (normal), 500 (medium), 600 (semibold), 700 (bold)

Use semantic classes for headings and body copy: `.h1 = text-3xl font-semibold`, `.h2 = text-2xl font-semibold`, `.body = text-base font-normal`.

## Spacing System

Use Tailwind spacing scale (`p-1` ... `p-8`) mapped to an 4px base (default Tailwind). Recommended scales:

- 0: `0` (0px)
- 1: `0.25rem` (4px)
- 2: `0.5rem` (8px)
- 3: `0.75rem` (12px)
- 4: `1rem` (16px)
- 6: `1.5rem` (24px)
- 8: `2rem` (32px)

Always use utility spacing classes or component props — avoid inline styles.

## Reusable Components

Files shipped under `web/src/components`:
- `Button.tsx` — primary/secondary variants, size options
- `Input.tsx` — labeled input with error state
- `Card.tsx` — surface container with shadow and padding
- `Navbar.tsx` — responsive top navigation

All components use Tailwind utility classes and accept `className` to allow extension.

## Layout Rules

- **Container width**: Use Tailwind container utilities. Max widths by breakpoint:
  - `sm`: 640px
  - `md`: 768px
  - `lg`: 1024px
  - `xl`: 1280px
  - `2xl`: 1536px

- **Responsive breakpoints** (Tailwind defaults):
  - `sm` ≥ 640px
  - `md` ≥ 768px
  - `lg` ≥ 1024px
  - `xl` ≥ 1280px
  - `2xl` ≥ 1536px

Use mobile-first approach: build styles for mobile, extend at `md` and `lg` as needed.

## Accessibility

- Buttons must have discernible text and ARIA where needed.
- Inputs must be associated with `label` elements.
- Contrast: ensure text over primary color meets WCAG AA.

## Tailwind Integration Notes

1. Install Tailwind and PostCSS in `web` project (if not already):

```bash
cd web
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

2. Configure `tailwind.config.cjs` to include `./src/**/*.{js,ts,jsx,tsx}`.
3. Import Tailwind in `web/src/main.tsx` or `index.css`:

```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

## Component Usage Examples

- Primary button: `<Button variant="primary">Save</Button>`
- Input with error: `<Input label="Email" error="Invalid email" />`
- Card: `<Card className="max-w-md">...</Card>`

## Governance

- All new UI work must use these components and Tailwind classes.
- Do not use inline styles; extend components via `className` only.

---

File created by the design system task. Update as the system evolves.
