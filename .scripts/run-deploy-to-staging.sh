echo ======================================================
echo === Run deploy at $HOST_USERNAME@$HOST_ADDRES
export SSHPASS=$HOST_PASSWORD;
sshpass -e ssh -o stricthostkeychecking=no $HOST_USERNAME@$HOST_ADDRESS "cd hosting && bash deploy-to-staging.sh $TRAVIS_BUILD_NUMBER";
echo Deploy completed;
