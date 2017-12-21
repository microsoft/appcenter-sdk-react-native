import { AppRegistry } from 'react-native';
import { StackNavigator } from 'react-navigation';

import MainScreen from './MainScreen';
import CrashesScreen from './CrashesScreen';
import AnalyticsScreen from './AnalyticsScreen';
import PushScreen from './PushScreen';
import AppCenterScreen from './AppCenterScreen';

const TestingSwift = StackNavigator({
  Main: { screen: MainScreen },
  Crashes: { screen: CrashesScreen },
  Analytics: { screen: AnalyticsScreen },
  Push: { screen: PushScreen },
  AppCenter: { screen: AppCenterScreen }
});

AppRegistry.registerComponent('TestingSwift', () => TestingSwift);
