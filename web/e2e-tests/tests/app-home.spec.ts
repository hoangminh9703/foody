import { test, expect } from '@playwright/test';

test.describe('Medicare home page', () => {
  test('loads the home page, clicks the CTA, and navigates between pages', async ({ page }) => {
    await page.goto('/');
    await expect(page.getByRole('heading', { name: /chào mừng đến với medicare/i })).toBeVisible();
    await expect(page.getByRole('button', { name: /đặt cơm ngay/i })).toBeVisible();

    await page.getByRole('button', { name: /đặt cơm ngay/i }).click();

    await page.goto('/menu');
    await page.waitForLoadState('domcontentloaded');
    await expect(page.getByRole('heading', { name: /medicare - đồ ăn theo ngày/i })).toBeVisible();

    await page.goBack();
    await page.waitForLoadState('domcontentloaded');
    await expect(page.getByRole('button', { name: /đặt cơm ngay/i })).toBeVisible();
  });
});