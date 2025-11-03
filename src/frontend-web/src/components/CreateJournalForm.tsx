// src/frontend-web/src/components/CreateJournalForm.tsx
"use client";

import { useCreateJournal } from "@/hooks/useCreateJournal";
import { useState } from "react";

export default function CreateJournalForm() {
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const createJournalMutation = useCreateJournal();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!title || !content) return;

    createJournalMutation.mutate({ title, content });

    // Reset form
    setTitle("");
    setContent("");
  };

  return (
    <form onSubmit={handleSubmit} className="p-4 border rounded-md shadow-sm mb-8">
      <h2 className="text-xl font-semibold mb-4">Create New Journal</h2>
      <div className="flex flex-col space-y-4">
        <input
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder="Journal Title"
          className="p-2 border rounded-md"
          disabled={createJournalMutation.isPending}
        />
        <textarea
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="What's on your mind?"
          className="p-2 border rounded-md"
          rows={4}
          disabled={createJournalMutation.isPending}
        />
        <button
          type="submit"
          className="p-2 bg-blue-500 text-white rounded-md disabled:bg-gray-400"
          disabled={createJournalMutation.isPending}
        >
          {createJournalMutation.isPending ? "Creating..." : "Create Journal"}
        </button>
        {createJournalMutation.isError && (
          <p className="text-red-500">
            Error: {createJournalMutation.error.message}
          </p>
        )}
      </div>
    </form>
  );
}
