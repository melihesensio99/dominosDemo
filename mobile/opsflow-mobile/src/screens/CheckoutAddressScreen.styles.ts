import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: theme.colors.background },
  content: { padding: theme.spacing.lg, paddingBottom: 40, gap: 14 },
  card: { backgroundColor: theme.colors.surface, borderRadius: 20, padding: 16, borderWidth: 1, borderColor: theme.colors.border },
  cardSelected: { borderColor: theme.colors.primary, borderWidth: 2 },
  title: { color: theme.colors.text, fontSize: 16, fontWeight: '800' },
  detail: { color: theme.colors.muted, marginTop: 6, lineHeight: 20 },
  input: { borderWidth: 1, borderColor: theme.colors.border, backgroundColor: theme.colors.surfaceSoft, borderRadius: 16, paddingHorizontal: 14, paddingVertical: 12, color: theme.colors.text, marginTop: 10 },
  primaryButton: { backgroundColor: theme.colors.primary, borderRadius: 18, paddingVertical: 15, alignItems: 'center', marginTop: 4 },
  primaryText: { color: '#fff', fontWeight: '800' },
  secondaryButton: { borderWidth: 1, borderColor: theme.colors.border, borderRadius: 18, paddingVertical: 14, alignItems: 'center' },
  secondaryText: { color: theme.colors.text, fontWeight: '800' },
  error: { color: theme.colors.danger, fontWeight: '700' },
});
