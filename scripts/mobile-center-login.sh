# Download and install NPM if it is not already installed
npm -v &>/dev/null
if [ $? -ne 0 ]; then
	# Install npm
	echo "Installing npm..."
    brew install npm
	if [ $? -ne 0 ]; then
    	echo "An error occured while downloading npm."
    	exit 1
	fi 
fi

# Is Mobile Center CLI installed?
npm list -g mobile-center-cli >/dev/null
if [ $? -ne 0 ]; then
	# Install Mobile Center CLI
	echo "Installing Mobile Center CLI..."
	npm install -g mobile-center-cli
	if [ $? -ne 0 ]; then
    	echo "An error occured while installing Mobile Center CLI."
    	exit 1
	fi
fi

# Log in to Mobile Center
echo "Logging in to mobile center..."
mobile-center login --token "$MOBILE_CENTER_API_TOKEN"
if [ $? -ne 0 ]; then
    echo "An error occured while logging into Mobile Center."
    exit 1
fi

exit 0