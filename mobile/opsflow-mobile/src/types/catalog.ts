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
