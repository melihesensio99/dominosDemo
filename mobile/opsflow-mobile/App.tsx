import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { SafeAreaView, View } from 'react-native';
import { StatusBar } from 'expo-status-bar';
import { AccountScreen } from './src/screens/AccountScreen';
import { BasketScreen } from './src/screens/BasketScreen';
import { HomeScreen } from './src/screens/HomeScreen';
import { MenuScreen } from './src/screens/MenuScreen';
import { BottomTabBar } from './src/components/BottomTabBar';
import { GlobalFeedbackLayer } from './src/components/GlobalFeedbackLayer';
import { FeedbackProvider } from './src/feedback';
import { useAppShell } from './src/hooks';
import { ROUTES } from './src/constants/routes';
import { styles } from './App.styles';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
});

function AppShell() {
  const app = useAppShell();
  const isAuthenticated = Boolean(app.user);

  return (
    <SafeAreaView style={styles.safe}>
      <StatusBar style="dark" />
      <View style={styles.container}>
        <GlobalFeedbackLayer />

        {isAuthenticated ? (
          <>
            {app.tab === ROUTES.HOME && (
              <HomeScreen
                onGoMenu={() => app.setTab(ROUTES.MENU)}
                lastOrderStatus={app.lastOrderStatus}
                isLoading={app.status.orders.isLoading}
                error={app.status.orders.error}
              />
            )}

            {app.tab === ROUTES.MENU && (
              <MenuScreen
                categories={app.categories}
                products={app.products}
                isLoading={app.status.catalog.isLoading}
                error={app.status.catalog.error}
                onAdd={(product) => void app.addItem(product.id)}
              />
            )}

            {app.tab === ROUTES.BASKET && (
              <BasketScreen
                basket={app.basket}
                products={app.products}
                isLoading={app.status.basket.isLoading}
                isPlacingOrder={app.status.orders.isLoading}
                error={app.status.basket.error ?? app.status.orders.error}
                onPlaceOrder={(payload) => void app.placeOrder(payload)}
                onGoMenu={() => app.setTab(ROUTES.MENU)}
              />
            )}

            {app.tab === ROUTES.ACCOUNT && (
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

            <BottomTabBar activeTab={app.tab} onChangeTab={app.setTab} />
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
      <FeedbackProvider>
        <AppShell />
      </FeedbackProvider>
    </QueryClientProvider>
  );
}
