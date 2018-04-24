/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import { AppRegistry, YellowBox } from 'react-native';

import { StackNavigator } from 'react-navigation';

import MainScreen from './MainScreen';
import CrashesScreen from './CrashesScreen';
import AnalyticsScreen from './AnalyticsScreen';
import PushScreen from './PushScreen';
import AppCenterScreen from './AppCenterScreen';

const TestApp = StackNavigator({
  Main: { screen: MainScreen },
  Crashes: { screen: CrashesScreen },
  Analytics: { screen: AnalyticsScreen },
  Push: { screen: PushScreen },
  AppCenter: { screen: AppCenterScreen }
});

AppRegistry.registerComponent('TestApp', () => TestApp);

// Ignore react-navigation 'isMounted' deprecation message (https://github.com/react-navigation/react-navigation/issues/3956)
YellowBox.ignoreWarnings(['Warning: isMounted(...) is deprecated', 'Module RCTImageLoader']);
