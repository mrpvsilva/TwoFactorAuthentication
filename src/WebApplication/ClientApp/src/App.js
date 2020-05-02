import React, { Component } from 'react';
import Routes from './routes';

import './custom.css'

export default class App extends Component {
  static displayName = App.name;

  render() {
    return (<Routes />);
  }
}
