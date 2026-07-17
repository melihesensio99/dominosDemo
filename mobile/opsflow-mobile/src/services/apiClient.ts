import axios, { AxiosError, type AxiosRequestConfig, type InternalAxiosRequestConfig } from 'axios';
import { ERROR_MESSAGES } from '../constants/errorMessages';
import type { Address } from '../types/common';
import type { AuthCredentials, SessionUser } from '../types/auth';
import type { Basket } from '../types/basket';
import type { Category, Product } from '../types/catalog';
import type { Order } from '../types/order';
import { tokenService } from './token.service';

const apiBase = process.env.EXPO_PUBLIC_API_BASE_URL ?? 'http://localhost:8000';

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
        return Promise.reject(new Error(ERROR_MESSAGES.SESSION_EXPIRED));
      }

      const message = extractErrorMessage(error);
      return Promise.reject(new Error(message));
    }

    return Promise.reject(new Error(ERROR_MESSAGES.REQUEST_FAILED));
  },
);

function extractErrorMessage(error: AxiosError): string {
  const data = error.response?.data;

  if (typeof data === 'string' && data.trim().length > 0) {
    return data;
  }

  if (typeof data === 'object' && data !== null) {
    if ('message' in data && typeof data.message === 'string') {
      return data.message;
    }

    if ('error' in data && typeof data.error === 'string') {
      return data.error;
    }
  }

  return error.message || ERROR_MESSAGES.REQUEST_FAILED;
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
  return request<SessionUser>('/proxy/auth/auth/login', { method: 'POST', body });
}

export function postRegister(body: AuthCredentials) {
  return request<SessionUser>('/proxy/auth/auth/register', { method: 'POST', body });
}

export function getProducts() {
  return request<Product[]>('/proxy/catalog/products');
}

export function getCategories() {
  return request<Category[]>('/proxy/catalog/categories');
}

export function getBasket(customerId: string) {
  return request<Basket>(`/proxy/basket/baskets/${customerId}`);
}

export function addBasketItem(customerId: string, body: { productId: string; quantity: number }) {
  return request<Basket>(`/proxy/basket/baskets/${customerId}/items`, { method: 'POST', body });
}

export function updateBasketItem(customerId: string, productId: string, body: { quantity: number }) {
  return request<Basket>(`/proxy/basket/baskets/${customerId}/items/${productId}`, { method: 'PUT', body });
}

export function removeBasketItem(customerId: string, productId: string) {
  return request<Basket>(`/proxy/basket/baskets/${customerId}/items/${productId}`, { method: 'DELETE' });
}

export function clearBasket(customerId: string) {
  return request<Basket>(`/proxy/basket/baskets/${customerId}`, { method: 'DELETE' });
}

export function getMyOrders() {
  return request<Order[]>('/proxy/order/orders/me');
}

export function createOrder(body: {
  customerId: string;
  items: { productId: string; quantity: number }[];
  shippingAddress: Address;
  billingAddress: Address;
  paymentMethod: number;
}) {
  return request<Order>('/proxy/order/orders', { method: 'POST', body });
}
