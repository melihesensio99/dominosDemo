import { Dimensions, Image, ScrollView, View } from 'react-native';
import { styles } from './HomeBannerCarousel.styles';

const bannerWidth = Dimensions.get('window').width - 32;

const bannerImages = [
  'https://res.cloudinary.com/dc2j01x6b/image/upload/WhatsApp_Image_2026-07-25_at_19.14.43.jpg',
  'https://res.cloudinary.com/dc2j01x6b/image/upload/WhatsApp_Image_2026-07-25_at_19.14.43_1.jpg',
  'https://res.cloudinary.com/dc2j01x6b/image/upload/WhatsApp_Image_2026-07-25_at_19.14.43_2.jpg',
  'https://res.cloudinary.com/dc2j01x6b/image/upload/WhatsApp_Image_2026-07-25_at_19.14.44.jpg',
  'https://res.cloudinary.com/dc2j01x6b/image/upload/WhatsApp_Image_2026-07-25_at_19.14.44_1.jpg',
];

export function HomeBannerCarousel() {
  return (
    <View style={styles.container}>
      <ScrollView
        horizontal
        pagingEnabled
        showsHorizontalScrollIndicator={false}
        decelerationRate="fast"
        snapToInterval={bannerWidth + 12}
        contentContainerStyle={styles.content}
      >
        {bannerImages.map((imageUrl) => (
          <Image key={imageUrl} source={{ uri: imageUrl }} style={[styles.image, { width: bannerWidth }]} resizeMode="cover" />
        ))}
      </ScrollView>
      <View style={styles.hint} />
    </View>
  );
}
