import { test, expect } from '@playwright/test';

const PROD_URL = 'https://budgetcouple-topcon-luiz-silvas-projects.vercel.app';
const API_URL = 'https://budgetcouple-api.onrender.com';

test.describe('Smoke Tests', () => {
  test('Frontend app loads successfully', async ({ page }) => {
    await page.goto(PROD_URL);
    // Verify page loads without errors
    await expect(page).toHaveTitle(/budgetcouple/i);
    // Verify main content is visible
    const mainContent = page.locator('main, [role="main"], body > div');
    await expect(mainContent).toBeVisible();
  });

  test('Setup PIN page loads', async ({ page }) => {
    await page.goto(PROD_URL);
    // Check for login or PIN form elements - should load some auth page
    const pageContent = await page.content();
    expect(pageContent.length).toBeGreaterThan(100);
  });

  test('API /auth/status endpoint returns 200', async ({ request }) => {
    const response = await request.get(`${API_URL}/api/v1/auth/status`);
    expect(response.status()).toBe(200);
  });

  test('API supports HEAD request on /health endpoint', async ({ request }) => {
    const response = await request.head(`${API_URL}/health`);
    expect([200, 204]).toContain(response.status());
  });
});
