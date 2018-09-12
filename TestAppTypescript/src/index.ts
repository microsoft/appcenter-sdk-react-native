import { AppRegistry } from "react-native";
import MainScreen from "./MainScreen";
import CrashesScreen from "./CrashesScreen";
import PushScreen from "./PushScreen";
import { StackNavigator } from "react-navigation";
import AppCenterScreen from "./AppCenterScreen";
import AnalyticsScreen from "./AnalyticsScreen";
const TestApp = StackNavigator({
  Main: MainScreen,
  Crashes: CrashesScreen,
  Analytics: AnalyticsScreen,
  Push: PushScreen,
  AppCenter: AppCenterScreen
});

AppRegistry.registerComponent("TestApp", () => TestApp);
