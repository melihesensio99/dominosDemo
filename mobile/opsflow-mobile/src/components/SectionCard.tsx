import type { ReactNode } from 'react';
import { Text, View } from 'react-native';
import { styles } from './SectionCard.styles';

interface SectionCardProps {
  title?: string;
  children: ReactNode;
}

export function SectionCard({ title, children }: SectionCardProps) {
  return (
    <View style={styles.card}>
      {title ? <Text style={styles.title}>{title}</Text> : null}
      {children}
    </View>
  );
}
