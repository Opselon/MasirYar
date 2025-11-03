// src/frontend-web/src/tests/components/organisms/RegistrationForm.test.tsx
import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { userEvent } from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { RegistrationForm } from '@/components/ui/organisms/RegistrationForm';
import * as api from '@/services/api';

// Mock the api module
vi.mock('@/services/api');

const queryClient = new QueryClient();

const Wrapper = ({ children }: { children: React.ReactNode }) => (
  <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
);

describe('RegistrationForm', () => {
  it('should render all form fields', () => {
    render(<Wrapper><RegistrationForm /></Wrapper>);
    expect(screen.getByLabelText(/نام کاربری/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/ایمیل/i)).toBeInTheDocument();
    expect(screen.getByLabelText('رمز عبور')).toBeInTheDocument();
    expect(screen.getByLabelText(/تکرار رمز عبور/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /ثبت‌نام/i })).toBeInTheDocument();
  });

  it('should show validation errors for invalid input', async () => {
    render(<Wrapper><RegistrationForm /></Wrapper>);
    const submitButton = screen.getByRole('button', { name: /ثبت‌نام/i });

    await userEvent.click(submitButton);

    expect(await screen.findByText(/نام کاربری باید حداقل ۳ کاراکتر باشد/i)).toBeVisible();
    expect(await screen.findByText(/لطفاً یک ایمیل معتبر وارد کنید/i)).toBeVisible();
    expect(await screen.findByText(/رمز عبور باید حداقل ۸ کاراکتر باشد/i)).toBeVisible();
  });

  it('should show a password mismatch error', async () => {
    render(<Wrapper><RegistrationForm /></Wrapper>);

    await userEvent.type(screen.getByLabelText('رمز عبور'), 'password123');
    await userEvent.type(screen.getByLabelText(/تکرار رمز عبور/i), 'password456');
    await userEvent.click(screen.getByRole('button', { name: /ثبت‌نام/i }));

    expect(await screen.findByText(/رمزهای عبور یکسان نیستند/i)).toBeVisible();
  });

  it('should call the registration API on successful submission', async () => {
    const mockRegisterUser = vi.spyOn(api, 'registerUser').mockResolvedValue({
      success: true,
      message: 'Registration successful!',
    });

    render(<Wrapper><RegistrationForm /></Wrapper>);

    await userEvent.type(screen.getByLabelText(/نام کاربری/i), 'testuser');
    await userEvent.type(screen.getByLabelText(/ایمیل/i), 'test@example.com');
    await userEvent.type(screen.getByLabelText('رمز عبور'), 'password123');
    await userEvent.type(screen.getByLabelText(/تکرار رمز عبور/i), 'password123');
    await userEvent.click(screen.getByRole('button', { name: /ثبت‌نام/i }));

    await waitFor(() => {
      expect(mockRegisterUser).toHaveBeenCalledWith(
        expect.objectContaining({
          username: 'testuser',
          email: 'test@example.com',
          password: 'password123',
          confirmPassword: 'password123',
        })
      );
    });

    expect(await screen.findByText(/Registration successful!/i)).toBeVisible();
  });
});
