import { ERROR_MESSAGES } from '../constants/errorMessages';
import { ROUTES } from '../constants/routes';
import { useFeedback } from '../feedback';
import { useAuth } from './useAuth';
import { useBasket } from './useBasket';
import { useCatalog } from './useCatalog';
import { useOrders } from './useOrders';
import { useUiState } from './useUiState';

export function useAppShell() {
  const ui = useUiState();
  const feedback = useFeedback();
  const auth = useAuth();
  const catalog = useCatalog();
  const basket = useBasket(auth.user?.userId);
  const orders = useOrders({
    customerId: auth.user?.userId,
    basket: basket.basket,
  });

  const signIn = async () => {
    try {
      feedback.showLoading(ERROR_MESSAGES.SIGNIN_LOADING);
      await auth.signIn();
      feedback.hideLoading();
      feedback.showSuccess(`${auth.mode === 'login' ? 'Giriş' : 'Kayıt'} başarılı.`);
      ui.setTab(ROUTES.HOME);
    } catch (error) {
      feedback.hideLoading();
      feedback.showError(error instanceof Error ? error.message : ERROR_MESSAGES.AUTH_FAILED);
    }
  };

  const addItem = async (productId: string) => {
    if (!auth.user) {
      ui.setTab(ROUTES.ACCOUNT);
      return;
    }

    try {
      feedback.showLoading(ERROR_MESSAGES.ADD_ITEM_LOADING);
      await basket.addItem(productId);
      feedback.hideLoading();
      feedback.showSuccess('Ürün sepete eklendi.');
      ui.setTab(ROUTES.BASKET);
    } catch (error) {
      feedback.hideLoading();
      feedback.showError(error instanceof Error ? error.message : ERROR_MESSAGES.BASKET_ADD_FAILED);
    }
  };

  const placeOrder = async (payload: Parameters<typeof orders.placeOrder>[0]) => {
    try {
      feedback.showLoading(ERROR_MESSAGES.ORDER_CREATING);
      const order = await orders.placeOrder(payload);

      if (!order) {
        feedback.hideLoading();
        feedback.showError(ERROR_MESSAGES.ORDER_NOT_CREATED);
        return;
      }

      feedback.hideLoading();
      feedback.showSuccess(ERROR_MESSAGES.ORDER_CREATED);
      ui.setTab(ROUTES.HOME);
    } catch (error) {
      feedback.hideLoading();
      feedback.showError(error instanceof Error ? error.message : ERROR_MESSAGES.ORDER_CREATE_FAILED);
    }
  };

  const signOut = () => {
    auth.signOut();
    feedback.showSuccess(ERROR_MESSAGES.SIGNOUT_SUCCESS);
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
