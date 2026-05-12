# Phase 4: Order Management API

**Status**: Not started  
**Estimated Duration**: 1 week  
**Priority**: HIGH (core business logic)  
**Dependencies**: Phase 1 (Setup), Phase 2 (Auth), Phase 3 (Menu API)

## Goal
Build backend API for customers to submit orders and for admins to manage, track, and update order status with validation, pagination, and business rule enforcement.

## Scope
- Create order request from customer (no auth required)
- Retrieve orders with filtering and pagination (admin only)
- Update order status with state machine validation (admin only)
- Retrieve order details with items (admin only)
- Calculate and persist order totals
- Validate against available menu

## Detailed Tasks

### Backend (.NET Core)

#### Task 1: Create OrderService in Application Layer
**Location**: `backend/src/Application/Services/OrderService.cs`

Steps:
1. Create `IOrderService` interface:
   - `Task<OrderDto> CreateOrderAsync(CreateOrderRequest request)`
   - `Task<OrderDto> GetOrderByIdAsync(int id)`
   - `Task<PagedResult<OrderDto>> GetOrdersAsync(OrderFilterRequest filter)`
   - `Task<OrderDto> UpdateOrderStatusAsync(int id, UpdateOrderStatusRequest request)`
   - `Task<bool> CanTransitionStatusAsync(OrderStatus from, OrderStatus to)`
   - `Task<bool> IsMenuAvailableAsync(DateTime date, MealType mealType)`

2. Create `OrderService` implementation:
   - Validate menu exists for order date/meal type
   - Calculate TotalQuantity and TotalPrice
   - Implement status state machine (valid transitions)
   - Map entities to DTOs
   - Log all operations
   - Handle concurrent updates gracefully

3. Create DTOs:
   ```csharp
   CreateOrderRequest {
     CustomerName: string
     CustomerPhone: string
     OrderDate: DateTime
     MealType: MealType
     Items: List<OrderItemRequest>
     Notes: string
   }
   
   OrderItemRequest {
     MenuItemId: int
     Quantity: int
     SpecialRequest: string
   }
   
   OrderDto {
     Id: int
     CustomerName: string
     CustomerPhone: string
     OrderDate: DateTime
     MealType: string
     Status: string
     Items: List<OrderItemDto>
     TotalQuantity: int
     TotalPrice: decimal
     Notes: string
     CreatedAt: DateTime
   }
   
   UpdateOrderStatusRequest {
     NewStatus: OrderStatus
   }
   ```

**Technical Notes**:
- Status state machine: New → Contacted → Confirmed → Completed/Cancelled
- Validate phone number format (basic: contains digits, 7-15 chars)
- Calculate totals: sum quantities and prices from items
- Store MenuItemName at time of order (denormalization for historical data)

#### Task 2: Create Order Commands (CQRS Pattern)
**Location**: 
- `backend/src/Application/Commands/CreateOrderCommand.cs`
- `backend/src/Application/Commands/UpdateOrderStatusCommand.cs`

Steps:
1. Create `CreateOrderCommand`:
   - Properties: CustomerName, CustomerPhone, OrderDate, MealType, Items, Notes
   - Validation: All required fields, valid date, valid items, menu exists

2. Create `CreateOrderCommandHandler`:
   - Validate command input
   - Verify menu exists for order date/type
   - Create OrderRequest and OrderRequestItems
   - Calculate totals
   - Save to database
   - Return created order DTO

3. Create `UpdateOrderStatusCommand`:
   - Properties: OrderId, NewStatus
   - Handler validates status transition
   - Updates status
   - Returns updated order DTO

4. Register handlers in DI

**Technical Notes**:
- Phone validation: `Regex.IsMatch(phone, @"^\d{7,15}$")`
- Validate items list is not empty
- Validate quantities are positive
- Store MenuItemName at order time for audit trail

#### Task 3: Create Order Queries (CQRS Pattern)
**Location**: 
- `backend/src/Application/Queries/GetOrderByIdQuery.cs`
- `backend/src/Application/Queries/GetOrdersQuery.cs`

Steps:
1. Create `GetOrderByIdQuery`:
   - Property: OrderId
   - Handler returns single `OrderDto` with all items

2. Create `GetOrdersQuery`:
   ```csharp
   class GetOrdersQuery : IRequest<PagedResult<OrderDto>> {
     public int Page { get; set; } = 1;
     public int PageSize { get; set; } = 10;
     public OrderStatus? Status { get; set; }
     public DateTime? DateFrom { get; set; }
     public DateTime? DateTo { get; set; }
     public MealType? MealType { get; set; }
     public string? CustomerName { get; set; }
   }
   ```

3. Implement query handlers:
   - Build LINQ with filters (status, date range, meal type)
   - Support customer name search (contains)
   - Sort by OrderDate descending
   - Eager load OrderRequestItems
   - Apply pagination
   - Return PagedResult with total count

**Technical Notes**:
- Use `.Include(o => o.Items)` to load items
- Filter by date range (OrderDate)
- Support partial customer name search
- Default sort: newest orders first

#### Task 4: Create Order API Controller
**Location**: `backend/src/Api/Controllers/OrderController.cs`

Steps:
1. Create `OrderController : ControllerBase`:
   ```
   POST   /api/orders                  - Create order (public, no auth)
   GET    /api/orders                  - List orders (admin only)
   GET    /api/orders/{id}             - Get order details (admin only)
   PATCH  /api/orders/{id}/status      - Update status (admin only)
   ```

2. Implement endpoints:
   - POST /api/orders (public):
     - Accept CreateOrderRequest
     - Return 201 Created with order ID
     - Return 400 for validation errors
   
   - GET /api/orders (admin):
     - Require [Authorize]
     - Accept query params: page, pageSize, status, dateFrom, dateTo, mealType, customerName
     - Return 200 with paginated list
   
   - GET /api/orders/{id} (admin):
     - Require [Authorize]
     - Return 200 with order and items
     - Return 404 if not found
   
   - PATCH /api/orders/{id}/status (admin):
     - Require [Authorize]
     - Accept UpdateOrderStatusRequest
     - Validate status transition
     - Return 200 with updated order
     - Return 400 for invalid transition

3. Add response DTOs and error handling

**Technical Notes**:
- POST /api/orders is public (no [Authorize])
- Filter endpoints are admin-only [Authorize]
- Return consistent error response format
- Include order ID in response headers for POST

#### Task 5: Create Order Repository Pattern (Infrastructure Layer)
**Location**: `backend/src/Infrastructure/Repositories/OrderRepository.cs`

Steps:
1. Create `IOrderRepository` interface:
   - `Task<OrderRequest> GetByIdAsync(int id)`
   - `Task<List<OrderRequest>> GetAllAsync(OrderFilterSpec filter)`
   - `Task<OrderRequest> AddAsync(OrderRequest order)`
   - `Task<OrderRequest> UpdateAsync(OrderRequest order)`
   - `Task SaveChangesAsync()`
   - `Task<int> CountAsync(OrderFilterSpec filter)`

2. Implement `OrderRepository`:
   - Query with Include() for items and menu item names
   - Implement filtering logic (status, date, meal type)
   - Handle pagination properly (skip/take)
   - Ensure related data is eagerly loaded

3. Register in DI

**Technical Notes**:
- Use Include() to load OrderRequestItems
- Don't load Menu entities (denormalized data stored in items)
- Support filtering by multiple criteria
- Handle sorting (by OrderDate desc)

#### Task 6: Implement Status State Machine Validation
**Location**: `backend/src/Domain/Services/OrderStatusTransitionValidator.cs`

Steps:
1. Create `OrderStatusTransitionValidator`:
   ```csharp
   class OrderStatusTransitionValidator {
       private static readonly Dictionary<OrderStatus, List<OrderStatus>> ValidTransitions = 
           new() {
               { OrderStatus.New, new List<OrderStatus> { OrderStatus.Contacted, OrderStatus.Cancelled } },
               { OrderStatus.Contacted, new List<OrderStatus> { OrderStatus.Confirmed, OrderStatus.Cancelled } },
               { OrderStatus.Confirmed, new List<OrderStatus> { OrderStatus.Completed, OrderStatus.Cancelled } },
               { OrderStatus.Completed, new List<OrderStatus>() },  // Terminal state
               { OrderStatus.Cancelled, new List<OrderStatus>() }   // Terminal state
           };
   }
   ```

2. Implement validation:
   - `IsValidTransition(from, to): bool`
   - Use in UpdateOrderStatusCommand handler
   - Return validation error for invalid transitions

3. Document state machine in comments:
   ```
   New → Contacted → Confirmed → Completed
         ↓          ↓           ↓
         Cancelled  Cancelled   Cancelled (from any state)
   ```

**Technical Notes**:
- Implement as domain service or value object
- Make state machine configurable for future phases
- Log all state transitions for audit

#### Task 7: Implement Phone Validation
**Location**: `backend/src/Application/Validators/CreateOrderValidator.cs`

Steps:
1. Create `CreateOrderValidator` using FluentValidation:
   ```csharp
   class CreateOrderValidator : AbstractValidator<CreateOrderCommand> {
       public CreateOrderValidator() {
           RuleFor(x => x.CustomerName)
               .NotEmpty().WithMessage("Customer name is required")
               .MaximumLength(100).WithMessage("Name too long");
           
           RuleFor(x => x.CustomerPhone)
               .NotEmpty().WithMessage("Phone is required")
               .Matches(@"^\d{7,15}$").WithMessage("Invalid phone format");
           
           RuleFor(x => x.Items)
               .NotEmpty().WithMessage("At least one item required");
       }
   }
   ```

2. Register validator in DI
3. Use in command handler

**Technical Notes**:
- Phone validation: 7-15 digits (support multiple formats)
- Validate required fields
- Validate list is not empty
- Custom messages for each validation

#### Task 8: Add Database Migration (if needed)
**Location**: `backend/Database/` (or use EF Core Migrations)

Steps:
1. If not using EF Core migrations, update SQL schema:
   - Verify OrderRequest table has all columns
   - Verify OrderRequestItem table has MenuItemName column
   - Ensure foreign key constraints exist
   - Add indexes on OrderDate, Status for query performance

2. If using EF Migrations:
   ```
   dotnet ef migrations add AddOrderManagementFeatures --project src/Infrastructure
   dotnet ef database update --project src/Infrastructure
   ```

**Technical Notes**:
- Verify cascade delete is set up (OrderRequest → OrderRequestItems)
- Add index on (OrderDate, Status) for filtering queries
- Consider index on CustomerPhone for lookups

### Testing Tasks

#### Task 9: Test Endpoints with Postman/Insomnia

Steps:
1. Create menus first (Phase 3)
2. Test endpoints:
   - POST /api/orders with valid data → 201 Created
   - POST /api/orders with no items → 400 Bad Request
   - POST /api/orders with invalid phone → 400 Bad Request
   - POST /api/orders with non-existent menu → 400 Bad Request
   - POST /api/orders for past date → 400 Bad Request
   - GET /api/orders without auth → 401 Unauthorized
   - GET /api/orders with auth → 200 OK with list
   - GET /api/orders?status=New → filtered results
   - GET /api/orders?dateFrom=...&dateTo=... → date range filtered
   - PATCH /api/orders/{id}/status with valid transition → 200 OK
   - PATCH /api/orders/{id}/status with invalid transition → 400 Bad Request

3. Test data calculations:
   - Create order with multiple items
   - Verify TotalQuantity = sum of quantities
   - Verify TotalPrice = sum of (quantity × unitPrice)

4. Test pagination:
   - Create 25+ orders
   - Get with page=1, pageSize=10
   - Verify returns correct count

#### Task 10: Add Integration Tests (Recommended)

Steps:
1. Create test class: `OrderControllerTests.cs`
2. Write tests:
   - CreateOrder_WithValidData_ReturnsCreated
   - CreateOrder_WithoutMenu_ReturnsBadRequest
   - CreateOrder_WithInvalidPhone_ReturnsBadRequest
   - UpdateOrderStatus_ValidTransition_ReturnsOk
   - UpdateOrderStatus_InvalidTransition_ReturnsBadRequest
   - GetOrders_WithStatusFilter_ReturnsFiltered
   - GetOrders_WithDateRange_ReturnsFiltered

### Documentation Tasks

#### Task 11: Update Swagger Documentation

Steps:
1. Add XML comments to OrderController
2. Document:
   - All endpoints with purpose
   - Request/response models
   - Status codes
   - Example requests/responses
   - Authentication requirements
3. Verify Swagger shows all endpoints correctly

## Acceptance Criteria
- [x] POST /api/orders creates order for valid customer and menu date
- [x] Order cannot be created for dates without menu
- [x] GET /api/orders returns filtered list for admin (paginated)
- [x] GET /api/orders/{id} returns order with all items
- [x] PATCH /api/orders/{id}/status updates status with validation
- [x] Invalid status transitions are rejected
- [x] TotalPrice is calculated correctly from items
- [x] TotalQuantity sums all item quantities
- [x] Phone number is validated for format
- [x] All required fields are validated
- [x] Order creation timestamp is recorded
- [x] Filtering by status, date range, meal type works

## Testing Procedures

### Manual Test Cases

1. **Create Valid Order**
   - Create menu for today (lunch)
   - POST to `/api/orders`:
     ```json
     {
       "customerName": "John Doe",
       "customerPhone": "1234567890",
       "orderDate": "2026-06-15",
       "mealType": "Lunch",
       "items": [
         {"menuItemId": 1, "quantity": 2, "specialRequest": "No onions"}
       ],
       "notes": "Deliver by 12pm"
     }
     ```
   - Verify: 201 Created with order ID

2. **Invalid Phone Format**
   - POST with phone: "abc123"
   - Verify: 400 Bad Request with message "Invalid phone format"

3. **No Menu Available**
   - POST order for date with no menu
   - Verify: 400 Bad Request with message "Menu not available for this date/type"

4. **Status Transitions**
   - Create order (status = New)
   - PATCH with status = Contacted → 200 OK
   - PATCH with status = Confirmed → 200 OK
   - PATCH with status = Completed → 200 OK
   - Try to transition from Completed → should fail (400)

5. **Totals Calculation**
   - Create order with:
     - Item 1: qty 2, price $5 = $10
     - Item 2: qty 3, price $3 = $9
   - Verify: TotalQuantity = 5, TotalPrice = 19

6. **Filtering Orders**
   - Create 5 orders with various statuses and dates
   - GET `/api/orders?status=New` → only New orders
   - GET `/api/orders?dateFrom=...&dateTo=...` → date range
   - GET `/api/orders?mealType=Lunch` → only Lunch orders

7. **Pagination**
   - Create 25 orders
   - GET `/api/orders?page=1&pageSize=10` → 10 items, totalCount=25
   - GET `/api/orders?page=3&pageSize=10` → last 5 items

### Authorization Checks
- Test GET /api/orders returns 401 without auth
- Test PATCH /api/orders/{id}/status returns 401 without auth
- Test POST /api/orders works without auth (public endpoint)

## Files to Create/Modify

### New Files
```
backend/src/Application/Services/OrderService.cs
backend/src/Application/Services/IOrderService.cs
backend/src/Application/Commands/CreateOrderCommand.cs
backend/src/Application/Commands/CreateOrderCommandHandler.cs
backend/src/Application/Commands/UpdateOrderStatusCommand.cs
backend/src/Application/Commands/UpdateOrderStatusCommandHandler.cs
backend/src/Application/Queries/GetOrderByIdQuery.cs
backend/src/Application/Queries/GetOrderByIdQueryHandler.cs
backend/src/Application/Queries/GetOrdersQuery.cs
backend/src/Application/Queries/GetOrdersQueryHandler.cs
backend/src/Application/DTOs/OrderDto.cs
backend/src/Application/DTOs/CreateOrderRequest.cs
backend/src/Application/DTOs/UpdateOrderStatusRequest.cs
backend/src/Application/DTOs/OrderItemDto.cs
backend/src/Application/Validators/CreateOrderValidator.cs
backend/src/Domain/Services/OrderStatusTransitionValidator.cs
backend/src/Infrastructure/Repositories/OrderRepository.cs
backend/src/Infrastructure/Repositories/IOrderRepository.cs
backend/src/Api/Controllers/OrderController.cs
backend/tests/Medicare.Api.IntegrationTests/OrderControllerTests.cs (optional)
```

### Modified Files
```
backend/src/Application/DependencyInjection.cs
backend/src/Infrastructure/DependencyInjection.cs
backend/src/Api/Program.cs
backend/src/Infrastructure/Data/MedicareDbContext.cs
```

## Dependencies
- MediatR (already used)
- FluentValidation (already used)
- Entity Framework Core (already installed)

## Related Documentation
- [DOMAIN_MODEL.md](../architecture/DOMAIN_MODEL.md) - OrderRequest and OrderRequestItem entities
- [API_DESIGN.md](../architecture/API_DESIGN.md) - Order API specification
- Phase 3 completed (Menu API required for validation)

## Notes
- Order creation is public (no auth required)
- Order management (view, update status) is admin-only
- Status machine is strict; invalid transitions return 400
- Consider webhook notifications in Phase 2B for status changes
- Consider generating order receipts in Phase 4B
