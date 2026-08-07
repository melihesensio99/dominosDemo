import { Pressable, ScrollView, Text, TextInput, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import { SectionCard } from '../components/SectionCard';
import type { SessionUser } from '../types/auth';
import type { Order } from '../types/order';
import type { Product } from '../types/catalog';
import { styles } from './AccountScreen.styles';
import { theme } from '../theme';

interface AccountScreenProps {
  user: SessionUser | null;
  mode: 'login' | 'register';
  email: string;
  password: string;
  isLoading?: boolean;
  error?: unknown;
  onChangeMode: (mode: 'login' | 'register') => void;
  onChangeEmail: (value: string) => void;
  onChangePassword: (value: string) => void;
  onAuth: () => void;
  onSignOut: () => void;
  orders?: Order[];
  products?: Product[];
  cancellingOrderId?: string | null;
  onCancelOrder?: (orderId: string) => void;
}

export function AccountScreen({
  user,
  mode,
  email,
  password,
  isLoading,
  error,
  onChangeMode,
  onChangeEmail,
  onChangePassword,
  onAuth,
  onSignOut,
  orders = [],
  products = [],
  cancellingOrderId = null,
  onCancelOrder,
}: AccountScreenProps) {
  const isAuthenticated = Boolean(user);

  return (
    <View style={styles.container}>
      <AppHeader
        title="Hesabım"
        subtitle={isAuthenticated ? 'Hesap bilgilerin' : 'Giriş yap veya yeni hesap oluştur'}
      />

      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {isAuthenticated ? (
          <>
            <SectionCard title="Hesap Bilgileri">
              <Text style={styles.info}>E-posta: {user?.email}</Text>

              <Pressable style={styles.signOutButton} onPress={onSignOut}>
                <Text style={styles.signOutText}>Çıkış Yap</Text>
              </Pressable>
            </SectionCard>

            <SectionCard title="Sipariş Geçmişi">
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
                    const canCancel = ['pending', 'confirmed'].includes(order.status.toLowerCase());

                    return (
                      <View key={order.id} style={{ borderBottomWidth: 1, borderBottomColor: '#f1f5f9', paddingVertical: 12 }}>
                        <View style={{ flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 6 }}>
                          <Text style={{ fontWeight: 'bold', fontSize: 13, color: '#0f172a' }}>
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
                                <Text style={{ fontSize: 13, color: '#334155' }}>
                                  • {item.quantity} x {prod?.name ?? 'Ürün'}
                                </Text>
                                {optionsStr ? (
                                  <Text style={{ fontSize: 11, color: '#94a3b8', marginLeft: 10 }}>
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
                            backgroundColor: order.status.toLowerCase() === 'delivered' ? '#dcfce7' : order.status.toLowerCase() === 'cancelled' ? '#fee2e2' : '#e0f2fe',
                          }}>
                            <Text style={{
                              fontSize: 10,
                              fontWeight: 'bold',
                              color: order.status.toLowerCase() === 'delivered' ? '#15803d' : order.status.toLowerCase() === 'cancelled' ? '#b91c1c' : '#0369a1',
                            }}>
                              {statusLabel}
                            </Text>
                          </View>
                          {order.totalPrice ? (
                            <Text style={{ fontWeight: '800', fontSize: 13, color: '#16a34a' }}>
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
          </>
        ) : (
          <SectionCard title="Giriş">
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
