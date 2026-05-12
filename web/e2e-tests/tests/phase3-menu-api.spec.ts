import { test, expect, APIRequestContext } from '@playwright/test';

let sharedSessionCookie: string = '';
let sharedApiContext: APIRequestContext;
let dateOffset = 1; // Start with tomorrow, increment for each test to avoid duplicates

const API_BASE_URL = 'https://localhost:5001';
const ADMIN_EMAIL = 'admin@medicare.local';
const ADMIN_PASSWORD = 'Admin123!';

function getNextDate() {
  const date = new Date();
  date.setDate(date.getDate() + dateOffset);
  dateOffset++;
  return date;
}

test.describe('Phase 3 - Menu API', () => {
  test.beforeAll(async ({ playwright }) => {
    // Create API context with SSL bypass for local testing
    sharedApiContext = await playwright.request.newContext({
      baseURL: API_BASE_URL,
      ignoreHTTPSErrors: true,
    });

    // Login once and reuse session
    const loginResponse = await sharedApiContext.post('/api/auth/login', {
      data: {
        email: ADMIN_EMAIL,
        password: ADMIN_PASSWORD,
      },
    });

    if (loginResponse.ok()) {
      const setCookie = loginResponse.headers()['set-cookie'];
      if (setCookie) {
        sharedSessionCookie = setCookie.split(';')[0];
      }
    }
  });

  test.afterAll(async () => {
    await sharedApiContext.dispose();
  });

  test('should create menu successfully', async () => {
    const tomorrow = getNextDate();

    const createResponse = await sharedApiContext.post('/api/menus', {
      headers: {
        'Cookie': sharedSessionCookie,
      },
      data: {
        dateApplied: tomorrow.toISOString(),
        mealType: 1, // Lunch
        description: 'Thực đơn trưa - Test Phase 3',
        items: [
          {
            name: 'Cơm gà',
            description: 'Cơm gà nướng',
            price: 55000,
            displayOrder: 1,
          },
          {
            name: 'Canh rau',
            description: 'Canh rau cải',
            price: 18000,
            displayOrder: 2,
          },
        ],
      },
    });

    expect(createResponse.ok()).toBeTruthy();
    expect(createResponse.status()).toBe(201);

    const menuData = await createResponse.json();
    expect(menuData.id).toBeDefined();
    expect(menuData.description).toBe('Thực đơn trưa - Test Phase 3');
    expect(menuData.items.length).toBe(2);
    expect(menuData.mealType).toBe('Lunch');
  });

  test('should get menu list with pagination', async () => {
    const getResponse = await sharedApiContext.get('/api/menus', {
      params: {
        page: 1,
        pageSize: 10,
      },
      headers: {
        'Cookie': sharedSessionCookie,
      },
    });

    expect(getResponse.ok()).toBeTruthy();
    expect(getResponse.status()).toBe(200);

    const pageData = await getResponse.json();
    expect(pageData.items).toBeDefined();
    expect(Array.isArray(pageData.items)).toBe(true);
    expect(pageData.totalCount).toBeGreaterThanOrEqual(0);
    expect(pageData.page).toBe(1);
    expect(pageData.pageSize).toBe(10);
  });

  test('should filter menus by date range and meal type', async () => {
    const dateFrom = new Date();
    dateFrom.setDate(dateFrom.getDate() + 1);
    const dateTo = new Date();
    dateTo.setDate(dateTo.getDate() + 30);

    const filterResponse = await sharedApiContext.get('/api/menus', {
      params: {
        page: 1,
        pageSize: 10,
        dateFrom: dateFrom.toISOString(),
        dateTo: dateTo.toISOString(),
        mealType: 1, // Lunch
      },
      headers: {
        'Cookie': sharedSessionCookie,
      },
    });

    expect(filterResponse.ok()).toBeTruthy();
    expect(filterResponse.status()).toBe(200);

    const pageData = await filterResponse.json();
    expect(Array.isArray(pageData.items)).toBe(true);

    // All items should be lunch menus
    pageData.items.forEach((menu: any) => {
      expect(menu.mealType).toBe('Lunch');
    });
  });

  test('should not create duplicate menu for same date and meal type', async () => {
    const testDate = getNextDate();

    // Create first menu
    const firstResponse = await sharedApiContext.post('/api/menus', {
      headers: {
        'Cookie': sharedSessionCookie,
      },
      data: {
        dateApplied: testDate.toISOString(),
        mealType: 2, // Dinner
        description: 'Thực đơn tối - Lần 1',
        items: [
          {
            name: 'Cơm bò',
            description: 'Cơm bò hầm',
            price: 68000,
            displayOrder: 1,
          },
        ],
      },
    });

    expect(firstResponse.ok()).toBeTruthy();

    // Try to create duplicate menu with same date but different description
    const duplicateResponse = await sharedApiContext.post('/api/menus', {
      headers: {
        'Cookie': sharedSessionCookie,
      },
      data: {
        dateApplied: testDate.toISOString(),
        mealType: 2, // Same dinner
        description: 'Thực đơn tối - Lần 2 (Duplicate)',
        items: [
          {
            name: 'Cơm gà',
            description: 'Cơm gà nướng',
            price: 55000,
            displayOrder: 1,
          },
        ],
      },
    });

    expect(duplicateResponse.status()).toBe(400);
    const errorData = await duplicateResponse.json();
    expect(errorData.message).toContain('already exists');
  });

  test('should return 401 without authentication', async () => {
    const unauthorizedResponse = await sharedApiContext.get('/api/menus', {
      headers: {
        'Cookie': '', // No session cookie
      },
    });

    expect(unauthorizedResponse.status()).toBe(401);
  });

  test('should get menu by id', async () => {
    // First get the list to find a menu
    const listResponse = await sharedApiContext.get('/api/menus', {
      params: {
        page: 1,
        pageSize: 1,
      },
      headers: {
        'Cookie': sharedSessionCookie,
      },
    });

    const pageData = await listResponse.json();
    if (pageData.items.length === 0) {
      test.skip();
    }

    const menuId = pageData.items[0].id;

    const detailResponse = await sharedApiContext.get(`/api/menus/${menuId}`, {
      headers: {
        'Cookie': sharedSessionCookie,
      },
    });

    expect(detailResponse.ok()).toBeTruthy();
    expect(detailResponse.status()).toBe(200);

    const menu = await detailResponse.json();
    expect(menu.id).toBe(menuId);
    expect(menu.items).toBeDefined();
    expect(Array.isArray(menu.items)).toBe(true);
  });

  test('should update menu successfully', async () => {
    // Create a menu first
    const testDate = getNextDate();

    const createResponse = await sharedApiContext.post('/api/menus', {
      headers: {
        'Cookie': sharedSessionCookie,
      },
      data: {
        dateApplied: testDate.toISOString(),
        mealType: 1,
        description: 'Original description',
        items: [
          {
            name: 'Cơm gà',
            description: 'Cơm gà nướng',
            price: 55000,
            displayOrder: 1,
          },
        ],
      },
    });

    const menu = await createResponse.json();
    const menuId = menu.id;

    // Update the menu
    const updateResponse = await sharedApiContext.put(`/api/menus/${menuId}`, {
      headers: {
        'Cookie': sharedSessionCookie,
      },
      data: {
        dateApplied: testDate.toISOString(),
        mealType: 1,
        description: 'Updated description',
        items: [
          {
            name: 'Cơm gà',
            description: 'Cơm gà nướng',
            price: 55000,
            displayOrder: 1,
          },
          {
            name: 'Salad',
            description: 'Salad tươi',
            price: 22000,
            displayOrder: 2,
          },
        ],
      },
    });

    expect(updateResponse.ok()).toBeTruthy();
    expect(updateResponse.status()).toBe(200);

    const updatedMenu = await updateResponse.json();
    expect(updatedMenu.description).toBe('Updated description');
    expect(updatedMenu.items.length).toBe(2);
  });

  test('should delete menu successfully', async () => {
    // Create a menu first
    const testDate = getNextDate();

    const createResponse = await sharedApiContext.post('/api/menus', {
      headers: {
        'Cookie': sharedSessionCookie,
      },
      data: {
        dateApplied: testDate.toISOString(),
        mealType: 1,
        description: 'Menu to be deleted',
        items: [
          {
            name: 'Cơm test',
            description: 'Test item',
            price: 50000,
            displayOrder: 1,
          },
        ],
      },
    });

    const menu = await createResponse.json();
    const menuId = menu.id;

    // Delete the menu
    const deleteResponse = await sharedApiContext.delete(`/api/menus/${menuId}`, {
      headers: {
        'Cookie': sharedSessionCookie,
      },
    });

    expect(deleteResponse.ok()).toBeTruthy();
    expect(deleteResponse.status()).toBe(204);

    // Verify menu is deleted
    const getResponse = await sharedApiContext.get(`/api/menus/${menuId}`, {
      headers: {
        'Cookie': sharedSessionCookie,
      },
    });

    expect(getResponse.status()).toBe(404);
  });

  test('should validate menu items have required fields', async () => {
    const testDate = getNextDate();

    // Try to create menu without items
    const noItemsResponse = await sharedApiContext.post('/api/menus', {
      headers: {
        'Cookie': sharedSessionCookie,
      },
      data: {
        dateApplied: testDate.toISOString(),
        mealType: 1,
        description: 'Menu without items',
        items: [],
      },
    });

    expect(noItemsResponse.status()).toBe(400);
    const errorData = await noItemsResponse.json();
    expect(errorData.message).toContain('item');
  });

  test('should validate item prices are non-negative', async () => {
    const testDate = getNextDate();

    // Try to create menu with negative price
    const negativePriceResponse = await sharedApiContext.post('/api/menus', {
      headers: {
        'Cookie': sharedSessionCookie,
      },
      data: {
        dateApplied: testDate.toISOString(),
        mealType: 1,
        description: 'Menu with negative price',
        items: [
          {
            name: 'Invalid item',
            description: 'Invalid item',
            price: -100, // Negative price
            displayOrder: 1,
          },
        ],
      },
    });

    expect(negativePriceResponse.status()).toBe(400);
    const errorData = await negativePriceResponse.json();
    expect(errorData.message).toContain('price');
  });

  test('Health check endpoint should be accessible', async () => {
    const healthResponse = await sharedApiContext.get('/api/health');
    expect(healthResponse.ok()).toBeTruthy();
    expect(healthResponse.status()).toBe(200);

    const healthData = await healthResponse.json();
    expect(healthData.status).toBe('healthy');
  });
});
