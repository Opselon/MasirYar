// src/frontend-web/src/services/api.ts
import { RegistrationFormValues, LoginFormValues } from '@/validators/auth';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:8000';

export const registerUser = async (data: RegistrationFormValues) => {
  // In a real app, you would make a POST request to your API
  console.log('Registering user:', data);
  // Simulate API call
  await new Promise(resolve => setTimeout(resolve, 1000));

  // Mock a successful registration
  return { success: true, message: 'Registration successful!' };

  /*
  // Example of a real API call:
  const response = await fetch(`${API_URL}/identity-api/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || 'Registration failed');
  }

  return response.json();
  */
};

export const loginUser = async (data: LoginFormValues) => {
  // In a real app, you would make a POST request to your API
  console.log('Logging in user:', data);
  // Simulate API call
  await new Promise(resolve => setTimeout(resolve, 1000));

  // Mock a successful login with a dummy token
  return { success: true, token: 'mock-jwt-token' };

  /*
  // Example of a real API call:
  const response = await fetch(`${API_URL}/identity-api/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || 'Login failed');
  }

  return response.json();
  */
};
