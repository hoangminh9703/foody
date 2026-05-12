# API Design (Phase 1-5 Roadmap)

## Overview
This document outlines the REST API design for the Medicare system, following REST principles and RESTful conventions.

## Base URL
- Development: `https://localhost:5001/api`
- Production: `https://api.medicare.local/api`

## Response Format

All responses follow a consistent JSON structure:

### Success Response (2xx)
```json
{
  "success": true,
  "data": { /* entity or list */ },
  "message": "Operation completed successfully"
}
```

### Error Response (4xx, 5xx)
```json
{
  "success": false,
  "error": {
    "code": "ERROR_CODE",
    "message": "Human-readable error message",
    "details": [ /* field-specific errors */ ]
  },
  "timestamp": "2026-05-11T10:30:00Z"
}
```

## Public Endpoints (No Authentication Required)

### 1. Health Check
```
GET /health
```
**Response (200):**
```json
{
  "status": "healthy",
  "timestamp": "2026-05-11T10:30:00Z"
}
```

### 2. Get Daily Menu (Phase 3)
```
GET /menus?date=2026-05-11&mealType=lunch
```

**Query Parameters:**
- `date` (string, required): YYYY-MM-DD format
- `mealType` (string, required): "lunch" or "dinner"

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "dateApplied": "2026-05-11",
    "mealType": "lunch",
    "description": "Today's special menu",
    "items": [
      {
        "id": 1,
        "name": "Cơm gà",
        "description": "Chicken rice with egg",
        "price": 5.50,
        "displayOrder": 1
      },
      {
        "id": 2,
        "name": "Cơm tấm",
        "description": "Broken rice with pork",
        "price": 4.50,
        "displayOrder": 2
      }
    ]
  }
}
```

### 3. Get Site Content (Phase 4)
```
GET /site-content/{key}
```

**Path Parameters:**
- `key` (string): Content key (e.g., "CompanyName", "CompanyPhone")

**Response (200):**
```json
{
  "success": true,
  "data": {
    "key": "CompanyPhone",
    "value": "+84 123 456 7890",
    "contentType": "text"
  }
}
```

### 4. Submit Order Request (Phase 3)
```
POST /orders
Content-Type: application/json

{
  "customerName": "Nguyễn Văn A",
  "customerPhone": "+84901234567",
  "orderDate": "2026-05-12",
  "mealType": "lunch",
  "items": [
    {
      "menuItemId": 1,
      "quantity": 2,
      "specialRequest": "Ít cay"
    },
    {
      "menuItemId": 2,
      "quantity": 1,
      "specialRequest": null
    }
  ],
  "notes": "Giao lúc 12:00 trưa"
}
```

**Response (201):**
```json
{
  "success": true,
  "data": {
    "id": 101,
    "customerName": "Nguyễn Văn A",
    "customerPhone": "+84901234567",
    "orderDate": "2026-05-12",
    "mealType": "lunch",
    "status": "new",
    "totalQuantity": 3,
    "totalPrice": 15.50,
    "notes": "Giao lúc 12:00 trưa",
    "createdAt": "2026-05-11T10:30:00Z"
  },
  "message": "Order created successfully"
}
```

## Protected Endpoints (Authentication Required - Phase 2+)

### Authentication Flow
1. Admin logs in with email/password (POST /auth/login)
2. Server returns session cookie (httpOnly, Secure, SameSite=Strict)
3. Subsequent requests include cookie automatically
4. Logout clears session

### 1. Admin Login (Phase 2)
```
POST /auth/login
Content-Type: application/json

{
  "email": "admin@medicare.local",
  "password": "Admin123!"
}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "email": "admin@medicare.local",
    "fullName": "System Administrator",
    "role": "admin",
    "lastLoginAt": "2026-05-11T10:30:00Z"
  },
  "message": "Login successful"
}
```

### 2. Admin Logout (Phase 2)
```
POST /auth/logout
```

**Response (200):**
```json
{
  "success": true,
  "message": "Logged out successfully"
}
```

### 3. List All Orders (Phase 3)
```
GET /orders?status=new&fromDate=2026-05-11&toDate=2026-05-15&page=1&pageSize=20
```

**Query Parameters:**
- `status` (string, optional): Filter by status (new, contacted, confirmed, completed, cancelled)
- `fromDate` (string, optional): Start date YYYY-MM-DD
- `toDate` (string, optional): End date YYYY-MM-DD
- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 20, max: 100)

**Response (200):**
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": 101,
        "customerName": "Nguyễn Văn A",
        "customerPhone": "+84901234567",
        "orderDate": "2026-05-12",
        "mealType": "lunch",
        "status": "new",
        "totalPrice": 15.50,
        "createdAt": "2026-05-11T10:30:00Z"
      }
    ],
    "totalCount": 42,
    "page": 1,
    "pageSize": 20,
    "totalPages": 3
  }
}
```

### 4. Get Order Details (Phase 3)
```
GET /orders/{orderId}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": 101,
    "customerName": "Nguyễn Văn A",
    "customerPhone": "+84901234567",
    "orderDate": "2026-05-12",
    "mealType": "lunch",
    "status": "new",
    "notes": "Giao lúc 12:00 trưa",
    "totalQuantity": 3,
    "totalPrice": 15.50,
    "createdAt": "2026-05-11T10:30:00Z",
    "updatedAt": null,
    "items": [
      {
        "id": 201,
        "menuItemName": "Cơm gà",
        "quantity": 2,
        "unitPrice": 5.50,
        "subtotal": 11.00,
        "specialRequest": "Ít cay"
      },
      {
        "id": 202,
        "menuItemName": "Cơm tấm",
        "quantity": 1,
        "unitPrice": 4.50,
        "subtotal": 4.50,
        "specialRequest": null
      }
    ]
  }
}
```

### 5. Update Order Status (Phase 3)
```
PATCH /orders/{orderId}/status
Content-Type: application/json

{
  "status": "contacted"
}
```

**Valid Status Transitions:**
- new → contacted, cancelled
- contacted → confirmed, cancelled
- confirmed → completed, cancelled
- completed → (no transitions)
- cancelled → (no transitions)

**Response (200):**
```json
{
  "success": true,
  "data": {
    "id": 101,
    "status": "contacted",
    "updatedAt": "2026-05-11T11:00:00Z"
  },
  "message": "Order status updated to contacted"
}
```

### 6. Create Menu (Phase 3)
```
POST /menus
Content-Type: application/json

{
  "dateApplied": "2026-05-15",
  "mealType": "lunch",
  "description": "Special menu for the week",
  "items": [
    {
      "name": "Cơm gà nướng",
      "description": "Grilled chicken with rice",
      "price": 6.00,
      "displayOrder": 1
    }
  ]
}
```

**Response (201):**
```json
{
  "success": true,
  "data": {
    "id": 10,
    "dateApplied": "2026-05-15",
    "mealType": "lunch",
    "description": "Special menu for the week",
    "isActive": true,
    "createdAt": "2026-05-11T11:00:00Z",
    "items": [
      {
        "id": 50,
        "name": "Cơm gà nướng",
        "description": "Grilled chicken with rice",
        "price": 6.00,
        "displayOrder": 1
      }
    ]
  }
}
```

### 7. Update Menu (Phase 3)
```
PUT /menus/{menuId}
Content-Type: application/json

{
  "description": "Updated description",
  "items": [
    {
      "id": 50,
      "name": "Cơm gà nướng",
      "description": "Grilled chicken with rice",
      "price": 6.50,
      "displayOrder": 1
    },
    {
      "name": "Cơm lợn kho",
      "description": "Braised pork with rice",
      "price": 5.50,
      "displayOrder": 2
    }
  ]
}
```

### 8. Delete Menu (Phase 3)
```
DELETE /menus/{menuId}
```

**Response (204):** No content

### 9. Update Site Content (Phase 3+)
```
PUT /site-content/{key}
Content-Type: application/json

{
  "value": "+84 987 654 3210",
  "contentType": "text"
}
```

**Response (200):**
```json
{
  "success": true,
  "data": {
    "key": "CompanyPhone",
    "value": "+84 987 654 3210",
    "contentType": "text",
    "updatedAt": "2026-05-11T11:00:00Z"
  }
}
```

## Error Codes

| Code | HTTP | Description |
|------|------|-------------|
| `INVALID_INPUT` | 400 | Validation error on input fields |
| `NOT_FOUND` | 404 | Resource not found |
| `UNAUTHORIZED` | 401 | Not authenticated |
| `FORBIDDEN` | 403 | Authenticated but not authorized |
| `CONFLICT` | 409 | Resource conflict (e.g., duplicate menu) |
| `INTERNAL_ERROR` | 500 | Server error |

## Rate Limiting (Future Enhancement)

In production, implement:
- 100 requests/minute per IP (public endpoints)
- 1000 requests/minute per session (authenticated endpoints)
- Return `429 Too Many Requests` when exceeded

## Versioning (Future Enhancement)

If API changes are needed:
- Introduce `/api/v2` endpoints
- Deprecate old endpoints (support for 6 months)
- Version in Accept header: `Accept: application/json; version=1`

## Documentation
- API documentation available at `/swagger/ui` (Swagger UI)
- OpenAPI spec available at `/swagger/openapi.json`
