echo ======================================================
echo === Push image ikitsfu/contestantregister:$TRAVIS_BUILD_NUMBER as ${DOCKER_USERNAME}
docker login -u ${DOCKER_USERNAME} -p ${DOCKER_PASSWORD}
docker push ikitsfu/contestantregister:$TRAVIS_BUILD_NUMBER