import { useMemo } from 'react';
import { Pressable, ScrollView, Text, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import { EmptyState } from '../components/EmptyState';
import { SectionCard } from '../components/SectionCard';
import type { Basket } from '../types/basket';
import type { Product } from '../types/catalog';
import { styles } from './BasketScreen.styles';

interface BasketScreenProps {
  basket: Basket | null;
  products: Product[];
  isLoading?: boolean;
  error?: unknown;
  onProceedCheckout: () => void;
  onGoMenu: () => void;
}

export function BasketScreen({
  basket,
  products,
  isLoading,
  error,
  onProceedCheckout,
  onGoMenu,
}: BasketScreenProps) {
  const total = useMemo(() => {
    return basket?.items.reduce((sum, item) => {
      return sum + item.totalPrice;
    }, 0) ?? 0;
  }, [basket, products]);

  const emptyState = isLoading ? (
    <EmptyState title="Sepet yukleniyor" message="Sepet bilgisi backend'den getiriliyor." />
  ) : error ? (
    <EmptyState title="Sepet alinamadi" message={error instanceof Error ? error.message : 'Sepet getirilemedi.'} />
  ) : (
    <EmptyState
      title="Sepetinde urun bulunmuyor."
      message="Menuye gidip birkac urun ekleyebilirsin."
      actionLabel="Menuye Git"
      onAction={onGoMenu}
    />
  );

  return (
    <View style={styles.container}>
      <AppHeader title="Sepetim" subtitle="Urunlerini kontrol et ve devam et" />
      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {!basket?.items.length ? (
          emptyState
        ) : (
          <>
            <SectionCard title="Urunler">
              <View style={styles.summaryRow}>
                <Text style={styles.summaryLabel}>{basket.itemCount} urun</Text>
                <Text style={styles.summaryPrice}>{total.toLocaleString('tr-TR')} TL</Text>
              </View>
              <View style={styles.items}>
                {basket.items.map((item) => {
                  return (
                    <View key={item.id} style={styles.itemRow}>
                      <View style={{ flex: 1 }}>
                        <Text style={styles.itemTitle}>{item.productName}</Text>
                        {item.selectedOptions.map((option) => (
                          <Text key={option.optionId} style={styles.itemMeta}>
                            {option.name}{option.priceAdjustment > 0 ? ` (+${option.priceAdjustment} TL)` : ''}
                          </Text>
                        ))}
                        <Text style={styles.itemMeta}>Adet: {item.quantity}</Text>
                      </View>
                      <Text style={styles.itemPrice}>
                        {item.totalPrice.toLocaleString('tr-TR')} TL
                      </Text>
                    </View>
                  );
                })}
              </View>
            </SectionCard>
            <Pressable style={styles.orderButton} onPress={onProceedCheckout}>
              <Text style={styles.orderButtonText}>Sepeti Onayla ve Devam Et</Text>
            </Pressable>
          </>
        )}
      </ScrollView>
    </View>
  );
}
