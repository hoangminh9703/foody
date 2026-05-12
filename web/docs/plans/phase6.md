# Phase 6: Order Form & Customer UI

**Status**: Not started  
**Estimated Duration**: 1 week  
**Priority**: HIGH (customer-facing feature)  
**Dependencies**: Phase 1 (Setup), Phase 4 (Order API)

## Goal
Build the customer-facing order submission form with menu selection, validation, and success/error feedback for easy order placement.

## Scope
- Order form component with customer info fields
- Menu item selection and quantity input
- Dynamic form validation (client-side + server-side)
- Submit order to API
- Show success/error messages
- Mobile-optimized responsive form
- No authentication required for customers

## Detailed Tasks

### Frontend (React + TypeScript)

#### Task 1: Create Order Form Component
**Location**: `web/src/components/OrderForm.tsx`

Steps:
1. Create controlled form component:
   ```typescript
   interface OrderFormState {
     customerName: string
     customerPhone: string
     orderDate: string
     mealType: 'Lunch' | 'Dinner'
     selectedItems: OrderItem[]
     notes: string
     isSubmitting: boolean
     error: string | null
     success: boolean
   }
   ```

2. Implement form handlers:
   - `handleInputChange()` for text inputs
   - `handleDateChange()` for date picker
   - `handleMealTypeChange()` for meal type selection
   - `handleAddItem()` to add item to order
   - `handleRemoveItem()` to remove item
   - `handleSubmit()` to submit order
   - `handleReset()` to clear form

3. Add form validation:
   - Required fields: name, phone, at least one item
   - Phone format: 7-15 digits
   - Date: must not be past date
   - Show validation errors next to fields

4. Style with design tokens:
   - Use colors.white, colors.black, colors.yellow
   - Use typography tokens
   - Proper spacing and layout
   - Responsive grid (1 column on mobile, 2 on desktop)

**Technical Notes**:
- Use `useState` for form state
- Use `useCallback` to memoize handlers
- Use React Hook Form for advanced form management (optional)
- Show loading spinner during submission
- Disable submit button while loading

#### Task 2: Create Menu Selector Component
**Location**: `web/src/components/MenuSelector.tsx`

Steps:
1. Create component to display available menu items:
   - Show menu items for selected date/meal type
   - Display item name, description, price
   - Input field for quantity
   - Add button to add to cart
   - Show error if no menu available

2. Fetch menu items:
   - Call `/api/menus` on date/mealType change
   - Handle loading state
   - Handle error state
   - Display empty state if no items

3. Implement item selection:
   - Track selected items with quantities
   - Update parent component state
   - Show selected items in summary

**Technical Notes**:
- Use `useEffect` to fetch menu items
- Cache menu items to avoid redundant API calls
- Show loading skeleton while fetching
- Show helpful message if no menu available

#### Task 3: Create Selected Items Review Component
**Location**: `web/src/components/OrderItemsReview.tsx`

Steps:
1. Create component showing selected items:
   - List of selected items with quantities
   - Show unit price and subtotal for each
   - Show total quantity and total price
   - Allow to remove items
   - Allow to edit quantities

2. Display item details:
   - Item name
   - Unit price
   - Quantity (with increment/decrement buttons)
   - Subtotal (quantity × price)
   - Special request field for each item

3. Summary section:
   - Total Items (count)
   - Total Quantity (sum of quantities)
   - Total Price (formatted as currency)

**Technical Notes**:
- Use memo to prevent unnecessary re-renders
- Format prices with 2 decimal places
- Color total price prominently (yellow background)
- Make increment/decrement buttons easy to use

#### Task 4: Create Form Validation Logic
**Location**: `web/src/services/formValidation.ts`

Steps:
1. Create validation functions:
   ```typescript
   validateCustomerName(name: string): string | null
   validateCustomerPhone(phone: string): string | null
   validateOrderDate(date: string): string | null
   validateSelectedItems(items: OrderItem[]): string | null
   ```

2. Implement validations:
   - Name: required, 2-100 chars
   - Phone: required, 7-15 digits only
   - Date: required, not past date
   - Items: required, at least 1 item

3. Create helper function:
   - `validateOrderForm(formData): ValidationErrors`
   - Returns object with error messages for each field
   - Empty object if all valid

4. Show inline errors:
   - Display next to each field in red
   - Clear on input change
   - Prevent submission if errors exist

**Technical Notes**:
- Phone regex: `^\d{7,15}$`
- Date validation: `new Date(date) > new Date()`
- Provide user-friendly error messages
- Validate both client-side and server-side

#### Task 5: Integrate with Order API
**Location**: `web/src/services/orderService.ts`

Steps:
1. Create order service:
   ```typescript
   submitOrder(orderData: CreateOrderRequest): Promise<OrderResponse>
   ```

2. Implement API call:
   - POST to `/api/orders` with order data
   - Handle success (show confirmation)
   - Handle errors (show error message)
   - Include retry logic for network errors (optional)

3. Error handling:
   - Network error: "Unable to submit order. Check connection."
   - Validation error: Display backend validation message
   - Server error: "Server error. Please try again later."
   - No menu available: "Menu not available for selected date"

**Technical Notes**:
- Set `credentials: 'include'` if using cookies
- Use fetch or axios with proper Content-Type
- Retry on network errors (exponential backoff optional)
- Log errors for debugging

#### Task 6: Create Success/Error Message Components
**Location**: 
- `web/src/components/SuccessMessage.tsx`
- `web/src/components/ErrorMessage.tsx`

Steps:
1. Create `SuccessMessage` component:
   - Show checkmark icon (✓)
   - Display message: "Order submitted successfully!"
   - Show order reference (if provided by API)
   - Show "New Order" button to reset form
   - Use yellow background (highlight success)

2. Create `ErrorMessage` component:
   - Show error icon (✗)
   - Display error message
   - Suggest action (e.g., "Try again" button)
   - Use red background (indicate error)
   - Include retry logic

3. Position messages:
   - Show above form on success
   - Show above form or inline on error
   - Auto-dismiss after 5 seconds (optional)
   - Allow manual close

**Technical Notes**:
- Make messages accessible (aria-live="polite")
- Use semantic HTML
- Provide icons for visual clarity
- Keep messages concise and actionable

#### Task 7: Handle Loading States
**Location**: `web/src/components/OrderForm.tsx` (extend)

Steps:
1. Show loading spinner during:
   - Menu item fetch
   - Form submission

2. Disable interactions:
   - Disable submit button while loading
   - Disable item selection during menu fetch
   - Show loading text on button

3. Create LoadingSpinner component:
   - Animated spinner (CSS or icon)
   - Loading message
   - Position centered or inline

**Technical Notes**:
- Use CSS animations for smooth spinner
- Show helpful loading messages
- Prevent double submission (button disabled)
- Timeout if loading takes too long (> 10 seconds)

#### Task 8: Implement Mobile Responsiveness
**Location**: `web/src/styles/` (utilize tokens)

Steps:
1. Use responsive design:
   - Mobile: 1 column layout
   - Tablet (768px): 2 column layout
   - Desktop: 2-3 column layout

2. Optimize for mobile:
   - Large touch targets (min 44px)
   - Clear labels above inputs
   - Full-width buttons
   - Stack sections vertically

3. Use CSS media queries or Tailwind:
   - `@media (min-width: 768px)` for tablet
   - `@media (min-width: 1024px)` for desktop

**Technical Notes**:
- Test on actual mobile devices
- Ensure input fields are not zoomed on focus
- Use viewport meta tag (should be in base HTML)
- Test keyboard navigation

#### Task 9: Add Special Requests Field (Optional)
**Location**: `web/src/components/OrderForm.tsx` (extend)

Steps:
1. Add optional special requests field:
   - Text area for each item
   - Show "Special requests?" label
   - Character limit: 500 chars (optional)
   - Show remaining chars count

2. Include in order submission:
   - Pass to API in each item
   - Display in order confirmation

**Technical Notes**:
- Make this field optional (no validation)
- Show helpful placeholder text
- Allow line breaks in textarea

### Testing Tasks

#### Task 10: Manual Testing

Steps:
1. Test form rendering:
   - Navigate to `/order` page
   - Verify form displays all fields
   - Verify form looks good on mobile/tablet/desktop

2. Test menu selection:
   - Select today's date
   - Select Lunch meal type
   - Verify menu items load
   - Verify prices display correctly

3. Test item selection:
   - Add item to cart
   - Verify appears in review section
   - Verify total updates correctly
   - Add multiple items
   - Verify total price and quantity calculated correctly

4. Test form validation:
   - Try to submit without name: error message appears
   - Try invalid phone (abc): error message appears
   - Try to submit without items: error message appears
   - Correct errors and submit succeeds

5. Test submission:
   - Fill complete form
   - Click submit
   - Verify loading state shows
   - Verify success message appears
   - Verify form resets for new order (optional)

6. Test error handling:
   - Simulate network error (offline mode)
   - Verify error message shows
   - Verify can retry

7. Test responsive design:
   - Test on mobile (320px width)
   - Test on tablet (768px width)
   - Test on desktop (1200px width)
   - Verify layout adapts properly

### Documentation Tasks

#### Task 11: Create User Instructions

Steps:
1. Create simple instructions for users:
   - How to select date/meal type
   - How to add items
   - How to submit order
   - What to expect after submission

2. Show in-form help text:
   - Placeholder text on inputs
   - Labels clearly identifying fields
   - Error messages guiding correction

## Acceptance Criteria
- [x] Form displays all required fields clearly
- [x] Form validates before submission
- [x] Customer cannot submit without selecting items
- [x] Phone number format is validated client-side
- [x] Form shows loading state during submission
- [x] Success message displays after submission
- [x] Error message shows if submission fails
- [x] Form is fully responsive on mobile
- [x] Special requests field is optional
- [x] Menu items show prices
- [x] Selected items display with quantities
- [x] Form can be reset for new order

## Testing Procedures

### Manual Test Cases

1. **Complete Valid Submission**
   - Navigate to `/order`
   - Enter name: "John Doe"
   - Enter phone: "1234567890"
   - Select today as order date
   - Select Lunch meal type
   - Select 2 items from menu
   - Enter special request: "No onions"
   - Click Submit
   - Verify: Loading state shows
   - Verify: Success message displays

2. **Validation Error: Empty Name**
   - Leave name empty
   - Click Submit
   - Verify: Error message "Name is required"
   - Error appears below name field

3. **Validation Error: Invalid Phone**
   - Enter phone: "abc123"
   - Click Submit
   - Verify: Error message "Invalid phone format"

4. **Validation Error: No Items**
   - Fill name and phone
   - Don't select any items
   - Click Submit
   - Verify: Error message "Please select at least one item"

5. **Form Reset After Success**
   - Submit valid order
   - See success message
   - Click "New Order" button
   - Verify: Form clears for new order

6. **Mobile Responsiveness**
   - Open on mobile (use DevTools)
   - Verify: Single column layout
   - Verify: Buttons are large (44px+ height)
   - Verify: Form is readable without horizontal scroll

7. **Menu Loading**
   - Change order date
   - Verify: Loading indicator shows
   - Verify: Menu items load for new date
   - Verify: Cannot submit while loading

### Keyboard Navigation
- Tab through all form fields
- Verify logical tab order
- Verify all buttons accessible via keyboard
- Verify Enter key submits form

## Files to Create/Modify

### New Files
```
web/src/components/OrderForm.tsx
web/src/components/MenuSelector.tsx
web/src/components/OrderItemsReview.tsx
web/src/components/SuccessMessage.tsx
web/src/components/ErrorMessage.tsx
web/src/components/LoadingSpinner.tsx
web/src/pages/OrderPage.tsx
web/src/services/formValidation.ts
web/src/services/orderService.ts
web/src/types/order.ts (TypeScript interfaces)
```

### Modified Files
```
web/src/App.tsx (add route for /order)
web/src/styles/tokens.ts (if new tokens needed)
```

## Dependencies
- React (already installed)
- React Router (already installed)
- No additional dependencies required

## Related Documentation
- [API_DESIGN.md](../architecture/API_DESIGN.md) - Order API specification
- Phase 4 completed (Order API required)

## Notes
- This is customer-facing component (public, no auth)
- Focus on user experience and mobile compatibility
- Consider implementing autocomplete for customer data (Phase 2B)
- Consider order receipt/confirmation email (Phase 2B)
