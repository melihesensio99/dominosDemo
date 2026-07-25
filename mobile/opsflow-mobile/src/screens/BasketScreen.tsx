import { useMemo } from 'react';
import { Image, Pressable, ScrollView, Text, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import { EmptyState } from '../components/EmptyState';
import type { Basket } from '../types/basket';
import type { Product } from '../types/catalog';
import { styles } from './BasketScreen.styles';

const basketBanner =
  'https://res.cloudinary.com/dc2j01x6b/image/upload/WhatsApp_Image_2026-07-25_at_19.14.44_1.jpg';

interface BasketScreenProps {
  basket: Basket | null;
  products: Product[];
  isLoading?: boolean;
  error?: unknown;
  onProceedCheckout: () => void;
  onGoMenu: () => void;
  onUpdateQuantity: (itemId: string, quantity: number) => void;
  onRemoveItem: (itemId: string) => void;
}

export function BasketScreen({
  basket,
  isLoading,
  error,
  onProceedCheckout,
  onGoMenu,
  onUpdateQuantity,
  onRemoveItem,
}: BasketScreenProps) {
  const total = useMemo(
    () => basket?.items.reduce((sum, item) => sum + item.totalPrice, 0) ?? 0,
    [basket],
  );

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
      <AppHeader title="Sepetim" subtitle="Siparisini kontrol et ve tamamla" />
      <ScrollView style={styles.scroll} contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {!basket?.items.length ? (
          emptyState
        ) : (
          <>
            <View style={styles.stepsCard}>
              <View style={styles.stepActive}>
                <Text style={styles.stepIcon}>🛒</Text>
                <Text style={styles.stepActiveText}>Sepetim</Text>
              </View>
              <Text style={styles.stepArrow}>›</Text>
              <View style={styles.step}>
                <Text style={styles.stepIcon}>▣</Text>
                <Text style={styles.stepText}>Siparis Onay</Text>
              </View>
              <Text style={styles.stepArrow}>›</Text>
              <View style={styles.step}>
                <Text style={styles.stepIcon}>▤</Text>
                <Text style={styles.stepText}>Siparis Sonuc</Text>
              </View>
            </View>

            <Image source={{ uri: basketBanner }} style={styles.banner} resizeMode="cover" />

            <View style={styles.itemsCard}>
              {basket.items.map((item) => (
                <View key={item.id} style={styles.itemRow}>
                  <View style={styles.itemHeader}>
                    <Text style={styles.itemTitle}>{item.productName}</Text>
                    <Text style={styles.itemPrice}>{item.totalPrice.toLocaleString('tr-TR')} TL</Text>
                  </View>
                  {item.selectedOptions.length > 0 && (
                    <Text style={styles.itemMeta} numberOfLines={1}>
                      {item.selectedOptions.map((option) => option.name).join(', ')}
                    </Text>
                  )}
                  <View style={styles.itemActions}>
                    <View style={styles.quantityControl}>
                      <Pressable
                        accessibilityLabel={`${item.productName} azalt`}
                        style={styles.quantityButton}
                        onPress={() => onUpdateQuantity(item.id, item.quantity - 1)}
                      >
                        <Text style={styles.quantityButtonText}>−</Text>
                      </Pressable>
                      <Text style={styles.quantityValue}>{item.quantity}</Text>
                      <Pressable
                        accessibilityLabel={`${item.productName} arttir`}
                        style={styles.quantityButton}
                        onPress={() => onUpdateQuantity(item.id, item.quantity + 1)}
                      >
                        <Text style={styles.quantityButtonText}>+</Text>
                      </Pressable>
                    </View>
                    <Pressable
                      accessibilityLabel={`${item.productName} sil`}
                      style={styles.deleteButton}
                      onPress={() => onRemoveItem(item.id)}
                    >
                      <Text style={styles.deleteIcon}>🗑</Text>
                    </Pressable>
                  </View>
                </View>
              ))}
            </View>

            <View style={styles.promoCard}>
              <Text style={styles.promoIcon}>🎁</Text>
              <Text style={styles.promoText}>Kampanya kodun varsa odeme adiminda uygulayabilirsin.</Text>
            </View>

            <View style={styles.checkoutBar}>
              <View>
                <Text style={styles.checkoutLabel}>{basket.totalQuantity} urun</Text>
                <Text style={styles.checkoutTotal}>{total.toLocaleString('tr-TR')} TL</Text>
              </View>
              <Pressable style={styles.checkoutButton} onPress={onProceedCheckout}>
                <Text style={styles.checkoutButtonText}>Odeme Yap</Text>
              </Pressable>
            </View>
          </>
        )}
      </ScrollView>
    </View>
  );
}
