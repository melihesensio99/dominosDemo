import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { basketService } from '../services';
import type { Basket } from '../types/basket';

export function useBasket(customerId?: string | null) {
  const queryClient = useQueryClient();
  const [isAddingItem, setIsAddingItem] = useState(false);
  const [isUpdatingItem, setIsUpdatingItem] = useState(false);
  const [actionError, setActionError] = useState<Error | null>(null);

  const basketQuery = useQuery({
    queryKey: ['basket', customerId],
    enabled: Boolean(customerId),
    queryFn: async () => {
      if (!customerId) {
        return null;
      }

      return basketService.getBasket();
    },
  });

  const addItem = async (productId: string, selectedOptionIds: string[] = [], quantity = 1) => {
    if (!customerId) {
      return;
    }

    try {
      setIsAddingItem(true);
      setActionError(null);
      await basketService.addItem({ productId, quantity, selectedOptionIds });
      await queryClient.invalidateQueries({ queryKey: ['basket'] });
    } catch (cause) {
      setActionError(cause instanceof Error ? cause : new Error('Sepete eklenemedi.'));
      throw cause;
    } finally {
      setIsAddingItem(false);
    }
  };

  const updateItem = async (itemId: string, quantity: number) => {
    try {
      setIsUpdatingItem(true);
      setActionError(null);

      if (quantity <= 0) {
        await basketService.removeItem(itemId);
      } else {
        await basketService.updateItem(itemId, { quantity });
      }

      await queryClient.invalidateQueries({ queryKey: ['basket'] });
    } catch (cause) {
      setActionError(cause instanceof Error ? cause : new Error('Sepet guncellenemedi.'));
      throw cause;
    } finally {
      setIsUpdatingItem(false);
    }
  };

  const removeItem = async (itemId: string) => {
    try {
      setIsUpdatingItem(true);
      setActionError(null);
      await basketService.removeItem(itemId);
      await queryClient.invalidateQueries({ queryKey: ['basket'] });
    } catch (cause) {
      setActionError(cause instanceof Error ? cause : new Error('Urun sepetten silinemedi.'));
      throw cause;
    } finally {
      setIsUpdatingItem(false);
    }
  };

  return {
    basket: basketQuery.data as Basket | null,
    isLoading: basketQuery.isLoading,
    error: actionError ?? basketQuery.error ?? null,
    isAddingItem,
    isUpdatingItem,
    addItem,
    updateItem,
    removeItem,
  };
}
