Pod::Spec.new do |s|
  s.name              = 'RNMobileCenter'
  s.version           = '0.0.1'
  s.summary           = 'React Native plugin for Mobile Center'
  s.license           = 'MIT'
  s.homepage          = 'https://github.com/Microsoft/MobileCenter-SDK-React-Native#readme'

  s.author           = { 'Microsoft' => 'mobilecentersdk@microsoft.com' }

  #s.source = { :git => 'https://github.com/Microsoft/MobileCenter-SDK-React-Native.git', :tag => 'v0.0.1' }
  s.source = { :path => '.' }

  s.platform          = :ios, '8.0'
  s.requires_arc      = true

  s.vendored_frameworks = 'RNMobileCenter/Products/RNMobileCenter/RNMobileCenter.framework'

  s.dependency 'MobileCenter'
end