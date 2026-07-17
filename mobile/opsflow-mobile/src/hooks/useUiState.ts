import { useState } from 'react';
import { ROUTES, type RouteKey } from '../constants/routes';

export function useUiState() {
  const [tab, setTab] = useState<RouteKey>(ROUTES.HOME);

  return {
    tab,
    setTab,
  };
}
