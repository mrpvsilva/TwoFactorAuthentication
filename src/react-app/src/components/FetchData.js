import React, { useState, useEffect } from 'react';
import Api from '../api';

export default function FetchData() {
  const [persons, setPersons] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Api.get('/persons')
      .then(({ data }) => {
        if (data) {
          setPersons(data);
          setLoading(false);
        }
      });
  }, []);

  return (
    <div>
      <h1 id="tabelLabel">Persons</h1>
      <p>This component demonstrates fetching data from the server.</p>
      {loading
        ? <p><em>Loading...</em></p>
        : (
          <table className='table table-striped' aria-labelledby="tabelLabel">
            <thead>
              <tr>
                <th>FirstName</th>
                <th>Email</th>
              </tr>
            </thead>
            <tbody>
              {persons.map(person => (
                <tr key={person.id}>
                  <td>{person.firstName}</td>
                  <td>{person.email}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )
      }
    </div>
  );
}
