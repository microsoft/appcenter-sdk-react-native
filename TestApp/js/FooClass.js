/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 * @flow
 */

import {BarClass} from './BarClass';

export class FooClass {
    constructor() {
    }

    method1(value) {
      return this.method2(value);
    }

    method2(value) {
      return this.fooInnermostMethod(value);
    }

    fooInnermostMethod(value) {
      return BarClass.barMethod1();
    }
}