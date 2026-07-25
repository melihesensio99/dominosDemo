import { StyleSheet } from 'react-native';
import { theme } from '../theme';

export const styles = StyleSheet.create({
  container: {
    marginHorizontal: -theme.spacing.lg,
  },
  content: {
    gap: 32,
    paddingHorizontal: theme.spacing.lg,
  },
  image: {
    height: 201,
    borderRadius: theme.radius.lg,
    backgroundColor: theme.colors.surface,
  },
  hint: {
    alignSelf: 'center',
    width: 36,
    height: 4,
    marginTop: 8,
    borderRadius: 4,
    backgroundColor: theme.colors.border,
  },
});
