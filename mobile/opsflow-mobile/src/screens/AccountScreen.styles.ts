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
  modeRow: {
    flexDirection: 'row',
    gap: 10,
    marginBottom: 10,
  },
  modeButton: {
    flex: 1,
    paddingVertical: 12,
    borderRadius: 18,
    backgroundColor: theme.colors.surfaceSoft,
    borderWidth: 1,
    borderColor: theme.colors.border,
    alignItems: 'center',
  },
  modeButtonActive: {
    backgroundColor: theme.colors.primary,
    borderColor: theme.colors.primary,
  },
  modeText: {
    color: theme.colors.text,
    fontWeight: '800',
  },
  modeTextActive: {
    color: '#fff',
  },
  input: {
    borderWidth: 1,
    borderColor: theme.colors.border,
    backgroundColor: theme.colors.surfaceSoft,
    borderRadius: 18,
    paddingHorizontal: 14,
    paddingVertical: 12,
    color: theme.colors.text,
    marginTop: 10,
  },
  authButton: {
    backgroundColor: theme.colors.primary,
    borderRadius: 18,
    paddingVertical: 14,
    alignItems: 'center',
    marginTop: 16,
  },
  authButtonText: {
    color: '#fff',
    fontWeight: '800',
  },
  info: {
    color: theme.colors.muted,
    lineHeight: 20,
  },
  signOutButton: {
    marginTop: 16,
    backgroundColor: theme.colors.surfaceSoft,
    borderRadius: 18,
    paddingVertical: 14,
    alignItems: 'center',
    borderWidth: 1,
    borderColor: theme.colors.border,
  },
  signOutText: {
    color: theme.colors.danger,
    fontWeight: '800',
  },
  cancelOrderButton: {
    marginTop: 10,
    borderWidth: 1,
    borderColor: '#fecaca',
    backgroundColor: '#fff1f2',
    borderRadius: 14,
    paddingVertical: 10,
    alignItems: 'center',
  },
  cancelOrderText: {
    color: '#b91c1c',
    fontWeight: '800',
    fontSize: 12,
  },
  errorText: {
    color: theme.colors.danger,
    marginTop: 10,
    lineHeight: 20,
    fontWeight: '600',
  },
  menuRow: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingVertical: 14,
    borderBottomWidth: 1,
    borderBottomColor: '#f1f5f9',
  },
  menuRowText: {
    fontSize: 14,
    fontWeight: '800',
    color: '#0f172a',
  },
  menuRowArrow: {
    fontSize: 14,
    color: '#94a3b8',
    fontWeight: '800',
  },
  backButton: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 6,
    marginBottom: 10,
  },
  backButtonText: {
    color: theme.colors.primary,
    fontWeight: '800',
    fontSize: 14,
  },
  addressCard: {
    padding: 12,
    borderWidth: 1,
    borderColor: '#e2e8f0',
    borderRadius: 14,
    backgroundColor: '#fff',
    marginBottom: 10,
  },
  addressTitle: {
    fontWeight: '800',
    fontSize: 14,
    color: '#0f172a',
    marginBottom: 4,
  },
  addressText: {
    color: '#475569',
    fontSize: 12,
    lineHeight: 18,
  },
  addAddressButton: {
    backgroundColor: theme.colors.primary,
    borderRadius: 14,
    paddingVertical: 12,
    alignItems: 'center',
    marginTop: 10,
  },
  addAddressButtonText: {
    color: '#fff',
    fontWeight: '800',
    fontSize: 13,
  },
});
