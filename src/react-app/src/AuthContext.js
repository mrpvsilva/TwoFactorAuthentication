import React, { createContext, useContext, useEffect, useState } from 'react';
import { navigate } from './navigate';
import { setAccessToken as storeSetToken, registerTokenChangeCallback } from './tokenStore';
import { authService } from './services/authService';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [accessToken, _setAccessToken] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  const setAccessToken = (token) => {
    _setAccessToken(token);
    storeSetToken(token);
  };

  useEffect(() => {
    const unregister = registerTokenChangeCallback(_setAccessToken);

    authService.refresh()
      .then(({ data }) => setAccessToken(data.accessToken))
      .catch(() => setAccessToken(null))
      .finally(() => setIsLoading(false));

    return unregister;
  }, []);

  const logout = async () => {
    try {
      await authService.logout();
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
