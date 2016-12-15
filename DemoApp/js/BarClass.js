/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

export class BarClass {
    static barMethod1(value1, value2) {
      this.crashApp(value1, value2);
    }

    static crashApp(value1, value2) {
      throw new Error('This is a JavaScript crash message');
    }
}