import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';

export default function PrivateRoute({ children }) {
  const token = localStorage.getItem('token');
  const location = useLocation();

  return token
    ? children
    : <Navigate to='/login' state={{ from: location }} replace />;
}
