require 'json'

package = JSON.parse(File.read(File.join(__dir__, 'RNSonomaCore', 'package.json')))

Pod::Spec.new do |s|
  s.name              = 'RNSonomaCore'
  s.version           = package['version']
  s.summary           = package['description']
  s.license           = package['license']
  s.homepage          = package['homepage']

  s.authors           = { 'Microsoft' => 'support@hockeyapp.net' }

  s.source = { :git => 'https://github.com/Microsoft/react-native-sonoma-private.git' }

  s.platform          = :ios, '8.0'
  s.requires_arc      = true

  s.source_files = 'RNSonomaCore/ios/RNSonomaCore/*.{h,m}'

  s.dependency 'Sonoma'
  s.frameworks = 'SonomaCore'
end