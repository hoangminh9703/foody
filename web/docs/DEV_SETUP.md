# Development Setup Guide

## Prerequisites
- .NET 8 SDK
- Node.js 18+ and npm
- SQL Server or SQL Server Express (local)
- Visual Studio Code or Visual Studio

## Backend Setup

### 1. Restore dependencies
```bash
cd backend
dotnet restore
```

### 2. Build the solution
```bash
dotnet build
```

### 3. Run the API server
```bash
dotnet run --project src/Api/Medicare.Api.csproj
```

The API will be available at `https://localhost:5001` with Swagger UI at `https://localhost:5001/swagger/ui`.

### 4. Run tests
```bash
dotnet test
```

## Frontend Setup

### 1. Install dependencies
```bash
cd web
npm install
```

### 2. Run development server
```bash
npm run dev
```

The frontend will be available at `http://localhost:3000`.

### 3. Build for production
```bash
npm run build
```

### 4. Format and lint code
```bash
npm run lint
npm run format
```

## Database Setup

### 1. Create database manually (SQL Server)
```sql
CREATE DATABASE Medicare;
CREATE DATABASE Medicare_Dev;
```

### 2. Apply migrations (when available in Phase 3)
```bash
cd backend
dotnet ef database update
```

## Common Commands

### Run both backend and frontend locally

**Terminal 1 - Backend:**
```bash
cd backend
dotnet run --project src/Api/Medicare.Api.csproj
```

**Terminal 2 - Frontend:**
```bash
cd web
npm run dev
```

### Verify health
- Backend health: `curl https://localhost:5001/api/health`
- Frontend: Open `http://localhost:3000`

## Troubleshooting

### Port already in use
- Backend port (5001): Change in Properties/launchSettings.json
- Frontend port (3000): Change in vite.config.ts

### Database connection errors
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure the database exists

### CORS errors
- Backend CORS is configured to allow all origins in development
- If issues persist, check appsettings.Development.json configuration

## Environment Variables

### Backend
- `ASPNETCORE_ENVIRONMENT`: Set to `Development` or `Production`

### Frontend
- Copy `.env.local.example` to `.env.local` and update values as needed
