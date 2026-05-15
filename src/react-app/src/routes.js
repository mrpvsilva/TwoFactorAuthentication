import React from 'react';
import { Routes, Route } from 'react-router-dom';
import PrivateRoute from './components/PrivateRoute';
import Layout from './components/Layout';
import Login from './components/Login';
import Register from './components/Register';
import TwoFactAuth from './components/TwoFactAuth';

export default function AppRoutes() {
  return (
    <Routes>
      <Route path='/login' element={<Login />} />
      <Route path='/register' element={<Register />} />
      <Route path='/twofactauth' element={<TwoFactAuth />} />
      <Route path='/*' element={<PrivateRoute><Layout /></PrivateRoute>} />
    </Routes>
  );
}
