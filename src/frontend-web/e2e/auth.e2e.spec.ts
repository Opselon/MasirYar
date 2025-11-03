import { test, expect } from '@playwright/test';

test.describe('Authentication Flow', () => {
  test('A new user should be able to register, log in, and see the dashboard', async ({ page }) => {
    const uniqueUser = `testuser_${Date.now()}`;
    const email = `${uniqueUser}@masiryar.test`;
    const password = 'StrongPassword123';

    // 1. Registration
    await page.goto('/register');
    await page.getByLabel('Username').fill(uniqueUser);
    await page.getByLabel('Email').fill(email);
    await page.getByLabel('Password').fill(password);
    await page.getByRole('button', { name: /register/i }).click();

    // Assert redirection to login page after successful registration
    await expect(page).toHaveURL(/.*login/);
    await expect(page.getByText(/registration successful/i)).toBeVisible();

    // 2. Login
    await page.getByLabel('Email or Username').fill(email);
    await page.getByLabel('Password').fill(password);
    await page.getByRole('button', { name: /log in/i }).click();

    // 3. Dashboard Verification
    // Assert successful login and navigation to the dashboard
    await expect(page).toHaveURL(/.*dashboard/);
    await expect(page.getByRole('heading', { name: `Welcome, ${uniqueUser}` })).toBeVisible();
  });

  test('Validation_ShouldShowErrors_OnInvalidRegistration', async ({ page }) => {
    // Arrange
    await page.goto('/register');

    // Act
    await page.getByRole('button', { name: /register/i }).click();

    // Assert
    await expect(page.getByText(/Username is required/i)).toBeVisible();
    await expect(page.getByText(/Email is required/i)).toBeVisible();
    await expect(page.getByText(/Password is required/i)).toBeVisible();
  });
});
