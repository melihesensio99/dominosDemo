import { ActivityIndicator, Text, View } from 'react-native';
import { styles } from './StatusBanner.styles';

type StatusBannerVariant = 'loading' | 'error' | 'success';

interface StatusBannerProps {
  variant: StatusBannerVariant;
  message: string;
}

const variantMeta: Record<StatusBannerVariant, { title: string; accessibilityLabel: string }> = {
  loading: {
    title: 'Yükleniyor',
    accessibilityLabel: 'İçerik yükleniyor',
  },
  error: {
    title: 'Hata',
    accessibilityLabel: 'Bir hata oluştu',
  },
  success: {
    title: 'Tamamlandı',
    accessibilityLabel: 'İşlem başarılı',
  },
};

export function StatusBanner({ variant, message }: StatusBannerProps) {
  return (
    <View
      style={[
        styles.container,
        variant === 'loading' && styles.loading,
        variant === 'error' && styles.error,
        variant === 'success' && styles.success,
      ]}
      accessibilityRole="alert"
      accessibilityLabel={variantMeta[variant].accessibilityLabel}
    >
      {variant === 'loading' ? (
        <View style={styles.row}>
          <ActivityIndicator color="#fff" />
          <View style={styles.copy}>
            <Text style={styles.title}>{variantMeta[variant].title}</Text>
            <Text style={styles.text}>{message}</Text>
          </View>
        </View>
      ) : (
        <View style={styles.row}>
          <View style={styles.badge}>
            <Text style={styles.badgeText}>{variantMeta[variant].title}</Text>
          </View>
          <View style={styles.copy}>
            <Text style={styles.title}>{variantMeta[variant].title}</Text>
            <Text style={styles.text}>{message}</Text>
          </View>
        </View>
      )}
    </View>
  );
}
