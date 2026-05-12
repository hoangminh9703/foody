# Medicare - Web bán hàng đồ ăn theo ngày

A modern, clean web application for food ordering by day (lunch and dinner), built with React + TypeScript on the frontend and ASP.NET Core on the backend.

## 🎯 Project Overview

This is a food ordering platform that allows customers to:
- View restaurant information on a landing page
- See daily menu for lunch and dinner
- Submit food orders through a form

Administrators can:
- Manage daily menus
- View and process customer orders
- Update order status

## 🛠️ Tech Stack

### Backend
- **Framework**: ASP.NET Core (.NET 8)
- **Architecture**: Clean Architecture
- **Database**: SQL Server
- **ORM**: Entity Framework Core

### Frontend
- **Framework**: React 18
- **Language**: TypeScript
- **Build Tool**: Vite
- **Styling**: Tokens-based (White, Yellow, Black palette)

## 📁 Project Structure

```
medicare/
├── backend/                 # ASP.NET Core backend
│   ├── src/
│   │   ├── Api/            # API controllers and entry point
│   │   ├── Domain/         # Domain entities and rules
│   │   ├── Application/    # Use cases and services
│   │   └── Infrastructure/ # Data access and external services
│   └── tests/              # Unit and integration tests
├── web/                    # React frontend
│   ├── src/
│   │   ├── app/           # Application shell and routing
│   │   ├── components/    # Reusable UI components
│   │   ├── features/      # Feature-specific modules
│   │   └── styles/        # Global styles and design tokens
│   └── docs/              # Frontend documentation
└── docs/                  # Project documentation
```

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- SQL Server or SQL Server Express

### Development Setup

See [DEV_SETUP.md](./web/docs/DEV_SETUP.md) for detailed instructions.

**Quick start (Windows):**
```bash
dev-start.bat
```

**Quick start (macOS/Linux):**
```bash
chmod +x dev-start.sh
./dev-start.sh
```

**Manual startup:**

Terminal 1 - Backend:
```bash
cd backend
dotnet run --project src/Api/Medicare.Api.csproj
```

Terminal 2 - Frontend:
```bash
cd web
npm install
npm run dev
```

## 🔗 URLs

- Frontend: `http://localhost:3000`
- Backend API: `https://localhost:5001`
- API Documentation (Swagger): `https://localhost:5001/swagger/ui`
- Health Check: `https://localhost:5001/api/health`

## 📚 Documentation

- [Development Setup Guide](./web/docs/DEV_SETUP.md)
- [Project Brief](./web/docs/brief.md)
- [Business Requirements Document](./web/docs/BRD.md)
- [Master Plan](./web/docs/plans/master-plan.md)
- [Architecture](./web/docs/architecture/)
- [Changelog](./web/docs/CHANGELOG.md)

## 📝 Development Guidelines

Follow the rules defined in [AGENTS.md](./web/AGENTS.md):
- Read all files in `/docs` before coding
- Plan before implementation
- Work phase-by-phase
- Update CHANGELOG.md after each task
- Follow the coding standards (Clean Architecture for backend, React + TypeScript for frontend)

## 🔄 Project Phases

1. **Phase 1**: Setup project (In Progress)
   - Project structure and configuration ✅
   - Development tooling ✅
   
2. **Phase 2**: Authentication
   - Admin login and session management
   
3. **Phase 3**: Core features
   - Menu management
   - Order request handling
   
4. **Phase 4**: Frontend
   - Landing page
   - Order form
   - Admin dashboard
   
5. **Phase 5**: Deployment
   - Production setup
   - CI/CD pipeline

## 📋 Style Guidelines

### Color Palette
- **Primary**: Yellow (#FFC107)
- **Dark**: Black (#000000)
- **Light**: White (#FFFFFF)

### Design Principles
- Modern and clean aesthetic
- Mobile-first responsive design
- Clear user flow and minimal steps

## 📧 Support

For questions or issues, please refer to the [Changelog](./web/docs/CHANGELOG.md) for recent updates or check the development guides.
