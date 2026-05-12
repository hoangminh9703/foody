import { test, expect, APIRequestContext } from '@playwright/test';

const API_BASE = 'https://localhost:5001';
const ADMIN_EMAIL = 'admin@medicare.local';
const ADMIN_PASSWORD = 'Admin123!';

let sharedApiContext: APIRequestContext;
let sharedSessionCookie = '';

test.describe('Phase 4 - Orders API', () => {
  test.beforeAll(async ({ playwright }) => {
    sharedApiContext = await playwright.request.newContext({
      baseURL: API_BASE,
      ignoreHTTPSErrors: true,
    });

    const loginRes = await sharedApiContext.post('/api/auth/login', {
      data: { email: ADMIN_EMAIL, password: ADMIN_PASSWORD },
    });
    if (loginRes.ok()) {
      const setCookie = loginRes.headers()['set-cookie'];
      if (setCookie) sharedSessionCookie = setCookie.split(';')[0];
    }
  });

  test.afterAll(async () => {
    await sharedApiContext.dispose();
  });

  test('create order - valid data', async () => {
    const today = new Date();
    const payload = {
      customerName: 'Nguyen Van A',
      customerPhone: '0912345678',
      orderDate: today.toISOString(),
      mealType: 1,
      items: [
        // menuId should exist for the chosen date; adjust if needed
        { menuId: 1, quantity: 1 },
      ],
      notes: 'Test order from Playwright',
    };

    const res = await sharedApiContext.post('/api/orders', {
      data: payload,
    });

    expect(res.ok()).toBeTruthy();
    expect(res.status()).toBe(201);
    const body = await res.json();
    expect(body.id).toBeDefined();
  });

  test('create order - invalid phone', async () => {
    const today = new Date();
    const payload = {
      customerName: 'Nguyen Van B',
      customerPhone: 'invalid-phone',
      orderDate: today.toISOString(),
      mealType: 1,
      items: [{ menuId: 1, quantity: 1 }],
    };

    const res = await sharedApiContext.post('/api/orders', { data: payload });
    // Expect validation error (400)
    expect(res.status()).toBe(400);
    const err = await res.json().catch(() => null);
    expect(err).not.toBeNull();
  });
});
