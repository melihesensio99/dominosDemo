import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { ERROR_MESSAGES } from '../constants/errorMessages';
import { orderService } from '../services';
import type { Address } from '../types/common';
import type { Basket } from '../types/basket';

interface UseOrdersParams {
  customerId?: string | null;
  basket: Basket | null;
}

export function useOrders({ customerId, basket }: UseOrdersParams) {
  const queryClient = useQueryClient();
  const [isPlacingOrder, setIsPlacingOrder] = useState(false);
  const [actionError, setActionError] = useState<Error | null>(null);

  const ordersQuery = useQuery({
    queryKey: ['orders', customerId],
    enabled: Boolean(customerId),
    refetchInterval: 10000,
    queryFn: async () => {
      if (!customerId) {
        return [];
      }

      return orderService.getMyOrders();
    },
  });

  const placeOrder = async ({
    shippingAddress,
    billingAddress,
    paymentMethod,
  }: {
    shippingAddress: Address;
    billingAddress: Address;
    paymentMethod: number;
  }) => {
    if (!customerId || !basket?.items.length) {
      return null;
    }

    try {
      setIsPlacingOrder(true);
      setActionError(null);

      const order = await orderService.createOrder({
        customerId,
        items: basket.items.map((item) => ({
          productId: item.productId,
          quantity: item.quantity,
        })),
        shippingAddress,
        billingAddress,
        paymentMethod,
      });

      await queryClient.invalidateQueries({ queryKey: ['orders'] });
      await queryClient.invalidateQueries({ queryKey: ['basket'] });
      return order;
    } catch (cause) {
      setActionError(cause instanceof Error ? cause : new Error(ERROR_MESSAGES.ORDER_CREATE_FAILED));
      throw cause;
    } finally {
      setIsPlacingOrder(false);
    }
  };

  return {
    orders: ordersQuery.data ?? [],
    lastOrder: ordersQuery.data?.[0],
    isLoading: ordersQuery.isLoading,
    error: actionError ?? ordersQuery.error ?? null,
    isPlacingOrder,
    placeOrder,
  };
}
