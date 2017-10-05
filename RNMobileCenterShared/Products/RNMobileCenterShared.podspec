Pod::Spec.new do |s|
  s.name              = 'RNMobileCenterShared'
  s.version           = '0.10.0'
  s.summary           = 'React Native plugin for Mobile Center'
  s.license           = { :type => 'MIT',  :file => 'RNMobileCenterShared/LICENSE' }
  s.homepage          = 'https://mobile.azure.com'
  s.documentation_url = "https://docs.mobile.azure.com"

  s.author           = { 'Microsoft' => 'mobilecentersdk@microsoft.com' }

  s.source = { :http => "https://github.com/Microsoft/MobileCenter-SDK-React-Native/releases/download/#{s.version}/MobileCenter-SDK-ReactNative-iOS-Pod-#{s.version}.zip" }

  s.platform          = :ios, '8.0'
  s.requires_arc      = true

  s.vendored_frameworks = 'RNMobileCenterShared/RNMobileCenterShared.framework'

  s.dependency 'MobileCenter/Core', '~> 0.13.0'
end
