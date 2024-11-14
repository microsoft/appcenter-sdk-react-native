// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { StyleSheet, Platform } from 'react-native';

export default StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: 'whitesmoke',
    ...Platform.select({
      ios: {
        paddingTop: 25
      }
    })
  },
  header: {
    marginLeft: 10,
    marginTop: 10,
    fontSize: 12
  },
  item: {
    flex: 1,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: 10,
    height: 44,
    borderWidth: StyleSheet.hairlineWidth,
    borderColor: 'gray',
    backgroundColor: 'white'
  },
  itemStretchable: {
    flex: 1,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: 10,
    borderWidth: StyleSheet.hairlineWidth,
    borderColor: 'gray',
    backgroundColor: 'white'
  },
  itemTitle: {
    color: 'black',
    marginRight: 5
  },
  itemButton: {
    color: 'cornflowerblue',
    textAlign: 'center',
    width: '100%'
  },
  itemInput: {
    flex: 1,
    padding: 5,
    marginTop: -5,
    marginBottom: -5,
    textAlign: 'right'
  },
  underlinedItemInput: {
    flex: 1,
    padding: 5,
    marginTop: -5,
    marginBottom: -5,
    textAlign: 'right',
    borderBottomWidth: 1
  },
  modalSelector: {
    borderColor: 'gray',
    backgroundColor: 'white'
  },
  modalTextInput: {
    borderBottomWidth: 1,
    borderBottomColor: 'gray',
  },
  modalItem: {
    flexDirection: 'row',
    alignItems: 'center',
    marginHorizontal: 8,
    marginVertical: 8
  },
  modalButton: {
    flex: 0.5,
    padding: 16,
    borderColor: 'grey',
    borderWidth: 1,
  },
  dialogInput: {
    ...Platform.select({
      ios: {
        backgroundColor: 'lightgrey'
      }
    })
  }
});
