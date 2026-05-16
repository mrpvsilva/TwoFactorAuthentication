import React, { createContext, useContext, useEffect, useState } from 'react';
import api from './api';
import { navigate } from './navigate';
import { setAccessToken as storeSetToken, registerTokenChangeCallback } from './tokenStore';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [accessToken, _setAccessToken] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  const setAccessToken = (token) => {
    _setAccessToken(token);
    storeSetToken(token);
  };

  useEffect(() => {
    registerTokenChangeCallback(_setAccessToken);

    api.post('/auth/refresh')
      .then(({ data }) => setAccessToken(data.accessToken))
      .catch(() => setAccessToken(null))
      .finally(() => setIsLoading(false));
  }, []);

  const logout = async () => {
    try {
      await api.post('/auth/logout');
    } catch {}
    setAccessToken(null);
    navigate('/login');
  };

  return (
    <AuthContext.Provider value={{ accessToken, isLoading, setAccessToken, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
