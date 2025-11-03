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
    return <div>در حال بارگذاری یادداشت‌ها...</div>;
  }

  if (isError) {
    return <div>خطا در دریافت یادداشت‌ها.</div>;
  }

  return (
    <div className="space-y-4">
      {journals?.map((journal) => (
        <div key={journal.id} className="p-4 border rounded-md shadow-sm bg-white">
          <h2 className="text-xl font-semibold">{journal.title}</h2>
          <p className="text-gray-600 mt-2">{journal.content}</p>
          <small className="text-gray-400 mt-4 block text-left">
            {new Date(journal.createdAt).toLocaleDateString('fa-IR')}
          </small>
        </div>
      ))}
    </div>
  );
}
