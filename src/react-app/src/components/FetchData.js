import React, { useState, useEffect } from 'react';
import Api from '../api';

export default function FetchData() {
  const [persons, setPersons] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const controller = new AbortController();

    Api.get('/persons', { signal: controller.signal })
      .then(({ data }) => setPersons(data))
      .catch(() => {})
      .finally(() => setLoading(false));

    return () => controller.abort();
  }, []);

  return (
    <div className="space-y-4">
      <h1 id="tabelLabel" className="text-2xl font-bold">Persons</h1>
      <p className="text-muted-foreground">This component demonstrates fetching data from the server.</p>
      {loading
        ? <p className="text-muted-foreground italic">Loading...</p>
        : (
          <div className="overflow-x-auto rounded-md border">
            <table className="min-w-full divide-y divide-border" aria-labelledby="tabelLabel">
              <thead className="bg-muted">
                <tr>
                  <th className="px-4 py-3 text-left text-sm font-medium">FirstName</th>
                  <th className="px-4 py-3 text-left text-sm font-medium">Email</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {persons.map(person => (
                  <tr key={person.id} className="hover:bg-muted/50">
                    <td className="px-4 py-3 text-sm">{person.firstName}</td>
                    <td className="px-4 py-3 text-sm">{person.email}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )
      }
    </div>
  );
}
