#!/usr/bin/env bash
VERSION=$( docker ps |
 grep production-app |
 egrep -o "ikitsfu/contestantregister:[0-9]+" |
 egrep -o [0-9]+
)

echo staging version is $VERSION
./deploy-to-prod.sh $VERSION