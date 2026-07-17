import { Text, View } from 'react-native';
import { styles } from './AppHeader.styles';

interface AppHeaderProps {
  title: string;
  subtitle?: string;
  badge?: string;
}

export function AppHeader({ title, subtitle, badge }: AppHeaderProps) {
  return (
    <View style={styles.container}>
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
