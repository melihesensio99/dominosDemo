import { useQuery, useQueryClient } from '@tanstack/react-query';
import { HubConnectionState } from '@microsoft/signalr';
import { useEffect, useMemo, useRef, useState } from 'react';
import { useAppStatus } from '../app-status';
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
  const { showError, showSuccess } = useAppStatus();
  const statusFeedbackRef = useRef({ showError });
  statusFeedbackRef.current = { showError };
  const [isPlacingOrder, setIsPlacingOrder] = useState(false);
  const [cancellingOrderId, setCancellingOrderId] = useState<string | null>(null);
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

    let isDisposed = false;
    let retryTimer: ReturnType<typeof setTimeout> | undefined;

    const refreshOrders = () => queryClient.invalidateQueries({
      queryKey: ['orders', customerId],
    });

    const connection = createNotificationTrackingConnection((notification) => {
      const normalizedStatus = notification.status.toLowerCase();

      if (normalizedStatus === 'cancelled') {
        statusFeedbackRef.current.showError(
          'Siparişiniz iptal edildi. Rezerve edilen ürünler stoğa geri bırakıldı.',
          8000,
        );
      }

      queryClient.setQueryData<Order[]>(['orders', customerId], (orders) =>
        orders?.map((order) =>
          order.id === notification.orderId
            ? {
                ...order,
                status: normalizedStatus,
                updatedAt: notification.updatedAt,
              }
            : order,
        ),
      );

      void refreshOrders();
    });

    const startConnection = async (): Promise<void> => {
      if (isDisposed || connection.state !== HubConnectionState.Disconnected) {
        return;
      }

      try {
        await connection.start();
      } catch {
        if (!isDisposed) {
          retryTimer = setTimeout(() => void startConnection(), 5000);
        }
      }
    };

    connection.onreconnected(() => {
      void refreshOrders();
    });
    connection.onclose(() => {
      if (!isDisposed) {
        retryTimer = setTimeout(() => void startConnection(), 5000);
      }
    });

    void startConnection();

    return () => {
      isDisposed = true;
      if (retryTimer) {
        clearTimeout(retryTimer);
      }
      void connection.stop();
    };
  }, [customerId, queryClient]);

  const { activeOrders, latestDeliveredOrder, visibleOrders } = useMemo(() => {
    const orders = [...(ordersQuery.data ?? [])]
      .sort((left, right) => Date.parse(right.createdAt) - Date.parse(left.createdAt));
    const active = orders.filter((order) =>
      ['pending', 'confirmed', 'preparing', 'shipped'].includes(order.status.toLowerCase()),
    );
    const latestDelivered = orders.find((order) => order.status.toLowerCase() === 'delivered') ?? null;
    const latestFinished = orders.find((order) =>
      ['delivered', 'cancelled'].includes(order.status.toLowerCase()),
    ) ?? null;

    return {
      activeOrders: active,
      latestDeliveredOrder: latestDelivered,
      visibleOrders: active.length > 0 ? active : latestFinished ? [latestFinished] : [],
    };
  }, [ordersQuery.data]);

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

  const cancelOrder = async (orderId: string) => {
    try {
      setCancellingOrderId(orderId);
      setActionError(null);
      await orderService.cancelOrder(orderId);
      await queryClient.invalidateQueries({ queryKey: ['orders', customerId] });
      showSuccess('Siparişiniz iptal edildi.');
    } catch (cause) {
      const error = cause instanceof Error ? cause : new Error('Sipariş iptal edilemedi.');
      setActionError(error);
      showError(error.message);
    } finally {
      setCancellingOrderId(null);
    }
  };

  return {
    orders: ordersQuery.data ?? [],
    activeOrders,
    latestDeliveredOrder,
    visibleOrders,
    isLoading: ordersQuery.isLoading,
    error: actionError ?? ordersQuery.error ?? null,
    isPlacingOrder,
    cancellingOrderId,
    placeOrder,
    cancelOrder,
  };
}
