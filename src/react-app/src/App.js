import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { setNavigate } from './navigate';
import { AuthProvider } from './AuthContext';
import Routes from './routes';
import './custom.css';

function NavigationSetter() {
  const navigate = useNavigate();
  useEffect(() => { setNavigate(navigate); }, [navigate]);
  return null;
}

function App() {
  return (
    <AuthProvider>
      <NavigationSetter />
      <Routes />
    </AuthProvider>
  );
}

export default App;
