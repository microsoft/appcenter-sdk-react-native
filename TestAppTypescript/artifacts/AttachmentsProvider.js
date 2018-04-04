var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { AsyncStorage } from 'react-native';
import RNFS from 'react-native-fs';
const TEXT_ATTACHMENT_KEY = 'TEXT_ATTACHMENT_KEY';
const BINARY_FILENAME_KEY = 'BINARY_FILENAME_KEY';
const BINARY_FILETYPE_KEY = 'BINARY_FILETYPE_KEY';
const BINARY_FILESIZE_KEY = 'BINARY_FILESIZE_KEY';
const DEFAULT_FILENAME = 'binary.txt';
const DEFAULT_ENCODING = 'utf8';
export default class AttachmentsProvider {
    static saveTextAttachment(value) {
        return __awaiter(this, void 0, void 0, function* () {
            yield AsyncStorage.setItem(TEXT_ATTACHMENT_KEY, value);
        });
    }
    static getTextAttachment() {
        return __awaiter(this, void 0, void 0, function* () {
            return getItemFromStorage(TEXT_ATTACHMENT_KEY, 'hello');
        });
    }
    static saveBinaryAttachment(name, data, type, size) {
        return __awaiter(this, void 0, void 0, function* () {
            AsyncStorage.setItem(BINARY_FILENAME_KEY, name);
            AsyncStorage.setItem(BINARY_FILETYPE_KEY, type);
            AsyncStorage.setItem(BINARY_FILESIZE_KEY, size);
            saveFileInDocumentsFolder(DEFAULT_FILENAME, data);
        });
    }
    static getBinaryAttachment() {
        return __awaiter(this, void 0, void 0, function* () {
            const path = `${RNFS.DocumentDirectoryPath}/${DEFAULT_FILENAME}`;
            let contents = '';
            try {
                contents = yield RNFS.readFile(path, DEFAULT_ENCODING);
            }
            catch (error) {
                console.log(`Error while reading binary attachment file, error: ${error}`);
            }
            return contents;
        });
    }
    static getBinaryName() {
        return __awaiter(this, void 0, void 0, function* () {
            return getItemFromStorage(BINARY_FILENAME_KEY);
        });
    }
    static getBinaryType() {
        return __awaiter(this, void 0, void 0, function* () {
            return getItemFromStorage(BINARY_FILETYPE_KEY);
        });
    }
    static getBinaryAttachmentInfo() {
        return __awaiter(this, void 0, void 0, function* () {
            const fileName = yield getItemFromStorage(BINARY_FILENAME_KEY);
            const fileSize = yield getItemFromStorage(BINARY_FILESIZE_KEY);
            return `${fileName} (${fileSize})`;
        });
    }
}
function getItemFromStorage(key, defaultValue = '') {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            return yield AsyncStorage.getItem(key);
        }
        catch (error) {
            console.error(`Error retrieving item with key: ${key}`);
            console.error(error.message);
        }
        return defaultValue;
    });
}
function saveFileInDocumentsFolder(fileName, data) {
    return __awaiter(this, void 0, void 0, function* () {
        const path = `${RNFS.DocumentDirectoryPath}/${fileName}`;
        RNFS.writeFile(path, data, DEFAULT_ENCODING)
            .then(() => console.log('Binary attachment saved'))
            .catch(err => console.error(err.message));
    });
}
//# sourceMappingURL=AttachmentsProvider.js.map