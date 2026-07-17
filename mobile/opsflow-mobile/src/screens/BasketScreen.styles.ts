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
  summaryRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },
  summaryLabel: {
    color: theme.colors.muted,
    fontWeight: '600',
  },
  summaryPrice: {
    color: theme.colors.text,
    fontWeight: '900',
    fontSize: 18,
  },
  items: {
    gap: 10,
  },
  itemRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderTopWidth: 1,
    borderTopColor: theme.colors.border,
    paddingTop: 12,
  },
  itemTitle: {
    color: theme.colors.text,
    fontWeight: '800',
  },
  itemMeta: {
    color: theme.colors.muted,
    marginTop: 3,
  },
  itemPrice: {
    color: theme.colors.text,
    fontWeight: '800',
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
  paymentRow: {
    flexDirection: 'row',
    gap: 10,
    flexWrap: 'wrap',
  },
  paymentChip: {
    paddingHorizontal: 14,
    paddingVertical: 10,
    borderRadius: 999,
    backgroundColor: theme.colors.surfaceSoft,
    borderWidth: 1,
    borderColor: theme.colors.border,
  },
  paymentChipActive: {
    backgroundColor: theme.colors.primary,
    borderColor: theme.colors.primary,
  },
  paymentText: {
    color: theme.colors.text,
    fontWeight: '700',
  },
  paymentTextActive: {
    color: '#fff',
  },
  orderButton: {
    backgroundColor: theme.colors.primary,
    borderRadius: 18,
    paddingVertical: 14,
    alignItems: 'center',
    marginTop: 16,
  },
  orderButtonText: {
    color: '#fff',
    fontWeight: '800',
  },
});
