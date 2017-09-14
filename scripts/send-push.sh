# Define default arguments.
ENVIRONMENT=""
PLATFORM=""

for i in "$@"; do
    case $1 in
        -e|--environment) ENVIRONMENT="-Environment=$2"; shift ;;
        -p|--platform) PLATFORM="-Platform=$2"; shift ;;
    *) shift ;;
    esac
    shift
done

./build.sh -s test-tools.cake -t="SendPushNotification" $PLATFORM $ENVIRONMENT