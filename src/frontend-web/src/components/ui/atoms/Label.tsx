// src/frontend-web/src/components/ui/atoms/Label.tsx
import React from 'react';
import { twMerge } from 'tailwind-merge';
import { clsx } from 'clsx';

export interface LabelProps extends React.LabelHTMLAttributes<HTMLLabelElement> {}

const Label = React.forwardRef<HTMLLabelElement, LabelProps>(
  ({ className, ...props }, ref) => {
    return (
      <label
        className={twMerge(
          clsx('text-sm font-medium text-gray-700 leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70', className)
        )}
        ref={ref}
        {...props}
      />
    );
  }
);
Label.displayName = 'Label';

export { Label };
