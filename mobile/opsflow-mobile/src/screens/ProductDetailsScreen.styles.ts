import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: theme.colors.background },
  content: { padding: theme.spacing.lg, paddingBottom: 36, gap: 16 },
  title: { color: theme.colors.text, fontSize: 26, fontWeight: '900' },
  description: { color: theme.colors.muted, lineHeight: 21 },
  group: { backgroundColor: theme.colors.surface, borderRadius: theme.radius.lg, padding: 16, gap: 10, borderWidth: 1, borderColor: theme.colors.border },
  groupTitle: { color: theme.colors.text, fontSize: 18, fontWeight: '900' },
  option: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: theme.colors.border },
  optionSelected: { backgroundColor: theme.colors.primarySoft, borderRadius: 12, paddingHorizontal: 10 },
  optionName: { color: theme.colors.text, fontWeight: '700' },
  optionPrice: { color: theme.colors.muted, fontWeight: '700' },
  error: { color: theme.colors.danger, fontWeight: '700' },
  addButton: { backgroundColor: theme.colors.primary, borderRadius: 18, paddingVertical: 16, alignItems: 'center' },
  addButtonText: { color: '#fff', fontSize: 16, fontWeight: '900' },
});
