# Phase 5: Site Content Management API

**Status**: Not started  
**Estimated Duration**: 3 days  
**Priority**: MEDIUM (nice-to-have for MVP)  
**Dependencies**: Phase 1 (Setup), Phase 2 (Auth)

## Goal
Enable admins to manage configurable landing page content without code changes (company info, contact details, service area) through a simple key-value API.

## Scope
- Read site content by key
- Update site content (admin only)
- Support for text, HTML, and JSON content types
- No versioning or history (can be added in Phase 2+ enhancement)
- Caching for performance

## Detailed Tasks

### Backend (.NET Core)

#### Task 1: Create SiteContentService in Application Layer
**Location**: `backend/src/Application/Services/SiteContentService.cs`

Steps:
1. Create `ISiteContentService` interface:
   - `Task<SiteContentDto> GetContentAsync(string key)`
   - `Task<Dictionary<string, string>> GetAllContentAsync(List<string> keys)`
   - `Task<SiteContentDto> UpdateContentAsync(string key, string value, ContentType contentType)`
   - `Task<bool> ContentExistsAsync(string key)`

2. Create `SiteContentService` implementation:
   - Validate content size limits
   - Validate content type
   - Map to/from DTOs
   - Add caching support
   - Log updates
   - Validate keys (predefined set)

3. Create `ContentType` enum:
   - Text
   - Html
   - Json

4. Create DTOs:
   ```csharp
   SiteContentDto {
     Key: string
     Value: string
     ContentType: string
     Description: string
     UpdatedAt: DateTime
   }
   
   UpdateSiteContentRequest {
     Value: string
     ContentType: ContentType
   }
   ```

5. Create predefined content keys:
   - CompanyName
   - CompanyPhone
   - CompanyDescription
   - CompanyAddress
   - ServiceArea
   - HeroTitle
   - HeroSubtitle
   - FooterText
   - ContactEmail

**Technical Notes**:
- Validate value length: max 5000 chars (configurable)
- Support JSON content for complex data
- Keep content keys predefined for security
- Cache content for 1 hour (configurable)

#### Task 2: Create SiteContent Commands (CQRS Pattern)
**Location**: `backend/src/Application/Commands/UpdateSiteContentCommand.cs`

Steps:
1. Create `UpdateSiteContentCommand`:
   - Properties: Key, Value, ContentType
   - Validation: Key exists, value length, content type valid

2. Create `UpdateSiteContentCommandHandler`:
   - Validate command
   - Check key is valid
   - Call service
   - Invalidate cache
   - Return updated content DTO

3. Create validator:
   ```csharp
   class UpdateSiteContentValidator : AbstractValidator<UpdateSiteContentCommand> {
       public UpdateSiteContentValidator() {
           RuleFor(x => x.Key)
               .NotEmpty()
               .Must(BeValidKey).WithMessage("Invalid content key");
           
           RuleFor(x => x.Value)
               .NotEmpty()
               .MaximumLength(5000).WithMessage("Content too large");
       }
   }
   ```

4. Register handler in DI

**Technical Notes**:
- Only allow predefined keys (whitelist)
- Validate HTML content if needed (basic HTML sanitization)
- Validate JSON content is valid JSON

#### Task 3: Create SiteContent Queries (CQRS Pattern)
**Location**: `backend/src/Application/Queries/GetSiteContentQuery.cs`

Steps:
1. Create `GetSiteContentQuery`:
   - Property: Key
   - Handler returns single `SiteContentDto`

2. Create `GetAllSiteContentQuery`:
   - Property: Keys (list)
   - Handler returns Dictionary<Key, SiteContentDto>

3. Implement query handlers:
   - Check cache first
   - Load from database if not cached
   - Cache for 1 hour
   - Return DTOs

4. Create `IContentCache` interface:
   - `T Get<T>(string key)`
   - `void Set<T>(string key, T value, TimeSpan expiration)`
   - `void Remove(string key)`

**Technical Notes**:
- Use IMemoryCache for caching
- Cache keys with prefix: `sitecontent_${key}`
- Set expiration to 1 hour
- Invalidate cache on update

#### Task 4: Create SiteContent API Controller
**Location**: `backend/src/Api/Controllers/SiteContentController.cs`

Steps:
1. Create `SiteContentController : ControllerBase`:
   ```
   GET    /api/site-content/{key}      - Get content (public)
   GET    /api/site-content            - Get multiple keys (public, query params)
   PUT    /api/site-content/{key}      - Update content (admin only)
   ```

2. Implement endpoints:
   - GET /api/site-content/{key} (public):
     - Return 200 with SiteContentDto
     - Return 404 if key not found
   
   - GET /api/site-content?keys=key1,key2,key3 (public):
     - Accept comma-separated keys
     - Return 200 with Dictionary<key, value>
     - Return 404 if any key not found
   
   - PUT /api/site-content/{key} (admin):
     - Require [Authorize]
     - Accept UpdateSiteContentRequest
     - Return 200 with updated content
     - Return 400 for invalid key/content

3. Handle errors appropriately

**Technical Notes**:
- GET endpoints are public (no [Authorize])
- PUT endpoint requires [Authorize]
- Return consistent response format
- Support multiple keys in single request

#### Task 5: Create SiteContent Repository Pattern (Infrastructure Layer)
**Location**: `backend/src/Infrastructure/Repositories/SiteContentRepository.cs`

Steps:
1. Create `ISiteContentRepository` interface:
   - `Task<SiteContent> GetByKeyAsync(string key)`
   - `Task<List<SiteContent>> GetByKeysAsync(List<string> keys)`
   - `Task<SiteContent> UpdateAsync(string key, string value)`
   - `Task SaveChangesAsync()`

2. Implement `SiteContentRepository`:
   - Query by key (case-insensitive)
   - Handle key validation
   - Update content with timestamp

3. Register in DI

**Technical Notes**:
- Make key lookup case-insensitive
- Update UpdatedAt timestamp on each update
- Use async methods

#### Task 6: Add Database Migration (if needed)

Steps:
1. Verify SiteContent table exists with:
   - Key (unique, not null)
   - Value (text, nullable)
   - ContentType (enum)
   - Description (optional)
   - UpdatedAt (timestamp)

2. Seed initial content:
   ```sql
   INSERT INTO SiteContent VALUES
   ('CompanyName', 'Medicare Food Co', 'Text', 'Company name', GETDATE()),
   ('CompanyPhone', '+1-555-0123', 'Text', 'Contact phone', GETDATE()),
   ('CompanyDescription', 'Fresh daily meals...', 'Html', 'Landing page description', GETDATE()),
   ...
   ```

3. Run migration if using EF Core

**Technical Notes**:
- Make Key column unique
- Add index on Key for fast lookup
- Store timestamps in UTC

### Testing Tasks

#### Task 7: Test Endpoints

Steps:
1. Test GET endpoints:
   - GET `/api/site-content/CompanyName` → 200 OK
   - GET `/api/site-content/CompanyPhone` → 200 OK
   - GET `/api/site-content/invalid` → 404 Not Found
   - GET `/api/site-content?keys=CompanyName,CompanyPhone` → 200 OK with both

2. Test PUT endpoint:
   - PUT `/api/site-content/CompanyPhone` without auth → 401 Unauthorized
   - PUT `/api/site-content/CompanyPhone` with auth → 200 OK
   - PUT `/api/site-content/invalid` with auth → 400 Bad Request
   - PUT with content > 5000 chars → 400 Bad Request

3. Test caching:
   - GET content (loads from DB)
   - Wait 1 second
   - Update content (invalidates cache)
   - GET content (should return new value)
   - Wait 1 hour
   - Cache should expire
   - GET content (reloads from DB)

#### Task 8: Add Integration Tests (Optional)

Steps:
1. Create test class: `SiteContentControllerTests.cs`
2. Write tests:
   - GetContent_WithValidKey_ReturnsOk
   - GetContent_WithInvalidKey_ReturnNotFound
   - UpdateContent_WithAuth_ReturnsOk
   - UpdateContent_WithoutAuth_ReturnsUnauthorized
   - UpdateContent_InvalidatesCachedContent

### Documentation Tasks

#### Task 9: Create Content Keys Documentation
**Location**: `backend/docs/SITE_CONTENT_KEYS.md`

Steps:
1. Document all predefined keys:
   ```
   ## Predefined Content Keys
   
   ### CompanyName (Text)
   Display name of the company on landing page
   Example: "Medicare Food Co"
   
   ### CompanyPhone (Text)
   Contact phone number
   Format: +1-XXX-XXXX
   Example: "+1-555-0123"
   
   ### CompanyDescription (Html)
   Description shown on landing page
   Can contain HTML for formatting
   Max length: 1000 chars
   
   ... (other keys)
   ```

2. Include usage instructions for admins
3. Include API examples

#### Task 10: Update Swagger Documentation

Steps:
1. Add XML comments to SiteContentController
2. Document all endpoints
3. Include example requests/responses
4. List all valid content keys

## Acceptance Criteria
- [x] GET /api/site-content/{key} returns content value
- [x] PUT /api/site-content/{key} updates content successfully
- [x] Only authenticated admins can update content
- [x] Content size is limited (max 5000 chars)
- [x] Invalid content type is rejected
- [x] Multiple content keys can be queried
- [x] Content is cached for better performance
- [x] Updated content is immediately reflected in API

## Testing Procedures

### Manual Test Cases

1. **Get Company Name**
   - GET `/api/site-content/CompanyName`
   - Verify: 200 OK
   - Response:
     ```json
     {
       "key": "CompanyName",
       "value": "Medicare Food Co",
       "contentType": "Text",
       "updatedAt": "2026-05-11T10:00:00Z"
     }
     ```

2. **Get Multiple Content Items**
   - GET `/api/site-content?keys=CompanyName,CompanyPhone,CompanyDescription`
   - Verify: 200 OK with all three items

3. **Update Content (Unauthorized)**
   - PUT `/api/site-content/CompanyName`
   - Body: `{"value": "New Name", "contentType": "Text"}`
   - Without authentication
   - Verify: 401 Unauthorized

4. **Update Content (Authorized)**
   - Log in as admin
   - PUT `/api/site-content/CompanyPhone`
   - Body: `{"value": "+1-555-9999", "contentType": "Text"}`
   - Verify: 200 OK with updated content

5. **Invalid Key**
   - PUT `/api/site-content/InvalidKey`
   - Verify: 400 Bad Request with message "Invalid content key"

6. **Content Too Large**
   - PUT `/api/site-content/CompanyDescription`
   - Body: 6000 character string
   - Verify: 400 Bad Request with message "Content exceeds maximum length"

7. **Invalid Content Type**
   - PUT `/api/site-content/CompanyName`
   - Body: `{"value": "Test", "contentType": "InvalidType"}`
   - Verify: 400 Bad Request

8. **Cache Validation**
   - GET `/api/site-content/CompanyName` (first time - DB load)
   - Update the content via API
   - GET `/api/site-content/CompanyName` (should return new value immediately)
   - Verify cache is invalidated on update

## Files to Create/Modify

### New Files
```
backend/src/Application/Services/SiteContentService.cs
backend/src/Application/Services/ISiteContentService.cs
backend/src/Application/Commands/UpdateSiteContentCommand.cs
backend/src/Application/Commands/UpdateSiteContentCommandHandler.cs
backend/src/Application/Queries/GetSiteContentQuery.cs
backend/src/Application/Queries/GetSiteContentQueryHandler.cs
backend/src/Application/DTOs/SiteContentDto.cs
backend/src/Application/DTOs/UpdateSiteContentRequest.cs
backend/src/Application/Validators/UpdateSiteContentValidator.cs
backend/src/Infrastructure/Repositories/SiteContentRepository.cs
backend/src/Infrastructure/Repositories/ISiteContentRepository.cs
backend/src/Infrastructure/Caching/IContentCache.cs
backend/src/Infrastructure/Caching/MemoryContentCache.cs
backend/src/Api/Controllers/SiteContentController.cs
backend/docs/SITE_CONTENT_KEYS.md
backend/tests/Medicare.Api.IntegrationTests/SiteContentControllerTests.cs (optional)
```

### Modified Files
```
backend/src/Application/DependencyInjection.cs
backend/src/Infrastructure/DependencyInjection.cs
backend/src/Api/Program.cs
backend/src/Infrastructure/Data/MedicareDbContext.cs (verify SiteContent table)
```

## Dependencies
- MediatR (already used)
- FluentValidation (already used)
- Entity Framework Core (already installed)
- Microsoft.Extensions.Caching.Memory (built-in)

## Related Documentation
- [DOMAIN_MODEL.md](../architecture/DOMAIN_MODEL.md) - SiteContent entity
- [API_DESIGN.md](../architecture/API_DESIGN.md) - API specification

## Notes
- This is a simple CRUD API for content management
- GET endpoints are public for frontend access
- PUT endpoints are admin-only for security
- Caching improves frontend performance
- Consider adding edit history in Phase 2B
- Consider adding multi-language support in Phase 3B
