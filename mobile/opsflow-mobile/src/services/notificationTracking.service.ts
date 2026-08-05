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
  updatedAt: string;
}

const hubUrl =
  process.env.EXPO_PUBLIC_NOTIFICATION_HUB_URL ??
  'http://localhost:5044/hubs/notifications';

export function createNotificationTrackingConnection(
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
