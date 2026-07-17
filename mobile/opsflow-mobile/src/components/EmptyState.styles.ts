import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  card: {
    backgroundColor: theme.colors.surface,
    borderRadius: theme.radius.lg,
    padding: 24,
    borderWidth: 1,
    borderColor: theme.colors.border,
    alignItems: 'center',
    gap: 10,
  },
  icon: {
    fontSize: 44,
  },
  title: {
    color: theme.colors.text,
    fontSize: 18,
    fontWeight: '800',
  },
  message: {
    color: theme.colors.muted,
    textAlign: 'center',
    lineHeight: 20,
  },
  button: {
    marginTop: 8,
    backgroundColor: theme.colors.primary,
    borderRadius: 18,
    paddingHorizontal: 18,
    paddingVertical: 12,
  },
  buttonText: {
    color: '#fff',
    fontWeight: '800',
  },
});
