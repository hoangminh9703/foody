# Phase 10: Testing & QA

**Status**: Not started  
**Estimated Duration**: 1.5 weeks  
**Priority**: HIGH (ensures reliability)  
**Dependencies**: Phases 1-9 (all must be functional)

## Goal
Implement comprehensive testing strategy across backend and frontend to ensure reliability, security, and correctness of all features.

## Scope
- Unit tests for business logic (backend)
- Integration tests for APIs
- Component tests for React components
- End-to-end testing for critical flows
- Performance testing
- Security testing
- Manual QA testing

## Detailed Tasks

### Backend Testing

#### Task 1: Unit Tests for Domain Layer
**Location**: `backend/tests/Medicare.Domain.Tests/`

Steps:
1. Create test classes for each entity:
   - `UserTests.cs`
   - `MenuTests.cs`
   - `OrderRequestTests.cs`
   - `SiteContentTests.cs`

2. Test entity validation rules:
   - Valid creation
   - Invalid data (null, empty, out of range)
   - Business rule enforcement
   - State transitions

3. Example tests:
   ```csharp
   [Fact]
   public void CreateUser_WithValidEmail_Succeeds() { }
   
   [Fact]
   public void CreateUser_WithInvalidEmail_ThrowsException() { }
   
   [Fact]
   public void OrderRequest_CannotTransitionFromCompletedToNew_ThrowsException() { }
   ```

4. Coverage goal: >80% of Domain layer

**Technical Notes**:
- Use xUnit for testing
- Use Moq for mocking dependencies
- Test happy path and error paths
- Test boundary conditions

#### Task 2: Unit Tests for Application Layer
**Location**: `backend/tests/Medicare.Application.Tests/`

Steps:
1. Create test classes for handlers:
   - `CreateOrderCommandHandlerTests.cs`
   - `UpdateMenuCommandHandlerTests.cs`
   - `GetMenusQueryHandlerTests.cs`
   - etc.

2. Test command/query handlers:
   - Valid command execution
   - Validation errors
   - Business rule violations
   - Success/failure cases

3. Mock repositories and services:
   ```csharp
   [Fact]
   public async Task CreateOrder_WithValidData_ReturnsOrderId() {
       // Arrange
       var mockRepo = new Mock<IOrderRepository>();
       var handler = new CreateOrderCommandHandler(mockRepo.Object);
       
       // Act
       var result = await handler.Handle(command, CancellationToken.None);
       
       // Assert
       Assert.NotNull(result);
       Assert.True(result.Id > 0);
   }
   ```

4. Coverage goal: >80% of Application layer

**Technical Notes**:
- Use Moq for mocking repositories
- Test validation separately
- Test business logic thoroughly
- Use test fixtures for common setup

#### Task 3: Integration Tests for API Endpoints
**Location**: `backend/tests/Medicare.Api.IntegrationTests/`

Steps:
1. Set up test infrastructure:
   - `IntegrationTestBase.cs` with WebApplicationFactory
   - Test database (in-memory or test instance)
   - Seeded test data

2. Create test classes for controllers:
   - `OrderControllerTests.cs`
   - `MenuControllerTests.cs`
   - `AuthControllerTests.cs`
   - etc.

3. Test API endpoints:
   ```csharp
   [Fact]
   public async Task CreateOrder_WithValidRequest_Returns201() {
       var request = new CreateOrderRequest { /* valid data */ };
       var response = await _client.PostAsJsonAsync("/api/orders", request);
       Assert.Equal(HttpStatusCode.Created, response.StatusCode);
   }
   
   [Fact]
   public async Task GetOrders_WithoutAuth_Returns401() {
       var response = await _client.GetAsync("/api/orders");
       Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
   }
   ```

4. Test scenarios:
   - Happy path (success)
   - Validation errors (400)
   - Authentication errors (401)
   - Authorization errors (403)
   - Not found errors (404)
   - Server errors (500)

5. Coverage goal: All CRUD endpoints tested

**Technical Notes**:
- Use WebApplicationFactory for test server
- Create test database with seed data
- Clean up between tests
- Test both success and failure paths
- Test authentication/authorization

#### Task 4: Security Testing
**Location**: Manual and automated tests

Steps:
1. SQL Injection testing:
   - Try inserting SQL in text fields
   - Verify queries are parameterized
   - Verify no SQL errors returned

2. XSS testing:
   - Try storing HTML/JavaScript
   - Verify content encoded on display
   - Check Content-Security-Policy headers

3. CSRF testing:
   - Verify POST/PUT/DELETE require valid tokens
   - Test cross-origin requests

4. Authentication testing:
   - Try accessing protected resources without auth
   - Verify 401 returned
   - Test session timeout
   - Verify logout clears all permissions

5. Authorization testing:
   - Test regular user cannot access admin endpoints
   - Verify role-based access control

**Technical Notes**:
- Use OWASP Testing Guide
- Test common vulnerabilities (OWASP Top 10)
- Use security headers (Content-Security-Policy, X-Frame-Options)
- Regular security audits

#### Task 5: Performance Testing
**Location**: Load testing scripts

Steps:
1. Database query performance:
   - Test with 1000+ records
   - Verify query times < 1 second
   - Verify N+1 queries are eliminated
   - Use SQL Profiler to analyze

2. API endpoint performance:
   - GET /api/orders with pagination: < 500ms
   - POST /api/orders: < 1000ms
   - Concurrent requests: handle 100+ simultaneously

3. Load testing:
   ```
   Use Apache JMeter or similar
   Test scenarios:
   - 100 concurrent users
   - 50 requests per second
   - 5 minute duration
   ```

**Technical Notes**:
- Profile before optimizing
- Set performance baselines
- Monitor in production
- Use APM tools (Application Insights optional)

### Frontend Testing

#### Task 6: Component Tests
**Location**: `web/src/__tests__/components/`

Steps:
1. Create test files for components:
   - `OrderForm.test.tsx`
   - `MenuDisplay.test.tsx`
   - `LoginPage.test.tsx`
   - etc.

2. Test component rendering:
   ```typescript
   test('OrderForm renders with all fields', () => {
     const { getByLabelText } = render(<OrderForm />);
     expect(getByLabelText('Name')).toBeInTheDocument();
     expect(getByLabelText('Phone')).toBeInTheDocument();
   });
   ```

3. Test user interactions:
   ```typescript
   test('Submit button submits form', async () => {
     const { getByRole } = render(<OrderForm />);
     const submitButton = getByRole('button', { name: 'Submit' });
     fireEvent.click(submitButton);
     // Assert form submission
   });
   ```

4. Test conditional rendering:
   ```typescript
   test('Shows error message on validation error', () => {
     const { getByText, getByRole } = render(<OrderForm />);
     fireEvent.click(getByRole('button', { name: 'Submit' }));
     expect(getByText('Name is required')).toBeInTheDocument();
   });
   ```

5. Use React Testing Library:
   - Test user behavior, not implementation
   - Use semantic queries (getByRole, getByLabelText)
   - Avoid testing implementation details

**Technical Notes**:
- Use Jest for testing
- Use React Testing Library
- Mock API calls with MSW (Mock Service Worker)
- Test accessibility queries

#### Task 7: Integration Tests (Frontend)
**Location**: `web/src/__tests__/integration/`

Steps:
1. Test component integration:
   ```typescript
   test('Order flow: select menu -> fill form -> submit', async () => {
     const { getByText, getByRole } = render(<OrderPage />);
     
     // Select menu items
     fireEvent.click(getByText('Chicken Rice'));
     
     // Fill form
     fireEvent.change(getByRole('textbox', { name: 'Name' }), {
       target: { value: 'John' }
     });
     
     // Submit
     fireEvent.click(getByRole('button', { name: 'Submit' }));
     
     // Assert success
     await waitFor(() => {
       expect(getByText('Order submitted')).toBeInTheDocument();
     });
   });
   ```

2. Test with API mocks:
   - Mock API responses
   - Test success and error paths
   - Test loading states

**Technical Notes**:
- Use MSW for mocking APIs
- Test complete user flows
- Use waitFor for async operations

#### Task 8: End-to-End Testing
**Location**: `e2e/` folder (optional but recommended)

Steps:
1. Set up E2E testing framework (Cypress or Playwright):
   ```
   npm install --save-dev cypress
   npx cypress open
   ```

2. Create E2E test specs:
   - `customer-order-flow.cy.ts`
   - `admin-dashboard.cy.ts`
   - `menu-management.cy.ts`

3. Test complete flows:
   ```typescript
   describe('Customer Order Flow', () => {
     it('User can place order from landing page', () => {
       cy.visit('/');
       cy.contains('Order Now').click();
       cy.get('[name="customerName"]').type('John Doe');
       cy.get('[name="customerPhone"]').type('1234567890');
       cy.get('button[type="submit"]').click();
       cy.contains('Order submitted').should('be.visible');
     });
   });
   ```

4. Test critical paths:
   - Customer order submission
   - Admin login and order management
   - Menu creation and update

**Technical Notes**:
- Use Cypress for easier syntax
- Test in real browser
- Include visual regression testing (optional)
- Run E2E tests in CI/CD

### Manual QA Testing

#### Task 9: Manual Testing Plan
**Location**: Test documentation

Steps:
1. Create test cases for all features:
   - Each user story should have test cases
   - Test happy path and error paths
   - Document expected results

2. Test on multiple browsers:
   - Chrome (latest)
   - Firefox (latest)
   - Safari (if on Mac)
   - Edge (if on Windows)

3. Test on multiple devices:
   - Desktop (1920x1080)
   - Laptop (1366x768)
   - Tablet (768x1024)
   - Mobile (375x667, 414x896)

4. Test accessibility:
   - Keyboard navigation
   - Screen reader (NVDA, JAWS, VoiceOver)
   - Color contrast
   - Focus indicators

**Technical Notes**:
- Use BrowserStack for cross-browser testing
- Test on real devices when possible
- Document bugs with screenshots
- Test with real data

#### Task 10: QA Checklist
**Location**: Documented checklist

Steps:
1. Create comprehensive checklist:
   - Feature completeness
   - Error handling
   - Performance
   - Security
   - Accessibility
   - Compatibility

2. Sign-off criteria:
   - All tests pass (unit, integration, E2E)
   - Code coverage > 80%
   - No critical bugs
   - No security vulnerabilities
   - Performance within SLA
   - Accessibility compliance (WCAG 2.1 AA)

3. Document test execution:
   - Test date
   - Tester name
   - Results
   - Bugs found

**Technical Notes**:
- Create formal test report
- Track metrics (pass rate, bug count)
- Prioritize bugs (critical, major, minor)
- Get sign-off from stakeholders

### Continuous Integration

#### Task 11: Set Up CI/CD Pipeline
**Location**: `.github/workflows/` or CI system config

Steps:
1. Create automated test pipeline:
   - Run on each commit/PR
   - Run unit tests (backend and frontend)
   - Run integration tests
   - Run linting/style checks
   - Generate coverage reports

2. GitHub Actions example:
   ```yaml
   name: Tests
   on: [push, pull_request]
   jobs:
     test:
       runs-on: ubuntu-latest
       steps:
         - uses: actions/checkout@v2
         - name: Run backend tests
           run: dotnet test backend/
         - name: Run frontend tests
           run: npm test --prefix web
   ```

3. Set up coverage reporting:
   - Publish coverage reports
   - Track coverage trends
   - Fail if coverage drops

**Technical Notes**:
- Run tests in parallel for speed
- Cache dependencies
- Set up notifications for failures
- Publish test results

### Test Documentation

#### Task 12: Create Test Documentation
**Location**: `backend/tests/README.md` and `web/README.md`

Steps:
1. Document testing strategy:
   - Testing pyramid (unit, integration, E2E)
   - Tools used
   - Coverage goals

2. Document how to run tests:
   ```
   Backend:
   dotnet test backend/
   
   Frontend:
   npm test
   ```

3. Document test organization:
   - Directory structure
   - Naming conventions
   - Test file patterns

4. Document common test patterns:
   - Mocking examples
   - Async test examples
   - Error handling examples

**Technical Notes**:
- Keep documentation up-to-date
- Include example tests
- Document best practices

## Acceptance Criteria
- [x] Domain and Application layers have >80% test coverage
- [x] API endpoints have integration tests
- [x] All CRUD operations are tested
- [x] Authentication/authorization flows are tested
- [x] Error scenarios are tested
- [x] React components have component tests
- [x] Critical user flows have E2E tests
- [x] No SQL injection vulnerabilities
- [x] No XSS vulnerabilities
- [x] All security headers are set
- [x] Load test shows acceptable response times
- [x] Performance is acceptable under load

## Testing Procedures

### Running Tests

1. **Run all backend tests**
   ```
   cd backend
   dotnet test
   ```

2. **Run all frontend tests**
   ```
   cd web
   npm test -- --coverage
   ```

3. **Run specific test file**
   ```
   dotnet test --filter ClassName
   npm test OrderForm.test.tsx
   ```

4. **Run E2E tests** (if implemented)
   ```
   npm run cypress
   ```

### Test Coverage Goals
- Backend: >80% (Domain + Application)
- Frontend: >70% (Components)
- Overall: >75%

## Files to Create/Modify

### New Files
```
backend/tests/Medicare.Domain.Tests/ (unit tests)
backend/tests/Medicare.Application.Tests/ (unit tests)
backend/tests/Medicare.Api.IntegrationTests/ (integration tests)
web/src/__tests__/ (Jest tests)
e2e/ (Cypress E2E tests)
.github/workflows/tests.yml (CI/CD)
docs/TESTING.md (test documentation)
```

### Modified Files
```
backend/*.csproj (add test project references)
web/package.json (add test scripts)
README.md (add testing section)
```

## Dependencies
- xUnit (backend testing)
- Moq (mocking)
- Jest (frontend testing)
- React Testing Library
- Cypress (E2E testing, optional)

## Related Documentation
- Phases 1-9 complete
- See individual phase documentation for component details

## Notes
- Test-driven development (TDD) recommended for future phases
- Aim for high-quality tests, not just coverage numbers
- Test behavior, not implementation
- Keep tests maintainable and fast
