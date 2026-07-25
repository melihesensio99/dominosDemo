import { Pressable, ScrollView, Text, TextInput, View } from 'react-native';
import { useState } from 'react';
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
  note: string;
  isPlacingOrder?: boolean;
  error?: unknown;
  onChangePaymentMethod: (value: number) => void;
  onChangeNote: (value: string) => void;
  onConfirm: () => void;
  onBack: () => void;
}

export function CheckoutPaymentScreen({
  basket,
  products,
  address,
  paymentMethod,
  note,
  isPlacingOrder,
  error,
  onChangePaymentMethod,
  onChangeNote,
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
      <ScrollView style={styles.scroll} contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {error ? <Text style={styles.error}>{error instanceof Error ? error.message : 'Siparis olusturulamadi.'}</Text> : null}
        <View style={styles.card}>
          <Text style={styles.title}>Teslimat adresi</Text>
          <Text style={styles.summary}>
            {address ? `${address.title}: ${address.street}, ${address.district}, ${address.city}` : 'Adres secilmedi.'}
          </Text>
        </View>
        <View style={styles.card}>
          <Text style={styles.title}>Odeme yontemi</Text>
          <PaymentMethodSelect paymentMethod={paymentMethod} onChange={onChangePaymentMethod} />
        </View>
        <View style={styles.card}>
          <Text style={styles.title}>Siparis notu</Text>
          <TextInput
            value={note}
            onChangeText={onChangeNote}
            placeholder="Kurye icin bir not ekle"
            placeholderTextColor={styles.placeholder.color}
            multiline
            maxLength={500}
            style={styles.noteInput}
          />
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

function PaymentMethodSelect({ paymentMethod, onChange }: { paymentMethod: number; onChange: (value: number) => void }) {
  const [isOpen, setIsOpen] = useState(false);
  const options = [{ label: 'Kart', value: 0 }, { label: 'Havale', value: 1 }, { label: 'Kapida', value: 2 }];
  const selected = options.find((option) => option.value === paymentMethod) ?? options[0];

  return (
    <View>
      <Pressable style={styles.selectButton} onPress={() => setIsOpen((current) => !current)}>
        <Text style={styles.selectText}>{selected.label}</Text>
        <Text style={styles.selectArrow}>{isOpen ? '⌃' : '⌄'}</Text>
      </Pressable>
      {isOpen ? (
        <View style={styles.selectOptions}>
          {options.map((option) => (
            <Pressable key={option.value} style={styles.selectOption} onPress={() => { onChange(option.value); setIsOpen(false); }}>
              <Text style={styles.selectOptionText}>{option.label}</Text>
            </Pressable>
          ))}
        </View>
      ) : null}
    </View>
  );
}
