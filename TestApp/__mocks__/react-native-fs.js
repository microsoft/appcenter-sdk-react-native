const RNFS = jest.mock('react-native-fs');
RNFS.DocumentDirectoryPath = jest.fn();
RNFS.readFile = jest.fn();
RNFS.writeFile = jest.fn();
export default RNFS;
