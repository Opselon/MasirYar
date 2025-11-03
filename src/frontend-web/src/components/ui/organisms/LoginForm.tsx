// src/frontend-web/src/components/ui/organisms/LoginForm.tsx
'use client';
import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { Button } from '../atoms/Button';
import { Input } from '../atoms/Input';
import { Label } from '../atoms/Label';
import { FormError } from '../atoms/FormError';
import { loginSchema, LoginFormValues } from '@/validators/auth';
import { loginUser } from '@/services/api';
import { useAuthStore } from '@/stores/authStore';

export const LoginForm = () => {
  const setToken = useAuthStore((state) => state.setToken);
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
  });

  const [formError, setFormError] = React.useState<string | null>(null);

  const mutation = useMutation({
    mutationFn: (data: LoginFormValues) => loginUser(data),
    onSuccess: (data) => {
      setToken(data.token);
      setFormError(null);
      // router.push('/dashboard');
    },
    onError: (error: Error) => {
      setFormError(error.message);
    },
  });

  const onSubmit = (data: LoginFormValues) => {
    mutation.mutate(data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      {formError && <FormError>{formError}</FormError>}
      <div>
        <Label htmlFor="email">ایمیل</Label>
        <Input id="email" type="email" {...register('email')} />
        <FormError>{errors.email?.message}</FormError>
      </div>
      <div>
        <Label htmlFor="password">رمز عبور</Label>
        <Input id="password" type="password" {...register('password')} />
        <FormError>{errors.password?.message}</FormError>
      </div>
      <Button type="submit" className="w-full" disabled={isSubmitting}>
        {isSubmitting ? 'در حال ورود...' : 'ورود'}
      </Button>
    </form>
  );
};
