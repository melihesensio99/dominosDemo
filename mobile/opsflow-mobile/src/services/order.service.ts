import { cancelOrder, createOrder, getMyOrders } from './apiClient';

export const orderService = {
  getMyOrders,
  createOrder,
  cancelOrder,
};
