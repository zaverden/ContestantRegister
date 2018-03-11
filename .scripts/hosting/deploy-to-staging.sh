#!/usr/bin/env bash
VERSION=$1
./rebuild-container.sh staging $VERSION 25000
# remove unused images
docker system prune --all --force
