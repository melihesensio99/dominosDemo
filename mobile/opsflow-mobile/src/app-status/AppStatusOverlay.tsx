import { View } from 'react-native';
import { useAppStatus } from './AppStatusProvider';
import { styles } from './AppStatusOverlay.styles';
import { LoadingOverlay } from './overlays/LoadingOverlay';
import { StatusBanner } from './overlays/StatusBanner';

export function AppStatusOverlay() {
  const status = useAppStatus();

  return (
    <>
      <LoadingOverlay message={status.loadingMessage} />

      {status.banner ? (
        <View style={styles.bannerWrapper}>
          <StatusBanner variant={status.banner.variant} message={status.banner.message} />
        </View>
      ) : null}
    </>
  );
}
