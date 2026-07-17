import { ActivityIndicator, Text, View } from 'react-native';
import { StatusBanner } from './StatusBanner';
import { styles } from './GlobalFeedbackLayer.styles';
import { useFeedback } from '../feedback';

export function GlobalFeedbackLayer() {
  const feedback = useFeedback();

  return (
    <>
      {feedback.loadingMessage ? (
        <View style={styles.spinnerOverlay} pointerEvents="auto">
          <View style={styles.spinnerCard}>
            <ActivityIndicator color="#fff" size="large" />
            <Text style={styles.spinnerText}>{feedback.loadingMessage}</Text>
          </View>
        </View>
      ) : null}

      {feedback.banner ? (
        <View style={styles.bannerWrapper}>
          <StatusBanner variant={feedback.banner.variant} message={feedback.banner.message} />
        </View>
      ) : null}
    </>
  );
}
