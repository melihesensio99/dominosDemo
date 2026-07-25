import { Pressable, Text, View } from 'react-native';
import { styles } from './AppHeader.styles';

interface AppHeaderProps {
  title: string;
  subtitle?: string;
  badge?: string;
  onBack?: () => void;
}

export function AppHeader({ title, subtitle, badge, onBack }: AppHeaderProps) {
  return (
    <View style={styles.container}>
      {onBack ? <Pressable onPress={onBack} style={styles.backButton}><Text style={styles.backText}>‹</Text></Pressable> : null}
      <View>
        <Text style={styles.title}>{title}</Text>
        {subtitle ? <Text style={styles.subtitle}>{subtitle}</Text> : null}
      </View>
      {badge ? (
        <View style={styles.badge}>
          <Text style={styles.badgeText}>{badge}</Text>
        </View>
      ) : null}
    </View>
  );
}
