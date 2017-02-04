Pod::Spec.new do |s|
  s.name              = 'RNMobileCenter'
  s.version           = '0.2.0'
  s.summary           = 'React Native plugin for Mobile Center'
  s.license           = { :type => 'MIT',  :file => 'RNMobileCenter/LICENSE' }
  s.homepage          = 'https://mobile.azure.com'
  s.documentation_url = "https://docs.mobile.azure.com"

  s.author           = { 'Microsoft' => 'mobilecentersdk@microsoft.com' }

  s.source = { :http => "https://github.com/Microsoft/MobileCenter-SDK-React-Native/releases/download/#{s.version}/MobileCenter-SDK-ReactNative-iOS-Pod-#{s.version}.zip" }

  s.platform          = :ios, '8.0'
  s.requires_arc      = true

  s.vendored_frameworks = 'RNMobileCenter/RNMobileCenter.framework'

  s.dependency 'MobileCenter', '~> 0.4.0'
end
