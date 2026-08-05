import { useQuery, useQueryClient } from '@tanstack/react-query';
import { useEffect, useState } from 'react';
import { basketService, orderService } from '../services';
import { createNotificationTrackingConnection } from '../services/notificationTracking.service';
import type { Order } from '../types/order';
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
    // SignalR is the primary update channel. Polling remains as a recovery
    // path for a dropped connection or a temporarily unavailable hub.
    refetchInterval: 30000,
    queryFn: async () => {
      if (!customerId) {
        return [];
      }

      return orderService.getMyOrders();
    },
  });

  useEffect(() => {
    if (!customerId) {
      return;
    }

    const connection = createNotificationTrackingConnection((notification) => {
      queryClient.setQueryData<Order[]>(['orders', customerId], (orders) =>
        orders?.map((order) =>
          order.id === notification.orderId
            ? {
                ...order,
                status: notification.status,
                updatedAt: notification.updatedAt,
              }
            : order,
        ),
      );

      void queryClient.invalidateQueries({
        queryKey: ['orders', customerId],
      });
    });

    const startConnection = async () => {
      try {
        await connection.start();
      } catch {
        // React Query polling keeps order tracking functional while SignalR
        // reconnects or the local Order API is unavailable.
      }
    };

    void startConnection();

    return () => {
      void connection.stop();
    };
  }, [customerId, queryClient]);

  const activeOrder = ordersQuery.data?.find((order) =>
    ['pending', 'confirmed', 'preparing', 'shipped'].includes(order.status),
  );

  const placeOrder = async ({
    shippingAddress,
    billingAddress,
    paymentMethod,
    note,
  }: {
    shippingAddress: Address;
    billingAddress: Address;
    paymentMethod: number;
    note?: string;
  }) => {
    if (!customerId || !basket?.items.length) {
      return null;
    }

    try {
      setIsPlacingOrder(true);
      setActionError(null);

      const order = await orderService.createOrder({
        items: basket.items.map((item) => ({
          productId: item.productId,
          quantity: item.quantity,
          selectedOptionIds: item.selectedOptions.map((option) => option.optionId),
        })),
        shippingAddress,
        billingAddress,
        paymentMethod,
        note: note?.trim() || undefined,
      });

      // The order is now owned by Order API; remove the checkout basket afterwards.
      await basketService.clearBasket();
      await queryClient.invalidateQueries({ queryKey: ['orders'] });
      await queryClient.invalidateQueries({ queryKey: ['basket'] });
      return order;
    } catch (cause) {
      setActionError(cause instanceof Error ? cause : new Error('Sipariş verilemedi.'));
      throw cause;
    } finally {
      setIsPlacingOrder(false);
    }
  };

  return {
    orders: ordersQuery.data ?? [],
    lastOrder: activeOrder,
    isLoading: ordersQuery.isLoading,
    error: actionError ?? ordersQuery.error ?? null,
    isPlacingOrder,
    placeOrder,
  };
}
