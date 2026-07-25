import { useEffect, useState } from 'react';
import type { UserAddress } from '../types/auth';
import type { Address } from '../types/common';

export type CheckoutStep = 'basket' | 'address' | 'payment';
export type AddressMode = 'list' | 'create';

const emptyAddress: Address = {
  street: '',
  district: '',
  city: '',
  postalCode: '',
  country: 'Turkey',
};

export function useCheckout(addresses: UserAddress[]) {
  const [step, setStep] = useState<CheckoutStep>('basket');
  const [addressMode, setAddressMode] = useState<AddressMode>('list');
  const [selectedAddressId, setSelectedAddressId] = useState<string | null>(null);
  const [draftAddress, setDraftAddress] = useState<Address>(emptyAddress);
  const [addressTitle, setAddressTitle] = useState('');
  const [paymentMethod, setPaymentMethod] = useState(0);
  const [note, setNote] = useState('');

  useEffect(() => {
    if (!selectedAddressId && addresses.length > 0) {
      setSelectedAddressId(addresses[0].id);
    }
  }, [addresses, selectedAddressId]);

  return {
    step,
    addressMode,
    selectedAddress: addresses.find((address) => address.id === selectedAddressId),
    selectedAddressId,
    draftAddress,
    addressTitle,
    setAddressTitle,
    paymentMethod,
    setPaymentMethod,
    note,
    setNote,
    setDraftAddress,
    selectAddress: setSelectedAddressId,
    begin: () => setStep('address'),
    beginAddAddress: () => setAddressMode('create'),
    cancelAddAddress: () => setAddressMode('list'),
    goToPayment: () => setStep('payment'),
    goBack: () => setStep((current) => (current === 'payment' ? 'address' : 'basket')),
    reset: () => {
      setStep('basket');
      setAddressMode('list');
      setSelectedAddressId(null);
      setDraftAddress(emptyAddress);
      setAddressTitle('');
      setPaymentMethod(0);
      setNote('');
    },
  };
}
