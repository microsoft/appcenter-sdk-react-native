require 'json'

package = JSON.parse(File.read(File.join(__dir__, '../', 'package.json')))

Pod::Spec.new do |s|
  s.name              = 'AppCenterReactNativePush'
  s.version           = package['version']
  s.summary           = package['description']
  s.license           = package['license']
  s.homepage          = package['homepage']
  s.documentation_url = "https://docs.microsoft.com/en-us/appcenter/"

  s.author            = { 'Microsoft' => 'appcentersdk@microsoft.com' }

  s.source            = { :git => "https://github.com/Microsoft/AppCenter-SDK-React-Native.git" }
  s.source_files      = "AppCenterReactNativePush/**/*.{h,m}"
  s.platform          = :ios, '9.0'
  s.requires_arc      = true

  s.vendored_frameworks = 'AppCenterReactNativeShared/AppCenterReactNativeShared.framework'

  s.dependency 'AppCenterReactNativeShared', '1.13.0'
  s.dependency 'AppCenter/Push', '1.13.0'
  s.dependency 'React'
end
