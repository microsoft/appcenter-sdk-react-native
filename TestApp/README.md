# Test Application for App Center React Native SDK

This test application serves as a tool to verify and test the integration of the App Center React Native SDK. It supports two integration methods for the SDK:

1. **Using Source Code**
2. **Using the NPM Package**

---

## Integration Methods

### 1. Using Source Code

To integrate the App Center SDK from source:

1. Run the following script:
   ```bash
   ./prepare-local-sdk-integration.sh
   ```

2. Build and run the application on your target platform (Android or iOS).

---

### 2. Using the NPM Package

To integrate the App Center SDK via the NPM package:

1. Run the following script:
   ```bash
   ./prepare-npm-sdk-integration.sh
   ```

2. Build and run the application on your target platform (Android or iOS).

---

## Notes

- **iOS:**
  - After running the scripts, you may need to perform the following steps manually:
    1. Navigate to the `ios` directory.
    2. Run `pod install`.
    3. Open the `.xcworkspace` file in Xcode and build the project.

- **Android:**
  - Ensure **Java 17** is installed and configured as the default Java version in your environment.

