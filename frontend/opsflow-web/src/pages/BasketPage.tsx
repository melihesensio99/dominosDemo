import { useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { basketApi, catalogApi, orderApi } from '../lib/api';
import { useSession } from '../lib/session';
import type { Address } from '../lib/types';

const defaultAddress: Address = {
  street: 'Ataturk Street 12',
  district: 'Kadikoy',
  city: 'Istanbul',
  postalCode: '34710',
  country: 'Turkey',
};

export default function BasketPage() {
  const { user } = useSession();
  const queryClient = useQueryClient();
  const [address, setAddress] = useState<Address>(defaultAddress);
  const [paymentMethod, setPaymentMethod] = useState(0);

  const basketQuery = useQuery({
    queryKey: ['basket', user?.userId],
    queryFn: () => basketApi.getBasket(user?.userId ?? ''),
    enabled: Boolean(user?.userId),
  });

  const productsQuery = useQuery({
    queryKey: ['catalog', 'products'],
    queryFn: catalogApi.getProducts,
  });

  const orderMutation = useMutation({
    mutationFn: async () => {
      if (!user || !basketQuery.data?.items.length) {
        throw new Error('Basket is empty.');
      }

      return orderApi.createOrder({
        customerId: user.userId,
        items: basketQuery.data.items.map((item) => ({
          productId: item.productId,
          quantity: item.quantity,
        })),
        shippingAddress: address,
        billingAddress: address,
        paymentMethod,
      });
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['orders'] });
      await queryClient.invalidateQueries({ queryKey: ['notifications'] });
      await queryClient.invalidateQueries({ queryKey: ['basket'] });
    },
  });

  const basket = basketQuery.data;
  const total = useMemo(() => {
    if (!basket?.items.length) {
      return 0;
    }

    return basket.items.reduce((sum, item) => {
      const product = productsQuery.data?.find((candidate) => candidate.id === item.productId);
      return sum + (product?.price ?? 0) * item.quantity;
    }, 0);
  }, [basket?.items, productsQuery.data]);

  return (
    <div className="space-y-6 pb-24 xl:pb-0">
      <div>
        <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Basket</div>
        <h2 className="mt-2 text-2xl font-semibold text-white">Checkout flow</h2>
        <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-300">
          Basket stays in Redis and the order request can be built directly from the items in the cart.
        </p>
      </div>

      {!user ? (
        <div className="rounded-[24px] border border-white/10 bg-slate-950/30 p-6 text-sm text-slate-300">
          Sign in first to see the basket and place an order.
        </div>
      ) : (
        <div className="grid gap-4 xl:grid-cols-[1.4fr_0.9fr]">
          <section className="rounded-[24px] border border-white/10 bg-slate-950/30 p-5">
            <div className="flex items-center justify-between">
              <div>
                <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Items</div>
                <div className="mt-2 text-lg font-semibold text-white">{basket?.itemCount ?? 0} products</div>
              </div>
              <div className="text-right">
                <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Total</div>
                <div className="mt-2 text-2xl font-semibold text-white">{total.toLocaleString('tr-TR')} TL</div>
              </div>
            </div>

            <div className="mt-5 space-y-3">
              {(basket?.items ?? []).map((item) => {
                const product = productsQuery.data?.find((candidate) => candidate.id === item.productId);
                return (
                  <div key={item.productId} className="rounded-2xl border border-white/10 bg-white/5 p-4">
                    <div className="flex items-center justify-between gap-3">
                      <div>
                        <div className="font-medium text-white">{product?.name ?? item.productId}</div>
                        <div className="mt-1 text-sm text-slate-400">Quantity: {item.quantity}</div>
                      </div>
                      <div className="text-sm font-semibold text-canvas-100">
                        {((product?.price ?? 0) * item.quantity).toLocaleString('tr-TR')} TL
                      </div>
                    </div>
                  </div>
                );
              })}

              {!basket?.items.length && (
                <div className="rounded-2xl border border-dashed border-white/15 bg-white/5 p-6 text-sm text-slate-400">
                  Basket is empty. Add something from the menu to continue.
                </div>
              )}
            </div>
          </section>

          <aside className="rounded-[24px] border border-white/10 bg-slate-950/30 p-5">
            <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Checkout</div>
            <div className="mt-4 space-y-3">
              {[
                ['street', 'Street'],
                ['district', 'District'],
                ['city', 'City'],
                ['postalCode', 'Postal code'],
                ['country', 'Country'],
              ].map(([key, label]) => (
                <label key={key} className="block">
                  <span className="mb-1 block text-xs uppercase tracking-[0.3em] text-slate-500">{label}</span>
                  <input
                    className="w-full rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-sm text-white outline-none transition placeholder:text-slate-500 focus:border-canvas-400/60"
                    value={address[key as keyof Address]}
                    onChange={(event) =>
                      setAddress((current) => ({ ...current, [key]: event.target.value }))
                    }
                  />
                </label>
              ))}

              <label className="block">
                <span className="mb-1 block text-xs uppercase tracking-[0.3em] text-slate-500">
                  Payment method
                </span>
                <select
                  className="w-full rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-sm text-white outline-none"
                  value={paymentMethod}
                  onChange={(event) => setPaymentMethod(Number(event.target.value))}
                >
                  <option value={0}>Card</option>
                  <option value={1}>Bank transfer</option>
                  <option value={2}>Cash on delivery</option>
                </select>
              </label>

              <button
                type="button"
                disabled={!basket?.items.length || orderMutation.isPending}
                onClick={() => orderMutation.mutate()}
                className="w-full rounded-2xl bg-canvas-500 px-4 py-3 text-sm font-semibold text-white transition hover:bg-canvas-400 disabled:cursor-not-allowed disabled:bg-white/10"
              >
                Place order
              </button>

              <div className="rounded-2xl border border-white/10 bg-white/5 p-4 text-sm text-slate-300">
                The order call will go through the gateway and can trigger the notification feed.
              </div>
            </div>
          </aside>
        </div>
      )}
    </div>
  );
}
