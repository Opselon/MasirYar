// src/frontend-web/src/hooks/useCreateJournal.ts
"use client";

import { useMutation, useQueryClient } from '@tanstack/react-query';
import { Journal } from './useJournals'; // Assuming Journal type is exported

// A mock API function to create a journal.
const createJournal = async (newJournal: { title: string; content: string }): Promise<Journal> => {
  console.log("Creating a new journal...", newJournal);
  // Simulate network delay
  await new Promise(resolve => setTimeout(resolve, 1500));

  // Simulate a potential error
  if (newJournal.title.toLowerCase().includes('error')) {
    throw new Error("Failed to create journal due to an error.");
  }

  // Return the new journal with a generated ID and timestamp
  return {
    id: Math.random().toString(36).substring(2, 9),
    ...newJournal,
    createdAt: new Date().toISOString(),
  };
};

export const useCreateJournal = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: createJournal,
    // When mutate is called:
    onMutate: async (newJournal) => {
      // Cancel any outgoing refetches (so they don't overwrite our optimistic update)
      await queryClient.cancelQueries({ queryKey: ['journals'] });

      // Snapshot the previous value
      const previousJournals = queryClient.getQueryData<Journal[]>(['journals']);

      // Optimistically update to the new value
      queryClient.setQueryData<Journal[]>(['journals'], (old) => [
        {
          id: 'temp-id', // Use a temporary ID
          ...newJournal,
          createdAt: new Date().toISOString(),
        },
        ...(old || []),
      ]);

      // Return a context object with the snapshotted value
      return { previousJournals };
    },
    // If the mutation fails, use the context returned from onMutate to roll back
    onError: (err, newJournal, context) => {
      queryClient.setQueryData(['journals'], context?.previousJournals);
    },
    // Always refetch after error or success:
    onSettled: () => {
      queryClient.invalidateQueries({ queryKey: ['journals'] });
    },
  });
};
