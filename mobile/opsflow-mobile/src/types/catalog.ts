export interface Product {
  id: string;
  name: string;
  description: string;
  imageUrl: string | null;
  price: number;
  categoryId: string;
  categoryName: string | null;
  optionGroups: ProductOptionGroup[];
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface ProductOptionGroup {
  id: string;
  name: string;
  selectionType: 'single' | 'multiple';
  isRequired: boolean;
  displayOrder: number;
  options: ProductOption[];
}

export interface ProductOption {
  id: string;
  name: string;
  priceAdjustment: number;
  isDefault: boolean;
  isActive: boolean;
  displayOrder: number;
}

export interface Category {
  id: string;
  name: string;
  slug: string;
  isActive: boolean;
  createdAt: string;
}
