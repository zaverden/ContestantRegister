echo ======================================================
echo === Build image ikitsfu/contestantregister:$TRAVIS_BUILD_NUMBER
docker build --tag ikitsfu/contestantregister:$TRAVIS_BUILD_NUMBER .