// src/frontend-web/playwright/pages/RegistrationPage.ts
import { Page, Locator } from '@playwright/test';

export class RegistrationPage {
  readonly page: Page;
  readonly usernameInput: Locator;
  readonly emailInput: Locator;
  readonly passwordInput: Locator;
  readonly confirmPasswordInput: Locator;
  readonly registerButton: Locator;

  constructor(page: Page) {
    this.page = page;
    this.usernameInput = page.getByLabel('نام کاربری');
    this.emailInput = page.getByLabel('ایمیل');
    this.passwordInput = page.getByLabel('رمز عبور');
    this.confirmPasswordInput = page.getByLabel('تکرار رمز عبور');
    this.registerButton = page.getByRole('button', { name: 'ثبت‌نام' });
  }

  async navigate() {
    await this.page.goto('/register');
  }

  async register(username: string, email: string, password: string) {
    await this.usernameInput.fill(username);
    await this.emailInput.fill(email);
    await this.passwordInput.fill(password);
    await this.confirmPasswordInput.fill(password);
    await this.registerButton.click();
  }
}
