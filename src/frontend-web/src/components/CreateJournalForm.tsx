// src/frontend-web/src/components/CreateJournalForm.tsx
"use client";

import { useCreateJournal } from "@/hooks/useCreateJournal";
import { useState } from "react";
import { Button } from "./ui/atoms/Button"; // Import the new Button component

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
    <form onSubmit={handleSubmit} className="p-4 border rounded-md shadow-sm mb-8 bg-gray-50">
      <h2 className="text-xl font-semibold mb-4">ایجاد یادداشت جدید</h2>
      <div className="flex flex-col space-y-4">
        <input
          type="text"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder="عنوان یادداشت"
          className="p-2 border rounded-md"
          disabled={createJournalMutation.isPending}
        />
        <textarea
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="چه چیزی در ذهن دارید؟"
          className="p-2 border rounded-md"
          rows={4}
          disabled={createJournalMutation.isPending}
        />
        <Button
          type="submit"
          disabled={createJournalMutation.isPending}
        >
          {createJournalMutation.isPending ? "در حال ایجاد..." : "ایجاد یادداشت"}
        </Button>
        {createJournalMutation.isError && (
          <p className="text-red-500">
            خطا: {createJournalMutation.error.message}
          </p>
        )}
      </div>
    </form>
  );
}
