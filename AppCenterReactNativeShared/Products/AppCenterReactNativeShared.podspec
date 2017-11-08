Pod::Spec.new do |s|
  s.name              = 'AppCenterReactNativeShared'
  s.version           = '0.99.0'
  s.summary           = 'React Native plugin for AppCenter'
  s.license           = { :type => 'MIT',  :file => 'AppCenterReactNativeShared/LICENSE' }
  s.homepage          = 'https://appcenter.ms'
  s.documentation_url = "https://docs.microsoft.com/en-us/appcenter/"

  s.author           = { 'Microsoft' => 'appcentersdk@microsoft.com' }

#  s.source = { :http => "https://github.com/Microsoft/AppCenter-SDK-React-Native/releases/download/#{s.version}/AppCenter-SDK-ReactNative-iOS-Pod-#{s.version}.zip" } # TODO: uncomment before release
  s.source = { :http => "https://mobilecentersdkdev.blob.core.windows.net/sdk/AppCenter-SDK-ReactNative-iOS-Pod-0.x.x.zip" } # TODO: comment out before release
  s.platform          = :ios, '8.0'
  s.requires_arc      = true

  s.vendored_frameworks = 'AppCenterReactNativeShared/AppCenterReactNativeShared.framework'

  s.dependency 'AppCenter/Core', '0.14.1-23' # TODO: uncomment before release
end
