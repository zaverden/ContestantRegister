#!/usr/bin/env bash
ENVIR=$1
VERSION=$2
PORT=$3
NAME=$ENVIR-app
docker pull ikitsfu/contestantregister:$VERSION
docker stop $NAME
docker rm $NAME
docker run \
 --name $NAME \
 --detach \
 --restart unless-stopped \
 --publish $PORT:5000 \
 --link pg \
 --link inbucket \
 --env-file $ENVIR.env \
 --volume /storage/$ENVIR-logs:/app/logs \
 --volume /storage/$ENVIR-data-protection-keys:/app/data-protection-keys \
 ikitsfu/contestantregister:$VERSION
