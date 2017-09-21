/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

export class BarClass {
  static barMethod1(value1, value2) {
    this.crashApp(value1, value2);
  }

  /* eslint-disable */
  static crashApp(value1, value2) {
    var thisVariableIsUndefined;
    thisVariableIsUndefined.invokingFunctionOnUndefined();
  }
  /* eslint-enable */
}
