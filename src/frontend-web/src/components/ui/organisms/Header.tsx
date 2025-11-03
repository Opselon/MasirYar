// src/frontend-web/src/components/ui/organisms/Header.tsx
import Link from 'next/link';
import React from 'react';

export default function Header() {
  return (
    <header className="bg-white shadow-md">
      <nav className="container mx-auto px-6 py-4 flex justify-between items-center">
        <Link href="/" className="text-xl font-bold text-gray-800">
          مسیر‌یار
        </Link>
        <div>
          <Link href="/journals" className="text-gray-600 hover:text-gray-800 mx-4">
            یادداشت‌ها
          </Link>
          {/* Add other links here */}
        </div>
      </nav>
    </header>
  );
}
