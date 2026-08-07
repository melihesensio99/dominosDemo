import { Image, Pressable, Text, View } from 'react-native';
import { useState } from 'react';
import { styles } from './AppHeader.styles';

interface AppHeaderProps {
  title: string;
  subtitle?: string;
  badge?: string;
  onBack?: () => void;
  logoUrl?: string;
  icon?: string;
}

export function AppHeader({ title, subtitle, badge, onBack, logoUrl, icon }: AppHeaderProps) {
  const [logoFailed, setLogoFailed] = useState(false);

  return (
    <View style={styles.container}>
      {onBack ? <Pressable onPress={onBack} style={styles.backButton}><Text style={styles.backText}>‹</Text></Pressable> : null}
      <View style={styles.brandRow}>
        {icon ? (
          <Text style={{ fontSize: 24, color: '#fff', marginRight: 4 }}>{icon}</Text>
        ) : logoUrl && !logoFailed ? (
          <Image source={{ uri: logoUrl }} style={styles.logo} resizeMode="contain" onError={() => setLogoFailed(true)} />
        ) : logoUrl ? (
          <View style={styles.logoFallback}><Text style={styles.logoFallbackText}>MP</Text></View>
        ) : null}
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
