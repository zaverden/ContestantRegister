#!/usr/bin/env bash
VERSION=$( docker ps -a |
 grep staging-app |
 egrep -o "ikitsfu/contestantregister:[0-9]+" |
 egrep -o [0-9]+
)

echo staging version is $VERSION
./deploy-to-staging.sh $VERSION