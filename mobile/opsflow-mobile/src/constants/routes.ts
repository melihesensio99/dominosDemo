export const ROUTES = {
  HOME: 'home',
  MENU: 'menu',
  BASKET: 'basket',
  ACCOUNT: 'account',
} as const;

export type RouteKey = (typeof ROUTES)[keyof typeof ROUTES];

export const ROUTE_LABELS: Record<RouteKey, string> = {
  home: 'Ana Sayfa',
  menu: 'Menü',
  basket: 'Sepetim',
  account: 'Hesabım',
};
