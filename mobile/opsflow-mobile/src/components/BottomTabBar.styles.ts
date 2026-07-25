import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    backgroundColor: theme.colors.surface,
    borderTopWidth: 1,
    borderTopColor: theme.colors.border,
    paddingHorizontal: 10,
    paddingTop: 8,
    paddingBottom: 14,
  },
  tab: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    gap: 6,
  },
  iconWrap: { height: 24, alignItems: 'center', justifyContent: 'center', position: 'relative' },
  icon: { color: theme.colors.muted, fontSize: 21 },
  iconActive: { color: theme.colors.primary },
  badge: { position: 'absolute', top: -10, right: -18, minWidth: 19, height: 19, paddingHorizontal: 4, borderRadius: 10, alignItems: 'center', justifyContent: 'center', backgroundColor: theme.colors.danger, borderWidth: 2, borderColor: theme.colors.surface },
  badgeText: { color: '#fff', fontSize: 10, lineHeight: 12, fontWeight: '900' },
  label: {
    color: theme.colors.muted,
    fontSize: 11,
    fontWeight: '600',
  },
  labelActive: {
    color: theme.colors.primary,
  },
  indicator: {
    width: 28,
    height: 4,
    borderRadius: 999,
    backgroundColor: 'transparent',
  },
  indicatorActive: {
    backgroundColor: theme.colors.primary,
  },
});
