import { useAppStatus } from './AppStatusProvider';
import { LoadingOverlay } from './overlays/LoadingOverlay';

export function AppStatusOverlay() {
  const status = useAppStatus();

  return <LoadingOverlay message={status.loadingMessage} />;
}
