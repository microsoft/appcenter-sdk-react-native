/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 * @flow
 */

import React from 'react';
import {
  SafeAreaView,
  StyleSheet,
  ScrollView,
  View,
  Text,
  SectionList,
  StatusBar,
  Switch,
  Button,
  TouchableOpacity,
  Alert,
} from 'react-native';

import AppCenter, {CustomProperties} from 'appcenter';
import Analytics from 'appcenter-analytics';
import Crashes, { UserConfirmation, ErrorAttachmentLog } from 'appcenter-crashes';

import {
  Header,
  LearnMoreLinks,
  Colors,
  DebugInstructions,
  ReloadInstructions,
} from 'react-native/Libraries/NewAppScreen';


const STARTUP_MODE = 'STARTUP_MODE';

const App: () => React$Node = () => {
  const switchRenderItemAnalytics = ({ item: { title, value, toggle } }) => (
    <View style={styles.sectionContainer.item}>
      <Text style={styles.sectionDescription}>{title}</Text>
      <Button title="Enable/Disable Analytics" onPress={toggle} />
    </View>
  );
  
  const actionRenderItemAppCenter = ({ item: { title, action } }) => (
    <Button title={title} style={styles.sectionContainer} onPress={action} />
  );

  const switchRenderItemAppCenter = ({ item: { title, value, toggle } }) => (
    <View style={styles.sectionContainer.item}>
      <Text style={styles.sectionDescription}>{title}</Text>
      <Button title="Enable/Disable App Center" onPress={toggle} />
    </View>
  );
  
  const actionRenderItemAnalytics = ({ item: { title, action } }) => (
    <Button title={title} style={styles.sectionContainer} onPress={action} />
  );
  
  const switchRenderItemCrashes = ({ item: { title, value, toggle } }) => (
    <View style={styles.sectionContainer.item}>
      <Text style={styles.sectionDescription}>{title}</Text>
      <Button title="Enable/Disable Crashes" onPress={toggle} />
    </View>
  );
  
  const actionRenderItemCrashes = ({ item: { title, action } }) => (
    <Button title={title} style={styles.sectionContainer} onPress={action} />
  );

  const [analyticsEnabled, setAnalyticsEnabled] = React.useState(false);
  const [appcenterEnabled, setAppcenterEnabled] = React.useState(true);
  const [crashesEnabled, setCrashesEnabled] = React.useState(false);

  const valueRenderItem = ({ item: { title, value, onChange, onSubmit } }) => (
    <View style={SharedStyles.item}>
      <Text style={SharedStyles.itemTitle}>{title}</Text>
      {onChange ? <TextInput style={SharedStyles.itemInput} onSubmitEditing={onSubmit} onChangeText={onChange}>{String(this.state[value])}</TextInput> : <Text>{String(this.state[value])}</Text>}
    </View>
  );
  const startupModeRenderItem = ({ item: { startupModes } }) => (
    <ModalSelector
      data={startupModes}
      initValue={this.state.startupMode.label}
      style={SharedStyles.modalSelector}
      selectTextStyle={SharedStyles.itemButton}
      onChange={async ({ key }) => {
          await AsyncStorage.setItem(STARTUP_MODE, key);
          this.setState({ startupMode: startupModes.filter(m => m.key === key)[0] }, this.selectStartup);
        }
      }
    />
  );

  return (
    <>
      <StatusBar barStyle="dark-content" />
      <SafeAreaView>
        <ScrollView
          contentInsetAdjustmentBehavior="automatic"
          style={styles.scrollView}>
          {global.HermesInternal == null ? null : (
            <View style={styles.engine}>
              <Text style={styles.footer}>Engine: Hermes</Text>
            </View>
          )}

        <Text style={styles.sectionTitle}>
            {"DemoApp"}
        </Text>
 
          <View>
            <View style={styles.body}>
              <Button title="Toggle App Center Set Enabled" onPress={async () => {
                await AppCenter.setEnabled(!appcenterEnabled);
                setAppcenterEnabled(await AppCenter.isEnabled());
                Alert.alert("Enabled: " + await AppCenter.isEnabled());
              }} />
              <Button title="Get App Center Is Enabled" onPress={async () => {
                let isEnabled = await AppCenter.isEnabled();
                Alert.alert("Is enabled: " + isEnabled);
              }} />
            </View>
              
            <View>
              <Button title="Set log level Verbose for App Center" onPress={async () => {
                await AppCenter.setLogLevel(AppCenter.LogLevel.VERBOSE);
                Alert.alert("Log level: " + await AppCenter.getLogLevel());
              }} />
              <Button title="Set log level Debug  for App Center" onPress={async () => {
                await AppCenter.setLogLevel(AppCenter.LogLevel.DEBUG);
                Alert.alert("Log level: " + await AppCenter.getLogLevel());
              }} />
            </View>

            <View>
              <Button title="Get Install ID of App Center" onPress={async () => {
                Alert.alert("Install ID: " + await AppCenter.getInstallId());
              }} />
              <Button title="Set User ID of App Center" onPress={async () => {
                AppCenter.setUserId("My User ID");
              }} />
            </View>
          </View>

          <View style={styles.body}>
            <SectionList
              renderItem={({ item }) => <Text style={[styles.sectionContainer, styles.sectionHeader]}>{item}</Text>}
              renderSectionHeader={({ section: { title } }) => <Text style={styles.sectionHeader}>{title}</Text>}
              keyExtractor={(item, index) => item + index}
              sections={[
                {
                  title: 'Settings',
                  data: [
                    {
                      title: 'Analytics',
                      value: 'analyticsEnabled',
                      toggle: async () => {
                        await Analytics.setEnabled(!analyticsEnabled);
                        setAnalyticsEnabled(await Analytics.isEnabled());
                        Alert.alert("Enabled: " + (await Analytics.isEnabled()));
                      }
                    },
                  ],
                  renderItem: switchRenderItemAnalytics
                },
                {
                  title: 'Actions',
                  data: [
                    {
                      title: 'Track event without properties',
                      action: () => {
                        const eventName = 'EventWithoutProperties';
                        Analytics.trackEvent(eventName);
                        //showEventToast(eventName);
                      }
                    },
                    {
                      title: 'Track event with properties',
                      action: () => {
                        const eventName = 'EventWithProperties';
                        Analytics.trackEvent(eventName, { property1: '100', property2: '200' });
                        //showEventToast(eventName);
                      }
                    },
                    {
                      title: 'Track event with long property value',
                      action: () => {
                        const eventName = 'EventWithLongProperties';
                        Analytics.trackEvent(eventName, { propertyValueTooLong: '12345678901234567890123456789012345678901234567890123456789012345' });
                        //showEventToast(eventName);
                      }
                    },
                  ],
                  renderItem: actionRenderItemAnalytics
                },
              ]}
            />
          </View>
          <View style={styles.body}>
            <SectionList
              renderItem={({ item }) => <Text style={[styles.sectionContainer, styles.sectionHeader]}>{item}</Text>}
              renderSectionHeader={({ section: { title } }) => <Text style={styles.sectionHeader}>{title}</Text>}
              keyExtractor={(item, index) => item + index}
              sections={[
                {
                  title: 'Settings',
                  data: [
                    {
                      title: 'Crashes',
                      value: 'crashesEnabled',
                      toggle: async () => {
                        await Crashes.setEnabled(!crashesEnabled);
                        setCrashesEnabled(await Crashes.isEnabled());
                        Alert.alert("Enabled: " + (await Crashes.isEnabled()));
                      }
                    },
                  ],
                  renderItem: switchRenderItemCrashes
                },
                {
                  title: 'Actions',
                  data: [
                    {
                      title: 'javascript crash',
                      action: () => {
                        throw "JavaScript crash";
                      }
                    },
                    {
                      title: 'Is Crashes Enabled?',
                      action: async () => {
                        const result = await Crashes.isEnabled()
                        Alert.alert("Crashes enabled?: " + result);
                      }
                    },
                    {
                      title: 'Has Crashed?',
                      action: async () => {
                        const result = await Crashes.hasCrashedInLastSession()
                        if (result) {
                          const crashReport = await Crashes.lastSessionCrashReport();
                          const crashReportString = JSON.stringify(crashReport, null, 4);
                          Alert.alert('App crashed in the last session. Crashes.lastSessionCrashReport(): ', crashReportString);
                        } else {
                          Alert.alert("Has Crashed: " + result);
                        }
                      }
                    },
                    {
                      title: 'Past Memory Warning?',
                      action: async () => {
                        const result = await Crashes.hasReceivedMemoryWarningInLastSession();
                        Alert.alert("Has recieved memory warning in last session: " + result);
                      }
                    },
                    {
                      title: 'Generate Test Crash',
                      action: async () => {
                        await Crashes.generateTestCrash();
                      }
                    },
                    {
                      title: 'Don\'t Send Crash Reports',
                      action: () => {
                        Crashes.notifyUserConfirmation(UserConfirmation.DONT_SEND);
                        
                      }
                    },
                    {
                      title: 'Send Crash Reports',
                      action: () => {
                        Crashes.notifyUserConfirmation(UserConfirmation.SEND);
                      }
                    },
                    {
                      title: 'Always Send Crash Reports',
                      action: () => {
                        Crashes.notifyUserConfirmation(UserConfirmation.ALWAYS_SEND);
                      }
                    },
                  ],
                  renderItem: actionRenderItemCrashes
                },
              ]}
            />
          </View>
        </ScrollView>
      </SafeAreaView>
    </>
  );
};

const styles = StyleSheet.create({
  scrollView: {
    backgroundColor: Colors.lighter,
  },
  engine: {
    position: 'absolute',
    right: 0,
  },
  body: {
    backgroundColor: Colors.white,
  },
  sectionContainer: {
    marginTop: 32,
    paddingHorizontal: 24,
  },
  sectionTitle: {
    fontSize: 24,
    fontWeight: '600',
    color: Colors.black,
  },
  sectionDescription: {
    marginTop: 8,
    fontSize: 18,
    fontWeight: '400',
    color: Colors.dark,
  },
  highlight: {
    fontWeight: '700',
  },
  footer: {
    color: Colors.dark,
    fontSize: 12,
    fontWeight: '600',
    padding: 4,
    paddingRight: 12,
    textAlign: 'right',
  },
});

export default App;
