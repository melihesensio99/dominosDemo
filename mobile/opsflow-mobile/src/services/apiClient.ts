import axios, { AxiosError, type AxiosRequestConfig, type InternalAxiosRequestConfig } from 'axios';
import type { Address } from '../types/common';
import type { AuthCredentials, SessionUser, UserAddress } from '../types/auth';
import type { Basket } from '../types/basket';
import type { Category, Product } from '../types/catalog';
import type { Order } from '../types/order';
import { endpoints } from './endpoints';
import { tokenService } from './token.service';

const apiBase = process.env.EXPO_PUBLIC_API_BASE_URL ?? 'http://localhost:5022';

const apiClient = axios.create({
  baseURL: apiBase,
  headers: {
    'Content-Type': 'application/json',
  },
});

apiClient.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = tokenService.getAccessToken();

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error: unknown) => {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 401) {
        tokenService.clearAccessToken();
        return Promise.reject(new Error('Oturum süresi doldu. Lütfen tekrar giriş yap.'));
      }

      return Promise.reject(new Error(extractErrorMessage(error)));
    }

    return Promise.reject(new Error('İstek başarısız oldu.'));
  },
);

function extractErrorMessage(error: AxiosError): string {
  const data = error.response?.data;

  if (typeof data === 'string' && data.trim().length > 0) {
    return data;
  }

  if (typeof data === 'object' && data !== null) {
    const problem = data as Record<string, unknown>;

    if (typeof problem.message === 'string') {
      return problem.message;
    }

    if (typeof problem.error === 'string') {
      return problem.error;
    }

    if (typeof problem.detail === 'string') {
      return problem.detail;
    }

    if (problem.errors && typeof problem.errors === 'object') {
      const validationMessages = Object.values(problem.errors as Record<string, unknown>)
        .flatMap((value) => (Array.isArray(value) ? value : [value]))
        .filter((value): value is string => typeof value === 'string');

      if (validationMessages.length > 0) {
        return validationMessages.join(' ');
      }
    }

    if (typeof problem.title === 'string') {
      return problem.title;
    }
  }

  return error.message || 'İstek başarısız oldu.';
}

type RequestOptions = Omit<AxiosRequestConfig, 'url' | 'baseURL' | 'data'> & {
  body?: unknown;
};

async function request<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const response = await apiClient.request<T>({
    url: path,
    method: options.method,
    params: options.params,
    headers: options.headers,
    data: options.body,
    signal: options.signal,
  });

  return response.data;
}

export function postLogin(body: AuthCredentials) {
  return request<SessionUser>(endpoints.auth.login, { method: 'POST', body });
}

export function postRegister(body: AuthCredentials) {
  return request<SessionUser>(endpoints.auth.register, { method: 'POST', body });
}

export function getAddresses() {
  return request<UserAddress[]>(endpoints.auth.addresses);
}

export function createAddress(body: Omit<UserAddress, 'id'>) {
  return request<UserAddress>(endpoints.auth.addresses, { method: 'POST', body });
}

export function deleteAddress(addressId: string) {
  return request<void>(endpoints.auth.addressById(addressId), { method: 'DELETE' });
}

export function getProducts() {
  return request<Product[]>(endpoints.catalog.products);
}

export function getCategories() {
  return request<Category[]>(endpoints.catalog.categories);
}

export function getBasket() {
  return request<Basket>(endpoints.basket.mine);
}

export function addBasketItem(body: { productId: string; quantity: number; selectedOptionIds?: string[] }) {
  return request<Basket>(endpoints.basket.items, { method: 'POST', body });
}

export function updateBasketItem(itemId: string, body: { quantity: number }) {
  return request<Basket>(endpoints.basket.itemById(itemId), { method: 'PUT', body });
}

export function removeBasketItem(itemId: string) {
  return request<Basket>(endpoints.basket.itemById(itemId), { method: 'DELETE' });
}

export function clearBasket() {
  return request<Basket>(endpoints.basket.mine, { method: 'DELETE' });
}

export function getMyOrders() {
  return request<Order[]>(endpoints.orders.me);
}

export function createOrder(body: {
  items: { productId: string; quantity: number; selectedOptionIds: string[] }[];
  shippingAddress: Address;
  billingAddress: Address;
  paymentMethod: number;
  note?: string;
}) {
  return request<Order>(endpoints.orders.create, { method: 'POST', body });
}

export function cancelOrder(orderId: string) {
  return request<Order>(endpoints.orders.cancel(orderId), { method: 'POST' });
}
