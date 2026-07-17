import { ROUTES } from '../constants/routes';
import { useAppStatus } from '../app-status';
import { useAuth } from './useAuth';
import { useBasket } from './useBasket';
import { useCatalog } from './useCatalog';
import { useOrders } from './useOrders';
import { useUiState } from './useUiState';

export function useAppShell() {
  const ui = useUiState();
  const appStatus = useAppStatus();
  const auth = useAuth();
  const catalog = useCatalog();
  const basket = useBasket(auth.user?.userId);
  const orders = useOrders({
    customerId: auth.user?.userId,
    basket: basket.basket,
  });

  const signIn = async () => {
    try {
      appStatus.showLoading('Giriş yapılıyor...');
      await auth.signIn();
      appStatus.hideLoading();
      appStatus.showSuccess(`${auth.mode === 'login' ? 'Giriş' : 'Kayıt'} başarılı.`);
      ui.setTab(ROUTES.HOME);
    } catch (error) {
      appStatus.hideLoading();
      appStatus.showError(error instanceof Error ? error.message : 'İşlem başarısız.');
    }
  };

  const addItem = async (productId: string) => {
    if (!auth.user) {
      ui.setTab(ROUTES.ACCOUNT);
      return;
    }

    try {
      appStatus.showLoading('Ürün sepete ekleniyor...');
      await basket.addItem(productId);
      appStatus.hideLoading();
      appStatus.showSuccess('Ürün sepete eklendi.');
      ui.setTab(ROUTES.BASKET);
    } catch (error) {
      appStatus.hideLoading();
      appStatus.showError(error instanceof Error ? error.message : 'Sepete eklenemedi.');
    }
  };

  const placeOrder = async (payload: Parameters<typeof orders.placeOrder>[0]) => {
    try {
      appStatus.showLoading('Sipariş oluşturuluyor...');
      const order = await orders.placeOrder(payload);

      if (!order) {
        appStatus.hideLoading();
        appStatus.showError('Sipariş oluşturulamadı.');
        return;
      }

      appStatus.hideLoading();
      appStatus.showSuccess('Sipariş oluşturuldu.');
      ui.setTab(ROUTES.HOME);
    } catch (error) {
      appStatus.hideLoading();
      appStatus.showError(error instanceof Error ? error.message : 'Sipariş verilemedi.');
    }
  };

  const signOut = () => {
    auth.signOut();
    appStatus.showSuccess('Oturum kapatıldı.');
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
    isLoading: basket.isLoading || basket.isAddingItem,
    error: basket.error,
  };

  const ordersStatus = {
    isLoading: orders.isLoading || orders.isPlacingOrder,
    error: orders.error,
  };

  const status = {
    isLoading: authStatus.isLoading || catalogStatus.isLoading || basketStatus.isLoading || ordersStatus.isLoading,
    error: authStatus.error ?? catalogStatus.error ?? basketStatus.error ?? ordersStatus.error,
    auth: authStatus,
    catalog: catalogStatus,
    basket: basketStatus,
    orders: ordersStatus,
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
    lastOrder: orders.lastOrder,
    lastOrderStatus: orders.lastOrder?.status,
    signIn,
    addItem,
    placeOrder,
    signOut,
  };
}
