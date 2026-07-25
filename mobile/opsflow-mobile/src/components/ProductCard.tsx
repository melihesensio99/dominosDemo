import { Image, Pressable, Text, View } from 'react-native';
import type { Product } from '../types/catalog';
import { styles } from './ProductCard.styles';

interface ProductCardProps {
  product: Product;
  onAdd: (product: Product) => void;
}

export function ProductCard({ product, onAdd }: ProductCardProps) {
  return (
    <View style={styles.card}>
      {product.imageUrl ? <Image source={{ uri: product.imageUrl }} style={styles.image} resizeMode="cover" /> : null}
      <View style={styles.topRow}>
        <View style={styles.categoryBadge}>
          <Text style={styles.categoryText}>{product.categoryName ?? 'Ürün'}</Text>
        </View>
        <Text style={styles.price}>{product.price.toLocaleString('tr-TR')} TL</Text>
      </View>

      <Text style={styles.title}>{product.name}</Text>
      <Text style={styles.description}>{product.description}</Text>

      <View style={styles.footer}>
        <Pressable style={styles.button} onPress={() => onAdd(product)}>
          <Text style={styles.buttonText}>Sepete Ekle</Text>
        </Pressable>
      </View>
    </View>
  );
}
