type TokenListener = (token: string | null) => void;

let accessToken: string | null = null;
const listeners = new Set<TokenListener>();

function emit() {
  listeners.forEach((listener) => listener(accessToken));
}

export const tokenService = {
  getAccessToken() {
    return accessToken;
  },
  setAccessToken(token: string) {
    accessToken = token;
    emit();
  },
  clearAccessToken() {
    if (accessToken === null) {
      return;
    }

    accessToken = null;
    emit();
  },
  subscribe(listener: TokenListener) {
    listeners.add(listener);
    listener(accessToken);

    return () => {
      listeners.delete(listener);
    };
  },
};
