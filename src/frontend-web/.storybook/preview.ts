// src/frontend-web/.storybook/preview.ts
import type { Preview } from "@storybook/react";
import "../src/app/globals.css"; // Import Tailwind's global styles

const preview: Preview = {
  parameters: {
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/i,
      },
    },
  },
};

export default preview;
