# Master Plan - Web bán hàng đồ ăn theo ngày

## Phase 1: Setup project ✅ COMPLETE

**Status**: Completed on 2026-05-11

### Tasks
- ✅ Confirm solution structure and create initial folder scaffold.
- ✅ Initialize ASP.NET Core .NET 8 backend with Clean Architecture projects.
- ✅ Initialize React + TypeScript frontend with routing and design tokens.
- ✅ Configure development tooling (ESLint, Prettier, EditorConfig).
- ✅ Prepare database schema with core entities.
- ✅ Add project documentation baseline.

### Expected output
- ✅ Project chạy được ở môi trường local.
- ✅ Cấu trúc thư mục rõ ràng cho backend, web và docs.
- ✅ Base UI shell sẵn sàng để phát triển feature tiếp theo.
- ✅ Database migrations hoặc schema khởi tạo hoàn chỉnh.

### How to test
- ✅ Chạy ứng dụng local và xác nhận trang chủ mặc định hiển thị.
- ✅ Kiểm tra lint và format chạy không lỗi trên code base mới.
- ✅ Xác nhận kết nối database và áp dụng schema/migration thành công.

### Deliverables
- [PHASE_1_COMPLETE.md](PHASE_1_COMPLETE.md) - Phase completion report
- [PHASE_1_VALIDATION.md](PHASE_1_VALIDATION.md) - Validation checklist
- [architecture/ARCHITECTURE.md](architecture/ARCHITECTURE.md) - System design
- [architecture/DOMAIN_MODEL.md](architecture/DOMAIN_MODEL.md) - Entity documentation
- [architecture/API_DESIGN.md](architecture/API_DESIGN.md) - API specification
- [backend/Medicare.sln](backend/Medicare.sln) - Backend solution
- [web/package.json](web/package.json) - Frontend dependencies
- [backend/Database/01_InitialSchema.sql](backend/Database/01_InitialSchema.sql) - Database schema

## Phase 2: Authentication & Authorization

**Status**: Not started  
**Estimated Duration**: 1 week

### Goal
Implement secure admin authentication and role-based access control so only authorized administrators can access the dashboard and management features.

### Scope
- Admin login system with email/password
- Session management with secure cookies
- Protected API endpoints and routes
- Logout and session timeout
- No customer authentication (scope excludes this)

### Tasks
- Design login API endpoint with validation
- Implement password verification with BCrypt
- Configure session/cookie middleware
- Create login page UI in React
- Add route guards for protected pages
- Implement logout functionality
- Add session timeout handling (auto-logout after inactivity)
- Set up HTTPS enforcement for auth endpoints
- Create admin authentication service in Application layer

### Acceptance Criteria
- [ ] Admin can log in with valid credentials
- [ ] Invalid credentials return appropriate error message
- [ ] Session is established and stored in secure cookies
- [ ] Unauthenticated users cannot access admin routes (API or UI)
- [ ] Admin can log out and session is cleared
- [ ] Swagger API shows protected endpoints requiring authentication
- [ ] CORS is properly configured for auth endpoints
- [ ] Session timeout is implemented (30 minutes of inactivity)
- [ ] All passwords in database are hashed with BCrypt
- [ ] Login form has basic client-side validation

### Testing
- Test login with valid email/password
- Test login with invalid email/password
- Test accessing protected routes without authentication
- Test logout clears session
- Test session persists across page reloads
- Test session expires after timeout
- Test manual session deletion clears authorization

---

## Phase 3: Menu Management API

**Status**: Not started  
**Estimated Duration**: 1 week

### Goal
Build backend API endpoints for admins to create, read, update, and manage daily menus with menu items, supporting lunch and dinner schedules.

### Scope
- Create menu for specific date and meal type
- View all menus with filtering
- Update existing menu and items
- Delete menu (cascade to items)
- Validate menu data and business rules
- No UI yet (API only)

### Tasks
- Create MenuService in Application layer with CRUD operations
- Implement CreateMenuCommand and handler
- Implement UpdateMenuCommand and handler
- Implement DeleteMenuCommand and handler
- Implement GetMenuQuery with filtering
- Add input validation (required fields, date range, no duplicates)
- Create MenuController with POST, GET, PUT, DELETE endpoints
- Add menu repository pattern in Infrastructure layer
- Implement business rule: no duplicate menu for same date + meal type
- Implement business rule: cannot modify past menus

### Acceptance Criteria
- [ ] POST /api/menus creates menu with items successfully
- [ ] GET /api/menus returns list of menus with optional filters
- [ ] GET /api/menus/{id} returns specific menu with items
- [ ] PUT /api/menus/{id} updates menu and items
- [ ] DELETE /api/menus/{id} deletes menu and cascade items
- [ ] Cannot create duplicate menu for same date + meal type
- [ ] Cannot modify menu for past dates
- [ ] Menu requires at least one item (validation error if not)
- [ ] All date inputs are validated for correct format
- [ ] API responses follow consistent response structure
- [ ] Swagger documentation is updated for all endpoints

### Testing
- Test creating valid menu with multiple items
- Test creating duplicate menu (should fail)
- Test updating menu items and prices
- Test deleting menu cascades to delete items
- Test filtering menus by date and meal type
- Test date validation rejects past dates
- Test menu items sorted by displayOrder
- Test price validation (>= 0)

---

## Phase 4: Order Management API

**Status**: Not started  
**Estimated Duration**: 1 week

### Goal
Build backend API for customers to submit orders and for admins to manage, track, and update order status.

### Scope
- Create order request from customer
- Retrieve orders with filtering (admin)
- Update order status (admin)
- Retrieve order details with items
- Calculate and persist order totals
- Validate against available menu

### Tasks
- Create OrderService in Application layer
- Implement CreateOrderCommand and handler
- Implement UpdateOrderStatusCommand and handler
- Implement GetOrderQuery with pagination and filters
- Add validation: order date matches available menu, required fields
- Add validation: phone number format validation
- Calculate TotalQuantity and TotalPrice before saving
- Create OrderRepository in Infrastructure layer
- Create OrderController with POST, GET, PATCH endpoints
- Implement status state machine (valid transitions)
- Add logging for order operations

### Acceptance Criteria
- [ ] POST /api/orders creates order for valid customer and menu date
- [ ] Order cannot be created for dates without menu
- [ ] GET /api/orders returns filtered list for admin (paginated)
- [ ] GET /api/orders/{id} returns order with all items
- [ ] PATCH /api/orders/{id}/status updates status with validation
- [ ] Invalid status transitions are rejected
- [ ] TotalPrice is calculated correctly from items
- [ ] TotalQuantity sums all item quantities
- [ ] Phone number is validated for format
- [ ] All required fields are validated
- [ ] Order creation timestamp is recorded
- [ ] Filtering by status, date range, meal type works

### Testing
- Test creating order with valid data
- Test order fails without matching menu
- Test invalid phone format rejected
- Test status transitions follow state machine
- Test order totals calculated correctly
- Test filtering orders by various criteria
- Test retrieving order with all items
- Test pagination of order list

---

## Phase 5: Site Content Management

**Status**: Not started  
**Estimated Duration**: 3 days

### Goal
Enable admins to manage configurable landing page content without code changes (company info, contact details, service area).

### Scope
- Read site content key-value pairs
- Update site content (admin only)
- Support for text, HTML, and JSON content types
- No versioning or history (Phase 2+ enhancement)

### Tasks
- Create SiteContentService in Application layer
- Implement GetSiteContent query
- Implement UpdateSiteContent command
- Create SiteContentController with GET and PUT endpoints
- Add validation for content size limits
- Implement content type validation
- Add caching for frequently accessed content
- Create predefined content keys documentation

### Acceptance Criteria
- [ ] GET /api/site-content/{key} returns content value
- [ ] PUT /api/site-content/{key} updates content successfully
- [ ] Only authenticated admins can update content
- [ ] Content size is limited (max 5000 chars)
- [ ] Invalid content type is rejected
- [ ] Multiple content keys can be queried
- [ ] Content is cached for better performance
- [ ] Updated content is immediately reflected in API

### Testing
- Test reading site content by key
- Test updating company name
- Test updating company phone
- Test content caching works
- Test size limit validation
- Test unauthorized users cannot update
- Test invalid content type rejected

---

## Phase 6: Order Form & Customer UI

**Status**: Not started  
**Estimated Duration**: 1 week

### Goal
Build the customer-facing order submission form with menu selection, validation, and success/error feedback.

### Scope
- Order form component with customer info fields
- Menu item selection and quantity input
- Dynamic form validation
- Submit order to API
- Show success/error messages
- Mobile-optimized form
- No authentication required for customers

### Tasks
- Create OrderForm component in React
- Implement form state management (controlled inputs)
- Create MenuSelector component for selecting items and quantities
- Add form validation (required fields, phone format, quantity > 0)
- Integrate with POST /api/orders endpoint
- Implement loading state during submission
- Show success message with order reference (if provided)
- Show error message with details if submission fails
- Add optional special requests field
- Style form with design tokens (white, yellow, black)
- Test form on mobile, tablet, desktop

### Acceptance Criteria
- [ ] Form displays all required fields clearly
- [ ] Form validates before submission
- [ ] Customer cannot submit without selecting items
- [ ] Phone number format is validated client-side
- [ ] Form shows loading state during submission
- [ ] Success message displays after submission
- [ ] Error message shows if submission fails
- [ ] Form is fully responsive on mobile
- [ ] Special requests field is optional
- [ ] Menu items show prices
- [ ] Selected items display with quantities
- [ ] Form can be reset for new order

### Testing
- Test submitting valid order
- Test validation errors for required fields
- Test invalid phone format rejected
- Test cannot submit without items
- Test success message appears
- Test error handling and display
- Test form responsiveness on mobile
- Test special requests are optional

---

## Phase 7: Landing Page & Menu Display

**Status**: Not started  
**Estimated Duration**: 1.5 weeks

### Goal
Build the public-facing landing page that showcases the restaurant and displays current daily menu with call-to-action for ordering.

### Scope
- Hero section with company branding
- Menu display by meal type (lunch/dinner)
- Daily menu loading from API
- Company info section with contact details
- Call-to-action buttons (Order button)
- Responsive design (mobile-first)
- No authentication required

### Tasks
- Create Hero component with messaging and CTA
- Create MenuDisplay component to show items by meal type
- Create CompanyInfo component for contact details
- Create MenuItem display with price and description
- Integrate with GET /api/menus endpoint
- Integrate with GET /api/site-content for company details
- Implement loading and empty states
- Add navigation to order form
- Style using design tokens (white, yellow, black palette)
- Ensure mobile responsiveness
- Add 'no menu available' message for unavailable dates

### Acceptance Criteria
- [ ] Landing page loads and displays correctly
- [ ] Hero section is prominent and has clear CTA button
- [ ] Current daily menu displays for both lunch and dinner
- [ ] Menu items show with price, name, and description
- [ ] Clicking order button navigates to order form
- [ ] Company contact info is displayed
- [ ] Page is fully responsive on mobile/tablet/desktop
- [ ] Menu items are sorted by displayOrder
- [ ] Loading state shown while fetching menu
- [ ] Empty state shown if no menu available
- [ ] Color palette matches brief (white, yellow, black)
- [ ] Font sizes are readable on mobile

### Testing
- Test page loads without errors
- Test menu displays for current date
- Test responsive layout on mobile/tablet/desktop
- Test menu sorting by displayOrder
- Test loading and empty states
- Test navigation to order form
- Test site content loads correctly
- Test menu updates when date changes

---

## Phase 8: Admin Dashboard UI

**Status**: Not started  
**Estimated Duration**: 1.5 weeks

### Goal
Build the admin management interface for viewing orders, updating status, and managing daily menus.

### Scope
- Dashboard layout with navigation
- Orders list with search and filters
- Order detail view
- Status update interface
- Menu management interface (create, edit, delete)
- Admin authentication required
- Responsive design

### Tasks
- Create DashboardLayout component with sidebar navigation
- Create OrdersList component with table/list view
- Create OrderDetail component with order items
- Create StatusUpdateForm for changing order status
- Create MenusList component with menu management
- Create MenuForm for creating/editing menus
- Implement search and filtering for orders
- Add delete confirmation dialogs
- Integrate with all admin API endpoints
- Add loading and error states
- Style consistently with design tokens

### Acceptance Criteria
- [ ] Dashboard loads only when authenticated
- [ ] Orders list displays all orders with pagination
- [ ] Filtering by status, date, meal type works
- [ ] Clicking order shows detail view with items
- [ ] Status can be updated from dropdown/buttons
- [ ] Menu list shows all menus
- [ ] Can create new menu from dashboard
- [ ] Can edit existing menu and items
- [ ] Can delete menu with confirmation
- [ ] Empty states shown for no data
- [ ] Loading states shown during operations
- [ ] All forms validate before submission
- [ ] Success/error messages shown appropriately
- [ ] Dashboard is responsive on tablet and desktop
- [ ] Logout button is accessible

### Testing
- Test dashboard loads with authentication
- Test orders list displays correctly
- Test filtering and search functionality
- Test viewing order details
- Test updating order status
- Test creating new menu
- Test editing menu items and prices
- Test deleting menu
- Test error handling
- Test responsive design

---

## Phase 9: Integration & Polish

**Status**: Not started  
**Estimated Duration**: 1 week

### Goal
Integrate all components, handle edge cases, add comprehensive error handling, and polish user experience across the entire application.

### Scope
- End-to-end flow testing (customer → order → admin)
- Comprehensive input validation
- Error handling and user feedback
- Performance optimization
- Logging and monitoring setup
- Edge case handling
- UI/UX polish and consistency

### Tasks
- Add comprehensive form validation (backend + frontend)
- Implement consistent error response handling
- Add toast/notification system for user feedback
- Optimize API calls (reduce unnecessary requests)
- Add request/response interceptors
- Implement error boundaries in React
- Add proper HTTP status code handling
- Add data sanitization for security
- Test end-to-end flows
- Optimize database queries (add indexes, eager loading)
- Add detailed logging for debugging
- Create user documentation

### Acceptance Criteria
- [ ] All form validations work on frontend and backend
- [ ] Error messages are user-friendly and helpful
- [ ] API errors return proper HTTP status codes
- [ ] Notifications show for all user actions (success/error)
- [ ] No console errors in browser (non-dev)
- [ ] Dates are handled consistently across app
- [ ] Prices are formatted correctly (2 decimals)
- [ ] Dates display in consistent format
- [ ] Phone numbers are validated internationally
- [ ] Database queries are optimized
- [ ] Application handles network failures gracefully
- [ ] Error logs are accessible for debugging

### Testing
- Test complete customer order flow
- Test complete admin menu management flow
- Test error handling for various API failures
- Test validation on all form inputs
- Test edge cases (very long names, special characters)
- Test concurrent operations (race conditions)
- Test with slow network (loading states)
- Test with missing data scenarios

---

## Phase 10: Testing & QA

**Status**: Not started  
**Estimated Duration**: 1.5 weeks

### Goal
Implement comprehensive testing strategy across backend and frontend to ensure reliability, security, and correctness of all features.

### Scope
- Unit tests for business logic (backend)
- Integration tests for APIs
- Component tests for React components
- End-to-end testing for critical flows
- Performance testing
- Security testing
- Manual QA testing

### Tasks
- Write unit tests for Domain layer entities
- Write unit tests for Application service handlers
- Write integration tests for API endpoints
- Write component tests for React components
- Create E2E test scripts for critical flows
- Implement test data fixtures
- Set up test CI/CD pipeline
- Add code coverage reporting
- Perform security audit (input sanitization, auth, HTTPS)
- Test performance under load
- Document test strategy and coverage

### Acceptance Criteria
- [ ] Domain and Application layers have >80% test coverage
- [ ] API endpoints have integration tests
- [ ] All CRUD operations are tested
- [ ] Authentication/authorization flows are tested
- [ ] Error scenarios are tested
- [ ] React components have component tests
- [ ] Critical user flows have E2E tests
- [ ] No SQL injection vulnerabilities
- [ ] No XSS vulnerabilities
- [ ] All security headers are set
- [ ] Load test shows acceptable response times
- [ ] Performance is acceptable under load

### Testing
- Run unit test suite and verify coverage
- Run integration test suite
- Run component test suite
- Execute E2E tests
- Perform manual testing of all features
- Test security vulnerabilities
- Load test API endpoints
- Test database backup/restore

---

## Phase 11: Deployment & Operations

**Status**: Not started  
**Estimated Duration**: 1 week

### Goal
Set up production environment, deployment pipeline, monitoring, and operational procedures for stable and reliable system operation.

### Scope
- Production environment setup
- Deployment automation
- Monitoring and logging
- Backup and recovery procedures
- Performance optimization
- Security hardening
- Documentation

### Tasks
- Set up production database (backup, recovery plan)
- Configure production web server (Kestrel/IIS)
- Set up SSL certificates (HTTPS)
- Configure environment variables for production
- Create deployment scripts and automation
- Set up application logging to files/cloud
- Configure monitoring and alerting
- Implement database backup strategy (daily)
- Set up log aggregation
- Optimize static asset serving (CDN, compression)
- Create runbook for common operational tasks
- Document deployment procedures
- Set up health check endpoints
- Create rollback procedures

### Acceptance Criteria
- [ ] Application runs on production server
- [ ] SSL certificate is installed and valid
- [ ] Database backup runs automatically
- [ ] All logs are captured and accessible
- [ ] Performance monitoring is active
- [ ] Alerts notify ops team of issues
- [ ] Health checks endpoints respond
- [ ] Deployment can be automated
- [ ] Rollback procedures are documented
- [ ] Recovery time objective (RTO) < 1 hour
- [ ] Recovery point objective (RPO) < 1 hour
- [ ] All documentation is current

### Testing
- Test production deployment
- Test health check endpoints
- Test application functionality in production
- Test database backup and recovery
- Test logging and monitoring
- Test alert notifications
- Test rollback procedure
- Test performance under production load
- Test disaster recovery scenario

---

## Phase 12: Post-Launch & Enhancements

**Status**: Not started  
**Estimated Duration**: Ongoing

### Goal
Monitor production system, address user feedback, and plan for future enhancements beyond MVP.

### Scope
- Monitor system health and performance
- Gather user feedback
- Fix production bugs
- Plan future features
- Performance improvements

### Future Enhancement Candidates
- Customer order history and account (requires authentication)
- Email notifications for order status changes
- Payment integration
- Promotional discounts and coupons
- Customer ratings and reviews
- Menu categories and filtering
- Catering orders (bulk orders)
- Multi-location support
- API rate limiting and security improvements
- Admin audit logs
- Advanced reporting and analytics

### Tasks
- Monitor production metrics daily
- Respond to user-reported issues
- Collect feedback from customers and admins
- Analyze usage patterns
- Plan feature roadmap for next quarter
- Prioritize enhancements based on impact

---

## Summary

| Phase | Focus | Duration | Status |
|-------|-------|----------|--------|
| 1 | Setup & Infrastructure | 1 day | ✅ Complete |
| 2 | Authentication | 1 week | Not started |
| 3 | Menu Management API | 1 week | Not started |
| 4 | Order Management API | 1 week | Not started |
| 5 | Site Content API | 3 days | Not started |
| 6 | Order Form UI | 1 week | Not started |
| 7 | Landing Page & Menu Display | 1.5 weeks | Not started |
| 8 | Admin Dashboard | 1.5 weeks | Not started |
| 9 | Integration & Polish | 1 week | Not started |
| 10 | Testing & QA | 1.5 weeks | Not started |
| 11 | Deployment & Operations | 1 week | Not started |
| 12 | Post-Launch | Ongoing | Not started |

**Total Estimated Duration**: ~12 weeks from Phase 2 start

## Phase Execution Notes

- Each phase builds on previous phases' foundation
- Phases 2-5 are backend-heavy (APIs)
- Phases 6-8 add frontend UI components
- Phase 9 integrates all components
- Phase 10-11 ensure quality and production readiness
- Phase 12 focuses on sustainability and growth

## Next Steps

1. Begin Phase 2: Authentication & Authorization
2. Review current codebase against Phase 2 goals
3. Create detailed task breakdown for Phase 2
4. Set up sprint planning for Phase 2 execution

