#!/bin/bash

# Development startup script for macOS/Linux

echo "Starting Medicare development environment..."
echo ""

echo "[1/2] Starting backend API on port 5001..."
cd backend
dotnet run --project src/Api/Medicare.Api.csproj &
BACKEND_PID=$!

sleep 3

echo "[2/2] Starting frontend on port 3000..."
cd ../web
npm install
npm run dev &
FRONTEND_PID=$!

echo ""
echo "Development environment started!"
echo "Backend: https://localhost:5001"
echo "Frontend: http://localhost:3000"
echo "Swagger UI: https://localhost:5001/swagger/ui"
echo ""
echo "Press Ctrl+C to stop all processes"

# Wait for both processes
wait $BACKEND_PID $FRONTEND_PID
