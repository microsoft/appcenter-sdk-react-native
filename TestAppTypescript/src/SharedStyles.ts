import { StyleSheet } from "react-native";

const SharedStyles = StyleSheet.create({
  heading: {
    fontSize: 24,
    textAlign: "center",
    marginBottom: 20
  },
  container: {
    flex: 1,
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "#F5FCFF"
  },
  button: {
    color: "#4444FF",
    fontSize: 18,
    textAlign: "center",
    margin: 10
  },
  enabledText: {
    fontSize: 14,
    textAlign: "center"
  },
  toggleEnabled: {
    color: "#4444FF",
    fontSize: 14,
    textAlign: "center",
    marginBottom: 10
  },
  dialogInput: {
    height: 40,
    borderColor: "gray",
    borderWidth: 1,
    margin: 8
  },
  dialogButton: {
    height: 50
  },
  dialogButtonContainer: {
    flex: 1,
    flexDirection: "row",
    justifyContent: "space-between"
  }
});

export default SharedStyles;
