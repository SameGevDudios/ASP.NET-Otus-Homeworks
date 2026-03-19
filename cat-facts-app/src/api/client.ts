import axios from 'axios';

export const apiClient = axios.create({
  baseURL: 'https://catfact.ninja',
  timeout: 5000,
});