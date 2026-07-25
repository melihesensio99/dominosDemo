import { useMemo, useState } from 'react';
import { Pressable, ScrollView, Text, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import { EmptyState } from '../components/EmptyState';
import { ProductCard } from '../components/ProductCard';
import type { Category, Product } from '../types/catalog';
import { styles } from './HomeScreen.styles';

interface HomeScreenProps {
  categories: Category[];
  products: Product[];
  isCatalogLoading?: boolean;
  catalogError?: unknown;
  onAdd: (product: Product) => void;
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
        <SectionTitle title="Kategoriler" />
        <ScrollView horizontal showsHorizontalScrollIndicator={false} contentContainerStyle={styles.chips}>
          {orderedCategories.map((category) => (
            <Pressable
              key={category.id}
              onPress={() => setSelectedCategory(category.id)}
              style={[styles.chip, selectedCategory === category.id && styles.chipActive]}
            >
              <Text style={[styles.chipText, selectedCategory === category.id && styles.chipTextActive]}>
                {category.name}
              </Text>
            </Pressable>
          ))}
        </ScrollView>

        {lastOrderStatus ? (
          <View style={styles.orderBanner}>
            <Text style={styles.orderBannerTitle}>Siparisiniz alindi</Text>
            <Text style={styles.orderBannerText}>{getOrderStatusText(lastOrderStatus)}</Text>
          </View>
        ) : null}

        <SectionTitle title={selectedCategory === null ? 'Tum urunler' : 'Secilen kategori'} />
        {visibleProducts.length > 0 ? (
          <View style={styles.productList}>
            {visibleProducts.map((product) => <ProductCard key={product.id} product={product} onAdd={onAdd} />)}
          </View>
        ) : isCatalogLoading ? (
          <EmptyState title="Urunler yukleniyor" message="Urunler backend'den getiriliyor." />
        ) : catalogError ? (
          <EmptyState title="Urunler alinamadi" message={catalogError instanceof Error ? catalogError.message : 'Urunler su anda getirilemedi.'} />
        ) : (
          <EmptyState title="Bu kategoride urun yok" message="Bu kategoriye urun eklendiginde burada gorunecek." />
        )}

        <SectionTitle title="Son durum" />
        <View style={styles.statusCard}>
          <Text style={lastOrderStatus ? styles.orderStatus : styles.infoText}>
            {lastOrderStatus
              ? getOrderStatusText(lastOrderStatus)
              : isLoading
                ? 'Son siparis bilgisi yukleniyor...'
                : error
                  ? error instanceof Error ? error.message : 'Siparis verilemedi.'
                  : 'Henuz siparis yok.'}
          </Text>
        </View>
      </ScrollView>
    </View>
  );
}

function SectionTitle({ title }: { title: string }) {
  return <Text style={styles.sectionTitle}>{title}</Text>;
}
