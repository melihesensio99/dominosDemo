import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  card: {
    backgroundColor: theme.colors.surface,
    borderRadius: theme.radius.lg,
    padding: theme.spacing.lg,
    borderWidth: 1,
    borderColor: theme.colors.border,
    gap: 8,
  },
  topRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  categoryBadge: {
    backgroundColor: theme.colors.primarySoft,
    paddingHorizontal: 10,
    paddingVertical: 6,
    borderRadius: 999,
  },
  categoryText: {
    color: theme.colors.primaryDark,
    fontSize: 12,
    fontWeight: '700',
  },
  price: {
    color: theme.colors.text,
    fontSize: 16,
    fontWeight: '800',
  },
  title: {
    color: theme.colors.text,
    fontSize: 18,
    fontWeight: '800',
  },
  description: {
    color: theme.colors.muted,
    lineHeight: 20,
  },
  footer: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    gap: 12,
    marginTop: 4,
  },
  meta: {
    color: theme.colors.muted,
    fontSize: 12,
  },
  button: {
    backgroundColor: theme.colors.primary,
    paddingHorizontal: 14,
    paddingVertical: 10,
    borderRadius: 16,
  },
  buttonText: {
    color: '#fff',
    fontWeight: '700',
    fontSize: 12,
  },
});
