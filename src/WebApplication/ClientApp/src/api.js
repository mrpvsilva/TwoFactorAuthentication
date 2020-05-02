import axios from 'axios'



const api = axios.create({
    baseURL: 'https://localhost:44308/api'
});

api.interceptors.response.use(res => res, error => {
    console.log(error);
    return error;
})

export default api;