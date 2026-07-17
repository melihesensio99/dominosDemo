export const endpoints = {
  auth: {
    login: '/proxy/auth/auth/login',
    register: '/proxy/auth/auth/register',
  },
  catalog: {
    products: '/proxy/catalog/products',
    categories: '/proxy/catalog/categories',
  },
  basket: {
    byCustomer: (customerId: string) => `/proxy/basket/baskets/${customerId}`,
    items: (customerId: string) => `/proxy/basket/baskets/${customerId}/items`,
    itemByProduct: (customerId: string, productId: string) =>
      `/proxy/basket/baskets/${customerId}/items/${productId}`,
  },
  orders: {
    me: '/proxy/order/orders/me',
    create: '/proxy/order/orders',
  },
} as const;
