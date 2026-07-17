import { addBasketItem, clearBasket, getBasket, removeBasketItem, updateBasketItem } from './apiClient';

export const basketService = {
  getBasket,
  addItem: addBasketItem,
  updateItem: updateBasketItem,
  removeItem: removeBasketItem,
  clearBasket,
};
