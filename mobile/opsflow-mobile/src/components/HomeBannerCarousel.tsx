import { Image, ScrollView, View, useWindowDimensions } from 'react-native';
import { styles } from './HomeBannerCarousel.styles';

const bannerGap = 32;

const bannerImages = [
  'https://res.cloudinary.com/dc2j01x6b/image/upload/WhatsApp_Image_2026-07-25_at_19.14.43.jpg',
  'https://res.cloudinary.com/dc2j01x6b/image/upload/WhatsApp_Image_2026-07-25_at_19.14.43_1.jpg',
  'https://res.cloudinary.com/dc2j01x6b/image/upload/WhatsApp_Image_2026-07-25_at_19.14.43_2.jpg',
  'https://res.cloudinary.com/dc2j01x6b/image/upload/WhatsApp_Image_2026-07-25_at_19.14.44.jpg',
  'https://res.cloudinary.com/dc2j01x6b/image/upload/WhatsApp_Image_2026-07-25_at_19.14.44_1.jpg',
];

export function HomeBannerCarousel() {
  const { width: windowWidth } = useWindowDimensions();
  const bannerWidth = Math.max(windowWidth - 48, 280);

  return (
    <View style={[styles.container, { width: windowWidth }]}>
      <ScrollView
        horizontal
        showsHorizontalScrollIndicator={false}
        decelerationRate="fast"
        snapToInterval={bannerWidth + bannerGap}
        snapToAlignment="start"
        disableIntervalMomentum
        style={{ width: windowWidth }}
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
