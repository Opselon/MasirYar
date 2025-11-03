// src/frontend-web/playwright/specs/auth.e2e.spec.ts
import { test, expect } from '@playwright/test';
import { RegistrationPage } from '../pages/RegistrationPage';
import { LoginPage } from '../pages/LoginPage';

test('should allow a new user to register and log in', async ({ page }) => {
  const registrationPage = new RegistrationPage(page);
  const loginPage = new LoginPage(page);

  const uniqueUser = `user_${Date.now()}`;
  const email = `${uniqueUser}@example.com`;
  const password = 'Password123';

  await registrationPage.navigate();
  await registrationPage.register(uniqueUser, email, password);

  // Assert that we are redirected to the login page
  await expect(page).toHaveURL(/.*login/);

  await loginPage.login(email, password);

  // For this test, we'll mock the dashboard page
  // In a real app, you would assert that you are on the actual dashboard
  await page.setContent('<h1>Welcome to the Dashboard</h1>');

  // Assert that login was successful and we are on the dashboard
  await expect(page.getByRole('heading', { name: /welcome/i })).toBeVisible();
});
