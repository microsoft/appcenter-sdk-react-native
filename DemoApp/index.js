import React, { Component } from 'react';
import {
  Alert,
  AppRegistry,
  StyleSheet,
  Text,
  View,
  ScrollView,
  TouchableOpacity,
  NativeModules
} from 'react-native';

import { StackNavigator } from 'react-navigation';

import MainScreen from './MainScreen';
import CrashesScreen from './CrashesScreen';
import AnalyticsScreen from './AnalyticsScreen';
import PushScreen from './PushScreen';
import MobileCenterScreen from './MobileCenterScreen';

const DemoApp = StackNavigator({
  Main: {screen: MainScreen},
  Crashes: {screen: CrashesScreen},
  Analytics: {screen: AnalyticsScreen},
  Push: {screen: PushScreen},
  MobileCenter: {screen: MobileCenterScreen}
});

AppRegistry.registerComponent('DemoApp', () => DemoApp);
