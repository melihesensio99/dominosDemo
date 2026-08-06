import type { Address } from './common';

export interface OrderItem {
  productId: string;
  quantity: number;
  selectedOptionIds: string[];
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
  totalPrice: number;
  createdAt: string;
  updatedAt: string | null;
}
