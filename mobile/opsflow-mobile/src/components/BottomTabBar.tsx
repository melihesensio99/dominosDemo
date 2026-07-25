import { Pressable, Text, View } from 'react-native';
import { ROUTE_LABELS, ROUTES, type RouteKey } from '../constants/routes';
import { styles } from './BottomTabBar.styles';

export type TabKey = RouteKey;

interface BottomTabBarProps {
  activeTab: TabKey;
  onChangeTab: (tab: TabKey) => void;
}

const tabs: Array<{ key: TabKey; label: string }> = [
  { key: ROUTES.HOME, label: ROUTE_LABELS.home },
  { key: ROUTES.BASKET, label: ROUTE_LABELS.basket },
  { key: ROUTES.ACCOUNT, label: ROUTE_LABELS.account },
];

export function BottomTabBar({ activeTab, onChangeTab }: BottomTabBarProps) {
  return (
    <View style={styles.container}>
      {tabs.map((tab) => {
        const active = activeTab === tab.key;

        return (
          <Pressable key={tab.key} onPress={() => onChangeTab(tab.key)} style={styles.tab}>
            <Text style={[styles.label, active && styles.labelActive]}>{tab.label}</Text>
            <View style={[styles.indicator, active && styles.indicatorActive]} />
          </Pressable>
        );
      })}
    </View>
  );
}
