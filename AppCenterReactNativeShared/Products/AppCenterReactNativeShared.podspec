Pod::Spec.new do |s|
  s.name              = 'AppCenterReactNativeShared'
  s.version           = '5.0.3'
  s.summary           = 'React Native plugin for Visual Studio App Center'
  s.license           = { :type => 'MIT',  :file => 'AppCenterReactNativeShared/LICENSE' }
  s.homepage          = 'https://github.com/microsoft/appcenter-sdk-react-native'
  s.documentation_url = "https://docs.microsoft.com/en-us/appcenter/"
  s.author            = { 'Microsoft' => 'appcentersdk@microsoft.com' }
  s.source            = { :http => "https://github.com/microsoft/appcenter-sdk-react-native/releases/download/#{s.version}/AppCenter-SDK-ReactNative-iOS-Pod-#{s.version}.zip" }
  s.platform          = :ios, '12.0'
  s.requires_arc      = true
  s.vendored_frameworks = 'AppCenterReactNativeShared/AppCenterReactNativeShared.xcframework'
  s.dependency 'AppCenter/Core', '5.0.6'
end
