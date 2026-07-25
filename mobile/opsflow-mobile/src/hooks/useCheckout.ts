import { useEffect, useState } from 'react';
import type { UserAddress } from '../types/auth';
import type { Address } from '../types/common';

export type CheckoutStep = 'basket' | 'address' | 'payment';

const emptyAddress: Address = {
  street: '',
  district: '',
  city: '',
  postalCode: '',
  country: 'Turkey',
};

export function useCheckout(addresses: UserAddress[]) {
  const [step, setStep] = useState<CheckoutStep>('basket');
  const [selectedAddressId, setSelectedAddressId] = useState<string | null>(null);
  const [draftAddress, setDraftAddress] = useState<Address>(emptyAddress);
  const [paymentMethod, setPaymentMethod] = useState(0);

  useEffect(() => {
    if (!selectedAddressId && addresses.length > 0) {
      setSelectedAddressId(addresses[0].id);
    }
  }, [addresses, selectedAddressId]);

  return {
    step,
    selectedAddress: addresses.find((address) => address.id === selectedAddressId),
    selectedAddressId,
    draftAddress,
    paymentMethod,
    setPaymentMethod,
    setDraftAddress,
    selectAddress: setSelectedAddressId,
    begin: () => setStep('address'),
    goToPayment: () => setStep('payment'),
    goBack: () => setStep((current) => (current === 'payment' ? 'address' : 'basket')),
    reset: () => {
      setStep('basket');
      setSelectedAddressId(null);
      setDraftAddress(emptyAddress);
      setPaymentMethod(0);
    },
  };
}
