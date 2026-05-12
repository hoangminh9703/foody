# Changelog

## 2026-05-12 - Phase 4 Order Management API

- Implemented order creation, lookup, list filtering, and status update backend APIs.
- Added order DTOs, request models, validators, CQRS handlers, and application service/repository layers.
- Added `OrderController` endpoints:
	- `POST /api/orders` (public)
	- `GET /api/orders` (admin only)
	- `GET /api/orders/{id}` (admin only)
	- `PATCH /api/orders/{id}/status` (admin only)
- Updated session middleware to allow anonymous order creation and CORS preflight requests.

## 2026-05-12 - Phase 3 Menu Management API

- Implemented Menu Management backend API with CQRS-style handlers for create, read, update, delete, and list with pagination/filtering.
- Added menu DTOs, request models, and paged result contracts in the Application layer.
- Added `IMenuRepository` and `MenuRepository` for EF Core menu persistence with eager-loading of menu items.
- Added `IMenuService` and `MenuService` with business rules:
	- no duplicate menu for same date + meal type
	- no create/update/delete for past dates
	- menu must include at least one item
	- item price must be >= 0 and max 2 decimal places
- Added `MenuController` endpoints:
	- `POST /api/menus`
	- `GET /api/menus`
	- `GET /api/menus/{id}`
	- `PUT /api/menus/{id}`
	- `DELETE /api/menus/{id}`
- Registered menu services/repositories/handlers in API dependency injection.

## 2026-05-12 - CD Temporarily Disabled

- Updated [.github/workflows/cd.yml](../../.github/workflows/cd.yml) to run manual only (`workflow_dispatch`).
- CI in [.github/workflows/ci.yml](../../.github/workflows/ci.yml) remains active for push and pull request validation.

## 2026-05-12 - CI/CD Pipeline

- Added GitHub Actions CI for backend restore/build/test and frontend lint/build.
- Added GitHub Actions CD for SSH-based VPS deployments to staging and production.
- Added a root docker-compose.yml for backend, PostgreSQL, and frontend services on a shared internal network.
- Switched the frontend Dockerfile to npm ci after adding the lockfile for deterministic builds.

## 2026-05-12 - Docker Setup

- Added `backend/Dockerfile` for multi-stage .NET 8 API builds.
- Added `web/Dockerfile` and `web/nginx.conf` for a production Vite build served by Nginx.
- Added `.dockerignore` files for both app roots to keep build contexts small.
- Updated `backend/src/Api/Program.cs` so HTTPS redirection stays enabled in development without breaking container HTTP startup.

## 2026-05-11 - Phase 1 Complete ✅

### Planning
- Drafted detailed Phase 1 setup plan in docs/plans/phase-1-plan.md.

### Task 1: Solution Structure
- Confirmed solution structure and created the initial folder scaffold.
- Created [docs/architecture/solution-structure.md](docs/architecture/solution-structure.md).

### Task 2: Backend Foundation
- Initialized ASP.NET Core .NET 8 backend with Clean Architecture projects.
- Created Medicare.sln with 5 projects (Domain, Application, Infrastructure, Api, Tests).
- Configured appsettings for Development and Production.
- Implemented health check endpoint and structured logging.

### Task 3: Frontend Foundation
- Initialized React + TypeScript frontend with Vite.
- Configured routing with React Router.
- Created token-based design system (White, Yellow, Black palette).
- Implemented base layout and reusable UI components (Button, Card, Container).

### Task 4: Development Tooling
- Configured ESLint with TypeScript and React support.
- Set up Prettier for code formatting.
- Added EditorConfig for consistent editor settings.
- Created development startup scripts (Windows and Unix).
- Wrote comprehensive [DEV_SETUP.md](docs/DEV_SETUP.md).

### Task 5: Data Model & Schema
- Designed 6 core domain entities: User, Menu, MenuItem, OrderRequest, OrderRequestItem, SiteContent.
- Configured MedicareDbContext with all relationships and constraints.
- Created SQL migration script [01_InitialSchema.sql](backend/Database/01_InitialSchema.sql).
- Wrote [DATABASE_SETUP.md](docs/DATABASE_SETUP.md) with complete schema documentation.

### Task 6: Documentation Baseline
- Created [docs/architecture/ARCHITECTURE.md](docs/architecture/ARCHITECTURE.md) - system architecture overview.
- Created [docs/architecture/DOMAIN_MODEL.md](docs/architecture/DOMAIN_MODEL.md) - detailed entity documentation.
- Created [docs/architecture/API_DESIGN.md](docs/architecture/API_DESIGN.md) - REST API specification for all phases.
- Created [docs/PHASE_1_COMPLETE.md](docs/PHASE_1_COMPLETE.md) - phase completion report.
- Created [docs/PHASE_1_VALIDATION.md](docs/PHASE_1_VALIDATION.md) - validation checklist.
- Updated [README.md](README.md) with project overview and quick start.
- Updated [master-plan.md](docs/plans/master-plan.md) marking Phase 1 as complete.

### Phase 1 Summary
**Status**: ✅ COMPLETE  
**Blockers**: None  
**Ready for Phase 2**: YES  
**Date Completed**: 2026-05-11

Phase 1 setup is complete. The project has a solid foundation with:
- ✅ Complete backend and frontend scaffold
- ✅ Database schema with all core entities
- ✅ Development environment fully configured
- ✅ Comprehensive architecture and API documentation
- ✅ Ready to begin Phase 2: Authentication

## 2026-05-11 - Fix

- Fixed TypeScript config warning: set `noEmit: true` to allow `allowImportingTsExtensions`.

## 2026-05-11 - Tests

- Added unit and integration tests for `HealthController` (Phase 1):
	- `backend/tests/HealthControllerUnitTests.cs`
	- `backend/tests/HealthIntegrationTests.cs`
	- Updated test project packages to include `FluentAssertions` and `Microsoft.AspNetCore.Mvc.Testing`.
	- Added `public partial class Program { }` to `backend/src/Api/Program.cs` to support `WebApplicationFactory`.
	- Fixed missing logging provider packages required for build (`Microsoft.Extensions.Logging.Console`, `Microsoft.Extensions.Logging.Debug`).

	## 2026-05-11 - Phase 2 DB Config

	- Switched backend DB provider to PostgreSQL (Npgsql).
	- Updated `backend/src/Api/appsettings.json` with example PostgreSQL connection string: `Host=localhost;Port=5432;Database=foody;Username=postgres;Password=root123`.
	- Added `Npgsql.EntityFrameworkCore.PostgreSQL` package to `Medicare.Infrastructure`.
	- Updated `Program.cs` to use `UseNpgsql(connectionString)`.

	## 2026-05-11 - Design System

	- Added UI design system documentation at `web/ui/design-system.md`.
	- Added Tailwind-ready reusable components under `web/src/components`:
		- `Button.tsx`
		- `Input.tsx`
		- `Card.tsx`
		- `Navbar.tsx`

	## 2026-05-11 - E2E Testing

	- Added standalone Playwright E2E project in `web/e2e-tests`.
	- Added Chromium-based tests for:
		- loading the app at `http://localhost:3000`
		- clicking the home CTA
		- navigating between pages
		- filling and submitting a form fixture
	- Configured non-headless browser execution and failure screenshots.



