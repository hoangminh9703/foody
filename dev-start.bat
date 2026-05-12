@echo off
REM Development startup script for Windows

echo Starting Medicare development environment...
echo.

echo [1/2] Starting backend API on port 5001...
start cmd /k "cd backend && dotnet run --project src\Api\Medicare.Api.csproj"

timeout /t 3 /nobreak

echo [2/2] Starting frontend on port 3000...
start cmd /k "cd web && npm install && npm run dev"

echo.
echo Development environment started!
echo Backend: https://localhost:5001
echo Frontend: http://localhost:3000
echo Swagger UI: https://localhost:5001/swagger/ui
