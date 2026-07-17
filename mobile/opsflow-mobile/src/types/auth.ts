export interface SessionUser {
  userId: string;
  email: string;
  role: string;
  accessToken: string;
}

export interface AuthCredentials {
  email: string;
  password: string;
}
