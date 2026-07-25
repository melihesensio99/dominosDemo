import { Image, Pressable, ScrollView, Text, View } from 'react-native';
import { AppHeader } from '../components/AppHeader';
import type { Product } from '../types/catalog';
import { useProductDetails } from '../hooks/useProductDetails';
import { styles } from './ProductDetailsScreen.styles';

interface ProductDetailsScreenProps {
  product: Product;
  onBack: () => void;
  onAdd: (selectedOptionIds: string[]) => void;
}

export function ProductDetailsScreen({ product, onBack, onAdd }: ProductDetailsScreenProps) {
  const details = useProductDetails(product);

  return (
    <View style={styles.container}>
      <AppHeader title={product.name} subtitle="Urununu kendi zevkine gore hazirla" />
      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        {product.imageUrl ? <Image source={{ uri: product.imageUrl }} style={styles.image} resizeMode="cover" /> : null}
        <Text style={styles.title}>{product.name}</Text>
        <Text style={styles.description}>{product.description}</Text>

        {product.optionGroups.map((group) => (
          <View key={group.id} style={styles.group}>
            <Text style={styles.groupTitle}>{group.name}</Text>
            {group.options.map((option) => {
              const selected = details.selectedOptionIds.includes(option.id);
              return (
                <Pressable
                  key={option.id}
                  onPress={() => details.toggleOption(group.id, option.id, group.selectionType)}
                  style={[styles.option, selected && styles.optionSelected]}
                >
                  <Text style={styles.optionName}>{selected ? '✓ ' : ''}{option.name}</Text>
                  <Text style={styles.optionPrice}>
                    {option.priceAdjustment > 0 ? `+${option.priceAdjustment} TL` : 'Ucretsiz'}
                  </Text>
                </Pressable>
              );
            })}
          </View>
        ))}

        {details.missingRequiredGroupName ? (
          <Text style={styles.error}>{details.missingRequiredGroupName} secimi zorunludur.</Text>
        ) : null}

        <Pressable
          style={[styles.addButton, details.missingRequiredGroupName ? { opacity: 0.5 } : null]}
          disabled={Boolean(details.missingRequiredGroupName)}
          onPress={() => onAdd(details.selectedOptionIds)}
        >
          <Text style={styles.addButtonText}>{details.totalPrice.toLocaleString('tr-TR')} TL - Sepete Ekle</Text>
        </Pressable>
        <Pressable onPress={onBack}>
          <Text style={styles.optionName}>Geri don</Text>
        </Pressable>
      </ScrollView>
    </View>
  );
}
