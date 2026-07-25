import { useMemo, useState } from 'react';
import { ScrollView, Text, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import { EmptyState } from '../components/EmptyState';
import { ProductCard } from '../components/ProductCard';
import type { Category, Product } from '../types/catalog';
import { styles } from './MenuScreen.styles';

interface MenuScreenProps {
  categories: Category[];
  products: Product[];
  isLoading?: boolean;
  error?: unknown;
  onAdd: (product: Product) => void;
  onOpenProduct?: (product: Product) => void;
}

export function MenuScreen({ categories, products, isLoading, error, onAdd, onOpenProduct }: MenuScreenProps) {
  const [selectedCategory, setSelectedCategory] = useState('all');

  const visibleProducts = useMemo(() => {
    if (selectedCategory === 'all') {
      return products;
    }

    return products.filter((product) => product.categoryId === selectedCategory);
  }, [products, selectedCategory]);

  return (
    <View style={styles.container}>
      <AppHeader title="Menü" subtitle="Ürün seç ve sepete ekle" />

      <ScrollView style={styles.scroll} contentContainerStyle={styles.content} showsVerticalScrollIndicator={false} bounces={false} alwaysBounceVertical={false}>
        {categories.length > 0 ? (
          <ScrollView horizontal showsHorizontalScrollIndicator={false} contentContainerStyle={styles.chips}>
            <Text
              onPress={() => setSelectedCategory('all')}
              style={[styles.chip, selectedCategory === 'all' && styles.chipActive]}
            >
              Tümü
            </Text>

            {categories.map((category) => {
              const active = selectedCategory === category.id;

              return (
                <Text
                  key={category.id}
                  onPress={() => setSelectedCategory(category.id)}
                  style={[styles.chip, active && styles.chipActive]}
                >
                  {category.name}
                </Text>
              );
            })}
          </ScrollView>
        ) : null}

        <Text style={styles.helper}>Kategori seç, ürüne dokun ve sepete ekle.</Text>

        {visibleProducts.length > 0 ? (
          <View style={styles.list}>
            {visibleProducts.map((product) => (
              <ProductCard key={product.id} product={product} onAdd={onAdd} onOpen={onOpenProduct} />
            ))}
          </View>
        ) : isLoading ? (
          <EmptyState title="Menü yükleniyor" message="Ürünler backend'den çekiliyor." />
        ) : error ? (
          <EmptyState
            title="Menü alınamadı"
            message={error instanceof Error ? error.message : 'Ürünler şu anda getirilemedi.'}
          />
        ) : (
          <EmptyState title="Şu anda ürün yok" message="Backend'den ürün gelince burada listelenecek. İstersen ürünleri elle ekleyip akışı birlikte test edebiliriz." />
        )}
      </ScrollView>
    </View>
  );
}
