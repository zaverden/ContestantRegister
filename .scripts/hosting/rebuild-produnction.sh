#!/usr/bin/env bash
VERSION=$( docker ps -a |
 grep production-app |
 egrep -o "ikitsfu/contestantregister:[0-9]+" |
 egrep -o [0-9]+
)

echo staging version is $VERSION
./deploy-to-production.sh $VERSION