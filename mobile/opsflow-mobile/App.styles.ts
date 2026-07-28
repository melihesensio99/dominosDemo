import { StyleSheet } from 'react-native';
import { theme } from './src/theme';

export const styles = StyleSheet.create({
  safe: {
    flex: 1,
    backgroundColor: theme.colors.background,
  },
  container: {
    flex: 1,
    minHeight: 0,
    backgroundColor: theme.colors.background,
  },
});
