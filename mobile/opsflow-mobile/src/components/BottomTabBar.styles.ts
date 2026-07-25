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
  iconWrap: { height: 24, alignItems: 'center', justifyContent: 'center' },
  icon: { color: theme.colors.muted, fontSize: 21 },
  iconActive: { color: theme.colors.primary },
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
