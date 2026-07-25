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

export interface UserAddress {
  id: string;
  title: string;
  street: string;
  district: string;
  city: string;
  postalCode: string;
  country: string;
}
