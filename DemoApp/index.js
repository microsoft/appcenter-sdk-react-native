/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import {
  AppRegistry
} from 'react-native';

import { StackNavigator } from 'react-navigation';

import MainScreen from './MainScreen';
import CrashesScreen from './CrashesScreen';
import AnalyticsScreen from './AnalyticsScreen';
import PushScreen from './PushScreen';
import MobileCenterScreen from './MobileCenterScreen';

const DemoApp = StackNavigator({
  Main: { screen: MainScreen },
  Crashes: { screen: CrashesScreen },
  Analytics: { screen: AnalyticsScreen },
  Push: { screen: PushScreen },
  MobileCenter: { screen: MobileCenterScreen }
});

AppRegistry.registerComponent('DemoApp', () => DemoApp);

