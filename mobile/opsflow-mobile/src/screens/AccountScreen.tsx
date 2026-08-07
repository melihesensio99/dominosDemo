import { useState } from 'react';
import { Image, Pressable, ScrollView, Text, TextInput, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import { SectionCard } from '../components/SectionCard';
import type { SessionUser, UserAddress } from '../types/auth';
import type { Order } from '../types/order';
import type { Product } from '../types/catalog';
import { styles } from './AccountScreen.styles';
import { theme } from '../theme';

interface AccountScreenProps {
  user: SessionUser | null;
  mode: 'login' | 'register';
  email: string;
  password: string;
  confirmPassword: string;
  isLoading?: boolean;
  error?: unknown;
  onChangeMode: (mode: 'login' | 'register') => void;
  onChangeEmail: (value: string) => void;
  onChangePassword: (value: string) => void;
  onChangeConfirmPassword: (value: string) => void;
  onAuth: () => void;
  onSignOut: () => void;
  orders?: Order[];
  products?: Product[];
  addresses?: UserAddress[];
  onAddAddress?: (
    title: string,
    street: string,
    district: string,
    city: string,
    postalCode: string,
    country: string
  ) => Promise<any>;
  cancellingOrderId?: string | null;
  onCancelOrder?: (orderId: string) => void;
}

export function AccountScreen({
  user,
  mode,
  email,
  password,
  confirmPassword,
  isLoading,
  error,
  onChangeMode,
  onChangeEmail,
  onChangePassword,
  onChangeConfirmPassword,
  onAuth,
  onSignOut,
  orders = [],
  products = [],
  addresses = [],
  onAddAddress,
  cancellingOrderId = null,
  onCancelOrder,
}: AccountScreenProps) {
  const isAuthenticated = Boolean(user);

  const [subView, setSubView] = useState<'profile' | 'orders' | 'addresses'>('profile');
  const [isAddingAddress, setIsAddingAddress] = useState(false);
  const [newTitle, setNewTitle] = useState('');
  const [newStreet, setNewStreet] = useState('');
  const [newDistrict, setNewDistrict] = useState('');
  const [newCity, setNewCity] = useState('');
  const [newPostalCode, setNewPostalCode] = useState('');
  const [newCountry, setNewCountry] = useState('Türkiye');

  return (
    <View style={styles.container}>
      <AppHeader
        title="Hesabım"
        subtitle={isAuthenticated ? 'Hesap bilgilerin' : 'Giriş yap veya yeni hesap oluştur'}
      />

      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {isAuthenticated ? (
          <>
            {subView === 'profile' && (
              <>
                <View style={styles.profileCard}>
                  <View style={styles.avatar}>
                    <Text style={styles.avatarText}>
                      {user?.email ? user.email.slice(0, 2).toUpperCase() : 'ME'}
                    </Text>
                  </View>
                  <View style={styles.profileDetails}>
                    <Text style={styles.profileName}>
                      {user?.email ? user.email.split('@')[0] : 'melih esen'}
                    </Text>
                    <Text style={styles.profilePhone}>(538) 088 07 90</Text>
                  </View>
                  <Text style={styles.arrowRight}>➔</Text>
                </View>

                {/* Grid Navigation */}
                <View style={styles.gridContainer}>
                  <Pressable style={styles.gridItem} onPress={() => setSubView('orders')}>
                    <View style={styles.gridIconWrap}>
                      <Text style={styles.gridIcon}>🛍️</Text>
                    </View>
                    <Text style={styles.gridLabel}>Siparişlerim</Text>
                  </Pressable>

                  <Pressable style={styles.gridItem} onPress={() => setSubView('addresses')}>
                    <View style={styles.gridIconWrap}>
                      <Text style={styles.gridIcon}>📍</Text>
                    </View>
                    <Text style={styles.gridLabel}>Adreslerim</Text>
                  </Pressable>

                  <Pressable style={styles.gridItem} onPress={() => {}}>
                    <View style={styles.gridIconWrap}>
                      <Text style={styles.gridIcon}>💳</Text>
                    </View>
                    <Text style={styles.gridLabel}>Kartlarım</Text>
                  </Pressable>
                </View>

                {/* Sign Out Button */}
                <Pressable style={styles.signOutButton} onPress={onSignOut}>
                  <Text style={styles.signOutText}>Çıkış Yap</Text>
                </Pressable>
              </>
            )}

            {subView === 'orders' && (
              <SectionCard title="Sipariş Geçmişi">
                <Pressable style={styles.backButton} onPress={() => setSubView('profile')}>
                  <Text style={styles.backButtonText}>⬅ Geri Dön</Text>
                </Pressable>

                {orders.length === 0 ? (
                  <Text style={{ color: theme.colors.muted, fontSize: 13, textAlign: 'center', marginVertical: 14 }}>
                    Henüz sipariş geçmişiniz bulunmuyor.
                  </Text>
                ) : (
                  [...orders]
                    .sort((a, b) => Date.parse(b.createdAt) - Date.parse(a.createdAt))
                    .map((order) => {
                      const date = new Date(order.createdAt).toLocaleDateString('tr-TR', {
                        day: '2-digit',
                        month: '2-digit',
                        year: 'numeric',
                        hour: '2-digit',
                        minute: '2-digit',
                      });
                      const statusLabel =
                        order.status.toLowerCase() === 'pending' ? 'Alındı' :
                        order.status.toLowerCase() === 'confirmed' ? 'Onaylandı' :
                        order.status.toLowerCase() === 'preparing' ? 'Hazırlanıyor' :
                        order.status.toLowerCase() === 'shipped' ? 'Yolda' :
                        order.status.toLowerCase() === 'delivered' ? 'Teslim Edildi' :
                        order.status.toLowerCase() === 'cancelled' ? 'İptal Edildi' : order.status;

                      const canCancel = order.status.toLowerCase() === 'pending';

                      return (
                        <View key={order.id} style={{ borderBottomWidth: 1, borderBottomColor: '#21262d', paddingVertical: 12 }}>
                          <View style={{ flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 6 }}>
                            <Text style={{ fontWeight: 'bold', fontSize: 13, color: '#e6edf3' }}>
                              #{order.id.slice(-8).toUpperCase()}
                            </Text>
                            <Text style={{ fontSize: 12, color: theme.colors.muted }}>
                              {date}
                            </Text>
                          </View>

                          <View style={{ marginBottom: 6 }}>
                            {order.items.map((item, idx) => {
                              const prod = products.find((p) => p.id === item.productId);
                              const optionsStr = prod?.optionGroups
                                ?.flatMap((g) => g.options)
                                ?.filter((o) => item.selectedOptionIds?.includes(o.id))
                                ?.map((o) => o.name)
                                ?.join(', ');

                              return (
                                <View key={`${item.productId}-${idx}`} style={{ marginVertical: 2 }}>
                                  <Text style={{ fontSize: 13, color: '#c9d1d9' }}>
                                    • {item.quantity} x {prod?.name ?? 'Ürün'}
                                  </Text>
                                  {optionsStr ? (
                                    <Text style={{ fontSize: 11, color: '#7d8590', marginLeft: 10 }}>
                                      Tercih: {optionsStr}
                                    </Text>
                                  ) : null}
                                </View>
                              );
                            })}
                          </View>

                          <View style={{ flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginTop: 4 }}>
                            <View style={{
                              paddingHorizontal: 8,
                              paddingVertical: 3,
                              borderRadius: 12,
                              backgroundColor: order.status.toLowerCase() === 'delivered' ? '#0d1f0d' : order.status.toLowerCase() === 'cancelled' ? '#3d1c1c' : '#0d2137',
                            }}>
                              <Text style={{
                                fontSize: 10,
                                fontWeight: 'bold',
                                color: order.status.toLowerCase() === 'delivered' ? '#3fb950' : order.status.toLowerCase() === 'cancelled' ? '#f85149' : '#58a6ff',
                              }}>
                                {statusLabel}
                              </Text>
                            </View>
                            {order.totalPrice ? (
                              <Text style={{ fontWeight: '800', fontSize: 13, color: '#3fb950' }}>
                                {order.totalPrice.toFixed(2)} TL
                              </Text>
                            ) : (
                              <Text style={{ fontSize: 12, color: theme.colors.muted }}>0.00 TL</Text>
                            )}
                          </View>

                          {canCancel && onCancelOrder ? (
                            <Pressable
                              style={styles.cancelOrderButton}
                              disabled={cancellingOrderId === order.id}
                              onPress={() => onCancelOrder(order.id)}
                            >
                              <Text style={styles.cancelOrderText}>
                                {cancellingOrderId === order.id ? 'İptal ediliyor...' : 'Siparişi İptal Et'}
                              </Text>
                            </Pressable>
                          ) : null}
                        </View>
                      );
                    })
                )}
              </SectionCard>
            )}

            {subView === 'addresses' && (
              <SectionCard title="Adreslerim">
                <Pressable style={styles.backButton} onPress={() => { setSubView('profile'); setIsAddingAddress(false); }}>
                  <Text style={styles.backButtonText}>⬅ Geri Dön</Text>
                </Pressable>

                {isAddingAddress ? (
                  <View style={{ marginTop: 10 }}>
                    <Text style={{ fontWeight: '800', fontSize: 14, color: '#e6edf3', marginBottom: 10 }}>Yeni Adres Ekle</Text>
                    
                    <TextInput
                      style={styles.input}
                      placeholder="Adres Başlığı (Örn: Ev, İş)"
                      placeholderTextColor={theme.colors.muted}
                      value={newTitle}
                      onChangeText={setNewTitle}
                    />
                    <TextInput
                      style={styles.input}
                      placeholder="Sokak / Cadde / Apartman"
                      placeholderTextColor={theme.colors.muted}
                      value={newStreet}
                      onChangeText={setNewStreet}
                    />
                    <TextInput
                      style={styles.input}
                      placeholder="İlçe"
                      placeholderTextColor={theme.colors.muted}
                      value={newDistrict}
                      onChangeText={setNewDistrict}
                    />
                    <TextInput
                      style={styles.input}
                      placeholder="Şehir"
                      placeholderTextColor={theme.colors.muted}
                      value={newCity}
                      onChangeText={setNewCity}
                    />
                    <TextInput
                      style={styles.input}
                      placeholder="Posta Kodu"
                      placeholderTextColor={theme.colors.muted}
                      value={newPostalCode}
                      onChangeText={setNewPostalCode}
                      keyboardType="numeric"
                    />
                    <TextInput
                      style={styles.input}
                      placeholder="Ülke"
                      placeholderTextColor={theme.colors.muted}
                      value={newCountry}
                      onChangeText={setNewCountry}
                    />

                    <Pressable
                      style={[styles.authButton, { marginTop: 16 }]}
                      onPress={async () => {
                        if (!newTitle.trim() || !newStreet.trim() || !newDistrict.trim() || !newCity.trim() || !newPostalCode.trim() || !newCountry.trim()) {
                          return;
                        }
                        if (onAddAddress) {
                          try {
                            await onAddAddress(newTitle.trim(), newStreet.trim(), newDistrict.trim(), newCity.trim(), newPostalCode.trim(), newCountry.trim());
                            setIsAddingAddress(false);
                            setNewTitle('');
                            setNewStreet('');
                            setNewDistrict('');
                            setNewCity('');
                            setNewPostalCode('');
                          } catch {
                            // error is handled by hook
                          }
                        }
                      }}
                    >
                      <Text style={styles.authButtonText}>Adresi Kaydet</Text>
                    </Pressable>

                    <Pressable
                      style={[styles.signOutButton, { marginTop: 8, borderColor: '#cbd5e1' }]}
                      onPress={() => setIsAddingAddress(false)}
                    >
                      <Text style={[styles.signOutText, { color: '#475569' }]}>İptal Et</Text>
                    </Pressable>
                  </View>
                ) : (
                  <>
                    <View style={{ marginVertical: 10 }}>
                      {addresses.length === 0 ? (
                        <Text style={{ color: theme.colors.muted, fontSize: 13, textAlign: 'center', marginVertical: 14 }}>
                          Kayıtlı adresiniz bulunmuyor.
                        </Text>
                      ) : (
                        addresses.map((address) => (
                          <View key={address.id} style={styles.addressCard}>
                            <Text style={styles.addressTitle}>{address.title}</Text>
                            <Text style={styles.addressText}>
                              {address.street}, {address.district}, {address.city}, {address.postalCode}, {address.country}
                            </Text>
                          </View>
                        ))
                      )}
                    </View>

                    <Pressable style={styles.addAddressButton} onPress={() => setIsAddingAddress(true)}>
                      <Text style={styles.addAddressButtonText}>➕ Yeni Adres Ekle</Text>
                    </Pressable>
                  </>
                )}
              </SectionCard>
            )}
          </>
        ) : (
          <SectionCard>
            <View style={{ alignItems: 'center', marginBottom: 14 }}>
              <Image
                source={{ uri: 'https://res.cloudinary.com/dc2j01x6b/image/upload/v1786076662/1b8169db840253547a07449fd7c120b4.jpg' }}
                style={{ width: 100, height: 100, borderRadius: 12, borderWidth: 1, borderColor: '#e2e8f0' }}
                resizeMode="cover"
              />
            </View>
            <View style={styles.modeRow}>
              <Pressable
                style={[styles.modeButton, mode === 'login' && styles.modeButtonActive]}
                onPress={() => onChangeMode('login')}
              >
                <Text style={[styles.modeText, mode === 'login' && styles.modeTextActive]}>Giriş</Text>
              </Pressable>
              <Pressable
                style={[styles.modeButton, mode === 'register' && styles.modeButtonActive]}
                onPress={() => onChangeMode('register')}
              >
                <Text style={[styles.modeText, mode === 'register' && styles.modeTextActive]}>Kayıt</Text>
              </Pressable>
            </View>

            <TextInput
              style={styles.input}
              placeholder="E-posta"
              placeholderTextColor={theme.colors.muted}
              value={email}
              onChangeText={onChangeEmail}
              autoCapitalize="none"
              keyboardType="email-address"
            />
            <TextInput
              style={styles.input}
              placeholder="Şifre"
              placeholderTextColor={theme.colors.muted}
              secureTextEntry
              value={password}
              onChangeText={onChangePassword}
            />

            {mode === 'register' ? (
              <TextInput
                style={styles.input}
                placeholder="Şifre tekrar"
                placeholderTextColor={theme.colors.muted}
                secureTextEntry
                value={confirmPassword}
                onChangeText={onChangeConfirmPassword}
              />
            ) : null}

            <Pressable
              style={[styles.authButton, isLoading && { opacity: 0.7 }]}
              disabled={Boolean(isLoading)}
              onPress={onAuth}
            >
              <Text style={styles.authButtonText}>
                {isLoading ? 'İşleniyor...' : mode === 'login' ? 'Giriş Yap' : 'Kayıt Ol'}
              </Text>
            </Pressable>

            {error ? (
              <Text style={styles.errorText}>
                {error instanceof Error ? error.message : 'İşlem sırasında hata oluştu.'}
              </Text>
            ) : null}
          </SectionCard>
        )}
      </ScrollView>
    </View>
  );
}
