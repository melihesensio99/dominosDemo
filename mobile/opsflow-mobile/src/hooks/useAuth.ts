import { useState } from 'react';
import { useEffect } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import { authService } from '../services';
import { tokenService } from '../services/token.service';
import type { AuthCredentials, RegisterCredentials, SessionUser } from '../types/auth';

export function useAuth() {
  const queryClient = useQueryClient();
  const [user, setUser] = useState<SessionUser | null>(null);
  const [mode, setMode] = useState<'login' | 'register'>('login');
  const [email, setEmail] = useState('admin@opsflow.ai');
  const [password, setPassword] = useState('P@ssw0rd123');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isSigningIn, setIsSigningIn] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    return tokenService.subscribe((token) => {
      if (!token) {
        setUser(null);
        setError(null);
        queryClient.clear();
      }
    });
  }, [queryClient]);

  const signIn = async () => {
    try {
      setIsSigningIn(true);
      setError(null);

      const loginPayload: AuthCredentials = { email, password };
      const registerPayload: RegisterCredentials = { email, password, confirmPassword };
      const session = mode === 'login'
        ? await authService.login(loginPayload)
        : await authService.register(registerPayload);
      setUser(session);
      tokenService.setAccessToken(session.accessToken);
      return session;
    } catch (cause) {
      const message = cause instanceof Error ? cause.message : 'İşlem başarısız.';
      setError(new Error(message));
      throw cause;
    } finally {
      setIsSigningIn(false);
    }
  };

  const signOut = () => {
    setUser(null);
    tokenService.clearAccessToken();
    queryClient.clear();
    setError(null);
  };

  return {
    user,
    mode,
    email,
    password,
    confirmPassword,
    setMode,
    setEmail,
    setPassword,
    setConfirmPassword,
    isSigningIn,
    error,
    signIn,
    signOut,
  };
}
