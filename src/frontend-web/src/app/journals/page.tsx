// src/frontend-web/src/app/journals/page.tsx
import JournalList from "@/components/JournalList";

// A mock API function for server-side fetching
const getJournals = async () => {
  // In a real app, this would fetch from your API
  // You can use fetch() directly in Server Components
  console.log("Fetching journals on the server...");
  return [
    { id: '1', title: 'Server-Fetched Entry 1', content: 'This was fetched on the server.', createdAt: new Date().toISOString() },
    { id: '2', title: 'Server-Fetched Entry 2', content: 'This also came from the server.', createdAt: new Date().toISOString() },
  ];
};

import CreateJournalForm from "@/components/CreateJournalForm";

// This is a React Server Component (RSC)
export default async function JournalsPage() {
  const initialData = await getJournals();

  return (
    <main className="p-8">
      <h1 className="text-2xl font-bold mb-4">یادداشت‌های من</h1>

      <CreateJournalForm />

      {/*
        We pass the server-fetched data as initialData to the client component.
        React Query will use this data on the initial render and won't refetch it on the client.
      */}
      <JournalList initialData={initialData} />
    </main>
  );
}
