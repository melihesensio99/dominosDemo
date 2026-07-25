import { Image, Pressable, Text, View } from 'react-native';
import { styles } from './AppHeader.styles';

interface AppHeaderProps {
  title: string;
  subtitle?: string;
  badge?: string;
  onBack?: () => void;
  logoUrl?: string;
}

export function AppHeader({ title, subtitle, badge, onBack, logoUrl }: AppHeaderProps) {
  return (
    <View style={styles.container}>
      {onBack ? <Pressable onPress={onBack} style={styles.backButton}><Text style={styles.backText}>‹</Text></Pressable> : null}
      <View style={styles.brandRow}>
        {logoUrl ? <Image source={{ uri: logoUrl }} style={styles.logo} resizeMode="contain" /> : null}
        <View>
          <Text style={styles.title}>{title}</Text>
          {subtitle ? <Text style={styles.subtitle}>{subtitle}</Text> : null}
        </View>
      </View>
      {badge ? (
        <View style={styles.badge}>
          <Text style={styles.badgeText}>{badge}</Text>
        </View>
      ) : null}
    </View>
  );
}
