// src/frontend-web/src/app/login/page.tsx
import { LoginForm } from '@/components/ui/organisms/LoginForm';
import React from 'react';

const LoginPage = () => {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center p-8 bg-gray-50">
      <div className="w-full max-w-md p-8 space-y-6 bg-white rounded-lg shadow-md">
        <h1 className="text-2xl font-bold text-center text-gray-800">
          ورود به حساب کاربری
        </h1>
        <LoginForm />
      </div>
    </main>
  );
};

export default LoginPage;
