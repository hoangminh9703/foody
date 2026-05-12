# Phase 8: Admin Dashboard UI

**Status**: Not started  
**Estimated Duration**: 1.5 weeks  
**Priority**: HIGH (admin-facing core feature)  
**Dependencies**: Phase 1 (Setup), Phase 2 (Auth), Phase 3-4 (APIs)

## Goal
Build the admin management interface for viewing orders, updating status, and managing daily menus.

## Scope
- Dashboard layout with sidebar navigation
- Orders list with search and filters
- Order detail view with items
- Status update interface
- Menu management interface (create, edit, delete)
- Admin authentication required
- Responsive design (tablet and desktop, not mobile-focused)

## Detailed Tasks

### Frontend (React + TypeScript)

#### Task 1: Create Dashboard Layout
**Location**: `web/src/layouts/DashboardLayout.tsx`

Steps:
1. Create layout structure:
   - Sidebar navigation (left)
   - Top header bar (user info, logout)
   - Main content area
   - Optional: Footer

2. Sidebar navigation:
   - Logo/Brand name
   - Menu items:
     - Dashboard (overview)
     - Orders
     - Menus
     - Settings (optional)
   - Logout button
   - Active page indicator

3. Header bar:
   - Current user name
   - Logout button
   - Time/date (optional)

4. Responsive design:
   - Desktop: Sidebar always visible
   - Tablet: Sidebar collapsible (hamburger menu)
   - (Mobile: Not supported for MVP)

**Technical Notes**:
- Use Flexbox for layout
- Make sidebar 250px wide (desktop)
- Use active route styling for nav items
- Smooth transitions for sidebar collapse

#### Task 2: Create Orders List Component
**Location**: `web/src/components/OrdersList.tsx`

Steps:
1. Create table/list view:
   - Order ID
   - Customer name
   - Order date
   - Meal type
   - Status (with color coding)
   - Total price
   - Actions (view, edit status)

2. Implement filtering:
   - Filter by status (dropdown)
   - Filter by date range
   - Filter by meal type
   - Search by customer name
   - Apply filters button or real-time

3. Implement pagination:
   - Page controls (Previous, Next)
   - Page size selector (10, 20, 50)
   - Total count display
   - Current page indicator

4. Styling:
   - Alternating row colors
   - Hover effects
   - Icons for status
   - Color-coded status badges

**Technical Notes**:
- Use `useEffect` to fetch orders on load/filter change
- Debounce search input (500ms)
- Support keyboard navigation (Enter to submit filters)
- Show loading state while fetching

#### Task 3: Create Order Detail Component
**Location**: `web/src/components/OrderDetail.tsx`

Steps:
1. Create detail view:
   - Order ID and creation date
   - Customer name and phone
   - Order date and meal type
   - Order items table:
     - Item name
     - Quantity
     - Unit price
     - Subtotal
     - Special request
   - Order totals
   - Notes section

2. Implement actions:
   - Close/back button
   - Update status dropdown
   - Cancel order button (optional)

3. Display status:
   - Current status badge (color-coded)
   - Status change history (optional)

**Technical Notes**:
- Fetch order details from API on load
- Format phone number (make clickable)
- Show loading skeleton while fetching
- Handle order not found (404) error

#### Task 4: Create Status Update Form
**Location**: `web/src/components/StatusUpdateForm.tsx`

Steps:
1. Create form:
   - Dropdown or buttons for status selection
   - Show valid transitions only
   - Show current status
   - Submit button

2. Implement status state machine:
   - Only show valid transitions
   - Disable invalid transitions
   - Show reason if transition not allowed

3. Handle submission:
   - Call PATCH `/api/orders/{id}/status`
   - Show loading state
   - Show success message
   - Show error message if fails
   - Refresh order details on success

**Technical Notes**:
- Parse valid transitions from state machine rules
- Disable buttons for invalid transitions
- Show confirmation dialog before changing status
- Update parent component state on success

#### Task 5: Create Menus List Component
**Location**: `web/src/components/MenusList.tsx`

Steps:
1. Create menu list view:
   - Date
   - Meal type (Lunch/Dinner)
   - Description
   - Number of items
   - Actions (view, edit, delete)

2. Implement sorting:
   - Sort by date (ascending/descending)
   - Sort by meal type

3. Implement filtering:
   - Filter by date range
   - Filter by meal type
   - Filter by "active" status

4. Show create button:
   - Button to open "Create Menu" modal
   - Access menu creation form

**Technical Notes**:
- Fetch menus from API with filters
- Support pagination if many menus
- Show loading state
- Delete confirmation required

#### Task 6: Create Menu Form Component
**Location**: `web/src/components/MenuForm.tsx`

Steps:
1. Create/edit form:
   - Date picker (required)
   - Meal type selector (required)
   - Description text area
   - Menu items section:
     - Item name, description, price, order
     - Add item button
     - Remove item button

2. Implement item management:
   - Add new item row
   - Remove item row
   - Validate item data
   - Show error if no items

3. Handle submit:
   - POST for create, PUT for edit
   - Show loading state
   - Show success/error message
   - Close modal on success
   - Refresh menu list

4. Modal/Dialog:
   - Show in modal overlay
   - Close button
   - Cancel/Save buttons

**Technical Notes**:
- Use controlled inputs with state
- Validate required fields
- Validate prices (>= 0)
- Prevent duplicate date + meal type
- Show validation errors inline

#### Task 7: Create Confirmation Dialog
**Location**: `web/src/components/ConfirmationDialog.tsx`

Steps:
1. Create reusable dialog:
   - Title
   - Message
   - Confirm button (red for destructive)
   - Cancel button
   - On confirm callback
   - On cancel callback

2. Use for:
   - Delete menu confirmation
   - Cancel order confirmation
   - Status change confirmation (optional)

**Technical Notes**:
- Modal overlay with focus trap
- Keyboard accessible (Esc to close)
- Semantic HTML (dialog element)
- Clear action buttons

#### Task 8: Implement Error Handling
**Location**: `web/src/components/ErrorBoundary.tsx`

Steps:
1. Create error boundary:
   - Catch render errors
   - Display error message
   - Show reload button
   - Log error for debugging

2. Implement API error handling:
   - Show toast notification for errors
   - Retry option for transient errors
   - Detailed error messages

3. Create error states:
   - 401 Unauthorized (redirect to login)
   - 404 Not Found (show friendly message)
   - 500 Server error (show friendly message)
   - Network error (show offline message)

**Technical Notes**:
- Use error boundary wrapper
- Implement fallback UI
- Log errors to console/backend
- Clear errors on successful action

#### Task 9: Add Loading States
**Location**: Multiple components

Steps:
1. Show spinners/skeletons for:
   - Orders list loading
   - Order detail loading
   - Menu items loading
   - Form submission

2. Disable interactions during loading:
   - Disable buttons
   - Disable form inputs
   - Show loading text

**Technical Notes**:
- Use skeleton loaders for lists/tables
- Show spinner for detail views
- Disable submit button during API call
- Timeout if loading > 30 seconds

### Testing Tasks

#### Task 10: Manual Testing

Steps:
1. Test authentication:
   - Navigate to `/admin`
   - Verify: Redirected to login if not authenticated
   - Log in successfully
   - Verify: Can access dashboard

2. Test orders list:
   - View all orders
   - Filter by status
   - Filter by date range
   - Search by customer name
   - Verify correct orders display

3. Test order detail:
   - Click on order
   - Verify: Detail view shows
   - Verify: All items display correctly
   - Verify: Totals calculated correctly

4. Test status update:
   - Click status dropdown
   - Select new status
   - Verify: Status updates in UI
   - Verify: List reflects change

5. Test menu management:
   - View all menus
   - Create new menu
   - Edit menu items
   - Delete menu (with confirmation)
   - Verify changes reflected in API

6. Test responsiveness:
   - Tablet view (768px)
   - Desktop view (1200px)
   - Verify layout adapts

7. Test logout:
   - Click logout button
   - Verify: Redirected to login
   - Verify: Cannot access admin routes

### Documentation Tasks

#### Task 11: Create Dashboard User Guide

Steps:
1. Document dashboard features
2. Document order management workflow
3. Document menu management workflow
4. Create screenshots for documentation

## Acceptance Criteria
- [x] Dashboard loads only when authenticated
- [x] Orders list displays all orders with pagination
- [x] Filtering by status, date, meal type works
- [x] Clicking order shows detail view with items
- [x] Status can be updated from dropdown/buttons
- [x] Menu list shows all menus
- [x] Can create new menu from dashboard
- [x] Can edit existing menu and items
- [x] Can delete menu with confirmation
- [x] Empty states shown for no data
- [x] Loading states shown during operations
- [x] All forms validate before submission
- [x] Success/error messages shown appropriately
- [x] Dashboard is responsive on tablet and desktop
- [x] Logout button is accessible

## Testing Procedures

### Manual Test Cases

1. **Access Dashboard**
   - Navigate to `/admin`
   - Without auth: Redirected to login
   - With auth: Dashboard loads

2. **View Orders List**
   - Orders table shows all orders
   - Columns: ID, Customer, Date, Status, Price
   - Can scroll horizontally (tablet)
   - Pagination works

3. **Filter Orders**
   - Filter by status (New/Contacted/etc)
   - Filter by date range
   - Filter by meal type
   - Search by customer name
   - All filters work together

4. **View Order Details**
   - Click order row
   - Detail modal/page opens
   - All order items display
   - Totals show correctly

5. **Update Order Status**
   - Click status in detail view
   - Dropdown shows valid options
   - Select new status
   - Shows confirmation
   - Status updates after confirm
   - List updates

6. **Create Menu**
   - Click "Create Menu" button
   - Form opens in modal
   - Enter date, meal type, items
   - Add multiple items
   - Submit
   - Menu appears in list

7. **Edit Menu**
   - Click edit on menu row
   - Form opens with current data
   - Change items/price
   - Submit
   - Updates reflected in API

8. **Delete Menu**
   - Click delete on menu row
   - Confirmation dialog shows
   - Click confirm
   - Menu removed from list

### Accessibility Checks
- Tab through all interactive elements
- Test with keyboard only (no mouse)
- Test with screen reader
- Verify heading hierarchy
- Verify form labels

## Files to Create/Modify

### New Files
```
web/src/layouts/DashboardLayout.tsx
web/src/pages/AdminPage.tsx
web/src/components/OrdersList.tsx
web/src/components/OrderDetail.tsx
web/src/components/StatusUpdateForm.tsx
web/src/components/MenusList.tsx
web/src/components/MenuForm.tsx
web/src/components/ConfirmationDialog.tsx
web/src/components/ErrorBoundary.tsx
web/src/services/adminService.ts (API integration)
web/src/styles/dashboard.css (optional)
```

### Modified Files
```
web/src/App.tsx (add admin routes)
web/src/components/Navigation.tsx (add admin link if authenticated)
```

## Dependencies
- React (already installed)
- React Router (already installed)

## Related Documentation
- [API_DESIGN.md](../architecture/API_DESIGN.md) - API specifications
- Phase 2 (Auth), Phase 3-4 (APIs) must be complete

## Notes
- Focus on tablet and desktop (not mobile)
- Admins will use daily, so UX should be efficient
- Consider bulk operations in Phase 2B (bulk status update)
- Consider export functionality in Phase 2B (CSV export)
