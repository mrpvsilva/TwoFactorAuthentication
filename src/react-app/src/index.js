import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { Router } from 'react-router-dom';
import { createBrowserHistory } from 'history';
import { positions, Provider } from "react-alert";
import AlertTemplate from "react-alert-template-basic";

import './index.css';
import App from './App';
import * as serviceWorker from './serviceWorker';

export const history = createBrowserHistory();
const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');

const options = {
  timeout: 2500,
  position: positions.TOP_RIGHT
};

ReactDOM.render(
  <React.StrictMode>
    <Router basename={baseUrl} history={history}>
      <Provider template={AlertTemplate} {...options}>
        <App />
      </Provider>
    </Router>
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
