import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 12,
    backgroundColor: theme.colors.primary,
    paddingHorizontal: theme.spacing.lg,
    paddingTop: 18,
    paddingBottom: 20,
    justifyContent: 'space-between',
  },
  backButton: { width: 36, height: 36, alignItems: 'center', justifyContent: 'center' },
  brandRow: { flexDirection: 'row', alignItems: 'center', flex: 1, gap: 10 },
  logo: { width: 42, height: 42, borderRadius: 10 },
  logoFallback: { width: 42, height: 42, borderRadius: 10, alignItems: 'center', justifyContent: 'center', backgroundColor: '#fff' },
  logoFallbackText: { color: theme.colors.primary, fontWeight: '900', fontSize: 13 },
  backText: { color: '#fff', fontSize: 38, lineHeight: 38, fontWeight: '300' },
  title: {
    color: '#fff',
    fontSize: 20,
    fontWeight: '800',
  },
  subtitle: {
    color: 'rgba(255,255,255,0.88)',
    marginTop: 4,
    fontSize: 12,
  },
  badge: {
    backgroundColor: 'rgba(255,255,255,0.18)',
    borderRadius: 999,
    paddingHorizontal: 12,
    paddingVertical: 8,
  },
  badgeText: {
    color: '#fff',
    fontSize: 12,
    fontWeight: '700',
  },
});
