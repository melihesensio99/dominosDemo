import { Pressable, ScrollView, Text, TextInput, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import type { UserAddress } from '../types/auth';
import type { Address } from '../types/common';
import { theme } from '../theme';
import { styles } from './CheckoutAddressScreen.styles';

interface CheckoutAddressScreenProps {
  addresses: UserAddress[];
  selectedAddressId: string | null;
  draftAddress: Address;
  isLoading?: boolean;
  error?: unknown;
  onSelectAddress: (id: string) => void;
  onChangeDraft: (address: Address) => void;
  onContinue: () => void;
  onBack: () => void;
}

export function CheckoutAddressScreen({
  addresses,
  selectedAddressId,
  draftAddress,
  isLoading,
  error,
  onSelectAddress,
  onChangeDraft,
  onContinue,
  onBack,
}: CheckoutAddressScreenProps) {
  return (
    <View style={styles.container}>
      <AppHeader title="Teslimat Adresi" subtitle="Kayitli adreslerinden birini sec" />
      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {error ? <Text style={styles.error}>{error instanceof Error ? error.message : 'Adresler getirilemedi.'}</Text> : null}

        {addresses.map((address) => (
          <Pressable
            key={address.id}
            style={[styles.card, selectedAddressId === address.id && styles.cardSelected]}
            onPress={() => onSelectAddress(address.id)}
          >
            <Text style={styles.title}>{address.title}</Text>
            <Text style={styles.detail}>
              {address.street}, {address.district}, {address.city} {address.postalCode}, {address.country}
            </Text>
          </Pressable>
        ))}

        <View style={styles.card}>
          <Text style={styles.title}>{addresses.length ? 'Yeni adres ekle' : 'Ilk adresini ekle'}</Text>
          {(['street', 'district', 'city', 'postalCode', 'country'] as const).map((key) => (
            <TextInput
              key={key}
              style={styles.input}
              placeholder={key}
              placeholderTextColor={theme.colors.muted}
              value={draftAddress[key]}
              onChangeText={(value) => onChangeDraft({ ...draftAddress, [key]: value })}
            />
          ))}
        </View>

        <Pressable style={styles.primaryButton} disabled={isLoading} onPress={onContinue}>
          <Text style={styles.primaryText}>{isLoading ? 'Hazirlaniyor...' : 'Adresi Onayla ve Devam Et'}</Text>
        </Pressable>
        <Pressable style={styles.secondaryButton} onPress={onBack}>
          <Text style={styles.secondaryText}>Sepete Don</Text>
        </Pressable>
      </ScrollView>
    </View>
  );
}
