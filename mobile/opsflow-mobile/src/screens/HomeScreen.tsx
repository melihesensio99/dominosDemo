import { useEffect, useMemo, useRef, useState } from 'react';
import { Animated, Pressable, ScrollView, Text, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import { EmptyState } from '../components/EmptyState';
import { HomeBannerCarousel } from '../components/HomeBannerCarousel';
import { ProductCard } from '../components/ProductCard';
import type { Category, Product } from '../types/catalog';
import { styles } from './HomeScreen.styles';

interface HomeScreenProps {
  categories: Category[];
  products: Product[];
  isCatalogLoading?: boolean;
  catalogError?: unknown;
  onAdd: (product: Product) => void;
  onOpenProduct?: (product: Product) => void;
  lastOrderStatus?: string;
  isLoading?: boolean;
  error?: unknown;
}

function getOrderStatusText(status?: string) {
  switch (status) {
    case 'pending': return 'Siparisiniz alindi, hazirlaniyor.';
    case 'confirmed': return 'Siparisiniz onaylandi.';
    case 'preparing': return 'Siparisiniz hazirlaniyor.';
    case 'shipped': return 'Siparisiniz yola cikti.';
    case 'delivered': return 'Siparisiniz teslim edildi.';
    default: return status;
  }
}

const categoryOrder = ['pizzalar', 'patatesler', 'tatlilar', 'soslar', 'icecekler'];

export function HomeScreen({
  categories,
  products,
  isCatalogLoading,
  catalogError,
  onAdd,
  onOpenProduct,
  lastOrderStatus,
  isLoading,
  error,
}: HomeScreenProps) {
  const [selectedCategory, setSelectedCategory] = useState<string | null>(null);
  const orderedCategories = useMemo(
    () => [...categories].sort((left, right) => categoryOrder.indexOf(left.slug) - categoryOrder.indexOf(right.slug)),
    [categories],
  );
  const visibleProducts = useMemo(
    () => selectedCategory === null
      ? products
      : products.filter((product) => product.categoryId === selectedCategory),
    [products, selectedCategory],
  );

  return (
    <View style={styles.container}>
      <AppHeader title="Domino's benzeri" subtitle="Urunleri kesfet ve sepete ekle" badge="MVP" />
      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        <HomeBannerCarousel />

        {lastOrderStatus || isLoading || error ? (
          <>
            <SectionTitle title="Sipariş durumu" />
            <View style={styles.statusCard}>
              <Text style={lastOrderStatus ? styles.orderStatus : styles.infoText}>
                {lastOrderStatus
                  ? getOrderStatusText(lastOrderStatus)
                  : isLoading
                    ? 'Son sipariş bilgisi yükleniyor...'
                    : error instanceof Error ? error.message : 'Sipariş verilemedi.'}
              </Text>
            </View>
          </>
        ) : null}

        <SectionTitle title="Kategoriler" />
        <ScrollView horizontal showsHorizontalScrollIndicator={false} contentContainerStyle={styles.chips}>
          {orderedCategories.map((category) => (
            <AnimatedCategoryChip
              key={category.id}
              category={category}
              active={selectedCategory === category.id}
              onPress={() => setSelectedCategory(category.id)}
            />
          ))}
        </ScrollView>

        <SectionTitle title={selectedCategory === null ? 'Tum urunler' : 'Secilen kategori'} />
        {visibleProducts.length > 0 ? (
          <View style={styles.productList}>
            {visibleProducts.map((product) => <ProductCard key={product.id} product={product} onAdd={onAdd} onOpen={onOpenProduct} />)}
          </View>
        ) : isCatalogLoading ? (
          <EmptyState title="Urunler yukleniyor" message="Urunler backend'den getiriliyor." />
        ) : catalogError ? (
          <EmptyState title="Urunler alinamadi" message={catalogError instanceof Error ? catalogError.message : 'Urunler su anda getirilemedi.'} />
        ) : (
          <EmptyState title="Bu kategoride urun yok" message="Bu kategoriye urun eklendiginde burada gorunecek." />
        )}

      </ScrollView>
    </View>
  );
}

function SectionTitle({ title }: { title: string }) {
  return <Text style={styles.sectionTitle}>{title}</Text>;
}

function AnimatedCategoryChip({
  category,
  active,
  onPress,
}: {
  category: Category;
  active: boolean;
  onPress: () => void;
}) {
  const scale = useRef(new Animated.Value(1)).current;
  const iconBySlug: Record<string, string> = {
    pizzalar: '🍕',
    patatesler: '🍟',
    tatlilar: '🍰',
    soslar: '🥣',
    icecekler: '🥤',
  };

  useEffect(() => {
    Animated.spring(scale, {
      toValue: active ? 1.05 : 1,
      useNativeDriver: true,
      friction: 7,
      tension: 90,
    }).start();
  }, [active, scale]);

  return (
    <Animated.View style={{ transform: [{ scale }] }}>
      <Pressable onPress={onPress} style={[styles.chip, active && styles.chipActive]}>
        <Text style={styles.categoryIcon}>{iconBySlug[category.slug] ?? '🍽️'}</Text>
        <Text style={[styles.chipText, active && styles.chipTextActive]}>{category.name}</Text>
      </Pressable>
    </Animated.View>
  );
}
