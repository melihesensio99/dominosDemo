import {
  HubConnectionBuilder,
  LogLevel,
  type HubConnection,
} from '@microsoft/signalr';
import { tokenService } from './token.service';

export interface OrderStatusChangedNotification {
  orderId: string;
  customerId: string;
  status: string;
  updatedAt: string | null;
}

const hubUrl =
  process.env.EXPO_PUBLIC_ORDER_HUB_URL ??
  'http://localhost:5093/hubs/orders';

export function createOrderTrackingConnection(
  onStatusChanged: (notification: OrderStatusChangedNotification) => void,
): HubConnection {
  const connection = new HubConnectionBuilder()
    .withUrl(hubUrl, {
      accessTokenFactory: () => tokenService.getAccessToken() ?? '',
    })
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Warning)
    .build();

  connection.on('OrderStatusChanged', onStatusChanged);
  return connection;
}
