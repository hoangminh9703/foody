# Architecture Overview

## System Architecture

The Medicare system follows a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────┐
│       Frontend (React + TS)          │
│  Landing Page | Order Form | Admin   │
└────────────┬────────────────────────┘
             │ HTTP/REST
┌────────────▼────────────────────────┐
│    API Gateway / Web Server          │
│    (ASP.NET Core - Port 5001)        │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│    Clean Architecture Layers         │
│  ┌──────────────────────────────┐   │
│  │    API Controllers Layer      │   │
│  ├──────────────────────────────┤   │
│  │  Application Services Layer   │   │
│  ├──────────────────────────────┤   │
│  │   Domain Business Logic       │   │
│  ├──────────────────────────────┤   │
│  │  Infrastructure / Persistence │   │
│  │  (EF Core + SQL Server)       │   │
│  └──────────────────────────────┘   │
└────────────┬────────────────────────┘
             │
┌────────────▼────────────────────────┐
│    SQL Server Database               │
│  (Users, Menus, Orders, Content)     │
└─────────────────────────────────────┘
```

## Technology Stack

### Backend
- **Language**: C# 12
- **Framework**: ASP.NET Core 8.0
- **Database**: SQL Server
- **ORM**: Entity Framework Core 8.0
- **Authentication**: Session-based (to be implemented in Phase 2)
- **Logging**: Structured logging with Microsoft.Extensions.Logging

### Frontend
- **Language**: TypeScript 5.2
- **Framework**: React 18
- **Build Tool**: Vite 5
- **Routing**: React Router v6
- **Styling**: Token-based CSS-in-JS (design tokens file)
- **HTTP Client**: Fetch API (to be wrapped in Phase 3)

## Project Structure

### Backend
```
backend/
├── Medicare.sln                    # Visual Studio solution file
├── src/
│   ├── Api/                        # Web API layer
│   │   ├── Program.cs              # Entry point and service configuration
│   │   ├── appsettings.json        # Default configuration
│   │   ├── appsettings.Development.json
│   │   ├── appsettings.Production.json
│   │   └── Controllers/            # HTTP endpoints
│   ├── Domain/                     # Domain layer (business rules)
│   │   └── Entities/               # Domain models
│   ├── Application/                # Application services layer
│   │   └── (to be added in Phase 2+)
│   └── Infrastructure/             # Infrastructure layer
│       ├── Data/                   # Database context and configuration
│       └── Logging/                # Logging setup
├── tests/                          # Unit and integration tests
└── Database/                       # Database schemas and migrations
```

### Frontend
```
web/
├── index.html                      # HTML entry point
├── package.json                    # Dependencies and scripts
├── tsconfig.json                   # TypeScript configuration
├── vite.config.ts                  # Vite configuration
├── .eslintrc.json                  # Linting rules
├── .prettierrc.json                # Code formatting rules
├── src/
│   ├── main.tsx                    # React app entry point
│   ├── app/                        # Application shell
│   │   ├── App.tsx                 # Root component with routing
│   │   └── Layout.tsx              # Base layout component
│   ├── components/                 # Reusable UI components
│   │   ├── Button.tsx
│   │   ├── Card.tsx
│   │   └── Container.tsx
│   ├── features/                   # Feature-specific modules
│   │   ├── landing/                # (to be added)
│   │   ├── order-form/             # (to be added)
│   │   └── admin/                  # (to be added)
│   └── styles/                     # Styling
│       ├── tokens.ts               # Design tokens
│       └── global.ts               # Global styles
└── docs/                           # Documentation
```

## Data Model

### Entity Relationships

```
User (1) ──────── (many) OrderRequest
         │
         └─ Admin manages

Menu (1) ──────── (many) MenuItem
   │                      │
   ├─ DateApplied         ├─ Price
   ├─ MealType            └─ DisplayOrder
   └─ IsActive

OrderRequest (1) ──────── (many) OrderRequestItem
   │                              │
   ├─ CustomerName               ├─ Quantity
   ├─ CustomerPhone              ├─ UnitPrice
   ├─ OrderDate                  └─ SpecialRequest
   ├─ Status (New/Contacted/
   │   Confirmed/Completed/
   │   Cancelled)
   └─ TotalPrice

SiteContent (Key-Value Store)
   ├─ CompanyName
   ├─ CompanyPhone
   └─ CompanyDescription
```

## API Endpoints (Phase 2-3)

### Public Endpoints
- `GET /api/menus?date={date}&mealType={lunch|dinner}` - Get daily menu
- `POST /api/orders` - Submit order request
- `GET /api/site-content/{key}` - Get landing page content

### Admin Endpoints (Protected)
- `POST /api/auth/login` - Admin login (Phase 2)
- `GET /api/orders` - List all orders
- `PATCH /api/orders/{id}/status` - Update order status
- `POST /api/menus` - Create menu
- `PUT /api/menus/{id}` - Update menu
- `DELETE /api/menus/{id}` - Delete menu
- `PUT /api/site-content/{key}` - Update landing page content

## Design Decisions

### 1. Clean Architecture
- **Rationale**: Provides clear separation between business logic, infrastructure, and presentation. Makes testing easier and code more maintainable.
- **Trade-off**: Slightly more code structure upfront, but pays off as features are added.

### 2. Token-Based Design System
- **Rationale**: Ensures consistent styling across frontend and makes future design changes centralized.
- **Color Palette**: White (#FFF), Yellow (#FFC107), Black (#000) as specified in brief.

### 3. SQL Server + EF Core
- **Rationale**: Provides strong typing, migrations support, and enterprise-grade database.
- **Alternative considered**: Could use PostgreSQL for cost reasons, but SQL Server was chosen per requirements.

### 4. React + Vite
- **Rationale**: Fast build times, modern tooling, good TypeScript support, and React ecosystem maturity.
- **Vite over CRA**: Significantly faster dev experience and build times.

### 5. Session-Based Authentication (Phase 2)
- **Rationale**: Simpler for admin-only access, suitable for small team usage.
- **Note**: Could be upgraded to JWT if multi-device admin support is needed later.

## Assumptions & Constraints

### Assumptions
1. Only one admin user per installation (password reset via admin tool).
2. Menu changes are infrequent (daily updates, not real-time).
3. Order requests don't require payment processing (Phase 1-3 scope).
4. No user accounts needed for customers (anonymous ordering).
5. Data volume is manageable (< 10K orders/month initially).

### Known Limitations
- No multi-language support (Vietnamese only in Phase 1).
- No image uploads for menu items (text-based only).
- No automated notifications (manual admin contact with customers).
- No real-time order tracking for customers.
- Single-timezone system (assumes local time handling only).

## Performance Considerations

### Frontend
- Vite provides fast HMR (hot module replacement) in dev.
- React is lazy-loaded for efficient bundle size.
- CSS-in-JS tokens reduce runtime overhead.

### Backend
- EF Core query optimization will be added during Phase 3 feature implementation.
- Connection pooling is handled by SQL Server driver.
- Logging level is debug in development, warning in production to minimize overhead.

### Database
- Indexed primary keys and unique constraints.
- Cascade delete configured for related entities.
- No stored procedures in Phase 1 (all logic in code).

## Security Baseline (Phase 1)

- Admin password hashed with BCrypt (10 rounds).
- CORS allows all origins in dev; will be restricted in production.
- HTTPS enforced on backend API.
- Input validation at API controller level (to be enhanced in Phase 2).
- No secrets in configuration files (use user-secrets in dev, env vars in prod).

## Scalability Path

### If traffic increases:
1. Add caching layer (Redis) for menu and site content.
2. Implement async processing for order notifications.
3. Move to database read replicas for reports.

### If order volume increases:
1. Add background job queue (Hangfire) for admin notifications.
2. Implement batch order processing.
3. Add data archiving strategy for old orders.

## Next Steps (Phase 2)

1. Implement admin authentication (login page and session management).
2. Build CRUD operations for menus (backend and admin UI).
3. Add order request API and form submission.
4. Implement order status tracking for admin.
5. Set up automated admin notifications.

## References

- [BRD](../BRD.md) - Full business requirements
- [Master Plan](../plans/master-plan.md) - Project phases and timeline
- [Database Setup](../DATABASE_SETUP.md) - Database initialization guide
- [Development Setup](../DEV_SETUP.md) - Local development environment
