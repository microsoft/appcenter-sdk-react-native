require 'json'

package = JSON.parse(File.read(File.join(__dir__, 'RNSonomaCore', 'package.json')))

Pod::Spec.new do |s|
  s.name              = 'RNSonomaCore'
  s.version           = '0.0.1'
  s.summary           = 'React Native plugin for Sonoma'
  s.license           = 'MIT'
  s.homepage          = 'https://github.com/Microsoft/MobileCenter-SDK-React-Native#readme'

  s.authors           = { 'Microsoft' => 'support@hockeyapp.net' }

  s.source = { :git => 'https://github.com/Microsoft/MobileCenter-SDK-React-Native.git', :tag => 'v0.0.1' }

  s.platform          = :ios, '8.0'
  s.requires_arc      = true

  s.vendored_frameworks = 'RNSonomaCore/Products/RNSonomaCore/RNSonomaCore.framework'

  s.dependency 'Sonoma'
end