# E2E Tests

Playwright-based end-to-end tests for the Medicare web app.

## Scripts

```bash
npm run test:e2e
npm run test:e2e:headed
npm run test:e2e:debug
```

## Setup

```bash
npm install
npx playwright install chromium
```

## Notes

- Tests run in Chromium.
- Browser launches in non-headless mode by default.
- Screenshots are captured automatically on failure.
- The suite includes one live app flow against `http://localhost:3000` and one local form fixture for fill/submission coverage.
