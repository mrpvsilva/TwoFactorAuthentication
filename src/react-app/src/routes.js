import React from 'react';
import { Routes, Route } from 'react-router-dom';
import PrivateRoute from './components/PrivateRoute';
import Layout from './components/Layout';
import Login from './components/Login';
import Register from './components/Register';
import TwoFactAuth from './components/TwoFactAuth';
import ForgotPassword from './components/ForgotPassword';
import ResetPasswordCode from './components/ResetPasswordCode';
import ResetPassword from './components/ResetPassword';

export default function AppRoutes() {
  return (
    <Routes>
      <Route path='/login' element={<Login />} />
      <Route path='/register' element={<Register />} />
      <Route path='/twofactauth' element={<TwoFactAuth />} />
      <Route path='/forgot-password' element={<ForgotPassword />} />
      <Route path='/reset-password/code' element={<ResetPasswordCode />} />
      <Route path='/reset-password/new' element={<ResetPassword />} />
      <Route path='/*' element={<PrivateRoute><Layout /></PrivateRoute>} />
    </Routes>
  );
}
