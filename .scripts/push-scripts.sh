echo ======================================================
echo === Push scripts to hosting $HOST_USERNAME@$HOST_ADDRES
echo list scripts
ls ./.scripts/hosting/
export SSHPASS=$HOST_PASSWORD
echo start copy
sshpass -e scp -o stricthostkeychecking=no ./.scripts/hosting/* $HOST_USERNAME@$HOST_ADDRESS:
echo done copy
