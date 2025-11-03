import { test, expect } from '@playwright/test';

test.describe('Journaling Flow', () => {
  test('UserCanCreate_View_AndDeleteAJournalEntry', async ({ page }) => {
    // Arrange: Login the user
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

    // Act: Create a journal entry
    await page.getByRole('link', { name: /journal/i }).click();
    await page.getByLabel('Title').fill('My First Journal');
    await page.getByLabel('Content').fill('This is my first journal entry.');
    await page.getByRole('button', { name: /save/i }).click();

    // Assert: View the journal entry
    await expect(page.getByText(/My First Journal/i)).toBeVisible();

    // Act: Delete the journal entry
    await page.getByRole('button', { name: /delete/i }).click();

    // Assert: The journal entry is gone
    await expect(page.getByText(/My First Journal/i)).not.toBeVisible();
  });
});
