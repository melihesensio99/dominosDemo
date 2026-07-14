import { useQuery } from '@tanstack/react-query';
import { orderApi } from '../lib/api';

const statusTone: Record<string, string> = {
  Pending: 'bg-amber-500/15 text-amber-200 ring-1 ring-amber-400/20',
  Completed: 'bg-emerald-500/15 text-emerald-200 ring-1 ring-emerald-400/20',
  Cancelled: 'bg-rose-500/15 text-rose-200 ring-1 ring-rose-400/20',
};

export default function OrdersPage() {
  const ordersQuery = useQuery({
    queryKey: ['orders'],
    queryFn: orderApi.getOrders,
    refetchInterval: 10000,
  });

  return (
    <div className="space-y-6 pb-24 xl:pb-0">
      <div>
        <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Orders</div>
        <h2 className="mt-2 text-2xl font-semibold text-white">Order timeline</h2>
        <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-300">
          Orders are read through TanStack Query, and later this page can switch to SignalR for live status.
        </p>
      </div>

      <div className="grid gap-4">
        {(ordersQuery.data ?? []).map((order) => (
          <article key={order.id} className="rounded-[24px] border border-white/10 bg-slate-950/30 p-5">
            <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
              <div>
                <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Order #{order.id.slice(0, 8)}</div>
                <div className="mt-2 text-lg font-semibold text-white">{order.items.length} items</div>
                <div className="mt-2 text-sm text-slate-400">Customer: {order.customerId}</div>
              </div>

              <div className={`rounded-2xl px-4 py-2 text-sm font-semibold ${statusTone[order.status] ?? 'bg-white/10 text-white'}`}>
                {order.status}
              </div>
            </div>

            <div className="mt-5 grid gap-3 md:grid-cols-2 xl:grid-cols-4">
              <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
                <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Payment</div>
                <div className="mt-2 text-sm text-white">
                  Method {order.payment.method} / {order.payment.status}
                </div>
              </div>
              <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
                <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Shipping</div>
                <div className="mt-2 text-sm text-white">
                  {order.shippingAddress.city}, {order.shippingAddress.district}
                </div>
              </div>
              <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
                <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Billing</div>
                <div className="mt-2 text-sm text-white">
                  {order.billingAddress.city}, {order.billingAddress.district}
                </div>
              </div>
              <div className="rounded-2xl border border-white/10 bg-white/5 p-4">
                <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Created</div>
                <div className="mt-2 text-sm text-white">
                  {new Date(order.createdAt).toLocaleString('tr-TR')}
                </div>
              </div>
            </div>

            <div className="mt-5 flex flex-wrap gap-2">
              {order.items.map((item) => (
                <span key={`${order.id}-${item.productId}`} className="rounded-full bg-canvas-500/15 px-3 py-1 text-xs text-canvas-100">
                  {item.productId} x{item.quantity}
                </span>
              ))}
            </div>
          </article>
        ))}

        {!ordersQuery.data?.length && (
          <div className="rounded-[24px] border border-dashed border-white/15 bg-white/5 p-6 text-sm text-slate-400">
            No orders yet. Create one from the basket page.
          </div>
        )}
      </div>
    </div>
  );
}
