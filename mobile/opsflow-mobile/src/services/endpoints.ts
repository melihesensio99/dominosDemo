export const endpoints = {
  auth: {
    login: '/proxy/auth/auth/login',
    register: '/proxy/auth/auth/register',
    addresses: '/proxy/auth/auth/addresses',
    addressById: (addressId: string) => `/proxy/auth/auth/addresses/${addressId}`,
  },
  catalog: {
    products: '/proxy/catalog/products',
    categories: '/proxy/catalog/categories',
  },
  basket: {
    mine: '/proxy/basket/baskets/me',
    items: '/proxy/basket/baskets/me/items',
    itemByProduct: (productId: string) =>
      `/proxy/basket/baskets/me/items/${productId}`,
  },
  orders: {
    me: '/proxy/order/orders/me',
    create: '/proxy/order/orders',
  },
} as const;
