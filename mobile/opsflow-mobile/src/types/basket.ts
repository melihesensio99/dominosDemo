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
