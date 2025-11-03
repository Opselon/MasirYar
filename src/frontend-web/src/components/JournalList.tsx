// src/frontend-web/src/components/JournalList.tsx
"use client";

import { useJournals } from "@/hooks/useJournals";

interface Journal {
  id: string;
  title: string;
  content: string;
  createdAt: string;
}

interface JournalListProps {
  initialData: Journal[];
}

export default function JournalList({ initialData }: JournalListProps) {
  const { data: journals, isLoading, isError } = useJournals(initialData);

  if (isLoading) {
    return <div>Loading journals...</div>;
  }

  if (isError) {
    return <div>Error fetching journals.</div>;
  }

  return (
    <div className="space-y-4">
      {journals?.map((journal) => (
        <div key={journal.id} className="p-4 border rounded-md shadow-sm">
          <h2 className="text-xl font-semibold">{journal.title}</h2>
          <p className="text-gray-600">{journal.content}</p>
          <small className="text-gray-400">{new Date(journal.createdAt).toLocaleDateString()}</small>
        </div>
      ))}
    </div>
  );
}
