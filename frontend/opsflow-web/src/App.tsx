import { NavLink, Navigate, Route, Routes, useNavigate } from 'react-router-dom';
import { useMemo } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { useSession } from './lib/session';
import HomePage from './pages/HomePage';
import MenuPage from './pages/MenuPage';
import BasketPage from './pages/BasketPage';
import OrdersPage from './pages/OrdersPage';
import NotificationsPage from './pages/NotificationsPage';
import AuthPage from './pages/AuthPage';

const navItems = [
  { to: '/', label: 'Dashboard' },
  { to: '/menu', label: 'Menu' },
  { to: '/basket', label: 'Basket' },
  { to: '/orders', label: 'Orders' },
  { to: '/notifications', label: 'Notifications' },
  { to: '/auth', label: 'Auth' },
];

export default function App() {
  const { user, clearUser } = useSession();
  const queryClient = useQueryClient();
  const navigate = useNavigate();

  const initials = useMemo(
    () =>
      user
        ? user.email
            .split('@')[0]
            .split(/[._-]/)
            .map((part) => part[0]?.toUpperCase() ?? '')
            .join('')
            .slice(0, 2)
        : 'OS',
    [user],
  );

  const signOut = () => {
    clearUser();
    queryClient.clear();
    navigate('/');
  };

  return (
    <div className="min-h-screen text-slate-100">
      <div className="mx-auto flex min-h-screen w-full max-w-[1600px] gap-4 px-4 py-4 lg:px-6">
        <aside className="hidden w-72 shrink-0 flex-col rounded-[28px] border border-white/10 bg-white/8 p-5 shadow-glow backdrop-blur xl:flex">
          <div className="rounded-[24px] border border-white/10 bg-slate-950/40 p-4">
            <div className="text-xs uppercase tracking-[0.4em] text-canvas-300">OpsFlow</div>
            <div className="mt-2 text-2xl font-semibold">Market Control</div>
            <p className="mt-2 text-sm leading-6 text-slate-300">
              Yemeksepeti ve Getir hissi veren, mikroservisleri canlı gösteren demo paneli.
            </p>
          </div>

          <nav className="mt-6 space-y-2">
            {navItems.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) =>
                  `block rounded-2xl px-4 py-3 text-sm font-medium transition ${
                    isActive
                      ? 'bg-canvas-500/20 text-white ring-1 ring-canvas-400/30'
                      : 'text-slate-300 hover:bg-white/5 hover:text-white'
                  }`
                }
              >
                {item.label}
              </NavLink>
            ))}
          </nav>

          <div className="mt-auto rounded-[24px] border border-white/10 bg-slate-950/40 p-4">
            <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Current user</div>
            {user ? (
              <>
                <div className="mt-3 flex items-center gap-3">
                  <div className="grid h-12 w-12 place-items-center rounded-2xl bg-canvas-500/20 text-sm font-semibold text-canvas-100">
                    {initials}
                  </div>
                  <div>
                    <div className="font-medium">{user.email}</div>
                    <div className="text-sm text-slate-400">{user.role}</div>
                  </div>
                </div>
                <button
                  type="button"
                  onClick={signOut}
                  className="mt-4 w-full rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-sm font-medium text-slate-200 transition hover:bg-white/10"
                >
                  Sign out
                </button>
              </>
            ) : (
              <button
                type="button"
                onClick={() => navigate('/auth')}
                className="mt-4 w-full rounded-2xl bg-canvas-500 px-4 py-3 text-sm font-semibold text-white transition hover:bg-canvas-400"
              >
                Sign in
              </button>
            )}
          </div>
        </aside>

        <main className="min-w-0 flex-1">
          <header className="mb-4 rounded-[28px] border border-white/10 bg-white/8 p-4 shadow-glow backdrop-blur lg:p-5">
            <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
              <div>
                <div className="text-xs uppercase tracking-[0.4em] text-canvas-300">OpsFlow Demo</div>
                <h1 className="mt-2 text-2xl font-semibold text-white lg:text-3xl">
                  Getir-style storefront with microservice-backed workflows
                </h1>
                <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-300 lg:text-base">
                  React, Vite, Tailwind, TanStack Query and SignalR-ready scaffolding connected to the
                  existing backend services through the gateway.
                </p>
              </div>

              <div className="grid gap-3 sm:grid-cols-2">
                <div className="rounded-2xl border border-white/10 bg-slate-950/30 px-4 py-3">
                  <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Gateway</div>
                  <div className="mt-1 text-sm font-medium text-white">localhost:8000</div>
                </div>
                <div className="rounded-2xl border border-white/10 bg-slate-950/30 px-4 py-3">
                  <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Frontend</div>
                  <div className="mt-1 text-sm font-medium text-white">localhost:5173</div>
                </div>
              </div>
            </div>
          </header>

          <div className="grid gap-4 xl:grid-cols-[1fr_320px]">
            <section className="min-w-0 rounded-[28px] border border-white/10 bg-white/8 p-4 shadow-glow backdrop-blur lg:p-6">
              <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/menu" element={<MenuPage />} />
                <Route path="/basket" element={<BasketPage />} />
                <Route path="/orders" element={<OrdersPage />} />
                <Route path="/notifications" element={<NotificationsPage />} />
                <Route path="/auth" element={<AuthPage />} />
                <Route path="*" element={<Navigate to="/" replace />} />
              </Routes>
            </section>

            <aside className="space-y-4">
              <div className="rounded-[28px] border border-white/10 bg-white/8 p-5 shadow-glow backdrop-blur">
                <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Learning stack</div>
                <div className="mt-3 space-y-2 text-sm text-slate-200">
                  <div>• Vite + React for a light storefront UI</div>
                  <div>• Tailwind for fast mobile-first layout</div>
                  <div>• TanStack Query for cached server state</div>
                  <div>• SignalR-ready channel for live status later</div>
                </div>
              </div>

              <div className="rounded-[28px] border border-canvas-400/20 bg-canvas-500/10 p-5 shadow-glow">
                <div className="text-xs uppercase tracking-[0.35em] text-canvas-200">Demo idea</div>
                <p className="mt-3 text-sm leading-6 text-canvas-50">
                  Login, browse catalog, push items into basket, place order and watch notifications as the
                  backend grows.
                </p>
              </div>
            </aside>
          </div>
        </main>
      </div>

      <nav className="fixed inset-x-0 bottom-0 z-10 border-t border-white/10 bg-slate-950/85 px-3 py-2 backdrop-blur xl:hidden">
        <div className="mx-auto grid max-w-5xl grid-cols-3 gap-2 sm:grid-cols-6">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                `rounded-2xl px-2 py-2 text-center text-xs font-medium transition ${
                  isActive
                    ? 'bg-canvas-500/20 text-white'
                    : 'text-slate-400 hover:bg-white/5 hover:text-white'
                }`
              }
            >
              {item.label}
            </NavLink>
          ))}
        </div>
      </nav>
    </div>
  );
}
