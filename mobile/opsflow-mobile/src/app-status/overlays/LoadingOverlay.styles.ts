import { StyleSheet } from 'react-native';
import { theme } from '../../theme';

export const styles = StyleSheet.create({
  overlay: {
    ...StyleSheet.absoluteFillObject,
    backgroundColor: 'rgba(10, 15, 30, 0.45)',
    justifyContent: 'center',
    alignItems: 'center',
    zIndex: 50,
  },
  card: {
    minWidth: 200,
    paddingHorizontal: 20,
    paddingVertical: 18,
    borderRadius: 24,
    backgroundColor: theme.colors.primary,
    gap: 12,
    alignItems: 'center',
    shadowColor: '#0f172a',
    shadowOpacity: 0.22,
    shadowRadius: 24,
    shadowOffset: { width: 0, height: 12 },
    elevation: 8,
  },
  text: {
    color: '#ffffff',
    fontSize: 15,
    fontWeight: '700',
  },
});
