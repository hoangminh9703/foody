# Changelog

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



