import { Image, Pressable, Text, View } from 'react-native';
import type { Product } from '../types/catalog';
import { styles } from './ProductCard.styles';

interface ProductCardProps {
  product: Product;
  onAdd: (product: Product) => void;
  onOpen?: (product: Product) => void;
}

export function ProductCard({ product, onAdd, onOpen }: ProductCardProps) {
  return (
    <View style={styles.card}>
      <Pressable onPress={() => onOpen?.(product)} style={styles.imageContainer}>
        {product.imageUrl ? <Image source={{ uri: product.imageUrl }} style={styles.image} resizeMode="cover" /> : null}
      </Pressable>
      <View style={styles.topRow}>
        <View style={styles.categoryBadge}>
          <Text style={styles.categoryText}>{product.categoryName ?? 'Ürün'}</Text>
        </View>
        <Text style={styles.price}>{product.price.toLocaleString('tr-TR')} TL</Text>
      </View>

      <Pressable onPress={() => onOpen?.(product)}>
        <Text style={styles.title}>{product.name}</Text>
      </Pressable>
      <Text style={styles.description}>{product.description}</Text>

      <Pressable style={styles.cardAction} onPress={() => onAdd(product)}>
        <Text style={styles.cardActionText}>+</Text>
      </Pressable>
    </View>
  );
}
