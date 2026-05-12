# Phase 3: Menu Management API

**Status**: Not started  
**Estimated Duration**: 1 week  
**Priority**: HIGH (required for core functionality)  
**Dependencies**: Phase 1 (Setup), Phase 2 (Auth - for protection)

## Goal
Build backend API endpoints for admins to create, read, update, and manage daily menus with menu items, supporting lunch and dinner schedules.

## Scope
- Create menu for specific date and meal type
- View all menus with filtering and pagination
- Update existing menu and items
- Delete menu (cascade to items)
- Validate menu data and business rules
- No UI yet (API only) - UI comes in Phase 8

## Detailed Tasks

### Backend (.NET Core)

#### Task 1: Create MenuService in Application Layer
**Location**: `backend/src/Application/Services/MenuService.cs`

Steps:
1. Create `IMenuService` interface with methods:
   - `Task<MenuDto> CreateMenuAsync(CreateMenuRequest request)`
   - `Task<MenuDto> GetMenuByIdAsync(int id)`
   - `Task<IEnumerable<MenuDto>> GetMenusAsync(MenuFilterRequest filter)`
   - `Task<MenuDto> UpdateMenuAsync(int id, UpdateMenuRequest request)`
   - `Task DeleteMenuAsync(int id)`
   - `Task<bool> MenuExistsForDateAndMealTypeAsync(DateTime date, MealType mealType)`

2. Create `MenuService` implementation:
   - Use repository pattern to access data
   - Validate business rules:
     - No duplicate menu for same date + meal type
     - Cannot modify past menus
     - Menu must have at least one item
     - All items must have valid prices (>= 0)
   - Map entities to DTOs using AutoMapper
   - Log all operations

3. Create DTOs:
   ```csharp
   CreateMenuRequest {
     DateApplied: DateTime
     MealType: MealType
     Description: string
     Items: List<MenuItemRequest>
   }
   
   MenuItemRequest {
     Name: string
     Description: string
     Price: decimal
     DisplayOrder: int
   }
   
   MenuDto {
     Id: int
     DateApplied: DateTime
     MealType: string
     Description: string
     Items: List<MenuItemDto>
     IsActive: bool
     CreatedAt: DateTime
   }
   ```

**Technical Notes**:
- Use MediatR for command/query separation if preferred
- Implement IValidatableObject for cross-field validation
- Add async/await properly
- Use dependency injection for repository access

#### Task 2: Create Menu Commands (CQRS Pattern)
**Location**: 
- `backend/src/Application/Commands/CreateMenuCommand.cs`
- `backend/src/Application/Commands/UpdateMenuCommand.cs`
- `backend/src/Application/Commands/DeleteMenuCommand.cs`

Steps:
1. Create `CreateMenuCommand`:
   - Properties: DateApplied, MealType, Description, Items
   - Validation: All required fields, valid dates, valid prices

2. Create `CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuDto>`:
   - Validate command
   - Check no duplicate exists
   - Call `MenuService.CreateMenuAsync()`
   - Return created menu DTO

3. Create `UpdateMenuCommand` and handler:
   - Properties: MenuId, DateApplied, Description, Items
   - Validate: Cannot update past menus
   - Call service and return updated menu

4. Create `DeleteMenuCommand` and handler:
   - Property: MenuId
   - Cascade delete items via EF Core
   - Return success/failure

5. Register all handlers in DI:
   ```csharp
   services.AddMediatR(typeof(CreateMenuCommandHandler));
   ```

**Technical Notes**:
- Use FluentValidation for command validation
- Ensure date validation (not in past for create, not past for update)
- Handle concurrent updates (optimistic locking optional)

#### Task 3: Create Menu Queries (CQRS Pattern)
**Location**: 
- `backend/src/Application/Queries/GetMenuByIdQuery.cs`
- `backend/src/Application/Queries/GetMenusQuery.cs`

Steps:
1. Create `GetMenuByIdQuery`:
   - Property: MenuId
   - Handler returns single `MenuDto`

2. Create `GetMenusQuery`:
   - Properties: Page, PageSize, DateFrom, DateTo, MealType
   - Handler returns paginated list with total count
   
   ```csharp
   class GetMenusQuery : IRequest<PagedResult<MenuDto>> {
     public int Page { get; set; } = 1;
     public int PageSize { get; set; } = 10;
     public DateTime? DateFrom { get; set; }
     public DateTime? DateTo { get; set; }
     public MealType? MealType { get; set; }
   }
   
   class PagedResult<T> {
     public List<T> Items { get; set; }
     public int TotalCount { get; set; }
     public int Page { get; set; }
     public int PageSize { get; set; }
   }
   ```

3. Implement query handlers:
   - Build LINQ queries with filters
   - Sort by date descending
   - Apply pagination (skip/take)
   - Include related items (eager load)
   - Map to DTOs

**Technical Notes**:
- Use `.Include()` to eager load MenuItems to avoid N+1 queries
- Implement sorting by DateApplied descending (newest first)
- Support filtering by date range (useful for future/past menus)
- Cache results in memory for performance (Phase 9 enhancement)

#### Task 4: Create Menu API Controller
**Location**: `backend/src/Api/Controllers/MenuController.cs`

Steps:
1. Create `MenuController : ControllerBase`:
   - Attribute: `[ApiController]`, `[Route("api/[controller]")]`
   - Inject `IMediator`

2. Implement endpoints:
   ```
   POST   /api/menus              - Create menu (admin only)
   GET    /api/menus              - List menus with filters (admin only)
   GET    /api/menus/{id}         - Get menu details (admin only)
   PUT    /api/menus/{id}         - Update menu (admin only)
   DELETE /api/menus/{id}         - Delete menu (admin only)
   ```

3. For each endpoint:
   - Add `[Authorize]` attribute (requires admin)
   - Validate input
   - Call MediatR command/query
   - Return appropriate HTTP status:
     - 201 Created (on POST)
     - 200 OK (on GET, PUT)
     - 204 No Content (on DELETE)
     - 400 Bad Request (validation errors)
     - 401 Unauthorized (not authenticated)
     - 404 Not Found (resource not found)

4. Add response DTOs and error handling:
   ```csharp
   [HttpPost]
   public async Task<ActionResult<MenuDto>> CreateMenu(CreateMenuRequest request)
   {
       var command = new CreateMenuCommand { /* map from request */ };
       var result = await _mediator.Send(command);
       return CreatedAtAction(nameof(GetMenu), new { id = result.Id }, result);
   }
   
   [HttpGet]
   public async Task<ActionResult<PagedResult<MenuDto>>> GetMenus(
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 10,
       [FromQuery] DateTime? dateFrom = null,
       [FromQuery] DateTime? dateTo = null,
       [FromQuery] MealType? mealType = null)
   {
       var query = new GetMenusQuery { Page = page, PageSize = pageSize, ... };
       var result = await _mediator.Send(query);
       return Ok(result);
   }
   ```

**Technical Notes**:
- Use `[Authorize]` on controller class or individual methods
- Use `[FromRoute]`, `[FromQuery]`, `[FromBody]` attributes explicitly
- Return proper HTTP status codes
- Include comprehensive error messages in responses

#### Task 5: Add Menu Repository Pattern (Infrastructure Layer)
**Location**: `backend/src/Infrastructure/Repositories/MenuRepository.cs`

Steps:
1. Create `IMenuRepository` interface:
   - `Task<Menu> GetByIdAsync(int id)`
   - `Task<List<Menu>> GetAllAsync(MenuFilterSpec filter)`
   - `Task<Menu> AddAsync(Menu menu)`
   - `Task<Menu> UpdateAsync(Menu menu)`
   - `Task DeleteAsync(int id)`
   - `Task<bool> ExistsAsync(DateTime date, MealType mealType)`
   - `Task SaveChangesAsync()`

2. Implement `MenuRepository` using EF Core:
   - Use DbContext to query and persist
   - Use Include() for eager loading
   - Implement filters in separate method
   - Add proper exception handling

3. Register in DI:
   ```csharp
   services.AddScoped<IMenuRepository, MenuRepository>();
   ```

**Technical Notes**:
- Use generic repository pattern for reusability
- Implement Unit of Work pattern if managing multiple repositories
- Use async methods throughout
- Handle DbUpdateConcurrencyException for optimistic locking

#### Task 6: Implement Business Rules and Validation
**Location**: `backend/src/Domain/Validators/MenuValidator.cs` (optional)

Steps:
1. Create validation rules:
   - No duplicate menu for same date + meal type
   - Cannot modify/delete past menus (configurable date)
   - Menu must have at least one item
   - Prices must be >= 0
   - Prices must have max 2 decimal places

2. Implement as:
   - FluentValidation validators (recommended)
   - Or custom validation in service layer
   - Or domain model validation

3. Use in command handlers:
   ```csharp
   var validator = new CreateMenuValidator();
   var validationResult = await validator.ValidateAsync(command);
   if (!validationResult.IsValid)
       throw new ValidationException(validationResult.Errors);
   ```

**Technical Notes**:
- Centralize validation for consistency
- Use FluentValidation for cleaner code
- Validate at API layer AND domain layer
- Return descriptive validation error messages

### Testing Tasks

#### Task 7: Test Endpoints with Postman/Insomnia

Steps:
1. Start backend API
2. Test each endpoint:
   - POST /api/menus with valid data → 201 Created
   - POST /api/menus with duplicate date+type → 400 Bad Request
   - POST /api/menus without auth → 401 Unauthorized
   - GET /api/menus → 200 OK with list
   - GET /api/menus?dateFrom=2026-06-01&dateTo=2026-06-30 → filtered results
   - GET /api/menus/{id} → 200 OK with menu details
   - PUT /api/menus/{id} → 200 OK with updated menu
   - DELETE /api/menus/{id} → 204 No Content
   - DELETE /api/menus/{id} again → 404 Not Found

3. Verify cascade delete:
   - Create menu with items
   - Delete menu
   - Verify items are also deleted from database

4. Verify data persistence:
   - Create menu
   - Restart API
   - Query menu again
   - Verify data still exists

#### Task 8: Add Integration Tests (Optional but Recommended)

Steps:
1. Create test project: `backend/tests/Medicare.Api.IntegrationTests/`
2. Create test class: `MenuControllerTests.cs`
3. Write tests for:
   - Creating valid menu
   - Creating duplicate menu (should fail)
   - Updating menu
   - Deleting menu
   - Filtering menus by date and type

```csharp
[Fact]
public async Task CreateMenu_WithValidData_ReturnsCreatedStatus()
{
    // Arrange
    var request = new CreateMenuRequest { /* valid data */ };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/menus", request);
    
    // Assert
    Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
    var content = await response.Content.ReadAsAsync<MenuDto>();
    Assert.NotNull(content);
}
```

**Technical Notes**:
- Use xUnit or NUnit for testing
- Use WebApplicationFactory for test server
- Use test database (in-memory or real)
- Clean up data between tests

### Documentation Tasks

#### Task 9: Update Swagger Documentation

Steps:
1. Add XML comments to MenuController:
   ```csharp
   /// <summary>
   /// Creates a new menu for a specific date and meal type
   /// </summary>
   /// <param name="request">Menu creation request with items</param>
   /// <returns>Created menu with ID</returns>
   /// <response code="201">Menu created successfully</response>
   /// <response code="400">Validation error or duplicate menu</response>
   /// <response code="401">Not authenticated</response>
   [HttpPost]
   public async Task<...>
   ```

2. Verify Swagger shows:
   - All endpoints
   - Request/response schemas
   - Required parameters
   - Status codes
   - Authorization requirement

3. Test Swagger UI in browser:
   - Navigate to `https://localhost:5001/swagger`
   - Expand MenuController endpoints
   - Try test requests

**Technical Notes**:
- Use `[ProducesResponseType]` for explicit status codes
- Use `[Produces("application/json")]` for content types
- Add example requests/responses in comments
- Verify all required fields are marked in schema

## Acceptance Criteria
- [x] POST /api/menus creates menu with items successfully
- [x] GET /api/menus returns list of menus with optional filters
- [x] GET /api/menus/{id} returns specific menu with items
- [x] PUT /api/menus/{id} updates menu and items
- [x] DELETE /api/menus/{id} deletes menu and cascade items
- [x] Cannot create duplicate menu for same date + meal type
- [x] Cannot modify menu for past dates
- [x] Menu requires at least one item (validation error if not)
- [x] All date inputs are validated for correct format
- [x] API responses follow consistent response structure
- [x] Swagger documentation is updated for all endpoints
- [x] All endpoints require authentication [Authorize]

## Testing Procedures

### Manual Test Cases

1. **Create Valid Menu**
   - Send POST to `/api/menus` with:
     ```json
     {
       "dateApplied": "2026-06-15",
       "mealType": "Lunch",
       "description": "Monday lunch menu",
       "items": [
         {"name": "Chicken Rice", "description": "Grilled chicken", "price": 5.50, "displayOrder": 1},
         {"name": "Vegetable Soup", "description": "Fresh soup", "price": 2.00, "displayOrder": 2}
       ]
     }
     ```
   - Verify: 201 Created returned with menu ID

2. **Create Duplicate Menu (Should Fail)**
   - Create menu for 2026-06-15 Lunch
   - Try to create another menu for same date+type
   - Verify: 400 Bad Request with message "Menu already exists for this date and meal type"

3. **Validate Menu Items**
   - Try to create menu with:
     - No items: Should fail with "Menu must have at least one item"
     - Negative price: Should fail with "Price must be >= 0"
     - Missing required fields: Should fail with appropriate messages

4. **Get Menus with Filters**
   - Create 3 menus on different dates
   - GET `/api/menus?dateFrom=2026-06-01&dateTo=2026-06-30&mealType=Lunch`
   - Verify: Only Lunch menus in June are returned

5. **Update Menu**
   - Create menu
   - PUT `/api/menus/{id}` with updated description
   - Verify: 200 OK with updated menu

6. **Cannot Modify Past Menu**
   - Create menu for past date (e.g., 2026-03-01)
   - Try to update it
   - Verify: 400 Bad Request with message "Cannot modify menu for past dates"

7. **Delete Menu Cascades to Items**
   - Create menu with 3 items
   - DELETE `/api/menus/{id}`
   - Verify: 204 No Content
   - GET `/api/menus/{id}`
   - Verify: 404 Not Found
   - Query database for MenuItems with deleted MenuId
   - Verify: Items are also deleted (cascade delete worked)

8. **Pagination**
   - Create 25 menus
   - GET `/api/menus?page=1&pageSize=10`
   - Verify: Returns first 10 menus, totalCount=25
   - GET `/api/menus?page=2&pageSize=10`
   - Verify: Returns next 10 menus

### Authorization Checks
- Test all endpoints return 401 without authentication
- Test all endpoints work after authentication
- Verify session token/cookie is required

## Files to Create/Modify

### New Files
```
backend/src/Application/Services/MenuService.cs
backend/src/Application/Services/IMenuService.cs
backend/src/Application/Commands/CreateMenuCommand.cs
backend/src/Application/Commands/CreateMenuCommandHandler.cs
backend/src/Application/Commands/UpdateMenuCommand.cs
backend/src/Application/Commands/UpdateMenuCommandHandler.cs
backend/src/Application/Commands/DeleteMenuCommand.cs
backend/src/Application/Commands/DeleteMenuCommandHandler.cs
backend/src/Application/Queries/GetMenuByIdQuery.cs
backend/src/Application/Queries/GetMenuByIdQueryHandler.cs
backend/src/Application/Queries/GetMenusQuery.cs
backend/src/Application/Queries/GetMenusQueryHandler.cs
backend/src/Application/DTOs/MenuDto.cs
backend/src/Application/DTOs/CreateMenuRequest.cs
backend/src/Application/DTOs/UpdateMenuRequest.cs
backend/src/Application/DTOs/MenuItemDto.cs
backend/src/Application/Validators/CreateMenuValidator.cs
backend/src/Infrastructure/Repositories/MenuRepository.cs
backend/src/Infrastructure/Repositories/IMenuRepository.cs
backend/src/Api/Controllers/MenuController.cs
backend/tests/Medicare.Api.IntegrationTests/MenuControllerTests.cs (optional)
```

### Modified Files
```
backend/src/Application/DependencyInjection.cs (register services)
backend/src/Infrastructure/DependencyInjection.cs (register repositories)
backend/src/Api/Program.cs (add AutoMapper profiles if needed)
backend/src/Infrastructure/Data/MedicareDbContext.cs (verify Menu/MenuItem relationships)
```

## Dependencies
- MediatR (for CQRS pattern)
- AutoMapper (for DTO mapping)
- FluentValidation (for validation)
- Entity Framework Core (already installed)

## Related Documentation
- [DOMAIN_MODEL.md](../architecture/DOMAIN_MODEL.md) - Menu and MenuItem entities
- [API_DESIGN.md](../architecture/API_DESIGN.md) - API specification
- Clean Architecture principles for service layer organization

## Notes
- This phase creates the API only; UI comes in Phase 8
- Use MediatR for command/query separation to keep code clean
- Implement comprehensive validation at both API and domain layers
- Consider adding menu template functionality in future enhancement
- Consider adding menu approval workflow in future enhancement
