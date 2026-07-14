export interface SessionUser {
  userId: string;
  email: string;
  role: string;
  accessToken: string;
}

export interface AuthCredentials {
  email: string;
  password: string;
}

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  stock: number;
  categoryId: string;
  categoryName: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  isActive: boolean;
  createdAt: string;
}

export interface BasketItem {
  productId: string;
  quantity: number;
  createdAt: string;
  updatedAt: string | null;
}

export interface Basket {
  customerId: string;
  items: BasketItem[];
  itemCount: number;
  totalQuantity: number;
  createdAt: string;
  updatedAt: string;
}

export interface Address {
  street: string;
  district: string;
  city: string;
  postalCode: string;
  country: string;
}

export interface OrderItem {
  productId: string;
  quantity: number;
}

export interface Order {
  id: string;
  customerId: string;
  items: OrderItem[];
  shippingAddress: Address;
  billingAddress: Address;
  payment: {
    method: number;
    status: string;
  };
  status: string;
  createdAt: string;
  updatedAt: string | null;
}

export interface NotificationItem {
  id: string;
  recipientId: string;
  message: string;
  status: string;
  createdAt: string;
}

export interface ApiEnvelope<T> {
  items: T;
}
