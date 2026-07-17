import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: theme.colors.background,
  },
  content: {
    padding: theme.spacing.lg,
    paddingBottom: 140,
    gap: 14,
  },
  chips: {
    gap: 10,
    paddingBottom: 4,
  },
  chip: {
    backgroundColor: theme.colors.surface,
    borderWidth: 1,
    borderColor: theme.colors.border,
    color: theme.colors.text,
    paddingHorizontal: 16,
    paddingVertical: 10,
    borderRadius: 999,
    marginRight: 10,
    fontWeight: '700',
    overflow: 'hidden',
  },
  chipActive: {
    backgroundColor: theme.colors.primary,
    borderColor: theme.colors.primary,
    color: '#fff',
  },
  helper: {
    color: theme.colors.muted,
    marginTop: 4,
    marginBottom: 6,
  },
  list: {
    gap: 12,
  },
});
