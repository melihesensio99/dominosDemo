import { useEffect, useMemo, useRef } from 'react';
import { Animated, Text, View } from 'react-native';
import type { Product } from '../types/catalog';
import type { Order } from '../types/order';
import { styles } from './OrderStatusCard.styles';

interface OrderStatusCardProps {
  order: Order;
  products: Product[];
}

const orderSteps = [
  { key: 'pending', label: 'Oluşturuldu' },
  { key: 'confirmed', label: 'Onaylandı' },
  { key: 'preparing', label: 'Hazırlanıyor' },
  { key: 'shipped', label: 'Yolda' },
  { key: 'delivered', label: 'Teslim edildi' },
];

const statusContent: Record<string, { title: string; message: string }> = {
  pending: { title: 'Sipariş oluşturuldu', message: 'Siparişiniz alındı.' },
  confirmed: { title: 'Sipariş onaylandı', message: 'Siparişiniz onaylandı.' },
  preparing: { title: 'Sipariş hazırlanıyor', message: 'Siparişiniz hazırlanıyor.' },
  shipped: { title: 'Siparişiniz yolda', message: 'Siparişiniz yola çıktı.' },
  delivered: { title: 'Sipariş teslim edildi', message: 'Siparişiniz teslim edildi.' },
};

export function OrderStatusCard({ order, products }: OrderStatusCardProps) {
  const pulse = useRef(new Animated.Value(0.92)).current;
  const status = order.status.toLowerCase();
  const activeIndex = Math.max(orderSteps.findIndex((step) => step.key === status), 0);
  const content = statusContent[status] ?? { title: 'Sipariş durumu', message: order.status };
  const productMap = useMemo(
    () => new Map(products.map((product) => [product.id, product])),
    [products],
  );

  useEffect(() => {
    const animation = Animated.loop(
      Animated.sequence([
        Animated.timing(pulse, { toValue: 1, duration: 900, useNativeDriver: true }),
        Animated.timing(pulse, { toValue: 0.92, duration: 900, useNativeDriver: true }),
      ]),
    );
    animation.start();
    return () => animation.stop();
  }, [pulse]);

  return (
    <View style={styles.card}>
      <View style={styles.header}>
        <Animated.View style={[styles.pulse, { transform: [{ scale: pulse }] }]} />
        <View style={styles.headerText}>
          <Text style={styles.title}>{content.title}</Text>
          <Text style={styles.status}>{content.message}</Text>
          <Text style={styles.orderNumber}>Sipariş #{order.id.slice(-8).toUpperCase()}</Text>
        </View>
      </View>

      <View style={styles.items}>
        {order.items.map((item, index) => {
          const product = productMap.get(item.productId);
          const selectedOptions = product?.optionGroups
            .flatMap((group) => group.options)
            .filter((option) => item.selectedOptionIds?.includes(option.id))
            .map((option) => option.name) ?? [];

          return (
            <View key={`${item.productId}-${index}`} style={styles.itemRow}>
              <View style={styles.itemText}>
                <Text style={styles.itemName}>{item.quantity} x {product?.name ?? 'Ürün'}</Text>
                {selectedOptions.length > 0 ? (
                  <Text style={styles.itemOptions}>{selectedOptions.join(', ')}</Text>
                ) : null}
              </View>
            </View>
          );
        })}
      </View>

      <View style={styles.timeline}>
        {orderSteps.map((step, index) => {
          const isCompleted = index <= activeIndex;
          return (
            <View key={step.key} style={styles.timelineStep}>
              <View style={[styles.timelineDot, isCompleted && styles.timelineDotActive]}>
                <Text style={[styles.timelineCheck, isCompleted && styles.timelineCheckActive]}>
                  {isCompleted ? '✓' : ''}
                </Text>
              </View>
              <Text style={[styles.timelineLabel, isCompleted && styles.timelineLabelActive]}>
                {step.label}
              </Text>
              {index < orderSteps.length - 1 ? (
                <View style={[styles.timelineLine, index < activeIndex && styles.timelineLineActive]} />
              ) : null}
            </View>
          );
        })}
      </View>
    </View>
  );
}
