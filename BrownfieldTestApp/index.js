// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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

const BrownfieldTestApp = StackNavigator({
Main: { screen: MainScreen },
Crashes: { screen: CrashesScreen },
Analytics: { screen: AnalyticsScreen },
Push: { screen: PushScreen }
});

AppRegistry.registerComponent('BrownfieldTestApp', () => BrownfieldTestApp);
