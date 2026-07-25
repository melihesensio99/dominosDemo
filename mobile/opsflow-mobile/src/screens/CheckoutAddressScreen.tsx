import { useState } from 'react';
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
  addressTitle: string;
  isLoading?: boolean;
  error?: unknown;
  onSelectAddress: (id: string) => void;
  onChangeDraft: (address: Address) => void;
  onChangeAddressTitle: (value: string) => void;
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
  addressTitle,
  isLoading,
  error,
  onSelectAddress,
  onChangeDraft,
  onChangeAddressTitle,
  onStartAddAddress,
  onCancelAddAddress,
  onSaveAddress,
  onContinue,
  onBack,
}: CheckoutAddressScreenProps) {
  const [isAddressListOpen, setIsAddressListOpen] = useState(false);
  const selectedAddress = addresses.find((address) => address.id === selectedAddressId);

  return (
    <View style={styles.container}>
      <AppHeader title="Siparis Onayi" subtitle="Teslimat bilgilerini sec ve devam et" />
      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {error ? <Text style={styles.error}>{error instanceof Error ? error.message : 'Adresler getirilemedi.'}</Text> : null}

        {addressMode === 'list' ? (
          <>
            <View style={styles.card}>
              <View style={styles.sectionTitleRow}>
                <View>
                  <Text style={styles.title}>Teslimat adresim</Text>
                  <Text style={styles.detail}>Siparisin bu adrese teslim edilecek.</Text>
                </View>
                <Text style={styles.infoIcon}>i</Text>
              </View>

              <Pressable style={styles.selectButton} onPress={() => setIsAddressListOpen((current) => !current)}>
                <View style={styles.selectTextGroup}>
                  <Text style={styles.selectLabel}>Adres sec</Text>
                  <Text style={styles.selectValue} numberOfLines={2}>
                    {selectedAddress ? `${selectedAddress.title} - ${selectedAddress.city}` : 'Kayitli adres seciniz'}
                  </Text>
                </View>
                <Text style={styles.selectArrow}>{isAddressListOpen ? '^' : 'v'}</Text>
              </Pressable>

              {isAddressListOpen ? (
                <View style={styles.selectOptions}>
                  {addresses.length === 0 ? <Text style={styles.emptyText}>Kayitli adresin bulunmuyor.</Text> : null}
                  {addresses.map((address) => (
                    <Pressable
                      key={address.id}
                      style={[styles.selectOption, selectedAddressId === address.id && styles.selectOptionSelected]}
                      onPress={() => {
                        onSelectAddress(address.id);
                        setIsAddressListOpen(false);
                      }}
                    >
                      <Text style={styles.optionTitle}>{address.title}</Text>
                      <Text style={styles.detail} numberOfLines={2}>
                        {address.street}, {address.district}, {address.city}
                      </Text>
                    </Pressable>
                  ))}
                </View>
              ) : null}

              <Pressable style={styles.secondaryButton} onPress={onStartAddAddress}>
                <Text style={styles.secondaryText}>+ Yeni adres ekle</Text>
              </Pressable>
            </View>
            <Pressable style={styles.primaryButton} disabled={isLoading || !selectedAddressId} onPress={onContinue}>
              <Text style={styles.primaryText}>{isLoading ? 'Hazirlaniyor...' : 'Secili adresle devam et'}</Text>
            </Pressable>
          </>
        ) : (
          <>
            <View style={styles.card}>
              <Text style={styles.title}>Yeni adres ekle</Text>
              <Text style={styles.detail}>Adresini kaydet, sonraki siparislerinde tekrar kullan.</Text>
              <TextInput
                style={styles.input}
                placeholder="Adres adi (Ev, Is, Annemin evi)"
                placeholderTextColor={theme.colors.muted}
                value={addressTitle}
                onChangeText={onChangeAddressTitle}
              />
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
              <Text style={styles.secondaryText}>Adres secimine don</Text>
            </Pressable>
          </>
        )}

        <Pressable style={styles.secondaryButton} onPress={onBack}>
          <Text style={styles.secondaryText}>Sepete don</Text>
        </Pressable>
      </ScrollView>
    </View>
  );
}
