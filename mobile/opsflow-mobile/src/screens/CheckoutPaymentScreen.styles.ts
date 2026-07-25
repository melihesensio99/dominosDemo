import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: theme.colors.background },
  scroll: { flex: 1 },
  content: { padding: theme.spacing.lg, paddingBottom: 40, gap: 14 },
  card: { backgroundColor: theme.colors.surface, borderRadius: 20, padding: 16, borderWidth: 1, borderColor: theme.colors.border },
  title: { color: theme.colors.text, fontSize: 16, fontWeight: '800', marginBottom: 12 },
  summary: { color: theme.colors.muted, lineHeight: 22 },
  selectButton: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', paddingHorizontal: 14, paddingVertical: 13, borderRadius: 14, backgroundColor: theme.colors.surfaceSoft, borderWidth: 1, borderColor: theme.colors.border },
  selectText: { color: theme.colors.text, fontWeight: '700' },
  selectArrow: { color: theme.colors.primary, fontSize: 22, fontWeight: '800' },
  selectOptions: { marginTop: 8, overflow: 'hidden', borderRadius: 14, borderWidth: 1, borderColor: theme.colors.border },
  selectOption: { paddingHorizontal: 14, paddingVertical: 13, backgroundColor: theme.colors.surface, borderBottomWidth: 1, borderBottomColor: theme.colors.border },
  selectOptionText: { color: theme.colors.text, fontWeight: '700' },
  noteInput: { minHeight: 82, textAlignVertical: 'top', padding: 12, color: theme.colors.text, backgroundColor: theme.colors.surfaceSoft, borderRadius: 14, borderWidth: 1, borderColor: theme.colors.border },
  placeholder: { color: theme.colors.muted },
  primaryButton: { backgroundColor: theme.colors.primary, borderRadius: 18, paddingVertical: 15, alignItems: 'center' },
  primaryText: { color: '#fff', fontWeight: '800' },
  secondaryButton: { borderWidth: 1, borderColor: theme.colors.border, borderRadius: 18, paddingVertical: 14, alignItems: 'center' },
  secondaryText: { color: theme.colors.text, fontWeight: '800' },
  error: { color: theme.colors.danger, fontWeight: '700' },
});
