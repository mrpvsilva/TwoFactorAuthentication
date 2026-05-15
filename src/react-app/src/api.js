import axios from 'axios';
import { toast } from 'react-toastify';
import { navigate } from './navigate';

const api = axios.create({
  baseURL: process.env.REACT_APP_API_ENDPOINT
});

api.interceptors.request.use((config) => {
  const raw = localStorage.getItem('token');
  if (raw) {
    const parsed = JSON.parse(raw);
    const accessToken = parsed?.accessToken ?? parsed?.AccessToken;
    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`;
    }
  }
  return config;
}, (error) => Promise.reject(error));

api.interceptors.response.use(res => res, error => {
  if (error.response?.status === 401) {
    localStorage.removeItem('token');
    navigate('/login');
    return Promise.reject(error);
  }

  const errors = error.response?.data;
  if (Array.isArray(errors) && errors.length > 0) {
    errors.forEach(err => toast.error(err.message));
  } else {
    toast.error('Request error');
  }

  return Promise.reject(error);
});

export default api;
