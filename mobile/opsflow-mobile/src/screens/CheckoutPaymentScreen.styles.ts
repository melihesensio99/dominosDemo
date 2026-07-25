import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: theme.colors.background },
  content: { padding: theme.spacing.lg, paddingBottom: 40, gap: 14 },
  card: { backgroundColor: theme.colors.surface, borderRadius: 20, padding: 16, borderWidth: 1, borderColor: theme.colors.border },
  title: { color: theme.colors.text, fontSize: 16, fontWeight: '800', marginBottom: 12 },
  summary: { color: theme.colors.muted, lineHeight: 22 },
  options: { flexDirection: 'row', flexWrap: 'wrap', gap: 10 },
  option: { paddingHorizontal: 14, paddingVertical: 11, borderRadius: 999, backgroundColor: theme.colors.surfaceSoft, borderWidth: 1, borderColor: theme.colors.border },
  optionSelected: { backgroundColor: theme.colors.primary, borderColor: theme.colors.primary },
  optionText: { color: theme.colors.text, fontWeight: '700' },
  optionTextSelected: { color: '#fff' },
  primaryButton: { backgroundColor: theme.colors.primary, borderRadius: 18, paddingVertical: 15, alignItems: 'center' },
  primaryText: { color: '#fff', fontWeight: '800' },
  secondaryButton: { borderWidth: 1, borderColor: theme.colors.border, borderRadius: 18, paddingVertical: 14, alignItems: 'center' },
  secondaryText: { color: theme.colors.text, fontWeight: '800' },
  error: { color: theme.colors.danger, fontWeight: '700' },
});
