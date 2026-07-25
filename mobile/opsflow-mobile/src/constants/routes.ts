export const ROUTES = {
  HOME: 'home',
  BASKET: 'basket',
  ACCOUNT: 'account',
} as const;

export type RouteKey = (typeof ROUTES)[keyof typeof ROUTES];

export const ROUTE_LABELS: Record<RouteKey, string> = {
  home: 'Ana Sayfa',
  basket: 'Sepetim',
  account: 'Hesabim',
};
