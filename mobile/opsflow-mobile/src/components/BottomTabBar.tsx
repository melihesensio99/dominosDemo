import { Animated, Pressable, Text, View } from 'react-native';
import { useEffect, useRef } from 'react';
import { ROUTE_LABELS, ROUTES, type RouteKey } from '../constants/routes';
import { styles } from './BottomTabBar.styles';

export type TabKey = RouteKey;

interface BottomTabBarProps {
  activeTab: TabKey;
  onChangeTab: (tab: TabKey) => void;
}

const tabs: Array<{ key: TabKey; label: string; icon: string }> = [
  { key: ROUTES.HOME, label: ROUTE_LABELS.home, icon: '⌂' },
  { key: ROUTES.BASKET, label: ROUTE_LABELS.basket, icon: '🛒' },
  { key: ROUTES.ACCOUNT, label: ROUTE_LABELS.account, icon: '♙' },
];

export function BottomTabBar({ activeTab, onChangeTab }: BottomTabBarProps) {
  return (
    <View style={styles.container}>
      {tabs.map((tab) => {
        const active = activeTab === tab.key;

        return (
          <AnimatedTab key={tab.key} tab={tab} active={active} onPress={() => onChangeTab(tab.key)} />
        );
      })}
    </View>
  );
}

function AnimatedTab({
  tab,
  active,
  onPress,
}: {
  tab: (typeof tabs)[number];
  active: boolean;
  onPress: () => void;
}) {
  const scale = useRef(new Animated.Value(1)).current;

  useEffect(() => {
    Animated.spring(scale, {
      toValue: active ? 1.12 : 1,
      useNativeDriver: true,
      friction: 7,
      tension: 90,
    }).start();
  }, [active, scale]);

  return (
    <Pressable onPress={onPress} style={styles.tab}>
      <Animated.View style={[styles.iconWrap, { transform: [{ scale }] }]}>
        <Text style={[styles.icon, active && styles.iconActive]}>{tab.icon}</Text>
      </Animated.View>
      <Text style={[styles.label, active && styles.labelActive]}>{tab.label}</Text>
      <View style={[styles.indicator, active && styles.indicatorActive]} />
    </Pressable>
  );
}
