// src/frontend-web/src/app/register/page.tsx
import { RegistrationForm } from '@/components/ui/organisms/RegistrationForm';
import React from 'react';

const RegisterPage = () => {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center p-8 bg-gray-50">
      <div className="w-full max-w-md p-8 space-y-6 bg-white rounded-lg shadow-md">
        <h1 className="text-2xl font-bold text-center text-gray-800">
          ایجاد حساب کاربری جدید
        </h1>
        <RegistrationForm />
      </div>
    </main>
  );
};

export default RegisterPage;
