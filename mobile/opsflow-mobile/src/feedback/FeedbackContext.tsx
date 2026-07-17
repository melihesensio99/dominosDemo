import { createContext, useContext, useEffect, useMemo, useRef, useState, type ReactNode } from 'react';

type FeedbackBanner = {
  variant: 'error' | 'success';
  message: string;
};

type FeedbackContextValue = {
  loadingMessage: string | null;
  banner: FeedbackBanner | null;
  showLoading: (message?: string) => void;
  hideLoading: () => void;
  showSuccess: (message: string, autoHideMs?: number) => void;
  showError: (message: string, autoHideMs?: number) => void;
  clearBanner: () => void;
};

const FeedbackContext = createContext<FeedbackContextValue | null>(null);

interface FeedbackProviderProps {
  children: ReactNode;
}

export function FeedbackProvider({ children }: FeedbackProviderProps) {
  const [loadingMessage, setLoadingMessage] = useState<string | null>(null);
  const [banner, setBanner] = useState<FeedbackBanner | null>(null);
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

  const showBanner = (variant: FeedbackBanner['variant'], message: string, autoHideMs = 3500) => {
    clearTimer();
    setLoadingMessage(null);
    setBanner({ variant, message });
    bannerTimerRef.current = setTimeout(() => {
      setBanner(null);
      bannerTimerRef.current = null;
    }, autoHideMs);
  };

  const value = useMemo<FeedbackContextValue>(
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

  return <FeedbackContext.Provider value={value}>{children}</FeedbackContext.Provider>;
}

export function useFeedback() {
  const context = useContext(FeedbackContext);

  if (!context) {
    throw new Error('useFeedback must be used within a FeedbackProvider.');
  }

  return context;
}
