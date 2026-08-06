import { useEffect, useMemo, useRef, useState } from 'react';
import { Animated, Pressable, ScrollView, Text, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import { EmptyState } from '../components/EmptyState';
import { HomeBannerCarousel } from '../components/HomeBannerCarousel';
import { OrderStatusCard } from '../components/OrderStatusCard';
import { ProductCard } from '../components/ProductCard';
import type { Category, Product } from '../types/catalog';
import type { Order } from '../types/order';
import { styles } from './HomeScreen.styles';

interface HomeScreenProps {
  categories: Category[];
  products: Product[];
  orders: Order[];
  hasActiveOrders: boolean;
  isCatalogLoading?: boolean;
  catalogError?: unknown;
  onAdd: (product: Product) => void;
  onOpenProduct?: (product: Product) => void;
  isLoading?: boolean;
  error?: unknown;
}

const categoryOrder = ['pizzalar', 'patatesler', 'tatlilar', 'soslar', 'icecekler'];
const logoUrl = 'https://res.cloudinary.com/dc2j01x6b/image/upload/v1785012397/Ads%C4%B1z.png';

export function HomeScreen({
  categories,
  products,
  orders,
  hasActiveOrders,
  isCatalogLoading,
  catalogError,
  onAdd,
  onOpenProduct,
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
      <AppHeader title="Melo's Pizza" subtitle="Ürünleri keşfet ve sepete ekle" logoUrl={logoUrl} />
      <ScrollView
        style={styles.scroll}
        contentContainerStyle={styles.content}
        showsVerticalScrollIndicator={false}
        bounces={false}
        alwaysBounceVertical={false}
      >
        <HomeBannerCarousel />

        {orders.length > 0 || isLoading || error ? (
          <>
            <SectionTitle title={hasActiveOrders ? 'Aktif siparişlerim' : 'Son siparişim'} />
            {orders.length > 0 ? (
              <View style={styles.orderCards}>
                {orders.map((order) => (
                  <OrderStatusCard key={order.id} order={order} products={products} />
                ))}
              </View>
            ) : (
              <View style={styles.statusCard}>
                <Text style={styles.infoText}>
                  {isLoading
                    ? 'Sipariş bilgileri yükleniyor...'
                    : error instanceof Error ? error.message : 'Sipariş bilgileri alınamadı.'}
                </Text>
              </View>
            )}
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

        <SectionTitle
          title={selectedCategory === null
            ? 'Tüm ürünler'
            : categories.find((category) => category.id === selectedCategory)?.name ?? 'Ürünler'}
        />
        {visibleProducts.length > 0 ? (
          <View style={styles.productList}>
            {visibleProducts.map((product) => (
              <ProductCard key={product.id} product={product} onAdd={onAdd} onOpen={onOpenProduct} />
            ))}
          </View>
        ) : isCatalogLoading ? (
          <EmptyState title="Ürünler yükleniyor" message="Ürünler backend'den getiriliyor." />
        ) : catalogError ? (
          <EmptyState
            title="Ürünler alınamadı"
            message={catalogError instanceof Error ? catalogError.message : 'Ürünler şu anda getirilemedi.'}
          />
        ) : (
          <EmptyState title="Bu kategoride ürün yok" message="Bu kategoriye ürün eklendiğinde burada görünecek." />
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
