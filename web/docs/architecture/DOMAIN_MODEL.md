# Domain Model Design

## Core Entities

### User Aggregate
Represents an admin or manager account in the system.

**Properties:**
- `Id` (int): Unique identifier
- `Email` (string): Unique email address
- `PasswordHash` (string): Bcrypt-hashed password
- `FullName` (string): Display name
- `Role` (enum): Admin or Manager
- `IsActive` (bool): Account status
- `LastLoginAt` (DateTime?): Last login timestamp
- `CreatedAt` (DateTime): Creation timestamp
- `UpdatedAt` (DateTime?): Last update timestamp

**Business Rules:**
- Email must be unique
- At least one admin user must exist
- Password must be hashed before storage
- Cannot have empty FullName

**Use Cases:**
- Login (Phase 2)
- Account management (Phase 2+)

---

### Menu Aggregate
Represents a daily menu for lunch or dinner.

**Properties:**
- `Id` (int): Unique identifier
- `DateApplied` (DateTime): Date this menu is valid for
- `MealType` (enum): Lunch (1) or Dinner (2)
- `Description` (string): Menu description/notes
- `IsActive` (bool): Whether this menu is currently available
- `Items` (collection): MenuItems belonging to this menu
- `CreatedAt` (DateTime): Creation timestamp
- `UpdatedAt` (DateTime?): Last update timestamp

**Business Rules:**
- Cannot have duplicate menu for same date + meal type
- Menu date must be today or in the future
- At least one item should exist (enforced at service level)
- Only one active menu per date + meal type
- Cannot modify past menus

**Use Cases:**
- Create daily menu (admin, Phase 3)
- View current/future menu (customers, Phase 4)
- Update menu (admin, Phase 3)
- Publish/unpublish menu (admin, Phase 3)

---

### MenuItem Aggregate
Individual dish in a menu.

**Properties:**
- `Id` (int): Unique identifier
- `MenuId` (int, FK): Parent menu
- `Name` (string): Dish name
- `Description` (string): Ingredients, preparation notes
- `Price` (decimal): Price per portion
- `DisplayOrder` (int): Sort order in UI
- `Menu` (navigation): Reference to parent menu
- `CreatedAt` (DateTime): Creation timestamp
- `UpdatedAt` (DateTime?): Last update timestamp

**Business Rules:**
- Price must be >= 0
- Name is required and unique within a menu
- DisplayOrder determines UI sorting
- Automatically deleted when menu is deleted

**Use Cases:**
- Add item to menu (admin, Phase 3)
- Remove item from menu (admin, Phase 3)
- Display menu items (customers, Phase 4)

---

### OrderRequest Aggregate
Customer's food order request.

**Properties:**
- `Id` (int): Unique identifier (could be order number)
- `CustomerName` (string): Customer full name
- `CustomerPhone` (string): Customer contact number
- `OrderDate` (DateTime): Desired delivery/pickup date
- `MealType` (enum): Lunch or Dinner
- `Status` (enum): New → Contacted → Confirmed → Completed/Cancelled
- `Notes` (string): Special requests from customer
- `TotalQuantity` (int): Total portions ordered
- `TotalPrice` (decimal): Total cost
- `Items` (collection): OrderRequestItems
- `CreatedAt` (DateTime): When order was placed
- `UpdatedAt` (DateTime?): Last status update

**Status Flow:**
```
New (1)
  ↓ (admin views order)
Contacted (2)
  ↓ (customer confirms via phone)
Confirmed (3)
  ↓ (order fulfilled)
Completed (4)

OR
Cancelled (5) [from any state]
```

**Business Rules:**
- Cannot create order for past dates
- Customer phone must be valid format (will add validation in Phase 2)
- Cannot edit order items after status > New
- TotalPrice is calculated from line items
- TotalQuantity is sum of all item quantities
- OrderDate should be valid menu date

**Use Cases:**
- Create order (customers, Phase 3)
- View order (customers, Phase 4)
- View all orders (admin, Phase 3)
- Update status (admin, Phase 3)
- Cancel order (admin, Phase 3)

---

### OrderRequestItem
Line item for an order request.

**Properties:**
- `Id` (int): Unique identifier
- `OrderRequestId` (int, FK): Parent order
- `MenuItemId` (int): Original menu item reference
- `MenuItemName` (string): Menu item name (denormalized for history)
- `Quantity` (int): Number of portions
- `UnitPrice` (decimal): Price per portion at time of order
- `Subtotal` (decimal): Quantity × UnitPrice
- `SpecialRequest` (string?): Special notes (no salt, extra sauce, etc.)
- `OrderRequest` (navigation): Reference to parent order
- `CreatedAt` (DateTime): Creation timestamp
- `UpdatedAt` (DateTime?): Last update timestamp

**Business Rules:**
- Quantity must be >= 1
- Denormalized MenuItemName preserves menu item name at order time
- Subtotal is calculated before insert/update
- Cannot edit if parent order status > New
- Special requests should be short (<200 chars)

**Use Cases:**
- Add items to order form (customers, Phase 3)
- View order items (admin, Phase 3)
- Calculate order total (service layer, Phase 3)

---

### SiteContent
Key-value store for configurable landing page content.

**Properties:**
- `Id` (int): Unique identifier
- `Key` (string): Unique content key
- `Value` (string): Content value
- `ContentType` (string): text, html, json
- `Description` (string?): Notes about this content
- `CreatedAt` (DateTime): Creation timestamp
- `UpdatedAt` (DateTime?): Last update timestamp

**Sample Keys:**
- `CompanyName`: "Medicare"
- `CompanyPhone`: "+84 123 456 7890"
- `CompanyDescription`: "Đặt cơm ngon mỗi ngày..."
- `WorkingHours`: "7:00 AM - 8:00 PM"
- `ServiceArea`: "District 1, District 2"

**Business Rules:**
- Key must be unique (case-insensitive)
- Key should be PascalCase
- Value size depends on ContentType
- Only admin can modify

**Use Cases:**
- Get site content for landing page (public, Phase 4)
- Update site content (admin, Phase 3+)

---

## Entity Relationships

### Deletion Rules
- **Menu → MenuItems**: Cascade (delete menu deletes all items)
- **OrderRequest → OrderRequestItems**: Cascade (delete order deletes all items)
- **User**: No cascade (admin users cannot be cascade deleted)
- **SiteContent**: No cascade (standalone)

### Query Patterns (Phase 3)
- Get active menu for date + meal type
- Get all orders by status
- Get orders for date range
- Get order with all items
- Search orders by customer phone or name

## Validation Rules (Phase 2+)

### At Domain Layer
- Required fields validation
- Enum value validation
- Price >= 0
- Quantity >= 1

### At API Layer
- Phone number format (Vietnam: 10 digits, may start with +84)
- Email format
- String length constraints
- Date range validation

### At Business Logic Layer
- Menu date cannot be in past
- Order date must match available menu
- Status transitions must follow state machine
- No duplicate active menus per date

## Design Patterns Used

1. **Aggregate Pattern**: Each entity with relationships (Menu + MenuItems, OrderRequest + OrderRequestItems)
2. **Value Objects**: Enums for Status, Role, MealType
3. **Denormalization**: MenuItemName stored in OrderRequestItem for audit trail
4. **Soft Delete**: Considered for Phase 2+ (currently hard delete)
5. **Timestamps**: CreatedAt and UpdatedAt on all entities for audit

## Future Extensions

- **ContactInfo Aggregate**: Separate entity for storing multiple phone/email per customer
- **MenuCategory**: Group menu items (appetizers, mains, desserts)
- **Discount/Coupon**: Support promotional pricing
- **Customer Account**: Optional customer login and order history
- **Rating/Review**: Allow customers to rate orders
