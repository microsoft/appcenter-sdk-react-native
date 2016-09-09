Pod::Spec.new do |s|
  s.name             = "AvalancheErrorReporting"
  s.version          = "0.0.1"
  s.summary          = "Sonoma Error Reporting SDK"

  s.homepage         = "https://github.com/Microsoft/AvalancheSDK-iOS"
  s.license          = 'MIT'
  s.author           = { "Microsoft Corporation" => "codepush@microsoft.com" }
  s.source           = { :git => "https://github.com/Microsoft/AvalancheSDK-iOS.git" }

  s.platform     = :ios, '7.0'
  s.requires_arc = true
  s.vendored_frameworks = 'AvalancheSDK-iOS/Products/AvalancheErrorReporting/AvalancheErrorReporting.framework'
end