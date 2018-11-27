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
  dialogInput: {
    ...Platform.select({
      ios: {
        backgroundColor: 'lightgrey'
      }
    })
  }
});
