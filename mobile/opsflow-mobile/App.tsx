import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useEffect, useState } from 'react';
import { SafeAreaView, View } from 'react-native';
import { StatusBar } from 'expo-status-bar';
import { AccountScreen } from './src/screens/AccountScreen';
import { BasketScreen } from './src/screens/BasketScreen';
import { CheckoutAddressScreen } from './src/screens/CheckoutAddressScreen';
import { CheckoutPaymentScreen } from './src/screens/CheckoutPaymentScreen';
import { HomeScreen } from './src/screens/HomeScreen';
import { ProductDetailsScreen } from './src/screens/ProductDetailsScreen';
import type { Product } from './src/types/catalog';
import { BottomTabBar } from './src/components/BottomTabBar';
import { AppStatusProvider, AppStatusOverlay } from './src/app-status';
import { useAppShell } from './src/hooks';
import { ROUTES } from './src/constants/routes';
import { styles } from './App.styles';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: { retry: 1, refetchOnWindowFocus: false },
  },
});

function AppShell() {
  const app = useAppShell();
  const isAuthenticated = Boolean(app.user);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);

  useEffect(() => {
    if (isAuthenticated) {
      app.setTab(ROUTES.HOME);
    }
  }, [app.setTab, isAuthenticated]);

  return (
    <SafeAreaView style={styles.safe}>
      <StatusBar style="dark" />
      <View style={styles.container}>
        <AppStatusOverlay />
        {isAuthenticated ? (
          <>
            {selectedProduct ? (
              <ProductDetailsScreen
                product={selectedProduct}
                onBack={() => setSelectedProduct(null)}
                onAdd={(optionIds) => {
                  setSelectedProduct(null);
                  void app.addItem(selectedProduct.id, optionIds);
                }}
              />
            ) : app.tab === ROUTES.HOME && (
              <HomeScreen
                categories={app.categories}
                products={app.products}
                isCatalogLoading={app.status.catalog.isLoading}
                catalogError={app.status.catalog.error}
                onAdd={setSelectedProduct}
                lastOrderStatus={app.lastOrderStatus}
                isLoading={app.status.orders.isLoading}
                error={app.status.orders.error}
              />
            )}
            {!selectedProduct && app.tab === ROUTES.BASKET && app.checkout.step === 'basket' && (
              <BasketScreen
                basket={app.basket}
                products={app.products}
                isLoading={app.status.basket.isLoading}
                error={app.status.basket.error}
                onProceedCheckout={app.checkout.begin}
                onGoMenu={() => app.setTab(ROUTES.HOME)}
              />
            )}
            {!selectedProduct && app.tab === ROUTES.BASKET && app.checkout.step === 'address' && (
              <CheckoutAddressScreen
                addresses={app.addresses}
                addressMode={app.checkout.addressMode}
                selectedAddressId={app.checkout.selectedAddressId}
                draftAddress={app.checkout.draftAddress}
                isLoading={app.status.addresses.isLoading}
                error={app.status.addresses.error}
                onSelectAddress={app.checkout.selectAddress}
                onChangeDraft={app.checkout.setDraftAddress}
                onStartAddAddress={app.checkout.beginAddAddress}
                onCancelAddAddress={app.checkout.cancelAddAddress}
                onSaveAddress={() => void app.saveNewAddress()}
                onContinue={() => void app.continueToPayment()}
                onBack={app.checkout.goBack}
              />
            )}
            {!selectedProduct && app.tab === ROUTES.BASKET && app.checkout.step === 'payment' && (
              <CheckoutPaymentScreen
                basket={app.basket}
                products={app.products}
                address={app.checkout.selectedAddress}
                paymentMethod={app.checkout.paymentMethod}
                isPlacingOrder={app.status.orders.isLoading}
                error={app.status.orders.error}
                onChangePaymentMethod={app.checkout.setPaymentMethod}
                onConfirm={() =>
                  void app.placeOrder({
                    shippingAddress: app.checkout.selectedAddress!,
                    billingAddress: app.checkout.selectedAddress!,
                    paymentMethod: app.checkout.paymentMethod,
                  })
                }
                onBack={app.checkout.goBack}
              />
            )}
            {!selectedProduct && app.tab === ROUTES.ACCOUNT && (
              <AccountScreen
                user={app.user}
                mode={app.mode}
                email={app.email}
                password={app.password}
                isLoading={app.status.auth.isLoading}
                error={app.status.auth.error}
                onChangeMode={app.setMode}
                onChangeEmail={app.setEmail}
                onChangePassword={app.setPassword}
                onAuth={() => void app.signIn()}
                onSignOut={app.signOut}
              />
            )}
            {!selectedProduct && app.checkout.step === 'basket' && <BottomTabBar activeTab={app.tab} onChangeTab={app.setTab} />}
          </>
        ) : (
          <AccountScreen
            user={app.user}
            mode={app.mode}
            email={app.email}
            password={app.password}
            isLoading={app.status.auth.isLoading}
            error={app.status.auth.error}
            onChangeMode={app.setMode}
            onChangeEmail={app.setEmail}
            onChangePassword={app.setPassword}
            onAuth={() => void app.signIn()}
            onSignOut={app.signOut}
          />
        )}
      </View>
    </SafeAreaView>
  );
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AppStatusProvider>
        <AppShell />
      </AppStatusProvider>
    </QueryClientProvider>
  );
}
