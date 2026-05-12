# Phase 7: Landing Page & Menu Display

**Status**: Not started  
**Estimated Duration**: 1.5 weeks  
**Priority**: HIGH (primary user entry point)  
**Dependencies**: Phase 1 (Setup), Phase 3 (Menu API), Phase 5 (Site Content API)

## Goal
Build the public-facing landing page that showcases the restaurant and displays current daily menu with call-to-action for ordering.

## Scope
- Hero section with company branding and messaging
- Menu display by meal type (lunch/dinner)
- Daily menu loading from API
- Company info section with contact details
- Call-to-action buttons (Order button)
- Responsive mobile-first design
- No authentication required

## Detailed Tasks

### Frontend (React + TypeScript)

#### Task 1: Create Landing Page Layout
**Location**: `web/src/pages/HomePage.tsx`

Steps:
1. Create page structure:
   - Hero section (top)
   - About section (company intro)
   - Menu section (current day)
   - CTA section (Call To Action)
   - Contact section (footer)

2. Implement responsive grid:
   - Mobile: 1 column
   - Tablet: 1-2 columns depending on section
   - Desktop: Full width with content containers

3. Style with design tokens:
   - White background (colors.white)
   - Black text (colors.black)
   - Yellow accents (colors.yellow) for CTAs and highlights
   - Use spacing tokens for consistent padding/margins

**Technical Notes**:
- Use semantic HTML (section, article, nav)
- Make page highly accessible
- Optimize for Core Web Vitals (LCP, CLS, FID)

#### Task 2: Create Hero Component
**Location**: `web/src/components/Hero.tsx`

Steps:
1. Create hero section:
   - Company name (large, bold)
   - Tagline/subtitle
   - "Order Now" call-to-action button
   - Optional: Hero background image/gradient

2. Implement responsiveness:
   - Mobile: Stacked layout, 20px padding
   - Desktop: Centered content, larger fonts

3. Make interactive:
   - Button navigates to `/order` page
   - Hover effects (yellow highlight)
   - Smooth scroll on mobile

**Technical Notes**:
- Use CSS Grid or Flexbox for centering
- Hero should be full viewport height (100vh) or at least 70vh
- Consider image optimization for performance
- Use semantic button elements

#### Task 3: Create Company Info Component
**Location**: `web/src/components/CompanyInfo.tsx`

Steps:
1. Display company information:
   - Company name
   - Description (from API)
   - Phone number (clickable tel: link)
   - Address
   - Service area
   - Hours of operation

2. Fetch from Site Content API:
   - Call `/api/site-content?keys=CompanyName,CompanyPhone,CompanyDescription,...`
   - Handle loading state
   - Handle error state
   - Cache data for performance

3. Layout:
   - Grid layout on desktop
   - Stack vertically on mobile
   - Use icons for phone, location, etc.

**Technical Notes**:
- Use `tel:` protocol for phone links
- Use `mailto:` for email
- Load site content on component mount
- Show placeholder while loading

#### Task 4: Create Menu Display Component
**Location**: `web/src/components/MenuDisplay.tsx`

Steps:
1. Create menu section:
   - Show current day's menu
   - Separate lunch and dinner if both available
   - Display items in grid or list

2. Implement menu item display:
   - Item name
   - Description
   - Price (formatted currency)
   - Optional: image

3. Fetch menu from API:
   - Call `/api/menus?dateFrom=today&dateTo=today&mealType=Lunch` (and Dinner)
   - Handle loading state (skeleton loader)
   - Handle empty state ("Menu coming soon")
   - Handle error state

4. Display sections:
   - One section per meal type
   - Sort items by displayOrder
   - Group related items if needed

**Technical Notes**:
- Use `useEffect` to fetch on mount
- Cache menu data (don't refetch on every render)
- Show loading skeleton for better UX
- Handle "no menu available" gracefully

#### Task 5: Create Menu Item Component
**Location**: `web/src/components/MenuItem.tsx`

Steps:
1. Create component for individual menu item:
   - Item name (bold)
   - Description (smaller text)
   - Price (yellow, prominent)
   - Optional: image thumbnail

2. Styling:
   - Card-like appearance with border/shadow
   - Yellow highlight on price
   - Readable on mobile

3. Handle interaction:
   - Show tooltip on hover (description)
   - Optional: Click to add to cart (Phase 6 integration)

**Technical Notes**:
- Use Card component for consistency
- Format price: `$5.50`
- Truncate long descriptions with ellipsis

#### Task 6: Create CTA Section
**Location**: `web/src/components/CTASection.tsx`

Steps:
1. Create prominent section:
   - Catchy headline ("Ready to order?")
   - Brief description
   - Large "Order Now" button
   - Secondary "Learn More" button (optional)

2. Make visually prominent:
   - Yellow background with black text (bold)
   - Full width section
   - Centered content
   - Large button (45px+ height)

3. Link to order form:
   - Primary button → `/order`
   - Secondary button → menu section (scroll)

**Technical Notes**:
- Use CSS gradient for background
- Ensure text contrast (WCAG AA minimum)
- Make buttons keyboard accessible

#### Task 7: Create Loading Skeleton
**Location**: `web/src/components/MenuSkeleton.tsx` (or `LoadingSkeleton.tsx`)

Steps:
1. Create skeleton for menu items:
   - Placeholder boxes matching item layout
   - Animated shimmer effect
   - Show while loading

2. Use CSS animations:
   - Shimmer effect (left to right)
   - Smooth, non-intrusive animation

**Technical Notes**:
- Don't use actual loading spinner (skeleton is better UX)
- Make skeleton match final layout
- Animate smoothly (ease-in-out)

#### Task 8: Integrate Navigation
**Location**: `web/src/components/Navigation.tsx` (modify)

Steps:
1. Add top navigation:
   - Company logo/name
   - Menu items (Home, Menu, Order, Contact)
   - Order button (floating or fixed on mobile)

2. Make sticky (optional):
   - Stay at top while scrolling
   - Show shadow when scrolled

3. Mobile hamburger menu (optional):
   - Collapsible menu on mobile
   - Smooth transition

**Technical Notes**:
- Use semantic nav element
- Ensure keyboard accessible
- Logo links to home page

### Integration Tasks

#### Task 9: Set Up Page Routes
**Location**: `web/src/App.tsx` (modify)

Steps:
1. Add route:
   ```typescript
   <Route path="/" element={<HomePage />} />
   <Route path="/order" element={<OrderPage />} />
   ```

2. Ensure home page is default route
3. Test routing works correctly

**Technical Notes**:
- Home page should be accessible at `/` and `/home`
- Ensure navigation between home and order works
- Test on different screen sizes

### Testing Tasks

#### Task 10: Manual Testing

Steps:
1. Test page load:
   - Navigate to `/`
   - Verify all sections load
   - Verify no console errors

2. Test menu loading:
   - Verify today's menu loads
   - Verify items display with prices
   - Verify lunch and dinner separated (if both exist)

3. Test responsiveness:
   - Mobile: 320px, 375px, 425px widths
   - Tablet: 768px width
   - Desktop: 1024px, 1440px widths
   - Verify no horizontal scrolling
   - Verify text is readable

4. Test interactions:
   - Click "Order Now" button
   - Verify navigates to `/order`
   - Click phone number
   - Verify opens phone dialer/mail
   - Test all links

5. Test accessibility:
   - Use screen reader (NVDA, JAWS)
   - Navigate using keyboard only
   - Verify all content accessible

6. Test performance:
   - Check Core Web Vitals
   - Load time < 3 seconds
   - First Contentful Paint < 1.8s

7. Test API integration:
   - Verify menu API called correctly
   - Verify site content API called correctly
   - Check network tab in DevTools

### Documentation Tasks

#### Task 11: Create Page Documentation

Steps:
1. Document page structure
2. Document components used
3. Document API integrations
4. Document performance tips

## Acceptance Criteria
- [x] Landing page loads and displays correctly
- [x] Hero section is prominent and has clear CTA button
- [x] Current daily menu displays for both lunch and dinner
- [x] Menu items show with price, name, and description
- [x] Clicking order button navigates to order form
- [x] Company contact info is displayed
- [x] Page is fully responsive on mobile/tablet/desktop
- [x] Menu items are sorted by displayOrder
- [x] Loading state shown while fetching menu
- [x] Empty state shown if no menu available
- [x] Color palette matches brief (white, yellow, black)
- [x] Font sizes are readable on mobile

## Testing Procedures

### Manual Test Cases

1. **Page Loads Successfully**
   - Navigate to `/`
   - Verify: Hero section visible
   - Verify: Menu section visible
   - Verify: Contact info visible
   - Verify: No errors in console

2. **Menu Displays Correctly**
   - Check today's date
   - Verify: Menu for today displays
   - Verify: Items have name, description, price
   - Verify: Items sorted by displayOrder

3. **Lunch/Dinner Separation**
   - If both menus exist:
     - Verify: Lunch section shows lunch items
     - Verify: Dinner section shows dinner items
   - If only one:
     - Verify: Only one section shows

4. **Order Button Navigation**
   - Click "Order Now" button on hero
   - Verify: Navigates to `/order`
   - Verify: Order form loads

5. **Company Info Display**
   - Verify: Company name displays
   - Verify: Company phone displays (clickable)
   - Verify: Company description displays
   - Verify: Contact info is accurate

6. **Responsive Design**
   - Open DevTools (F12)
   - Mobile view (375px):
     - Single column layout
     - Menu items stack
     - Buttons full width
   - Tablet view (768px):
     - Columns appear
     - Layout adapts
   - Desktop view (1440px):
     - Full layout
     - Good spacing

7. **Loading States**
   - Open DevTools network tab
   - Throttle to "Fast 3G"
   - Navigate to `/`
   - Verify: Loading skeleton shows
   - Verify: Content loads progressively
   - Verify: Not stuck in loading state

8. **Empty State**
   - Create a date with no menu
   - Try to view landing page for that date
   - Verify: "Menu coming soon" message shows
   - Verify: Still can click "Order Now"

### Accessibility Checks
- Tab through page (no keyboard traps)
- Verify all text has sufficient contrast
- Verify heading hierarchy (h1, h2, h3)
- Test with screen reader
- Verify all links have descriptive text

## Files to Create/Modify

### New Files
```
web/src/pages/HomePage.tsx
web/src/components/Hero.tsx
web/src/components/MenuDisplay.tsx
web/src/components/MenuItem.tsx
web/src/components/CompanyInfo.tsx
web/src/components/CTASection.tsx
web/src/components/MenuSkeleton.tsx
web/src/services/menuService.ts
web/src/services/siteContentService.ts
web/src/styles/landing-page.css (optional, or use inline styles)
```

### Modified Files
```
web/src/App.tsx (add route)
web/src/components/Navigation.tsx (update)
web/src/styles/tokens.ts (if new tokens needed)
```

## Dependencies
- React (already installed)
- React Router (already installed)

## Related Documentation
- [API_DESIGN.md](../architecture/API_DESIGN.md) - API specifications
- Phase 3 (Menu API) and Phase 5 (Site Content API) must be complete

## Notes
- This is the primary entry point for customers
- Focus on mobile experience first
- Optimize images for performance
- Consider SEO (meta tags, structured data in Phase 2B)
- Consider lazy loading images (Phase 2B)
