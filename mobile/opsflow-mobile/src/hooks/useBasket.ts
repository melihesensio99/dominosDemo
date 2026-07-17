import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { ERROR_MESSAGES } from '../constants/errorMessages';
import { basketService } from '../services';
import type { Basket } from '../types/basket';

export function useBasket(customerId?: string | null) {
  const queryClient = useQueryClient();
  const [isAddingItem, setIsAddingItem] = useState(false);
  const [actionError, setActionError] = useState<Error | null>(null);

  const basketQuery = useQuery({
    queryKey: ['basket', customerId],
    enabled: Boolean(customerId),
    queryFn: async () => {
      if (!customerId) {
        return null;
      }

      return basketService.getBasket(customerId);
    },
  });

  const addItem = async (productId: string) => {
    if (!customerId) {
      return;
    }

    try {
      setIsAddingItem(true);
      setActionError(null);
      await basketService.addItem(customerId, { productId, quantity: 1 });
      await queryClient.invalidateQueries({ queryKey: ['basket'] });
    } catch (cause) {
      setActionError(cause instanceof Error ? cause : new Error(ERROR_MESSAGES.BASKET_ADD_FAILED));
      throw cause;
    } finally {
      setIsAddingItem(false);
    }
  };

  return {
    basket: basketQuery.data as Basket | null,
    isLoading: basketQuery.isLoading,
    error: actionError ?? basketQuery.error ?? null,
    isAddingItem,
    addItem,
  };
}
