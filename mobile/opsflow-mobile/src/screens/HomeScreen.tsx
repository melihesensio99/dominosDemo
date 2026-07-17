import { Pressable, Text, View } from 'react-native';
import { ERROR_MESSAGES } from '../constants/errorMessages';
import { AppHeader } from '../components/AppHeader';
import { SectionCard } from '../components/SectionCard';
import { styles } from './HomeScreen.styles';

interface HomeScreenProps {
  onGoMenu: () => void;
  lastOrderStatus?: string;
  isLoading?: boolean;
  error?: unknown;
}

export function HomeScreen({ onGoMenu, lastOrderStatus, isLoading, error }: HomeScreenProps) {
  return (
    <View style={styles.container}>
      <AppHeader title="Domino's benzeri" subtitle="Basit sipariş uygulaması" badge="MVP" />

      <View style={styles.content}>
        <SectionCard>
          <Text style={styles.heroTitle}>Sıcak pizza, hızlı sepet, net sipariş.</Text>
          <Text style={styles.heroText}>Menüye gir, ürünü seç, sepete ekle ve sipariş ver.</Text>
          <View style={styles.actions}>
            <Pressable style={styles.primaryButton} onPress={onGoMenu}>
              <Text style={styles.primaryButtonText}>Menüye Git</Text>
            </Pressable>
            <Pressable style={styles.secondaryButton} onPress={onGoMenu}>
              <Text style={styles.secondaryButtonText}>Siparişe Başla</Text>
            </Pressable>
          </View>
        </SectionCard>

        <SectionCard title="Son durum">
          <Text style={styles.infoText}>
            {lastOrderStatus
              ? `Son sipariş durumun: ${lastOrderStatus}`
              : isLoading
                ? 'Son sipariş bilgisi yükleniyor...'
                : error
                  ? error instanceof Error
                    ? error.message
                    : ERROR_MESSAGES.ORDER_CREATE_FAILED
                  : ERROR_MESSAGES.ORDER_EMPTY}
          </Text>
        </SectionCard>
      </View>
    </View>
  );
}
