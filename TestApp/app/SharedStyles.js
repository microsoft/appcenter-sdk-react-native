import { StyleSheet, Platform } from 'react-native';

export default StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: 'whitesmoke',
    ...Platform.select({
      ios: {
        paddingTop: 15
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
  dialogInput: {
    height: 40,
    borderColor: 'gray',
    borderWidth: 1,
    margin: 8
  },
  dialogButton: {
    height: 50,
    padding: 8
  },
  dialogButtonContainer: {
    flex: 1,
    flexDirection: 'row',
    justifyContent: 'flex-end'
  },
  modalSelector: {
    borderColor: 'gray',
    backgroundColor: 'white'
  },
});
