import type { Metadata } from "next";
import "./globals.css";
import Providers from "../components/Providers"; // Use relative path

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
      <body>
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
