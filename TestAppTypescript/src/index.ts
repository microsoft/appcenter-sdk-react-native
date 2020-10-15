import { AppRegistry } from "react-native";
import MainScreen from "./MainScreen";
import CrashesScreen from "./CrashesScreen";
import { StackNavigator } from "react-navigation";
import AppCenterScreen from "./AppCenterScreen";
import AnalyticsScreen from "./AnalyticsScreen";
const TestApp = StackNavigator({
  Main: MainScreen,
  Crashes: CrashesScreen,
  Analytics: AnalyticsScreen,
  AppCenter: AppCenterScreen
});

AppRegistry.registerComponent("TestApp", () => TestApp);
