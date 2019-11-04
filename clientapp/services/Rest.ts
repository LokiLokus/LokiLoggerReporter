import axios from 'axios';

export const Rest = axios.create({
  baseURL: `http://jsonplaceholder.typicode.com/`,
})
