import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import type { ReactNode } from 'react';
import type { SessionUser } from './types';

const storageKey = 'opsflow.session';

interface SessionContextValue {
  user: SessionUser | null;
  setUser: (user: SessionUser | null) => void;
  clearUser: () => void;
}

const SessionContext = createContext<SessionContextValue | undefined>(undefined);

export function SessionProvider({ children }: { children: ReactNode }) {
  const [user, setUserState] = useState<SessionUser | null>(null);

  useEffect(() => {
    const raw = window.localStorage.getItem(storageKey);
    if (!raw) {
      return;
    }

    try {
      setUserState(JSON.parse(raw) as SessionUser);
    } catch {
      window.localStorage.removeItem(storageKey);
    }
  }, []);

  const value = useMemo<SessionContextValue>(() => ({
    user,
    setUser: (nextUser) => {
      setUserState(nextUser);
      if (nextUser) {
        window.localStorage.setItem(storageKey, JSON.stringify(nextUser));
      } else {
        window.localStorage.removeItem(storageKey);
      }
    },
    clearUser: () => {
      setUserState(null);
      window.localStorage.removeItem(storageKey);
    },
  }), [user]);

  return <SessionContext.Provider value={value}>{children}</SessionContext.Provider>;
}

export function useSession() {
  const context = useContext(SessionContext);
  if (!context) {
    throw new Error('useSession must be used inside SessionProvider');
  }

  return context;
}
