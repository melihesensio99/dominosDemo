import { ROUTES } from '../constants/routes';
import { useAppStatus } from '../app-status';
import { useAuth } from './useAuth';
import { useBasket } from './useBasket';
import { useCatalog } from './useCatalog';
import { useAddresses } from './useAddresses';
import { useCheckout } from './useCheckout';
import { useOrders } from './useOrders';
import { useUiState } from './useUiState';

export function useAppShell() {
  const ui = useUiState();
  const appStatus = useAppStatus();
  const auth = useAuth();
  const catalog = useCatalog();
  const basket = useBasket(auth.user?.userId);
  const addresses = useAddresses(auth.user?.userId);
  const checkout = useCheckout(addresses.addresses);
  const orders = useOrders({
    customerId: auth.user?.userId,
    basket: basket.basket,
  });

  const signIn = async () => {
    try {
      appStatus.showLoading('Giris yapiliyor...');
      await auth.signIn();
      appStatus.hideLoading();
      appStatus.showSuccess(`${auth.mode === 'login' ? 'Giris' : 'Kayit'} basarili.`);
      ui.setTab(ROUTES.HOME);
    } catch (error) {
      appStatus.hideLoading();
      appStatus.showError(error instanceof Error ? error.message : 'Islem basarisiz.');
    }
  };

  const addItem = async (productId: string, selectedOptionIds: string[] = [], quantity = 1) => {
    if (!auth.user) {
      ui.setTab(ROUTES.ACCOUNT);
      return;
    }

    try {
      appStatus.showLoading('Urun sepete ekleniyor...');
      await basket.addItem(productId, selectedOptionIds, quantity);
      appStatus.hideLoading();
      appStatus.showSuccess('Urun sepete eklendi.');
      ui.setTab(ROUTES.BASKET);
    } catch (error) {
      appStatus.hideLoading();
      appStatus.showError(error instanceof Error ? error.message : 'Sepete eklenemedi.');
    }
  };

  const updateBasketItem = async (itemId: string, quantity: number) => {
    try {
      await basket.updateItem(itemId, quantity);
    } catch (error) {
      appStatus.showError(error instanceof Error ? error.message : 'Sepet guncellenemedi.');
    }
  };

  const removeBasketItem = async (itemId: string) => {
    try {
      await basket.removeItem(itemId);
    } catch (error) {
      appStatus.showError(error instanceof Error ? error.message : 'Urun sepetten silinemedi.');
    }
  };

  const placeOrder = async (payload: Parameters<typeof orders.placeOrder>[0]) => {
    try {
      appStatus.showLoading('Siparis olusturuluyor...');
      const order = await orders.placeOrder(payload);

      if (!order) {
        appStatus.hideLoading();
        appStatus.showError('Siparis olusturulamadi.');
        return;
      }

      appStatus.hideLoading();
      checkout.reset();
      appStatus.showSuccess('Siparis olusturuldu.');
      ui.setTab(ROUTES.HOME);
    } catch (error) {
      appStatus.hideLoading();
      appStatus.showError(error instanceof Error ? error.message : 'Siparis verilemedi.');
    }
  };

  const signOut = () => {
    auth.signOut();
    checkout.reset();
    appStatus.showSuccess('Oturum kapatildi.');
    ui.setTab(ROUTES.HOME);
  };

  const authStatus = {
    isLoading: auth.isSigningIn,
    error: auth.error,
  };
  const catalogStatus = {
    isLoading: catalog.isLoading,
    error: catalog.error,
  };
  const basketStatus = {
    isLoading: basket.isLoading || basket.isAddingItem || basket.isUpdatingItem,
    error: basket.error,
  };
  const ordersStatus = {
    isLoading: orders.isLoading || orders.isPlacingOrder,
    error: orders.error,
  };
  const addressesStatus = {
    isLoading: addresses.isLoading,
    error: addresses.error,
  };

  const continueToPayment = async () => {
    if (!checkout.selectedAddress) {
      appStatus.showError('Lutfen devam etmek icin bir adres sec.');
      return;
    }

    checkout.goToPayment();
  };

  const saveNewAddress = async () => {
    const draft = checkout.draftAddress;
    if (!checkout.addressTitle.trim() || !draft.street || !draft.district || !draft.city || !draft.postalCode || !draft.country) {
      appStatus.showError('Lutfen adres bilgilerini eksiksiz doldur.');
      return;
    }

    try {
      appStatus.showLoading('Adres kaydediliyor...');
      const created = await addresses.addAddress({ title: checkout.addressTitle.trim(), ...draft });
      checkout.selectAddress(created.id);
      checkout.cancelAddAddress();
      appStatus.hideLoading();
      appStatus.showSuccess('Adres kaydedildi. Lutfen adresini sec.');
    } catch (error) {
      appStatus.hideLoading();
      appStatus.showError(error instanceof Error ? error.message : 'Adres kaydedilemedi.');
    }
  };

  const status = {
    isLoading:
      authStatus.isLoading ||
      catalogStatus.isLoading ||
      basketStatus.isLoading ||
      ordersStatus.isLoading ||
      addressesStatus.isLoading,
    error: authStatus.error ?? catalogStatus.error ?? basketStatus.error ?? ordersStatus.error ?? addressesStatus.error,
    auth: authStatus,
    catalog: catalogStatus,
    basket: basketStatus,
    orders: ordersStatus,
    addresses: addressesStatus,
  };

  return {
    tab: ui.tab,
    setTab: ui.setTab,
    status,
    user: auth.user,
    mode: auth.mode,
    email: auth.email,
    password: auth.password,
    setMode: auth.setMode,
    setEmail: auth.setEmail,
    setPassword: auth.setPassword,
    categories: catalog.categories,
    products: catalog.products,
    basket: basket.basket,
    addresses: addresses.addresses,
    checkout,
    continueToPayment,
    saveNewAddress,
    lastOrder: orders.lastOrder,
    lastOrderStatus: orders.lastOrder?.status,
    signIn,
    addItem,
    updateBasketItem,
    removeBasketItem,
    placeOrder,
    signOut,
  };
}
