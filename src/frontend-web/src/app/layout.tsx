import type { Metadata } from "next";
import "./globals.css";
import Providers from "../components/Providers";
import Header from "@/components/ui/organisms/Header"; // Corrected alias path

export const metadata: Metadata = {
  title: "پلتفرم رشد شخصی مسیر‌یار",
  description: "پلتفرمی برای رشد شخصی و یادداشت‌نویسی",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="fa" dir="rtl">
      <body className="bg-gray-100">
        <Providers>
          <Header />
          <main>{children}</main>
        </Providers>
      </body>
    </html>
  );
}
