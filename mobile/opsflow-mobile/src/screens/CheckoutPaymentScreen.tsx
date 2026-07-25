import { Pressable, ScrollView, Text, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import type { Basket } from '../types/basket';
import type { Product } from '../types/catalog';
import type { UserAddress } from '../types/auth';
import { styles } from './CheckoutPaymentScreen.styles';

interface CheckoutPaymentScreenProps {
  basket: Basket | null;
  products: Product[];
  address?: UserAddress;
  paymentMethod: number;
  isPlacingOrder?: boolean;
  error?: unknown;
  onChangePaymentMethod: (value: number) => void;
  onConfirm: () => void;
  onBack: () => void;
}

export function CheckoutPaymentScreen({
  basket,
  products,
  address,
  paymentMethod,
  isPlacingOrder,
  error,
  onChangePaymentMethod,
  onConfirm,
  onBack,
}: CheckoutPaymentScreenProps) {
  const total = basket?.items.reduce((sum, item) => {
    const product = products.find((candidate) => candidate.id === item.productId);
    return sum + (product?.price ?? 0) * item.quantity;
  }, 0) ?? 0;

  return (
    <View style={styles.container}>
      <AppHeader title="Odeme ve Onay" subtitle="Siparis bilgilerini son kez kontrol et" />
      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {error ? <Text style={styles.error}>{error instanceof Error ? error.message : 'Siparis olusturulamadi.'}</Text> : null}
        <View style={styles.card}>
          <Text style={styles.title}>Teslimat adresi</Text>
          <Text style={styles.summary}>
            {address ? `${address.title}: ${address.street}, ${address.district}, ${address.city}` : 'Adres secilmedi.'}
          </Text>
        </View>
        <View style={styles.card}>
          <Text style={styles.title}>Odeme yontemi</Text>
          <View style={styles.options}>
            {[{ label: 'Kart', value: 0 }, { label: 'Havale', value: 1 }, { label: 'Kapida', value: 2 }].map((option) => (
              <Pressable
                key={option.value}
                style={[styles.option, paymentMethod === option.value && styles.optionSelected]}
                onPress={() => onChangePaymentMethod(option.value)}
              >
                <Text style={[styles.optionText, paymentMethod === option.value && styles.optionTextSelected]}>{option.label}</Text>
              </Pressable>
            ))}
          </View>
        </View>
        <View style={styles.card}>
          <Text style={styles.title}>Siparis ozeti</Text>
          <Text style={styles.summary}>{basket?.itemCount ?? 0} urun</Text>
          <Text style={styles.title}>{total.toLocaleString('tr-TR')} TL</Text>
        </View>
        <Pressable style={styles.primaryButton} disabled={isPlacingOrder} onPress={onConfirm}>
          <Text style={styles.primaryText}>{isPlacingOrder ? 'Siparis veriliyor...' : 'Siparisi Onayla'}</Text>
        </Pressable>
        <Pressable style={styles.secondaryButton} onPress={onBack}>
          <Text style={styles.secondaryText}>Adrese Don</Text>
        </Pressable>
      </ScrollView>
    </View>
  );
}
