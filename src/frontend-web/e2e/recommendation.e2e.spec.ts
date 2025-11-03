import { test, expect } from '@playwright/test';

test.describe('Recommendation Flow', () => {
  test('AfterCreatingAJournal_ShouldDisplayRelatedCourses', async ({ page }) => {
    // Arrange: Login the user and create a journal entry
    const uniqueUser = `testuser_${Date.now()}`;
    const email = `${uniqueUser}@masiryar.test`;
    const password = 'StrongPassword123';
    await page.goto('/register');
    await page.getByLabel('Username').fill(uniqueUser);
    await page.getByLabel('Email').fill(email);
    await page.getByLabel('Password').fill(password);
    await page.getByRole('button', { name: /register/i }).click();
    await page.goto('/login');
    await page.getByLabel('Email or Username').fill(email);
    await page.getByLabel('Password').fill(password);
    await page.getByRole('button', { name: /log in/i }).click();
    await expect(page).toHaveURL(/.*dashboard/);
    await page.getByRole('link', { name: /journal/i }).click();
    await page.getByLabel('Title').fill('My First Journal');
    await page.getByLabel('Content').fill('This is my first journal entry about software engineering.');
    await page.getByRole('button', { name: /save/i }).click();

    // Act: Navigate to the recommendations page
    await page.getByRole('link', { name: /recommendations/i }).click();

    // Assert: Check for related course recommendations
    await expect(page.getByText(/Software Engineering 101/i)).toBeVisible();
  });
});
