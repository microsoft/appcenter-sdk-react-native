require 'json'

package = JSON.parse(File.read(File.join(__dir__, 'package.json')))

Pod::Spec.new do |s|
  s.name              = 'RNSonomaCrashes'
  s.version           = package['version']
  s.summary           = package['description']
  s.license           = package['license']
  s.homepage          = package['homepage']

  s.authors           = { 'Microsoft Corporation' => 'codepush@microsoft.com' }

  s.source = { :http => 'https://github.com/Microsoft/react-native-sonoma-private' }

  s.platform          = :ios, '7.0'
  s.requires_arc      = true

  s.source_files = 'ios/RNSonomaCrashes/*.{h,m}'

  s.dependency 'React'
  s.dependency 'SonomaCore'
end