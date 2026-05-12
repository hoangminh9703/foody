# Phase 1 Validation Checklist

Use this checklist to verify that Phase 1 setup is complete and working correctly.

## Backend Setup

### Project Structure
- [ ] `backend/Medicare.sln` exists
- [ ] `backend/src/Api/` folder exists with Medicare.Api.csproj
- [ ] `backend/src/Domain/` folder exists with Medicare.Domain.csproj
- [ ] `backend/src/Application/` folder exists with Medicare.Application.csproj
- [ ] `backend/src/Infrastructure/` folder exists with Medicare.Infrastructure.csproj
- [ ] `backend/tests/` folder exists with Medicare.Tests.csproj

### Backend Compilation
- [ ] Run `cd backend && dotnet restore` - succeeds without errors
- [ ] Run `dotnet build` - succeeds without errors
- [ ] Run `dotnet test` - test project loads (may have no tests yet)

### Configuration Files
- [ ] `src/Api/appsettings.json` exists
- [ ] `src/Api/appsettings.Development.json` exists
- [ ] `src/Api/appsettings.Production.json` exists
- [ ] All appsettings files contain valid JSON

### Database Context
- [ ] `src/Infrastructure/Data/MedicareDbContext.cs` exists
- [ ] DbSet properties exist for: Users, Menus, MenuItems, OrderRequests, OrderRequestItems, SiteContents
- [ ] Entity configuration in OnModelCreating is complete

### Domain Entities
- [ ] `src/Domain/Entities/BaseEntity.cs` exists
- [ ] `src/Domain/Entities/User.cs` exists with UserRole enum
- [ ] `src/Domain/Entities/Menu.cs` exists with MealType enum
- [ ] `src/Domain/Entities/MenuItem.cs` exists
- [ ] `src/Domain/Entities/OrderRequest.cs` exists with OrderStatus enum
- [ ] `src/Domain/Entities/OrderRequestItem.cs` exists
- [ ] `src/Domain/Entities/SiteContent.cs` exists

### API Endpoints
- [ ] `src/Api/Controllers/HealthController.cs` exists
- [ ] Run `dotnet run --project src/Api/Medicare.Api.csproj`
- [ ] Server starts on https://localhost:5001
- [ ] Visit `https://localhost:5001/swagger/ui` - Swagger UI loads
- [ ] Call `https://localhost:5001/api/health` - returns `{ "status": "healthy" }`

### Database Setup
- [ ] SQL Server is installed and running
- [ ] Databases created: Medicare, Medicare_Dev
- [ ] SQL script `backend/Database/01_InitialSchema.sql` exists
- [ ] Run the SQL script against both databases - succeeds
- [ ] All 6 tables exist in database:
  - [ ] Users (with admin@medicare.local default user)
  - [ ] Menus
  - [ ] MenuItems
  - [ ] OrderRequests
  - [ ] OrderRequestItems
  - [ ] SiteContents

### Logging
- [ ] `src/Infrastructure/Logging/LoggingExtensions.cs` exists
- [ ] Logging is configured in Program.cs
- [ ] Running backend produces console output with log messages

## Frontend Setup

### Project Structure
- [ ] `web/package.json` exists
- [ ] `web/tsconfig.json` exists
- [ ] `web/tsconfig.node.json` exists
- [ ] `web/vite.config.ts` exists
- [ ] `web/index.html` exists

### Dependencies
- [ ] Run `cd web && npm install` - completes successfully
- [ ] Check `node_modules/` folder is created
- [ ] `package-lock.json` exists

### TypeScript Compilation
- [ ] Run `npm run build` - succeeds with no errors
- [ ] `dist/` folder is created with compiled assets

### Linting & Formatting
- [ ] `.eslintrc.json` exists with React and TypeScript config
- [ ] `.prettierrc.json` exists
- [ ] `.eslintignore` exists
- [ ] `.prettierignore` exists
- [ ] `.editorconfig` exists
- [ ] Run `npm run lint` - completes without errors (or only warnings)
- [ ] Run `npm run format` - completes without errors

### Frontend Components
- [ ] `src/main.tsx` exists
- [ ] `src/app/App.tsx` exists
- [ ] `src/app/Layout.tsx` exists
- [ ] `src/styles/tokens.ts` exists with color, typography, spacing tokens
- [ ] `src/styles/global.ts` exists
- [ ] `src/components/Button.tsx` exists
- [ ] `src/components/Card.tsx` exists
- [ ] `src/components/Container.tsx` exists

### Frontend Runtime
- [ ] Run `npm run dev` - dev server starts on http://localhost:3000
- [ ] Open http://localhost:3000 in browser
- [ ] Page loads with title "Medicare - Đồ ăn theo ngày"
- [ ] Black header with yellow border is visible
- [ ] Home page displays with "Chào mừng đến với Medicare" text
- [ ] "Đặt cơm ngay" button is visible and styled in yellow

### Environment Variables
- [ ] `.env.local.example` exists
- [ ] Copy `.env.local.example` to `.env.local` (optional for dev)
- [ ] Frontend can run without .env.local (uses defaults)

## Documentation

### Architecture Docs
- [ ] `docs/architecture/solution-structure.md` exists
- [ ] `docs/architecture/ARCHITECTURE.md` exists
- [ ] `docs/architecture/DOMAIN_MODEL.md` exists
- [ ] `docs/architecture/API_DESIGN.md` exists

### Setup Guides
- [ ] `docs/DEV_SETUP.md` exists
- [ ] `docs/DATABASE_SETUP.md` exists
- [ ] `README.md` in project root exists

### Project Planning
- [ ] `docs/plans/phase-1-plan.md` exists
- [ ] `docs/plans/master-plan.md` exists
- [ ] `docs/PHASE_1_COMPLETE.md` exists

### Change Log
- [ ] `docs/CHANGELOG.md` exists
- [ ] All Phase 1 tasks are documented

## Development Scripts

### Startup Scripts
- [ ] `dev-start.bat` exists (Windows)
- [ ] `dev-start.sh` exists (Unix/Mac)
- [ ] Both scripts are executable

### Running Together
- [ ] Run `dev-start.bat` (Windows) or `./dev-start.sh` (Unix/Mac)
- [ ] Both backend and frontend start automatically
- [ ] Backend runs on https://localhost:5001
- [ ] Frontend runs on http://localhost:3000
- [ ] Both are accessible from browser

## Code Quality

### EditorConfig
- [ ] `.editorconfig` exists in both `backend/` and `web/`
- [ ] Settings are consistent across backend and frontend

### Gitignore
- [ ] `.gitignore` exists in `backend/` with C# patterns
- [ ] `.gitignore` exists in `web/` with Node patterns
- [ ] No node_modules/ in tracking
- [ ] No bin/, obj/ in tracking
- [ ] No .env files in tracking

## Integration Tests

### Backend + Database
- [ ] With backend running and SQL Server with schema:
  - [ ] Health endpoint responds
  - [ ] Backend logs show no errors

### Frontend + Backend
- [ ] Start both applications
- [ ] Frontend loads on http://localhost:3000
- [ ] Frontend can make HTTP request to backend health endpoint
  - [ ] (Configure CORS if needed - already done in backend)

### Full Local Development
- [ ] Open two terminals
- [ ] Terminal 1: `cd backend && dotnet run --project src/Api/Medicare.Api.csproj`
- [ ] Terminal 2: `cd web && npm run dev`
- [ ] Both start without errors
- [ ] Can access frontend on http://localhost:3000
- [ ] Can access backend API on https://localhost:5001/api/health

## Post-Completion

Once all checks pass:

- [ ] Read [PHASE_1_COMPLETE.md](docs/PHASE_1_COMPLETE.md)
- [ ] Review [ARCHITECTURE.md](docs/architecture/ARCHITECTURE.md)
- [ ] Familiarize with [API_DESIGN.md](docs/architecture/API_DESIGN.md)
- [ ] Understand the [DOMAIN_MODEL.md](docs/architecture/DOMAIN_MODEL.md)
- [ ] Plan Phase 2: Authentication with team

## Troubleshooting

### Backend Won't Start
- [ ] Check .NET 8 SDK is installed: `dotnet --version`
- [ ] Check appsettings.json has valid JSON
- [ ] Check database connection string is correct
- [ ] Check SQL Server is running

### Frontend Won't Start
- [ ] Check Node.js 18+ is installed: `node --version`
- [ ] Run `npm install` again to ensure all dependencies
- [ ] Check port 3000 is not in use
- [ ] Delete `node_modules/` and reinstall if issues persist

### Database Connection Error
- [ ] Verify SQL Server is running
- [ ] Check connection string in appsettings.json
- [ ] Ensure Medicare and Medicare_Dev databases exist
- [ ] Run the SQL schema script if tables don't exist

### Lint/Format Errors
- [ ] Run `npm install` to ensure all ESLint packages are present
- [ ] Run `npm run format` to auto-fix formatting issues
- [ ] Check `.eslintrc.json` syntax is valid

## Sign-Off

Date Completed: __________
Checked By: __________
Notes: __________

Once all checks pass and Phase 1 is validated, you're ready to begin Phase 2: Authentication.
