echo ======================================================
echo === Run deploy at $HOST_USERNAME@$HOST_ADDRES
export SSHPASS=$HOST_PASSWORD;
sshpass -e ssh -o stricthostkeychecking=no $HOST_USERNAME@$HOST_ADDRESS "bash hosting/deploy-to-staging.sh $TRAVIS_BUILD_NUMBER";
echo Deploy completed;
