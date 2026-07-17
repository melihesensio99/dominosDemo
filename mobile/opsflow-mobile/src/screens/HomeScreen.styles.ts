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
});
