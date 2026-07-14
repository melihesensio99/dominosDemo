import { useQuery } from '@tanstack/react-query';
import { notificationApi } from '../lib/api';
import { useSession } from '../lib/session';

export default function NotificationsPage() {
  const { user } = useSession();

  const notificationsQuery = useQuery({
    queryKey: ['notifications'],
    queryFn: notificationApi.getNotifications,
    refetchInterval: 8000,
  });

  return (
    <div className="space-y-6 pb-24 xl:pb-0">
      <div>
        <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Notifications</div>
        <h2 className="mt-2 text-2xl font-semibold text-white">Event inbox</h2>
        <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-300">
          This page reads the MongoDB-backed notification list and is ready to switch to SignalR push later.
        </p>
      </div>

      <div className="grid gap-4 xl:grid-cols-[0.9fr_1.1fr]">
        <div className="rounded-[24px] border border-white/10 bg-slate-950/30 p-5">
          <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Live status</div>
          <div className="mt-3 text-sm leading-6 text-slate-300">
            {user
              ? 'You are signed in, so future SignalR updates can be attached to this customer feed.'
              : 'Sign in to bind the feed to a customer and see order events in a personalized lane.'}
          </div>
          <div className="mt-4 rounded-2xl border border-white/10 bg-white/5 p-4 text-sm text-slate-300">
            Current source: polling + TanStack Query cache
          </div>
        </div>

        <div className="space-y-3">
          {(notificationsQuery.data?.items ?? []).map((notification) => (
            <article key={notification.id} className="rounded-[24px] border border-white/10 bg-slate-950/30 p-5">
              <div className="flex items-start justify-between gap-3">
                <div>
                  <div className="text-xs uppercase tracking-[0.35em] text-slate-400">
                    {notification.status}
                  </div>
                  <p className="mt-3 text-sm leading-6 text-white">{notification.message}</p>
                </div>
                <div className="text-xs text-slate-500">
                  {new Date(notification.createdAt).toLocaleString('tr-TR')}
                </div>
              </div>
            </article>
          ))}

          {!notificationsQuery.data?.items.length && (
            <div className="rounded-[24px] border border-dashed border-white/15 bg-white/5 p-6 text-sm text-slate-400">
              No notifications yet. Inventory and order events will appear here.
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
