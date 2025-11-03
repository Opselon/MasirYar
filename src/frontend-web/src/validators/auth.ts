// src/frontend-web/src/validators/auth.ts
import { z } from 'zod';

export const registrationSchema = z.object({
  username: z.string().min(3, { message: 'نام کاربری باید حداقل ۳ کاراکتر باشد' }),
  email: z.string().email({ message: 'لطفاً یک ایمیل معتبر وارد کنید' }),
  password: z.string().min(8, { message: 'رمز عبور باید حداقل ۸ کاراکتر باشد' }),
  confirmPassword: z.string(),
}).refine(data => data.password === data.confirmPassword, {
  message: 'رمزهای عبور یکسان نیستند',
  path: ['confirmPassword'],
});

export const loginSchema = z.object({
  email: z.string().email({ message: 'لطفاً یک ایمیل معتبر وارد کنید' }),
  password: z.string().min(1, { message: 'رمز عبور الزامی است' }),
});

export type RegistrationFormValues = z.infer<typeof registrationSchema>;
export type LoginFormValues = z.infer<typeof loginSchema>;
