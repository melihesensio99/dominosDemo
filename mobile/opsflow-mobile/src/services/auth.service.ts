import type { AuthCredentials } from '../types/auth';
import { createAddress, deleteAddress, getAddresses, postLogin, postRegister } from './apiClient';

export const authService = {
  login: (body: AuthCredentials) => postLogin(body),
  register: (body: AuthCredentials) => postRegister(body),
  getAddresses,
  createAddress,
  deleteAddress,
};
