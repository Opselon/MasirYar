// src/frontend-web/src/components/ui/atoms/FormError.stories.tsx
import type { Meta, StoryObj } from '@storybook/react';
import { FormError } from './FormError';

const meta = {
  title: 'UI/Atoms/FormError',
  component: FormError,
  parameters: {
    layout: 'centered',
  },
} satisfies Meta<typeof FormError>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    children: 'این فیلد الزامی است.',
  },
};

export const NoError: Story = {
  args: {
    children: null,
  },
};
