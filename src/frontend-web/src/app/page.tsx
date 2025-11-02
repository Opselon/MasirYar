'use client';

export default function Home() {
  const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:8000';

  return (
    <main className="flex min-h-screen flex-col items-center justify-center p-24">
      <div className="z-10 max-w-5xl w-full items-center justify-center font-mono text-sm">
        <h1 className="text-4xl font-bold text-center mb-8">
          Personal Growth Platform
        </h1>
        <p className="text-center text-gray-600 mb-4">
          Welcome to the Personal Growth Platform
        </p>
        <p className="text-center text-sm text-gray-400">
          API Gateway: {apiUrl}
        </p>
      </div>
    </main>
  );
}
