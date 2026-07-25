import { useMemo, useState } from 'react';
import type { Product } from '../types/catalog';

export function useProductDetails(product: Product) {
  const [quantity, setQuantity] = useState(1);
  const [selectedOptionIds, setSelectedOptionIds] = useState<string[]>(() =>
    product.optionGroups
      .flatMap((group) => group.options)
      .filter((option) => option.isDefault)
      .map((option) => option.id),
  );

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
    quantity,
    increaseQuantity: () => setQuantity((current) => current + 1),
    decreaseQuantity: () => setQuantity((current) => Math.max(1, current - 1)),
    totalPrice: (product.price + selectedOptions.reduce((total, option) => total + option.priceAdjustment, 0)) * quantity,
    missingRequiredGroupName: missingRequiredGroup?.name,
    toggleOption,
  };
}
