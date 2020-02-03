require 'json'

package = JSON.parse(File.read(File.join(__dir__, '../package.json')))

Pod::Spec.new do |s|
  # The name is hardcoded due to a name conflict, resulting in the error 'Errno::ENOENT - No such file or directory @ rb_sysopen - appcenter.podspec.json' error.
  # See https://github.com/microsoft/appcenter-sdk-react-native/issues/760
  s.name              = 'appcenter-core'
  s.version           = package['version']
  s.summary           = package['description']
  s.license           = package['license']
  s.homepage          = package['homepage']
  s.documentation_url = "https://docs.microsoft.com/en-us/appcenter/"

  s.author           = { 'Microsoft' => 'appcentersdk@microsoft.com' }

  s.source            = { :git => "https://github.com/microsoft/appcenter-sdk-react-native.git" }
  s.source_files      = "AppCenterReactNative/**/*.{h,m}"
  s.platform          = :ios, '9.0'
  s.requires_arc      = true

  s.vendored_frameworks = 'AppCenterReactNativeShared/AppCenterReactNativeShared.framework'
  s.dependency 'AppCenterReactNativeShared'
  s.dependency 'React'
  s.static_framework = true
end
