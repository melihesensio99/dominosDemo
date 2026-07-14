import type { AuthCredentials, Basket, Category, NotificationItem, Order, Product, SessionUser } from './types';

const apiBase = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8000';

type RequestOptions = Omit<RequestInit, 'body'> & {
  body?: unknown;
};

async function request<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const response = await fetch(`${apiBase}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers ?? {}),
    },
    body: options.body === undefined ? undefined : JSON.stringify(options.body),
  });

  const payload = await parseResponse(response);

  if (!response.ok) {
    const message = typeof payload === 'object' && payload !== null && 'message' in payload
      ? String((payload as { message?: string }).message ?? 'Request failed')
      : 'Request failed';
    throw new Error(message);
  }

  return payload as T;
}

async function parseResponse(response: Response) {
  const text = await response.text();
  if (!text) {
    return null;
  }

  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
}

export const authApi = {
  login: (body: AuthCredentials) => request<SessionUser>('/proxy/auth/auth/login', { method: 'POST', body }),
  register: (body: AuthCredentials) => request<SessionUser>('/proxy/auth/auth/register', { method: 'POST', body }),
};

export const catalogApi = {
  getProducts: () => request<Product[]>('/proxy/catalog/products'),
  getCategories: () => request<Category[]>('/proxy/catalog/categories'),
};

export const basketApi = {
  getBasket: (customerId: string) => request<Basket>(`/proxy/basket/baskets/${customerId}`),
  addItem: (customerId: string, body: { productId: string; quantity: number }) =>
    request<Basket>(`/proxy/basket/baskets/${customerId}/items`, { method: 'POST', body }),
  updateItem: (customerId: string, productId: string, body: { quantity: number }) =>
    request<Basket>(`/proxy/basket/baskets/${customerId}/items/${productId}`, { method: 'PUT', body }),
  removeItem: (customerId: string, productId: string) =>
    request<Basket>(`/proxy/basket/baskets/${customerId}/items/${productId}`, { method: 'DELETE' }),
  clearBasket: (customerId: string) =>
    request<Basket>(`/proxy/basket/baskets/${customerId}`, { method: 'DELETE' }),
};

export const orderApi = {
  getOrders: () => request<Order[]>('/proxy/order/orders'),
  createOrder: (body: unknown) => request<Order>('/proxy/order/orders', { method: 'POST', body }),
};

export const notificationApi = {
  getNotifications: () => request<{ items: NotificationItem[] }>('/proxy/notification/notifications'),
};
