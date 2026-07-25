export interface BasketItem {
  id: string;
  productId: string;
  productName: string;
  basePrice: number;
  unitPrice: number;
  totalPrice: number;
  quantity: number;
  selectedOptions: SelectedBasketOption[];
  createdAt: string;
  updatedAt: string | null;
}

export interface SelectedBasketOption {
  optionId: string;
  groupName: string;
  name: string;
  priceAdjustment: number;
}

export interface Basket {
  customerId: string;
  items: BasketItem[];
  itemCount: number;
  totalQuantity: number;
  createdAt: string;
  updatedAt: string;
}
