import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: theme.colors.background },
  content: { padding: theme.spacing.lg, paddingBottom: 150, gap: 16 },
  image: { width: '100%', height: 220, borderRadius: theme.radius.lg, backgroundColor: theme.colors.surface },
  title: { color: theme.colors.text, fontSize: 26, fontWeight: '900' },
  description: { color: theme.colors.muted, lineHeight: 21 },
  group: { backgroundColor: theme.colors.surface, borderRadius: theme.radius.lg, padding: 16, gap: 10, borderWidth: 1, borderColor: theme.colors.border },
  groupTitle: { color: theme.colors.text, fontSize: 18, fontWeight: '900' },
  option: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: theme.colors.border },
  optionSelected: { backgroundColor: theme.colors.primarySoft, borderRadius: 12, paddingHorizontal: 10 },
  optionName: { color: theme.colors.text, fontWeight: '700' },
  optionPrice: { color: theme.colors.muted, fontWeight: '700' },
  error: { color: theme.colors.danger, fontWeight: '700' },
  bottomBar: { flexDirection: 'row', alignItems: 'center', gap: 12, padding: 14, backgroundColor: theme.colors.surface, borderTopWidth: 1, borderTopColor: theme.colors.border },
  quantityControl: { flexDirection: 'row', alignItems: 'center', gap: 10, backgroundColor: theme.colors.surfaceSoft, borderRadius: 18, padding: 6 },
  quantityButton: { width: 34, height: 34, borderRadius: 17, alignItems: 'center', justifyContent: 'center', backgroundColor: theme.colors.primary },
  quantityButtonText: { color: '#fff', fontSize: 24, lineHeight: 26, fontWeight: '800' },
  quantity: { minWidth: 22, textAlign: 'center', color: theme.colors.text, fontSize: 17, fontWeight: '800' },
  addButton: { flex: 1, flexDirection: 'row', justifyContent: 'space-between', backgroundColor: theme.colors.success, borderRadius: 18, paddingHorizontal: 16, paddingVertical: 15, alignItems: 'center' },
  addButtonText: { color: '#fff', fontSize: 16, fontWeight: '900' },
  addButtonPrice: { color: '#fff', fontSize: 17, fontWeight: '900' },
});
