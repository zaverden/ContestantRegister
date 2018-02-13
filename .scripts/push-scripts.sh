echo ======================================================
echo === Push scripts to hosting $HOST_USERNAME@$HOST_ADDRES
echo list scripts
ls ./.scripts/hosting/
export SSHPASS=$HOST_PASSWORD
echo start copy
sshpass -e scp -o stricthostkeychecking=no -r ./.scripts/hosting/ $HOST_USERNAME@$HOST_ADDRESS:
sshpass -e ssh -o stricthostkeychecking=no $HOST_USERNAME@$HOST_ADDRESS "chmod +x ./hosting/*.sh";

echo done copy
