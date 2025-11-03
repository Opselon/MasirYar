// src/frontend-web/src/components/ui/atoms/Input.stories.tsx
import type { Meta, StoryObj } from '@storybook/react';
import { Input } from './Input';

const meta = {
  title: 'UI/Atoms/Input',
  component: Input,
  parameters: {
    layout: 'centered',
  },
  argTypes: {
    variant: {
      control: { type: 'radio' },
      options: ['default', 'error'],
    },
    placeholder: {
      control: 'text',
    },
    disabled: {
      control: 'boolean',
    },
  },
} satisfies Meta<typeof Input>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    variant: 'default',
    placeholder: 'ایمیل خود را وارد کنید',
  },
};

export const Error: Story = {
  args: {
    variant: 'error',
    placeholder: 'ایمیل نامعتبر',
  },
};

export const Disabled: Story = {
  args: {
    variant: 'default',
    placeholder: 'غیرفعال',
    disabled: true,
  },
};
