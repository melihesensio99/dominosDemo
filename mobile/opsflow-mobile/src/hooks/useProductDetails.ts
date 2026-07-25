import { useMemo, useState } from 'react';
import type { Product } from '../types/catalog';

export function useProductDetails(product: Product) {
  const [selectedOptionIds, setSelectedOptionIds] = useState<string[]>([]);

  const toggleOption = (groupId: string, optionId: string, selectionType: 'single' | 'multiple') => {
    const group = product.optionGroups.find((item) => item.id === groupId);
    if (!group) return;

    setSelectedOptionIds((current) => {
      if (selectionType === 'single') {
        return [...current.filter((id) => !group.options.some((option) => option.id === id)), optionId];
      }

      return current.includes(optionId)
        ? current.filter((id) => id !== optionId)
        : [...current, optionId];
    });
  };

  const selectedOptions = useMemo(
    () => product.optionGroups.flatMap((group) => group.options.filter((option) => selectedOptionIds.includes(option.id))),
    [product.optionGroups, selectedOptionIds],
  );

  const missingRequiredGroup = product.optionGroups.find(
    (group) => group.isRequired && !group.options.some((option) => selectedOptionIds.includes(option.id)),
  );

  return {
    selectedOptionIds,
    totalPrice: product.price + selectedOptions.reduce((total, option) => total + option.priceAdjustment, 0),
    missingRequiredGroupName: missingRequiredGroup?.name,
    toggleOption,
  };
}
