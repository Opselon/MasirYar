// src/frontend-web/src/hooks/useJournals.ts
"use client";

import { useQuery } from '@tanstack/react-query';

// Define the type for a journal entry
interface Journal {
  id: string;
  title: string;
  content: string;
  createdAt: string;
}

// A mock API function to fetch journals. In a real app, this would be an HTTP request.
const fetchJournals = async (): Promise<Journal[]> => {
  console.log("Fetching journals...");
  // Simulate network delay
  await new Promise(resolve => setTimeout(resolve, 1000));

  // Mock data
  return [
    { id: '1', title: 'First Entry', content: 'This is the first journal entry.', createdAt: new Date().toISOString() },
    { id: '2', title: 'Second Entry', content: 'This is another entry.', createdAt: new Date().toISOString() },
  ];
};

export const useJournals = (initialData?: Journal[]) => {
  return useQuery<Journal[]>({
    queryKey: ['journals'],
    queryFn: fetchJournals,
    initialData: initialData,
  });
};
