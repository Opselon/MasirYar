// src/frontend-web/src/tests/components/organisms/LoginForm.test.tsx
import React from 'react';
import { render, screen, waitFor } from '@testing-library/react';
import { userEvent } from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { LoginForm } from '@/components/ui/organisms/LoginForm';
import * as api from '@/services/api';

// Mock the api module
vi.mock('@/services/api');

const queryClient = new QueryClient();

const Wrapper = ({ children }: { children: React.ReactNode }) => (
  <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
);

describe('LoginForm', () => {
  it('should render all form fields', () => {
    render(<Wrapper><LoginForm /></Wrapper>);
    expect(screen.getByLabelText(/ایمیل/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/رمز عبور/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /ورود/i })).toBeInTheDocument();
  });

  it('should show validation errors for invalid input', async () => {
    render(<Wrapper><LoginForm /></Wrapper>);
    const submitButton = screen.getByRole('button', { name: /ورود/i });

    await userEvent.click(submitButton);

    expect(await screen.findByText(/لطفاً یک ایمیل معتبر وارد کنید/i)).toBeVisible();
    expect(await screen.findByText(/رمز عبور الزامی است/i)).toBeVisible();
  });

  it('should call the login API on successful submission', async () => {
    const mockLoginUser = vi.spyOn(api, 'loginUser').mockResolvedValue({
      success: true,
      token: 'mock-jwt-token',
    });

    render(<Wrapper><LoginForm /></Wrapper>);

    await userEvent.type(screen.getByLabelText(/ایمیل/i), 'test@example.com');
    await userEvent.type(screen.getByLabelText(/رمز عبور/i), 'password123');
    await userEvent.click(screen.getByRole('button', { name: /ورود/i }));

    await waitFor(() => {
      expect(mockLoginUser).toHaveBeenCalledWith(
        expect.objectContaining({
          email: 'test@example.com',
          password: 'password123',
        })
      );
    });
  });
});
