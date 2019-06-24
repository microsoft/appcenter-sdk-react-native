import AsyncStorage from '@react-native-community/async-storage';
import RNFS from "react-native-fs";

const TEXT_ATTACHMENT_KEY = "TEXT_ATTACHMENT_KEY";
const BINARY_FILENAME_KEY = "BINARY_FILENAME_KEY";
const BINARY_FILETYPE_KEY = "BINARY_FILETYPE_KEY";
const BINARY_FILESIZE_KEY = "BINARY_FILESIZE_KEY";

const DEFAULT_FILENAME = "binary.txt";
const DEFAULT_ENCODING = "utf8";

export default class AttachmentsProvider {
  static async saveTextAttachment(value: string) {
    await AsyncStorage.setItem(TEXT_ATTACHMENT_KEY, value);
  }

  static async getTextAttachment() {
    return getItemFromStorage(TEXT_ATTACHMENT_KEY, "hello");
  }

  static async saveBinaryAttachment(
    name: string,
    data: string,
    type: string,
    size: string
  ) {
    AsyncStorage.setItem(BINARY_FILENAME_KEY, name);
    AsyncStorage.setItem(BINARY_FILETYPE_KEY, type);
    AsyncStorage.setItem(BINARY_FILESIZE_KEY, size);
    saveFileInDocumentsFolder(DEFAULT_FILENAME, data);
  }

  static async getBinaryAttachment() {
    const path = `${RNFS.DocumentDirectoryPath}/${DEFAULT_FILENAME}`;
    let contents = "";
    try {
      contents = await RNFS.readFile(path, DEFAULT_ENCODING);
    } catch (error) {
      console.log(
        `Error while reading binary attachment file, error: ${error}`
      );
    }
    return contents;
  }

  static async getBinaryName() {
    return getItemFromStorage(BINARY_FILENAME_KEY);
  }

  static async getBinaryType() {
    return getItemFromStorage(BINARY_FILETYPE_KEY);
  }

  static async getBinaryAttachmentInfo() {
    const fileName = await getItemFromStorage(BINARY_FILENAME_KEY);
    const fileSize = await getItemFromStorage(BINARY_FILESIZE_KEY);
    return `${fileName} (${fileSize})`;
  }
}

async function getItemFromStorage(key: string, defaultValue = "") : Promise<string> {
  try {
    const item = await AsyncStorage.getItem(key);
    return item || defaultValue;
  } catch (error) {
    console.error(`Error retrieving item with key: ${key}`);
    console.error(error.message);
  }
  return defaultValue;
}

async function saveFileInDocumentsFolder(fileName: string, data: string) {
  const path = `${RNFS.DocumentDirectoryPath}/${fileName}`;
  RNFS.writeFile(path, data, DEFAULT_ENCODING)
    .then(() => console.log("Binary attachment saved"))
    .catch(err => console.error(err.message));
}
