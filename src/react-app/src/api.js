import axios from 'axios';
import { history } from './index';

console.log(process.env.REACT_APP_API_ENDPOINT)

const api = axios.create({
    baseURL: process.env.REACT_APP_API_ENDPOINT
});


api.interceptors.request.use(async (config) => {


    const token = localStorage.getItem('token');
    if (token) {
        const { accessToken } = JSON.parse(token);
        config.headers.Authorization = `Bearer ${accessToken}`;
    }

    return config;
}, (error) => {
    // I cand handle a request with errors here
    return Promise.reject(error);
});

api.interceptors.response.use(res => res, error => {

    const { status } = error.response;

    console.log(error);

    if (error.response && error.response.status === 401) {
        localStorage.removeItem('token');
        history.push('/login');
    }
    // Do something with response error
    return Promise.reject(error)

});

export default api;