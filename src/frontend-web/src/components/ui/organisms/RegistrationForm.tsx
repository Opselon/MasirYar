// src/frontend-web/src/components/ui/organisms/RegistrationForm.tsx
'use client';
import React from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useMutation } from '@tanstack/react-query';
import { Button } from '../atoms/Button';
import { Input } from '../atoms/Input';
import { Label } from '../atoms/Label';
import { FormError } from '../atoms/FormError';
import { registrationSchema, RegistrationFormValues } from '@/validators/auth';
import { registerUser } from '@/services/api';

export const RegistrationForm = () => {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<RegistrationFormValues>({
    resolver: zodResolver(registrationSchema),
  });

  const [formError, setFormError] = React.useState<string | null>(null);
  const [formSuccess, setFormSuccess] = React.useState<string | null>(null);

  const mutation = useMutation({
    mutationFn: (data: RegistrationFormValues) => registerUser(data),
    onSuccess: (data) => {
      setFormSuccess(data.message);
      setFormError(null);
      // router.push('/login'); // Redirect on success
    },
    onError: (error: Error) => {
      setFormError(error.message);
      setFormSuccess(null);
    },
  });

  const onSubmit = (data: RegistrationFormValues) => {
    mutation.mutate(data);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      {formError && <FormError>{formError}</FormError>}
      {formSuccess && <p className="text-sm font-medium text-green-500">{formSuccess}</p>}
      <div>
        <Label htmlFor="username">نام کاربری</Label>
        <Input id="username" type="text" {...register('username')} />
        <FormError>{errors.username?.message}</FormError>
      </div>
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
      <div>
        <Label htmlFor="confirmPassword">تکرار رمز عبور</Label>
        <Input id="confirmPassword" type="password" {...register('confirmPassword')} />
        <FormError>{errors.confirmPassword?.message}</FormError>
      </div>
      <Button type="submit" className="w-full" disabled={isSubmitting}>
        {isSubmitting ? 'در حال ثبت نام...' : 'ثبت‌نام'}
      </Button>
    </form>
  );
};
