import * as React from 'react';
import { cn } from 'src/lib/utils';

function Card({ className, ...props }) {
  return (
    <div
      className={cn('rounded-xl border bg-card text-card-foreground shadow', className)}
      {...props}
    />
  );
}

function CardContent({ className, ...props }) {
  return (
    <div className={cn('p-6', className)} {...props} />
  );
}

export { Card, CardContent };
