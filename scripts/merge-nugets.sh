# Define constants
NUGET_MAC=$1
NUGET_WINDOWS=$2
NUSPEC=$3
VERSION=$4
WORKING_DIR="temporary_nuget_folder"
NUGET_MAC_UNZIPPED="mac_nuget_folder"
NUGET_WINDOWS_UNZIPPED="windows_nuget_folder"
MAC_DIR=$TEMP_DIRECTORY/$NUGET_MAC_UNZIPPED
WINDOWS_DIR=$TEMP_DIRECTORY/$NUGET_WINDOWS_UNZIPPED
OUTPUT_DIRECTORY="../output"

# Unzip the nuget packages and send them to a temporary directory
unzip $NUGET_MAC -d $MAC_DIR
unzip $NUGET_WINDOWS -d $WINDOWS_DIR

# Create the Nuget pakage
nuget pack $NUSPEC properties "mac_dir=$MAC_DIR;windows_dir=$WINDOWS_DIR;version=$VERSION" outputdirectory $OUTPUT_DIRECTORY

# Clean up
rm -rf $TEMP_DIRECTORY
