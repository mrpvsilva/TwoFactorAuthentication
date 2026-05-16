import api from '../api';

export const authService = {
  login: (data) => api.post('/auth', data),
  logout: () => api.post('/auth/logout'),
  refresh: () => api.post('/auth/refresh'),
  verifyCode: (data) => api.post('/auth/VerifyCode', data),
  addTwoFactor: (data) => api.post('/auth/AddTwoFactAuth', data),
  resetTotp: () => api.delete('/auth/totp'),
  register: (data) => api.post('/account', data),
};
