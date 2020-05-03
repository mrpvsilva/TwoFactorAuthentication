import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import { Switch, Route, Redirect } from 'react-router-dom';

import { Home } from './Home';
import { FetchData } from './FetchData';

export class Layout extends Component {
  static displayName = Layout.name;

  render() {
    return (
      <div>
        <NavMenu history={this.props.history} />
        <Container>
          <Switch>
            <Route path='/Home' component={Home} />
            <Route path='/fetch-data' component={FetchData} />
            <Redirect from="*" to="/Home" />
          </Switch>
        </Container>
      </div>
    );
  }
}
