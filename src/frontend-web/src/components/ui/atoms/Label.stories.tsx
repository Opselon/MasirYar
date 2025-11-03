// src/frontend-web/src/components/ui/atoms/Label.stories.tsx
import type { Meta, StoryObj } from '@storybook/react';
import { Label } from './Label';

const meta = {
  title: 'UI/Atoms/Label',
  component: Label,
  parameters: {
    layout: 'centered',
  },
} satisfies Meta<typeof Label>;

export default meta;
type Story = StoryObj<typeof meta>;

export const Default: Story = {
  args: {
    children: 'آدرس ایمیل',
  },
};
