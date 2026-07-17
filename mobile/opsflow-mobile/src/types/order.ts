import type { Address } from './common';

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
