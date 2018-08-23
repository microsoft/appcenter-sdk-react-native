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
    color: 'black'
  },
  itemButton: {
    color: 'cornflowerblue',
    textAlign: 'center',
    width: '100%'
  },
  modalSelector: {
    borderColor: 'gray',
    backgroundColor: 'white'
  },
});
