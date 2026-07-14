import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { useEffect, useState } from 'react';

type RealtimeState = 'idle' | 'connecting' | 'connected' | 'fallback';

const signalRUrl = import.meta.env.VITE_SIGNALR_URL ?? '/signalr/live';

export function useRealtimeBridge(enabled: boolean) {
  const [state, setState] = useState<RealtimeState>(enabled ? 'connecting' : 'idle');

  useEffect(() => {
    if (!enabled) {
      setState('idle');
      return;
    }

    let stopped = false;
    const connection = new HubConnectionBuilder()
      .withUrl(signalRUrl)
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    connection
      .start()
      .then(() => {
        if (!stopped) {
          setState('connected');
        }
      })
      .catch(() => {
        if (!stopped) {
          setState('fallback');
        }
      });

    return () => {
      stopped = true;
      void connection.stop();
    };
  }, [enabled]);

  return state;
}
