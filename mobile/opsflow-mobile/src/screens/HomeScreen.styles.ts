import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: theme.colors.background,
  },
  content: {
    padding: theme.spacing.lg,
    gap: theme.spacing.lg,
    paddingBottom: 140,
  },
  sectionTitle: { color: theme.colors.text, fontSize: 21, fontWeight: '900' },
  chips: { gap: 10, paddingBottom: 2 },
  chip: { flexDirection: 'row', alignItems: 'center', gap: 7, backgroundColor: theme.colors.surface, borderWidth: 1, borderColor: theme.colors.border, borderRadius: 18, paddingHorizontal: 16, paddingVertical: 12 },
  chipActive: { backgroundColor: theme.colors.primary, borderColor: theme.colors.primary },
  chipText: { color: theme.colors.text, fontWeight: '800' },
  chipTextActive: { color: '#fff' },
  categoryIcon: { fontSize: 18 },
  productList: { gap: 12 },
  statusCard: { backgroundColor: theme.colors.surface, borderRadius: theme.radius.lg, padding: theme.spacing.lg, borderWidth: 1, borderColor: theme.colors.border },
  orderBanner: { backgroundColor: theme.colors.primarySoft, borderRadius: theme.radius.lg, padding: theme.spacing.lg, borderWidth: 1, borderColor: theme.colors.primary },
  orderBannerTitle: { color: theme.colors.primaryDark, fontSize: 18, fontWeight: '900' },
  orderBannerText: { color: theme.colors.primaryDark, marginTop: 6 },
  heroTitle: {
    color: theme.colors.text,
    fontSize: 28,
    lineHeight: 34,
    fontWeight: '900',
  },
  heroText: {
    color: theme.colors.muted,
    marginTop: 10,
    lineHeight: 21,
  },
  actions: {
    flexDirection: 'row',
    gap: 10,
    marginTop: 16,
  },
  primaryButton: {
    flex: 1,
    backgroundColor: theme.colors.primary,
    paddingVertical: 14,
    borderRadius: 18,
    alignItems: 'center',
  },
  primaryButtonText: {
    color: '#fff',
    fontWeight: '800',
  },
  secondaryButton: {
    flex: 1,
    backgroundColor: theme.colors.primarySoft,
    paddingVertical: 14,
    borderRadius: 18,
    alignItems: 'center',
  },
  secondaryButtonText: {
    color: theme.colors.primaryDark,
    fontWeight: '800',
  },
  infoText: {
    color: theme.colors.muted,
    lineHeight: 20,
  },
  orderStatus: {
    color: theme.colors.primaryDark,
    fontSize: 18,
    lineHeight: 24,
    fontWeight: '800',
  },
  orderHint: {
    color: theme.colors.muted,
    lineHeight: 20,
    marginTop: 8,
  },
});
