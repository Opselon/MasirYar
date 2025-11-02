import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Personal Growth Platform",
  description: "A platform for personal growth and journaling",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  );
}

