import { useMemo, useState } from 'react';
import { Pressable, ScrollView, Text, TextInput, View } from 'react-native';
import { ERROR_MESSAGES } from '../constants/errorMessages';
import { AppHeader } from '../components/AppHeader';
import { EmptyState } from '../components/EmptyState';
import { SectionCard } from '../components/SectionCard';
import type { Address } from '../types/common';
import type { Basket } from '../types/basket';
import type { Product } from '../types/catalog';
import { styles } from './BasketScreen.styles';
import { theme } from '../theme';

interface BasketScreenProps {
  basket: Basket | null;
  products: Product[];
  isLoading?: boolean;
  isPlacingOrder?: boolean;
  error?: unknown;
  onPlaceOrder: (payload: {
    shippingAddress: Address;
    billingAddress: Address;
    paymentMethod: number;
  }) => void;
  onGoMenu: () => void;
}

const defaultAddress: Address = {
  street: 'Bağdat Caddesi 42',
  district: 'Kadıköy',
  city: 'İstanbul',
  postalCode: '34710',
  country: 'Turkey',
};

export function BasketScreen({
  basket,
  products,
  isLoading,
  isPlacingOrder,
  error,
  onPlaceOrder,
  onGoMenu,
}: BasketScreenProps) {
  const [address, setAddress] = useState<Address>(defaultAddress);
  const [paymentMethod, setPaymentMethod] = useState(0);

  const total = useMemo(() => {
    if (!basket?.items.length) {
      return 0;
    }

    return basket.items.reduce((sum, item) => {
      const product = products.find((candidate) => candidate.id === item.productId);
      return sum + (product?.price ?? 0) * item.quantity;
    }, 0);
  }, [basket, products]);

  const emptyState = isLoading ? (
    <EmptyState title={ERROR_MESSAGES.BASKET_LOADING} message={ERROR_MESSAGES.BASKET_FETCHING} />
  ) : error ? (
    <EmptyState
      title="Sepet alınamadı"
      message={error instanceof Error ? error.message : ERROR_MESSAGES.CART_LOADING_FAILED}
    />
  ) : (
    <EmptyState
      title={ERROR_MESSAGES.BASKET_EMPTY}
      message={ERROR_MESSAGES.BASKET_EMPTY_HINT}
      actionLabel="Menüye Git"
      onAction={onGoMenu}
    />
  );

  return (
    <View style={styles.container}>
      <AppHeader title="Sepetim" subtitle="Adres ve ödeme ile siparişi tamamla" />

      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {!basket?.items.length ? (
          emptyState
        ) : (
          <>
            <SectionCard title="Ürünler">
              <View style={styles.summaryRow}>
                <Text style={styles.summaryLabel}>{basket.itemCount} ürün</Text>
                <Text style={styles.summaryPrice}>{total.toLocaleString('tr-TR')} TL</Text>
              </View>

              <View style={styles.items}>
                {basket.items.map((item) => {
                  const product = products.find((candidate) => candidate.id === item.productId);

                  return (
                    <View key={item.productId} style={styles.itemRow}>
                      <View style={{ flex: 1 }}>
                        <Text style={styles.itemTitle}>{product?.name ?? item.productId}</Text>
                        <Text style={styles.itemMeta}>Adet: {item.quantity}</Text>
                      </View>
                      <Text style={styles.itemPrice}>
                        {((product?.price ?? 0) * item.quantity).toLocaleString('tr-TR')} TL
                      </Text>
                    </View>
                  );
                })}
              </View>
            </SectionCard>

            <SectionCard title="Teslimat adresi">
              {(['street', 'district', 'city', 'postalCode', 'country'] as const).map((key) => (
                <TextInput
                  key={key}
                  style={styles.input}
                  placeholder={key}
                  placeholderTextColor={theme.colors.muted}
                  value={address[key]}
                  onChangeText={(value) => setAddress((current) => ({ ...current, [key]: value }))}
                />
              ))}
            </SectionCard>

            <SectionCard title="Ödeme yöntemi">
              <View style={styles.paymentRow}>
                {[
                  { label: 'Kart', value: 0 },
                  { label: 'Havale', value: 1 },
                  { label: 'Kapıda', value: 2 },
                ].map((item) => {
                  const active = paymentMethod === item.value;

                  return (
                    <Pressable
                      key={item.value}
                      style={[styles.paymentChip, active && styles.paymentChipActive]}
                      onPress={() => setPaymentMethod(item.value)}
                    >
                      <Text style={[styles.paymentText, active && styles.paymentTextActive]}>{item.label}</Text>
                    </Pressable>
                  );
                })}
              </View>

              <Pressable
                style={[styles.orderButton, isPlacingOrder && { opacity: 0.7 }]}
                disabled={Boolean(isPlacingOrder)}
                onPress={() =>
                  onPlaceOrder({
                    shippingAddress: address,
                    billingAddress: address,
                    paymentMethod,
                  })
                }
              >
                <Text style={styles.orderButtonText}>{isPlacingOrder ? 'Sipariş Veriliyor...' : 'Siparişi Ver'}</Text>
              </Pressable>
            </SectionCard>
          </>
        )}
      </ScrollView>
    </View>
  );
}
