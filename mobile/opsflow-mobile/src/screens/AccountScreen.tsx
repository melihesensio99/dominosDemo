import { Pressable, ScrollView, Text, TextInput, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import { SectionCard } from '../components/SectionCard';
import type { SessionUser } from '../types/auth';
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
}: AccountScreenProps) {
  return (
    <View style={styles.container}>
      <AppHeader title="Hesabım" subtitle="Giriş yap, oturumu bağla" />

      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
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
          />
          <TextInput
            style={styles.input}
            placeholder="Şifre"
            placeholderTextColor={theme.colors.muted}
            secureTextEntry
            value={password}
            onChangeText={onChangePassword}
          />

          <Pressable style={[styles.authButton, isLoading && { opacity: 0.7 }]} disabled={Boolean(isLoading)} onPress={onAuth}>
            <Text style={styles.authButtonText}>{isLoading ? 'İşleniyor...' : mode === 'login' ? 'Giriş Yap' : 'Kayıt Ol'}</Text>
          </Pressable>
          {error ? (
            <Text style={styles.errorText}>{error instanceof Error ? error.message : 'İşlem sırasında hata oluştu.'}</Text>
          ) : null}
        </SectionCard>

        <SectionCard title="Oturum">
          {user ? (
            <>
              <Text style={styles.info}>E-posta: {user.email}</Text>
              <Text style={styles.info}>Rol: {user.role}</Text>
              <Text style={styles.info}>Kullanıcı ID: {user.userId}</Text>

              <Pressable style={styles.signOutButton} onPress={onSignOut}>
                <Text style={styles.signOutText}>Çıkış Yap</Text>
              </Pressable>
            </>
          ) : (
            <Text style={styles.info}>Henüz giriş yapılmadı.</Text>
          )}
        </SectionCard>
      </ScrollView>
    </View>
  );
}
