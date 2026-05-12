# Database Setup Guide

## Overview

The Medicare project uses SQL Server as the database with Entity Framework Core for ORM and migrations.

## Database Schema

The initial schema includes the following tables:

### Users
- Stores admin and manager user accounts
- Fields: Email (unique), PasswordHash, FullName, Role, IsActive, LastLoginAt, CreatedAt, UpdatedAt

### Menus
- Daily menus for lunch (cơm trưa) and dinner (cơm chiều)
- Fields: DateApplied, MealType (Lunch=1, Dinner=2), Description, IsActive, CreatedAt, UpdatedAt

### MenuItems
- Individual menu items that belong to a Menu
- Fields: MenuId (FK), Name, Description, Price, DisplayOrder, CreatedAt, UpdatedAt

### OrderRequests
- Customer food order requests
- Fields: CustomerName, CustomerPhone, OrderDate, MealType, Status, Notes, TotalQuantity, TotalPrice, CreatedAt, UpdatedAt
- Status enum: New=1, Contacted=2, Confirmed=3, Completed=4, Cancelled=5

### OrderRequestItems
- Line items for each order request
- Fields: OrderRequestId (FK), MenuItemId, MenuItemName, Quantity, UnitPrice, Subtotal, SpecialRequest, CreatedAt, UpdatedAt

### SiteContents
- Key-value store for configurable landing page content
- Fields: Key (unique), Value, ContentType, Description, CreatedAt, UpdatedAt

## Setup Methods

### Method 1: Entity Framework Core Migrations (Recommended)

#### Prerequisites
- .NET 8 SDK installed
- SQL Server available locally or remote

#### Steps

1. **Ensure database exists:**
   ```sql
   CREATE DATABASE Medicare;
   CREATE DATABASE Medicare_Dev;
   ```

2. **Apply migrations (when available):**
   ```bash
   cd backend
   dotnet ef database update --startup-project src/Api/Medicare.Api.csproj
   ```

3. **Verify schema:**
   - Connect to the database in SQL Server Management Studio
   - Check that all tables are created

### Method 2: Direct SQL Script

#### Prerequisites
- SQL Server Management Studio or similar SQL client
- SQL Server database instance running

#### Steps

1. **Create databases:**
   ```sql
   CREATE DATABASE Medicare;
   CREATE DATABASE Medicare_Dev;
   ```

2. **Run the SQL script:**
   - Open `backend/Database/01_InitialSchema.sql` in SQL Server Management Studio
   - Select the target database (Medicare or Medicare_Dev)
   - Execute the script (F5)

3. **Verify schema:**
   - In Object Explorer, expand the database
   - Confirm all tables are present

## Default Credentials

### Admin User
- **Email**: `admin@medicare.local`
- **Password**: `Admin123!`
- **Role**: Admin

**⚠️ Important**: Change this password immediately after first login in production.

## Connection Strings

### Development
```
Server=(local);Database=Medicare_Dev;Integrated Security=true;TrustServerCertificate=true
```

### Production
```
Server=prod-server;Database=Medicare_Prod;User Id=sa;Password=YourSecurePassword;TrustServerCertificate=true
```

See `appsettings.json` and `appsettings.Development.json` for actual configuration.

## Backup and Recovery

### Backup
```sql
BACKUP DATABASE Medicare TO DISK = 'C:\Backups\Medicare_Full.bak'
WITH INIT, COMPRESSION, DESCRIPTION = 'Medicare Full Backup';
```

### Recovery
```sql
RESTORE DATABASE Medicare FROM DISK = 'C:\Backups\Medicare_Full.bak'
WITH REPLACE;
```

## Troubleshooting

### Connection Errors
- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure the user has permissions to create databases

### Migration Failures
- Check that all entity classes are properly defined
- Verify DbContext is correctly configured
- Run `dotnet ef migrations list` to see migration history

### Schema Mismatch
- If the application fails to start due to schema mismatch:
  1. Drop and recreate the database
  2. Re-run migrations or SQL script
  3. Verify all entity definitions match the database schema

## Future Changes

When modifying the data model:

1. **Add/modify entities** in the Domain layer
2. **Update DbContext** in Infrastructure/Data/MedicareDbContext.cs
3. **Create a new migration** (when EF Core migrations are enabled):
   ```bash
   dotnet ef migrations add MigrationName --startup-project src/Api/Medicare.Api.csproj
   ```
4. **Apply the migration**:
   ```bash
   dotnet ef database update --startup-project src/Api/Medicare.Api.csproj
   ```
