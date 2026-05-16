import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { Spinner } from 'reactstrap';
import { useAuth } from '../AuthContext';

export default function PrivateRoute({ children }) {
  const { accessToken, isLoading } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ height: '100vh' }}>
        <Spinner />
      </div>
    );
  }

  return accessToken
    ? children
    : <Navigate to='/login' state={{ from: location }} replace />;
}
