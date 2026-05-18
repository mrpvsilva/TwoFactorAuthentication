import api from '../api';

export const passwordService = {
  forgot: (email) => api.post('/password/forgot', { email }),
  verifyCode: (email, code) => api.post('/password/verify-code', { email, code }),
  reset: (email, code, password) => api.post('/password/reset', { email, code, password }),
};
