import { ActivityIndicator, Text, View } from 'react-native';
import { styles } from './LoadingOverlay.styles';

interface LoadingOverlayProps {
  message: string | null;
}

export function LoadingOverlay({ message }: LoadingOverlayProps) {
  if (!message) {
    return null;
  }

  return (
    <View style={styles.overlay} pointerEvents="auto">
      <View style={styles.card}>
        <ActivityIndicator color="#fff" size="large" />
        <Text style={styles.text}>{message}</Text>
      </View>
    </View>
  );
}
