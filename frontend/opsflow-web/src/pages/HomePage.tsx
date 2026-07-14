import { useQuery } from '@tanstack/react-query';
import { catalogApi, notificationApi, orderApi } from '../lib/api';
import { useSession } from '../lib/session';
import { useRealtimeBridge } from '../lib/realtime';

const fallbackHighlights = [
  { title: 'Fast basket flow', value: 'Redis-backed' },
  { title: 'Stock awareness', value: 'gRPC lookup' },
  { title: 'Order lifecycle', value: 'Outbox + RabbitMQ' },
  { title: 'Notification feed', value: 'MongoDB' },
];

export default function HomePage() {
  const { user } = useSession();
  const liveState = useRealtimeBridge(Boolean(user));

  const productsQuery = useQuery({
    queryKey: ['catalog', 'products'],
    queryFn: catalogApi.getProducts,
  });

  const ordersQuery = useQuery({
    queryKey: ['orders'],
    queryFn: orderApi.getOrders,
  });

  const notificationsQuery = useQuery({
    queryKey: ['notifications'],
    queryFn: notificationApi.getNotifications,
  });

  const productCount = productsQuery.data?.length ?? 0;
  const orderCount = ordersQuery.data?.length ?? 0;
  const notificationCount = notificationsQuery.data?.items?.length ?? 0;

  return (
    <div className="space-y-6 pb-24 xl:pb-0">
      <div className="grid gap-4 lg:grid-cols-[1.5fr_1fr]">
        <div className="rounded-[24px] border border-white/10 bg-gradient-to-br from-canvas-500/20 via-canvas-500/10 to-transparent p-6">
          <div className="inline-flex rounded-full border border-canvas-300/20 bg-canvas-300/10 px-3 py-1 text-xs uppercase tracking-[0.35em] text-canvas-100">
            Demo dashboard
          </div>
          <h2 className="mt-4 max-w-2xl text-3xl font-semibold leading-tight text-white">
            Food-delivery style storefront, built to showcase the backend from browser to broker.
          </h2>
          <p className="mt-4 max-w-3xl text-sm leading-7 text-slate-300 lg:text-base">
            This interface is wired for the existing services and keeps the learning flow visible: auth,
            catalog, basket, order, notification and the gateway between them.
          </p>

          <div className="mt-6 grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
            {[
              { label: 'Products', value: productCount || 'seed data' },
              { label: 'Orders', value: orderCount || 'seed data' },
              { label: 'Notifications', value: notificationCount || 'seed data' },
              { label: 'Realtime', value: liveState },
            ].map((card) => (
              <div key={card.label} className="rounded-2xl border border-white/10 bg-slate-950/35 p-4">
                <div className="text-xs uppercase tracking-[0.35em] text-slate-400">{card.label}</div>
                <div className="mt-2 text-lg font-semibold text-white">{String(card.value)}</div>
              </div>
            ))}
          </div>
        </div>

        <div className="rounded-[24px] border border-white/10 bg-slate-950/35 p-6">
          <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Focus points</div>
          <div className="mt-4 space-y-3">
            {fallbackHighlights.map((item) => (
              <div key={item.title} className="rounded-2xl border border-white/10 bg-white/5 p-4">
                <div className="text-sm font-medium text-white">{item.title}</div>
                <div className="mt-1 text-sm text-slate-400">{item.value}</div>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className="rounded-[24px] border border-white/10 bg-white/5 p-5">
        <div className="text-xs uppercase tracking-[0.35em] text-slate-400">What this demo proves</div>
        <div className="mt-4 grid gap-3 md:grid-cols-2 xl:grid-cols-4">
          {[
            'API gateway gives one browser entrypoint.',
            'TanStack Query keeps server state sane.',
            'Tailwind makes the UI move fast without bloating CSS.',
            'SignalR slot is ready for live order status later.',
          ].map((text) => (
            <div key={text} className="rounded-2xl border border-white/10 bg-slate-950/30 p-4 text-sm leading-6 text-slate-300">
              {text}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
