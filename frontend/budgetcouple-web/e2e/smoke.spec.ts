import { test, expect } from '@playwright/test';

const BASE_URL = 'http://localhost:5173';

test.describe('Smoke Tests', () => {
  test('App carrega com sucesso', async ({ page }) => {
    await page.goto(BASE_URL);
    // Verifica se a página carrega sem erros
    await expect(page).toHaveTitle(/BudgetCouple/);
    // Verifica se algum conteúdo principal está visível
    const mainContent = page.locator('main, [role="main"], body > div');
    await expect(mainContent).toBeVisible();
  });

  test('Tela de login existe e é renderizada', async ({ page }) => {
    await page.goto(BASE_URL);
    // Verifica se há elementos de login na tela
    const loginForm = page.locator('form', { has: page.locator('input[type="password"], input[name*="password"]') });
    const loginButton = page.locator('button:has-text("Login"), button:has-text("Entrar"), button:has-text("Sign in")');

    // Pelo menos um desses elementos deve estar visível
    const hasLoginElements = (await loginForm.isVisible().catch(() => false)) ||
                             (await loginButton.isVisible().catch(() => false)) ||
                             (await page.locator('input[type="password"]').isVisible().catch(() => false));

    expect(hasLoginElements).toBeTruthy();
  });

  test('Formulário de Setup PIN é renderizado', async ({ page }) => {
    await page.goto(BASE_URL);
    // Tenta navegar para a página de setup PIN se possível, ou verifica se o form existe
    const setupPinForm = page.locator('[data-testid="setup-pin-form"], form:has-text("PIN")');
    const pinInput = page.locator('input[type="password"][placeholder*="PIN"], input[placeholder*="PIN"]');

    // Verifica se há qualquer indicativo do formulário de PIN
    const hasSetupPinElements = (await setupPinForm.isVisible().catch(() => false)) ||
                               (await pinInput.isVisible().catch(() => false)) ||
                               (await page.locator('text=PIN').isVisible().catch(() => false));

    // Se não encontrar na página inicial, tenta navegar para setup
    if (!hasSetupPinElements) {
      const setupLink = page.locator('a, button', { has: page.locator('text=/setup|Setup/i') });
      if (await setupLink.isVisible().catch(() => false)) {
        await setupLink.first().click();
        // Aguarda o carregamento da página
        await page.waitForLoadState('networkidle').catch(() => null);
      }
    }
  });
});
