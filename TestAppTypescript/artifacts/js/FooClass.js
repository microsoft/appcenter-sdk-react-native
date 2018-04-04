import { BarClass } from './BarClass';
export class FooClass {
    method1(value) {
        return this.method2(value);
    }
    method2(value) {
        return this.fooInnermostMethod(value);
    }
    fooInnermostMethod() {
        return BarClass.barMethod1();
    }
}
//# sourceMappingURL=FooClass.js.map