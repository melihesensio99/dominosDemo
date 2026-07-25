import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: theme.colors.background },
  scroll: { flex: 1 },
  content: { padding: theme.spacing.lg, paddingBottom: 40, gap: 14 },
  card: { backgroundColor: theme.colors.surface, borderRadius: 22, padding: 16, borderWidth: 1, borderColor: theme.colors.border, gap: 12 },
  sectionTitleRow: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'flex-start' },
  title: { color: theme.colors.text, fontSize: 17, fontWeight: '900' },
  detail: { color: theme.colors.muted, marginTop: 6, lineHeight: 20 },
  infoIcon: { width: 24, height: 24, borderRadius: 12, textAlign: 'center', textAlignVertical: 'center', color: '#fff', backgroundColor: theme.colors.muted, fontWeight: '900' },
  selectButton: { flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', padding: 14, borderRadius: 16, backgroundColor: theme.colors.surfaceSoft, borderWidth: 1, borderColor: theme.colors.primarySoft },
  selectTextGroup: { flex: 1, paddingRight: 10 },
  selectLabel: { color: theme.colors.primaryDark, fontWeight: '800', fontSize: 12 },
  selectValue: { color: theme.colors.text, fontWeight: '800', marginTop: 4 },
  selectArrow: { color: theme.colors.primary, fontSize: 22, fontWeight: '900' },
  selectOptions: { borderWidth: 1, borderColor: theme.colors.border, borderRadius: 16, overflow: 'hidden' },
  selectOption: { padding: 14, backgroundColor: theme.colors.surface, borderBottomWidth: 1, borderBottomColor: theme.colors.border },
  selectOptionSelected: { backgroundColor: theme.colors.primarySoft },
  optionTitle: { color: theme.colors.text, fontWeight: '900' },
  emptyText: { color: theme.colors.muted, padding: 14 },
  input: { borderWidth: 1, borderColor: theme.colors.border, backgroundColor: theme.colors.surfaceSoft, borderRadius: 16, paddingHorizontal: 14, paddingVertical: 12, color: theme.colors.text, marginTop: 2 },
  primaryButton: { backgroundColor: theme.colors.primary, borderRadius: 18, paddingVertical: 15, alignItems: 'center', marginTop: 4 },
  primaryText: { color: '#fff', fontWeight: '800' },
  secondaryButton: { borderWidth: 1, borderColor: theme.colors.border, borderRadius: 18, paddingVertical: 14, alignItems: 'center' },
  secondaryText: { color: theme.colors.text, fontWeight: '800' },
  error: { color: theme.colors.danger, fontWeight: '700' },
});
