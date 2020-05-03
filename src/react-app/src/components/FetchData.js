import React, { Component } from 'react';
import Api from '../api';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { persons: [], loading: true };
  }

  componentDidMount() {
    this.populatePersonsData();
  }

  static renderPersonsTable(persons) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>FirstName</th>
            <th>Email</th>
          </tr>
        </thead>
        <tbody>
          {persons.map(person =>
            <tr key={person.id}>
              <td>{person.firstName}</td>
              <td>{person.email}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderPersonsTable(this.state.persons);

    return (
      <div>
        <h1 id="tabelLabel" >Persons</h1>
        <p>This component demonstrates fetching data from the server.</p>
        {contents}
      </div>
    );
  }

  populatePersonsData() {
    Api.get('/persons')
      .then(({ data }) => {
        if (data) {
          this.setState({ persons: data, loading: false });
        }
      })
  }
}
