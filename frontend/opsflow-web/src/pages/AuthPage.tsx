import { useMutation } from '@tanstack/react-query';
import { useState } from 'react';
import { authApi } from '../lib/api';
import { useSession } from '../lib/session';

export default function AuthPage() {
  const { user, setUser, clearUser } = useSession();
  const [mode, setMode] = useState<'login' | 'register'>('login');
  const [email, setEmail] = useState('admin@opsflow.ai');
  const [password, setPassword] = useState('P@ssw0rd123');
  const [message, setMessage] = useState<string | null>(null);

  const authMutation = useMutation({
    mutationFn: async () => {
      const payload = { email, password };
      return mode === 'login' ? authApi.login(payload) : authApi.register(payload);
    },
    onSuccess: (session) => {
      setUser(session);
      setMessage(`${mode === 'login' ? 'Login' : 'Registration'} completed for ${session.email}.`);
    },
    onError: (error) => {
      setMessage(error instanceof Error ? error.message : 'Authentication failed.');
    },
  });

  return (
    <div className="space-y-6 pb-24 xl:pb-0">
      <div>
        <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Auth</div>
        <h2 className="mt-2 text-2xl font-semibold text-white">Login and register panel</h2>
        <p className="mt-2 max-w-3xl text-sm leading-6 text-slate-300">
          A simple session store keeps the signed-in user around so basket and orders can use the same customer id.
        </p>
      </div>

      <div className="grid gap-4 xl:grid-cols-[1fr_0.85fr]">
        <section className="rounded-[24px] border border-white/10 bg-slate-950/30 p-5">
          <div className="flex gap-2">
            <button
              type="button"
              onClick={() => setMode('login')}
              className={`rounded-full px-4 py-2 text-sm font-medium transition ${
                mode === 'login' ? 'bg-canvas-500 text-white' : 'bg-white/5 text-slate-300'
              }`}
            >
              Login
            </button>
            <button
              type="button"
              onClick={() => setMode('register')}
              className={`rounded-full px-4 py-2 text-sm font-medium transition ${
                mode === 'register' ? 'bg-canvas-500 text-white' : 'bg-white/5 text-slate-300'
              }`}
            >
              Register
            </button>
          </div>

          <div className="mt-5 space-y-4">
            <label className="block">
              <span className="mb-1 block text-xs uppercase tracking-[0.3em] text-slate-500">Email</span>
              <input
                className="w-full rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-sm text-white outline-none focus:border-canvas-400/60"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
              />
            </label>
            <label className="block">
              <span className="mb-1 block text-xs uppercase tracking-[0.3em] text-slate-500">Password</span>
              <input
                type="password"
                className="w-full rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-sm text-white outline-none focus:border-canvas-400/60"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
              />
            </label>

            <button
              type="button"
              onClick={() => authMutation.mutate()}
              className="w-full rounded-2xl bg-canvas-500 px-4 py-3 text-sm font-semibold text-white transition hover:bg-canvas-400"
            >
              {authMutation.isPending ? 'Working...' : mode === 'login' ? 'Sign in' : 'Create account'}
            </button>

            {message && (
              <div className="rounded-2xl border border-white/10 bg-white/5 p-4 text-sm text-slate-300">
                {message}
              </div>
            )}
          </div>
        </section>

        <aside className="space-y-4">
          <div className="rounded-[24px] border border-white/10 bg-slate-950/30 p-5">
            <div className="text-xs uppercase tracking-[0.35em] text-slate-400">Current session</div>
            {user ? (
              <div className="mt-4 space-y-2 text-sm text-slate-300">
                <div>Email: {user.email}</div>
                <div>Role: {user.role}</div>
                <div>User id: {user.userId}</div>
                <button
                  type="button"
                  onClick={() => clearUser()}
                  className="mt-3 rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-sm font-medium text-slate-200 transition hover:bg-white/10"
                >
                  Clear session
                </button>
              </div>
            ) : (
              <div className="mt-4 text-sm text-slate-400">No user signed in yet.</div>
            )}
          </div>

          <div className="rounded-[24px] border border-canvas-400/20 bg-canvas-500/10 p-5 text-sm leading-6 text-canvas-50">
            Use the default admin account from the backend seed to move faster during demos.
          </div>
        </aside>
      </div>
    </div>
  );
}
