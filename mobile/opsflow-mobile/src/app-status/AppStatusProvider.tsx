import {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
  type ReactNode,
} from 'react';

type AppStatusBanner = {
  variant: 'error' | 'success';
  message: string;
};

type AppStatusContextValue = {
  loadingMessage: string | null;
  banner: AppStatusBanner | null;
  showLoading: (message?: string) => void;
  hideLoading: () => void;
  showSuccess: (message: string, autoHideMs?: number) => void;
  showError: (message: string, autoHideMs?: number) => void;
  clearBanner: () => void;
};

const AppStatusContext = createContext<AppStatusContextValue | null>(null);

interface AppStatusProviderProps {
  children: ReactNode;
}

export function AppStatusProvider({ children }: AppStatusProviderProps) {
  const [loadingMessage, setLoadingMessage] = useState<string | null>(null);
  const [banner, setBanner] = useState<AppStatusBanner | null>(null);
  const bannerTimerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const clearTimer = () => {
    if (bannerTimerRef.current) {
      clearTimeout(bannerTimerRef.current);
      bannerTimerRef.current = null;
    }
  };

  useEffect(() => {
    return () => {
      clearTimer();
    };
  }, []);

  const showLoading = (message = 'İşlem sürüyor...') => {
    clearTimer();
    setBanner(null);
    setLoadingMessage(message);
  };

  const hideLoading = () => {
    setLoadingMessage(null);
  };

  const clearBanner = () => {
    clearTimer();
    setBanner(null);
  };

  const showBanner = (variant: AppStatusBanner['variant'], message: string, autoHideMs = 3500) => {
    clearTimer();
    setLoadingMessage(null);
    setBanner({ variant, message });
    bannerTimerRef.current = setTimeout(() => {
      setBanner(null);
      bannerTimerRef.current = null;
    }, autoHideMs);
  };

  const value = useMemo<AppStatusContextValue>(
    () => ({
      loadingMessage,
      banner,
      showLoading,
      hideLoading,
      showSuccess: (message, autoHideMs) => showBanner('success', message, autoHideMs),
      showError: (message, autoHideMs) => showBanner('error', message, autoHideMs),
      clearBanner,
    }),
    [banner, loadingMessage],
  );

  return <AppStatusContext.Provider value={value}>{children}</AppStatusContext.Provider>;
}

export function useAppStatus() {
  const context = useContext(AppStatusContext);

  if (!context) {
    throw new Error('useAppStatus must be used within an AppStatusProvider.');
  }

  return context;
}
