import 'react-toastify/dist/ReactToastify.css';
import React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';

import './index.css';
import App from './App';

const baseUrl = document.getElementsByTagName('base')[0]?.getAttribute('href') ?? '/';

const root = createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <BrowserRouter basename={baseUrl}>
      <App />
      <ToastContainer position="top-right" autoClose={2500} />
    </BrowserRouter>
  </React.StrictMode>
);
