# Solution Structure - Phase 1

## Confirmed layout

### Backend
- `backend/src/Domain`
- `backend/src/Application`
- `backend/src/Infrastructure`
- `backend/src/Api`
- `backend/tests`

### Frontend
- `web/src/app`
- `web/src/components`
- `web/src/features`
- `web/src/styles`

## Responsibility split

### Domain
Holds core business entities and domain rules.

### Application
Holds use cases, application services, and orchestration logic.

### Infrastructure
Holds persistence, external integrations, and framework-specific implementations.

### Api
Holds HTTP endpoints, composition root, and API-level configuration.

### Frontend app
Holds the React shell, routes, and top-level page composition.

### Frontend components
Holds reusable UI components shared across screens.

### Frontend features
Holds feature-oriented modules for landing page, request form, and admin screens.

### Frontend styles
Holds global styles, tokens, and shared styling utilities.

## Notes
- This structure is the basis for Clean Architecture on the backend and React + TypeScript on the frontend.
- Later phase tasks will fill these folders with actual project files and implementation code.
