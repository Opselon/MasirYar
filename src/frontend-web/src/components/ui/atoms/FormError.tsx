// src/frontend-web/src/components/ui/atoms/FormError.tsx
import React from 'react';
import { twMerge } from 'tailwind-merge';
import { clsx } from 'clsx';

export interface FormErrorProps extends React.HTMLAttributes<HTMLParagraphElement> {}

const FormError = React.forwardRef<HTMLParagraphElement, FormErrorProps>(
  ({ className, children, ...props }, ref) => {
    if (!children) {
      return null;
    }

    return (
      <p
        className={twMerge(
          clsx('text-sm font-medium text-red-500', className)
        )}
        ref={ref}
        {...props}
      >
        {children}
      </p>
    );
  }
);
FormError.displayName = 'FormError';

export { FormError };
