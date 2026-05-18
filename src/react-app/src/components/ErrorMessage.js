import React from 'react';

export default function ErrorMessage({ error }) {
  return error ? <p className="text-sm text-red-500 mt-1">{error.message}</p> : null;
}
