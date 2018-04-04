var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import React from 'react';
import { Text, View, ScrollView, TouchableOpacity } from 'react-native';
import Push from 'appcenter-push';
import SharedStyles from './SharedStyles';
export default class PushScreen extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            pushEnabled: false
        };
        this.toggleEnabled = this.toggleEnabled.bind(this);
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            const component = this;
            const pushEnabled = yield Push.isEnabled();
            component.setState({ pushEnabled });
        });
    }
    toggleEnabled() {
        return __awaiter(this, void 0, void 0, function* () {
            yield Push.setEnabled(!this.state.pushEnabled);
            const pushEnabled = yield Push.isEnabled();
            this.setState({ pushEnabled });
        });
    }
    render() {
        return (<View style={SharedStyles.container}>
                <ScrollView>
                    <Text style={SharedStyles.heading}>
                        Test Push
          </Text>

                    <Text style={SharedStyles.enabledText}>
                        Push enabled: {this.state.pushEnabled ? 'yes' : 'no'}
                    </Text>
                    <TouchableOpacity onPress={this.toggleEnabled}>
                        <Text style={SharedStyles.toggleEnabled}>
                            toggle
            </Text>
                    </TouchableOpacity>

                </ScrollView>
            </View>);
    }
}
//# sourceMappingURL=PushScreen.js.map