import { useQuery, useQueryClient } from '@tanstack/react-query';
import { authService } from '../services';
import type { UserAddress } from '../types/auth';

export function useAddresses(userId?: string | null) {
  const queryClient = useQueryClient();
  const addressesQuery = useQuery({
    queryKey: ['addresses', userId],
    enabled: Boolean(userId),
    queryFn: () => authService.getAddresses(),
  });

  const addAddress = async (address: Omit<UserAddress, 'id'>) => {
    const created = await authService.createAddress(address);
    await queryClient.invalidateQueries({ queryKey: ['addresses', userId] });
    return created;
  };

  return {
    addresses: addressesQuery.data ?? [],
    isLoading: addressesQuery.isLoading,
    error: addressesQuery.error ?? null,
    addAddress,
  };
}
