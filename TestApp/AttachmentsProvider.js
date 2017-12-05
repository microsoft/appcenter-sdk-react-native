import React, { Component } from 'react';
import { AsyncStorage} from 'react-native';
import RNFS from 'react-native-fs';

const TEXT_ATTACHMENT_KEY = 'TEXT_ATTACHMENT_KEY';
const BINARY_FILENAME_KEY = 'BINARY_FILENAME_KEY';
const BINARY_FILETYPE_KEY = 'BINARY_FILETYPE_KEY';
const BINARY_FILESIZE_KEY = 'BINARY_FILESIZE_KEY';

const DEFAULT_FILENAME = 'binary.txt';
const DEFAULT_ENCODING = 'utf8';

export default class AttachmentsProvider  {

    static async saveTextAttachment(value) {
        await AsyncStorage.setItem(TEXT_ATTACHMENT_KEY, value);
    }

    static async getTextAttachment() {
        return await getItemFromStorage(TEXT_ATTACHMENT_KEY, 'hello');
    }

    static async saveBinaryAttachment(name, data, type, size) {
        AsyncStorage.setItem(BINARY_FILENAME_KEY, name);
        AsyncStorage.setItem(BINARY_FILETYPE_KEY, type);
        AsyncStorage.setItem(BINARY_FILESIZE_KEY, size);
        saveFileInDocumentsFolder(DEFAULT_FILENAME, data);
    }

    static async getBinaryAttachment() {
        var path = RNFS.DocumentDirectoryPath + '/' + DEFAULT_FILENAME;
        var contents = '';
        try {
            contents = await RNFS.readFile(path, DEFAULT_ENCODING)
        } catch (error) {
            console.log('Error while reading binary attachment file');
        }
        return contents;
    }

    static async getBinaryName() {
        return await getItemFromStorage(BINARY_FILENAME_KEY);
    }

    static async getBinaryType() {
        return await getItemFromStorage(BINARY_FILETYPE_KEY);
    }

    static async getBinaryAttachmentInfo() {
        let fileName = await getItemFromStorage(BINARY_FILENAME_KEY);
        let fileSize = await getItemFromStorage(BINARY_FILESIZE_KEY);
        return fileName + ' (' + fileSize + ')';
    }
}

async function getItemFromStorage(key, defaultValue = '') {
    try {
        return await AsyncStorage.getItem(key);
    } catch (error) {
        console.log('Error retrieving item with key: ' + key);
        console.log(error.message);
    }
    return defaultValue;
}

async function saveFileInDocumentsFolder(fileName, data) {
    var path = RNFS.DocumentDirectoryPath + '/' + fileName;
    RNFS.writeFile(path, data, DEFAULT_ENCODING)
        .then((success) => {
            console.log('Binary attachment saved');
        })
        .catch((err) => {
            console.log(err.message);
        });
}