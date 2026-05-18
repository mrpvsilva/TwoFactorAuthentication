import React from 'react';
import { Card, CardContent } from 'src/components/ui/card';

export default function AuthCard({ children }) {
  return (
    <div className="flex min-h-screen items-start justify-center px-4 pt-[10vh]">
      <div className="w-full max-w-sm">
        <Card>
          <CardContent className="p-6">{children}</CardContent>
        </Card>
      </div>
    </div>
  );
}
