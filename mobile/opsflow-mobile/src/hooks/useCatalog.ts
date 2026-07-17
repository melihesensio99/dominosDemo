import { useQuery } from '@tanstack/react-query';
import { catalogService } from '../services';

export function useCatalog() {
  const categoriesQuery = useQuery({
    queryKey: ['catalog', 'categories'],
    queryFn: catalogService.getCategories,
  });

  const productsQuery = useQuery({
    queryKey: ['catalog', 'products'],
    queryFn: catalogService.getProducts,
  });

  return {
    categories: categoriesQuery.data ?? [],
    products: productsQuery.data ?? [],
    isLoading: categoriesQuery.isLoading || productsQuery.isLoading,
    error: categoriesQuery.error ?? productsQuery.error ?? null,
  };
}
