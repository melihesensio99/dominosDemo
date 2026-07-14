import { useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { basketApi, catalogApi } from '../lib/api';
import { useSession } from '../lib/session';
import type { Category, Product } from '../lib/types';

const fallbackCategories: Category[] = [
  { id: 'cat-drinks', name: 'Drinks', slug: 'drinks', isActive: true, createdAt: new Date().toISOString() },
  { id: 'cat-bakery', name: 'Bakery', slug: 'bakery', isActive: true, createdAt: new Date().toISOString() },
  { id: 'cat-dinner', name: 'Dinner', slug: 'dinner', isActive: true, createdAt: new Date().toISOString() },
];

const fallbackProducts: Product[] = [
  {
    id: 'p-100',
    name: 'Starter Box',
    description: 'A compact meal bundle for quick demos.',
    price: 100,
    stock: 25,
    categoryId: 'cat-dinner',
    categoryName: 'Dinner',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: null,
  },
  {
    id: 'p-200',
    name: 'Pro Box',
    description: 'A bigger bundle for a proper order journey.',
    price: 250,
    stock: 12,
    categoryId: 'cat-dinner',
    categoryName: 'Dinner',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: null,
  },
  {
    id: 'p-300',
    name: 'Cola',
    description: 'Cold drink for the basket panel.',
    price: 35,
    stock: 60,
    categoryId: 'cat-drinks',
    categoryName: 'Drinks',
    isActive: true,
    createdAt: new Date().toISOString(),
    updatedAt: null,
  },
];

export default function MenuPage() {
  const { user } = useSession();
  const queryClient = useQueryClient();
  const [selectedCategory, setSelectedCategory] = useState<string>('all');

  const categoriesQuery = useQuery({
    queryKey: ['catalog', 'categories'],
    queryFn: catalogApi.getCategories,
    placeholderData: fallbackCategories,
  });

  const productsQuery = useQuery({
    queryKey: ['catalog', 'products'],
    queryFn: catalogApi.getProducts,
    placeholderData: fallbackProducts,
  });

  const addItem = useMutation({
    mutationFn: async ({ productId }: { productId: string }) => {
      if (!user) {
        throw new Error('Please sign in first.');
      }

      return basketApi.addItem(user.userId, { productId, quantity: 1 });
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['basket'] });
    },
  });

  const filteredProducts = useMemo(() => {
    if (selectedCategory === 'all') {
      return productsQuery.data ?? [];
    }

    return (productsQuery.data ?? []).filter((product) => product.categoryId === selectedCategory);
  }, [productsQuery.data, selectedCategory]);

  return (
    <div className="space-y-6 pb-24 xl:pb-0">
      <div className="flex flex-col gap-3">
        <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Catalog</div>
        <h2 className="text-2xl font-semibold text-white">Choose products like a delivery app</h2>
        <p className="max-w-3xl text-sm leading-6 text-slate-300">
          The menu is fetched through TanStack Query and can add items directly into the basket service.
        </p>
      </div>

      <div className="flex flex-wrap gap-2">
        <button
          type="button"
          onClick={() => setSelectedCategory('all')}
          className={`rounded-full px-4 py-2 text-sm font-medium transition ${
            selectedCategory === 'all'
              ? 'bg-canvas-500 text-white'
              : 'bg-white/5 text-slate-300 hover:bg-white/10'
          }`}
        >
          All
        </button>
        {(categoriesQuery.data ?? fallbackCategories).map((category) => (
          <button
            key={category.id}
            type="button"
            onClick={() => setSelectedCategory(category.id)}
            className={`rounded-full px-4 py-2 text-sm font-medium transition ${
              selectedCategory === category.id
                ? 'bg-canvas-500 text-white'
                : 'bg-white/5 text-slate-300 hover:bg-white/10'
            }`}
          >
            {category.name}
          </button>
        ))}
      </div>

      <div className="grid gap-4 md:grid-cols-2 2xl:grid-cols-3">
        {filteredProducts.map((product) => (
          <article
            key={product.id}
            className="overflow-hidden rounded-[24px] border border-white/10 bg-slate-950/30 p-4 transition hover:-translate-y-1 hover:bg-slate-950/45"
          >
            <div className="flex items-start justify-between gap-3">
              <div>
                <div className="text-xs uppercase tracking-[0.35em] text-canvas-200">
                  {product.categoryName ?? 'Menu'}
                </div>
                <h3 className="mt-2 text-lg font-semibold text-white">{product.name}</h3>
              </div>
              <div className="rounded-2xl bg-canvas-500/15 px-3 py-2 text-sm font-semibold text-canvas-100">
                {product.price.toLocaleString('tr-TR')} TL
              </div>
            </div>

            <p className="mt-4 text-sm leading-6 text-slate-300">{product.description}</p>

            <div className="mt-5 flex items-center justify-between text-sm text-slate-400">
              <span>Stock: {product.stock}</span>
              <span>{product.isActive ? 'Active' : 'Disabled'}</span>
            </div>

            <button
              type="button"
              disabled={!user || addItem.isPending}
              onClick={() => addItem.mutate({ productId: product.id })}
              className="mt-4 w-full rounded-2xl bg-canvas-500 px-4 py-3 text-sm font-semibold text-white transition hover:bg-canvas-400 disabled:cursor-not-allowed disabled:bg-white/10"
            >
              {user ? 'Add to basket' : 'Sign in to add'}
            </button>
          </article>
        ))}
      </div>
    </div>
  );
}
