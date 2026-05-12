# Phase 2: Authentication & Authorization

**Status**: Not started  
**Estimated Duration**: 1 week  
**Priority**: HIGH (blocks all admin features)

## Goal
Implement secure admin authentication and role-based access control so only authorized administrators can access the dashboard and management features.

## Scope
- Admin login system with email/password
- Session management with secure cookies
- Protected API endpoints and routes
- Logout and session timeout
- No customer authentication (scope excludes this)

## Detailed Tasks

### Backend (.NET Core)

#### Task 1: Create Authentication Service in Application Layer
**Location**: `backend/src/Application/Services/AuthenticationService.cs`

Steps:
1. Create `IAuthenticationService` interface with methods:
   - `Task<AuthResult> LoginAsync(string email, string password)`
   - `Task<bool> ValidatePasswordAsync(string password, string hash)`
   - `Task<User> GetUserByEmailAsync(string email)`

2. Implement `AuthenticationService` class:
   - Use BCrypt.Net-Next for password verification
   - Query User entity from database
   - Return success/failure with user details
   - Log authentication attempts

3. Create `AuthResult` DTO:
   - Success (bool)
   - Message (string)
   - UserId (int)
   - Email (string)
   - FullName (string)

**Technical Notes**:
- Use dependency injection for database access
- Implement proper error messages (avoid "user not found" leaking user existence)
- Add rate limiting preparation for future phases

#### Task 2: Create Login Command and Handler (CQRS Pattern)
**Location**: `backend/src/Application/Commands/LoginCommand.cs` and `LoginCommandHandler.cs`

Steps:
1. Create `LoginCommand` class:
   - Email: string (required, validated)
   - Password: string (required)

2. Create `LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>`:
   - Validate input (email format, password length)
   - Call `AuthenticationService.LoginAsync()`
   - Return result

3. Register handler in Application DI:
   - Add MediatR handler registration

**Technical Notes**:
- Use FluentValidation for LoginCommand validation
- Ensure email format validation
- Require minimum password length (8 chars for validation, accept any length for hash check)

#### Task 3: Create Login API Controller
**Location**: `backend/src/Api/Controllers/AuthController.cs`

Steps:
1. Create `AuthController : ControllerBase`:
   - Endpoint: `POST /api/auth/login`
   - Accept `LoginRequest` DTO (Email, Password)
   - Call `LoginCommand` via MediatR
   - Return `200 Ok` with user info on success
   - Return `401 Unauthorized` on failure
   - Return `400 BadRequest` on validation error

2. Implement request/response DTOs:
   ```
   LoginRequest: { email, password }
   LoginResponse: { userId, email, fullName, token, expiresAt }
   ```

3. Add XML documentation comments for Swagger

**Technical Notes**:
- Use `[ApiController]` attribute
- Use FluentValidation or DataAnnotations for validation
- Do NOT log passwords in any form
- Include timestamp in response for client-side clock sync

#### Task 4: Configure Session/Cookie Middleware
**Location**: `backend/src/Api/Program.cs` (modify)

Steps:
1. Add session services in `Program.cs`:
   ```
   services.AddSession(options => {
       options.IdleTimeout = TimeSpan.FromMinutes(30);
       options.Cookie.Name = "Medicare_SessionId";
       options.Cookie.HttpOnly = true;
       options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
       options.Cookie.SameSite = SameSiteMode.Strict;
   })
   ```

2. Add session middleware in pipeline:
   ```
   app.UseSession();
   ```

3. Store user info in session after login:
   ```
   HttpContext.Session.SetInt32("UserId", user.Id);
   HttpContext.Session.SetString("Email", user.Email);
   HttpContext.Session.SetString("FullName", user.FullName);
   ```

**Technical Notes**:
- Set `HttpOnly = true` (prevents JavaScript access)
- Set `SecurePolicy = Always` (HTTPS only)
- Set `SameSite = Strict` (CSRF protection)
- Set appropriate timeout (30 minutes recommended)

#### Task 5: Create Session Validation Middleware
**Location**: `backend/src/Api/Middleware/SessionValidationMiddleware.cs`

Steps:
1. Create custom middleware to validate protected endpoints:
   - Check if request path requires authentication (e.g., `/api/admin/*`)
   - Verify session exists and is valid
   - Return `401 Unauthorized` if not authenticated
   - Attach user context for use in handlers

2. Register middleware in `Program.cs`:
   ```
   app.UseMiddleware<SessionValidationMiddleware>();
   ```

**Technical Notes**:
- Use attribute `[Authorize]` on protected endpoints as alternative/complement
- Consider ClaimsIdentity for more advanced scenarios in later phases
- Log authentication failures for security audit

#### Task 6: Create Logout Endpoint
**Location**: `backend/src/Api/Controllers/AuthController.cs` (extend)

Steps:
1. Add logout endpoint:
   - `POST /api/auth/logout`
   - Clear session: `HttpContext.Session.Clear()`
   - Clear authentication cookie
   - Return `200 Ok`

2. Add session timeout logic:
   - Implement background service to clean expired sessions (future enhancement)
   - Or rely on session middleware timeout

**Technical Notes**:
- Ensure all session data is cleared completely
- Consider adding to audit log

### Frontend (React + TypeScript)

#### Task 7: Create Authentication Context
**Location**: `web/src/context/AuthContext.tsx`

Steps:
1. Create `AuthContext` interface:
   ```typescript
   interface AuthContextType {
     isAuthenticated: boolean;
     user: User | null;
     login: (email: string, password: string) => Promise<void>;
     logout: () => Promise<void>;
     isLoading: boolean;
     error: string | null;
   }
   ```

2. Create `AuthContext` with `createContext()`

3. Create `AuthProvider` component:
   - Manage authentication state
   - Handle login/logout API calls
   - Persist user state (session storage or memory)
   - Handle errors gracefully

**Technical Notes**:
- Use TypeScript strictly for type safety
- Do NOT store password in context
- Consider using `useReducer` for complex state management
- Use useCallback to prevent unnecessary re-renders

#### Task 8: Create Login Page Component
**Location**: `web/src/pages/LoginPage.tsx`

Steps:
1. Create login form with:
   - Email input field
   - Password input field
   - Submit button
   - Error message display
   - Loading state during submission

2. Implement form handling:
   - Use `useState` for form state
   - Validate email format client-side
   - Call `AuthContext.login()` on submit
   - Handle errors with user-friendly messages
   - Redirect to dashboard on success

3. Style with design tokens:
   - Use colors.white, colors.yellow, colors.black
   - Use typography tokens for consistency
   - Ensure mobile responsiveness
   - Use `Container` and `Button` components

**Technical Notes**:
- Use controlled inputs with state
- Add form validation before submission
- Prevent submission during loading
- Use React Router's `useNavigate` to redirect
- Show password strength indicator (optional)

#### Task 9: Create Protected Route Component
**Location**: `web/src/components/ProtectedRoute.tsx`

Steps:
1. Create `ProtectedRoute` component:
   - Check `AuthContext.isAuthenticated`
   - Redirect to login if not authenticated
   - Render component if authenticated
   - Show loading state while checking auth

2. Use in routing:
   ```typescript
   <Route 
     path="/admin/*" 
     element={<ProtectedRoute><DashboardLayout /></ProtectedRoute>} 
   />
   ```

**Technical Notes**:
- Use React Router v6 patterns
- Check authentication on mount and when it changes
- Handle async auth checks properly

#### Task 10: Create Auth Service (API Integration)
**Location**: `web/src/services/authService.ts`

Steps:
1. Create `authService` with functions:
   - `login(email: string, password: string): Promise<LoginResponse>`
   - `logout(): Promise<void>`
   - `getCurrentUser(): Promise<User | null>`
   - `isAuthenticated(): boolean`

2. Implement API calls:
   - POST to `/api/auth/login` with credentials
   - POST to `/api/auth/logout`
   - Handle response parsing and errors
   - Use fetch or axios with proper headers

3. Handle errors:
   - Network errors
   - 401 Unauthorized
   - 400 Bad Request (validation)
   - 500 Server errors

**Technical Notes**:
- Set `credentials: 'include'` for cookie-based auth
- Add request/response interceptors
- Handle Content-Type headers properly
- Add retry logic for transient errors (optional)

#### Task 11: Add Login Link to Navigation
**Location**: `web/src/components/Navigation.tsx` (modify or create)

Steps:
1. Create `Navigation` component or update Layout:
   - Show "Login" link if not authenticated
   - Show user name and "Logout" button if authenticated
   - Use AuthContext to check status

2. Implement logout handler:
   - Call `authService.logout()`
   - Clear auth context
   - Redirect to home page

**Technical Notes**:
- Make navigation responsive
- Use icons for better UX
- Handle loading state in logout button

### Integration Tasks

#### Task 12: Test Authentication Flow
**Location**: Manual testing steps

Steps:
1. Start backend: `dotnet run --project backend/src/Api/Medicare.Api.csproj`
2. Start frontend: `npm run dev` in web folder
3. Navigate to login page
4. Test valid credentials (admin@medicare.local / Admin123!)
5. Verify redirect to dashboard
6. Test invalid credentials
7. Test logout
8. Verify protected routes require login

#### Task 13: Update Swagger/OpenAPI Documentation
**Location**: `backend/src/Api/Program.cs` (modify)

Steps:
1. Add authentication scheme to Swagger:
   ```csharp
   options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
   {
       Name = "Authorization",
       Type = SecuritySchemeType.Http,
       Scheme = "bearer"
   });
   ```

2. Mark protected endpoints with `[Authorize]`

3. Add security requirements to Swagger

**Technical Notes**:
- Use JWT bearer scheme for consistency with future phases
- Document all authentication requirements in Swagger

## Acceptance Criteria
- [x] Admin can log in with valid credentials
- [x] Invalid credentials return appropriate error message
- [x] Session is established and stored in secure cookies
- [x] Unauthenticated users cannot access admin routes (API or UI)
- [x] Admin can log out and session is cleared
- [x] Swagger API shows protected endpoints requiring authentication
- [x] CORS is properly configured for auth endpoints
- [x] Session timeout is implemented (30 minutes of inactivity)
- [x] All passwords in database are hashed with BCrypt
- [x] Login form has basic client-side validation

## Testing Procedures

### Manual Test Cases

1. **Valid Login**
   - Navigate to `/login`
   - Enter email: `admin@medicare.local`
   - Enter password: `Admin123!`
   - Click login
   - Verify: Redirected to `/admin` dashboard

2. **Invalid Email**
   - Navigate to `/login`
   - Enter email: `nonexistent@test.com`
   - Enter password: `anypassword`
   - Click login
   - Verify: Error message "Invalid email or password"

3. **Invalid Password**
   - Navigate to `/login`
   - Enter email: `admin@medicare.local`
   - Enter password: `wrongpassword`
   - Click login
   - Verify: Error message "Invalid email or password"

4. **Protected Route Access**
   - Without logging in, try to access `/admin`
   - Verify: Redirected to `/login`

5. **Session Persistence**
   - Log in successfully
   - Refresh page
   - Verify: Still logged in (session persists)

6. **Logout**
   - Log in successfully
   - Click logout button
   - Verify: Redirected to `/` homepage
   - Try to access `/admin`
   - Verify: Redirected to `/login` (session cleared)

7. **Session Timeout**
   - Log in successfully
   - Wait 30 minutes without activity
   - Verify: Session expires, request to protected route returns 401

8. **Form Validation**
   - Try to submit empty form
   - Verify: Error message for required fields
   - Try invalid email format (no @)
   - Verify: Client-side validation error

### Browser Console Checks
- No JavaScript errors
- No sensitive data logged
- Session cookie visible in DevTools (HttpOnly, Secure, SameSite flags set)

### Backend Logs
- Verify authentication attempts are logged
- Verify failed login attempts are logged
- Verify no passwords are logged

## Files to Create/Modify

### New Files
```
backend/src/Application/Services/AuthenticationService.cs
backend/src/Application/Services/IAuthenticationService.cs
backend/src/Application/Commands/LoginCommand.cs
backend/src/Application/Commands/LoginCommandHandler.cs
backend/src/Application/DTOs/AuthResult.cs
backend/src/Api/Controllers/AuthController.cs
backend/src/Api/Middleware/SessionValidationMiddleware.cs
web/src/context/AuthContext.tsx
web/src/pages/LoginPage.tsx
web/src/components/ProtectedRoute.tsx
web/src/services/authService.ts
```

### Modified Files
```
backend/src/Api/Program.cs (add session config, middleware)
backend/src/Infrastructure/Data/MedicareDbContext.cs (if needed)
web/src/App.tsx (add routes, context provider)
web/src/styles/tokens.ts (if new tokens needed)
```

## Dependencies
- Backend: BCrypt.Net-Next (already installed)
- Frontend: None additional needed (use built-in hooks)

## Related Documentation
- See [DOMAIN_MODEL.md](../architecture/DOMAIN_MODEL.md) for User entity
- See [API_DESIGN.md](../architecture/API_DESIGN.md) for auth endpoints
- See [DEV_SETUP.md](../DEV_SETUP.md) for environment setup

## Notes
- This phase is critical as it blocks all admin features
- Ensure HTTPS in production for secure authentication
- Session-based auth is sufficient for MVP; JWT can be added in Phase 2B
- Consider implementing "Remember Me" in future enhancement
