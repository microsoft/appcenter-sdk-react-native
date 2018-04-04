export class BarClass {
    static barMethod1(value1, value2) {
        this.crashApp(value1, value2);
    }
    static crashApp(value1, value2) {
        let thisVariableIsUndefined;
        thisVariableIsUndefined.invokingFunctionOnUndefined();
    }
}
//# sourceMappingURL=BarClass.js.map