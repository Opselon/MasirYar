# سند طراحی فنی: پلتفرم UI نسل آینده (Next-Gen UI Platform)

## ۱. اصول راهنما (Guiding Principles)

- **عملکرد در اولویت (Performance-First):** زمان بارگذاری اولیه (Initial Load Time) و تعاملات باید بسیار سریع باشد. استفاده حداکثری از Server-Side Rendering (SSR) و Static Site Generation (SSG).
- **تجربه توسعه درجه یک (DX-First):** توسعه‌دهندگان باید بتوانند به سرعت و با اطمینان، کامپوننت‌ها و ویژگی‌های جدید را توسعه دهند.
- **سیستم طراحی منسجم (Cohesive Design System):** تمام اجزای بصری باید از یک منبع حقیقت واحد (Single Source of Truth) پیروی کنند.
- **تایپینگ سرتاسری (End-to-End Typing):** مدل‌های داده باید بین Backend و Frontend به اشتراک گذاشته شوند تا از خطاهای زمان اجرا جلوگیری شود.

## ۲. معماری State Management

- **ابزار اصلی:** **Zustand**
- **توجیه:** Zustand به دلیل سادگی API، Boilerplate بسیار کم، و عملکرد بهینه (جلوگیری از re-renderهای غیرضروری) به Redux Toolkit ترجیح داده می‌شود.
- **ساختار Store:** Storeها به صورت ماژولار و بر اساس دامنه (domain) طراحی می‌شوند.
  ```typescript
  // example: stores/auth.ts
  import { create } from 'zustand';
  import { immer } from 'zustand/middleware/immer';

  interface AuthState {
    user: User | null;
    token: string | null;
    actions: {
      login: (user: User, token: string) => void;
      logout: () => void;
    };
  }

  export const useAuthStore = create<AuthState>()(
    immer((set) => ({
      user: null,
      token: null,
      actions: {
        login: (user, token) => set((state) => {
          state.user = user;
          state.token = token;
        }),
        logout: () => set({ user: null, token: null }),
      },
    }))
  );
  ```

## ۳. استراتژی واکشی داده (Data Fetching Strategy)

- **ابزار اصلی:** **React Query (TanStack Query)**
- **توجیه:** React Query یک راهکار جامع برای واکشی، کشینگ، همگام‌سازی و به‌روزرسانی داده‌های سرور است.
- **یکپارچه‌سازی با Next.js 14:**
  - داده‌های اولیه در **Server Components** واکشی شده و به **Client Components** به عنوان `initialData` پاس داده می‌شوند. این کار از درخواست‌های تکراری در سمت کلاینت جلوگیری کرده و سرعت بارگذاری اولیه را بهبود می‌بخشد.
- **Optimistic Updates:** برای عملیاتی مانند ایجاد یا ویرایش ژورنال، از Optimistic Updates استفاده می‌کنیم تا UI بلافاصله و بدون منتظر ماندن برای پاسخ سرور، به‌روز شود.

## ۴. سیستم طراحی (Design System)

- **ابزار مستندسازی:** **Storybook** برای توسعه ایزوله و مستندسازی کامپوننت‌ها.
- **ساختار کامپوننت (Atomic Design):**
  - `atoms`: کامپوننت‌های پایه (Button, Input, Text).
  - `molecules`: ترکیب اتم‌ها (Search Bar, Form Field).
  - `organisms`: بخش‌های پیچیده UI (Header, Journal Entry Card).
- **استایلینگ:** **Tailwind CSS** به همراه `clsx` (برای کلاس‌های شرطی) و `tailwind-merge` (برای جلوگیری از تداخل کلاس‌ها).

## ۵. تایپینگ سرتاسری و اشتراک‌گذاری کد

- **تولید خودکار تایپ‌ها:** از **OpenAPI (Swagger)** که توسط `IdentityService` تولید می‌شود، برای تولید خودکار تایپ‌های TypeScript و کلاینت‌های API با ابزاری مانند `openapi-typescript-codegen` استفاده می‌کنیم.
- **پکیج اشتراکی:** یک پکیج `npm` داخلی به نام `@masiryar/shared-types` ایجاد می‌شود که شامل تایپ‌ها و اینترفیس‌های مشترک بین Frontend و Backend است.

## ۶. تستینگ Frontend

- **Unit/Integration Tests:** **Vitest** + **React Testing Library** برای تست کامپوننت‌ها و هوک‌ها.
- **End-to-End (E2E) Tests:** **Playwright** برای نوشتن تست‌های E2E که جریان‌های کاربری کامل (مانند ثبت‌نام، ورود، و ایجاد ژورنال) را شبیه‌سازی می‌کنند.
