import { Pressable, ScrollView, Text, TextInput, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import type { UserAddress } from '../types/auth';
import type { Address } from '../types/common';
import type { AddressMode } from '../hooks/useCheckout';
import { theme } from '../theme';
import { styles } from './CheckoutAddressScreen.styles';

interface CheckoutAddressScreenProps {
  addresses: UserAddress[];
  addressMode: AddressMode;
  selectedAddressId: string | null;
  draftAddress: Address;
  isLoading?: boolean;
  error?: unknown;
  onSelectAddress: (id: string) => void;
  onChangeDraft: (address: Address) => void;
  onStartAddAddress: () => void;
  onCancelAddAddress: () => void;
  onSaveAddress: () => void;
  onContinue: () => void;
  onBack: () => void;
}

export function CheckoutAddressScreen({
  addresses,
  addressMode,
  selectedAddressId,
  draftAddress,
  isLoading,
  error,
  onSelectAddress,
  onChangeDraft,
  onStartAddAddress,
  onCancelAddAddress,
  onSaveAddress,
  onContinue,
  onBack,
}: CheckoutAddressScreenProps) {
  return (
    <View style={styles.container}>
      <AppHeader
        title="Teslimat Adresi"
        subtitle={addressMode === 'list' ? 'Kayitli adreslerinden birini sec' : 'Yeni teslimat adresini ekle'}
      />
      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {error ? <Text style={styles.error}>{error instanceof Error ? error.message : 'Adresler getirilemedi.'}</Text> : null}

        {addressMode === 'list' ? (
          <>
            <View style={styles.sectionHeader}>
              <Text style={styles.title}>Adreslerim</Text>
              <Text style={styles.detail}>Devam etmek icin bir adres sec.</Text>
            </View>
            {addresses.length === 0 ? <Text style={styles.emptyText}>Kayitli adresin bulunmuyor.</Text> : null}
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
                {selectedAddressId === address.id ? <Text style={styles.selectedText}>Secildi</Text> : null}
              </Pressable>
            ))}
            <Pressable style={styles.secondaryButton} onPress={onStartAddAddress}>
              <Text style={styles.secondaryText}>+ Yeni adres ekle</Text>
            </Pressable>
            <Pressable style={styles.primaryButton} disabled={isLoading || !selectedAddressId} onPress={onContinue}>
              <Text style={styles.primaryText}>{isLoading ? 'Hazirlaniyor...' : 'Secili adresle devam et'}</Text>
            </Pressable>
          </>
        ) : (
          <>
            <View style={styles.card}>
              <Text style={styles.title}>Yeni adres bilgileri</Text>
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
            <Pressable style={styles.primaryButton} disabled={isLoading} onPress={onSaveAddress}>
              <Text style={styles.primaryText}>{isLoading ? 'Kaydediliyor...' : 'Adresi kaydet'}</Text>
            </Pressable>
            <Pressable style={styles.secondaryButton} onPress={onCancelAddAddress}>
              <Text style={styles.secondaryText}>Adres listesine don</Text>
            </Pressable>
          </>
        )}
        <Pressable style={styles.secondaryButton} onPress={onBack}>
          <Text style={styles.secondaryText}>Sepete Don</Text>
        </Pressable>
      </ScrollView>
    </View>
  );
}
