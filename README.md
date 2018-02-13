# ContestantRegister

[![Build Status](https://travis-ci.org/zaverden/ContestantRegister.svg?branch=master)](https://travis-ci.org/zaverden/ContestantRegister)

## Hosting

Change user
```bash
sudo su app_user
cd ~/hosting
```

- `./production.env` and `./staging.env` - environment variables to start containers
- `./deploy-from-staging-to-production.sh` - get version from staging and start it on production
- `./rebuild-production.sh` - restart production container with new evn params
- `./rebuild-staging.sh` - restart staging container with new evn params
- `./deploy-to-production.sh 22` - deploy specific build to production
- `./deploy-to-staging.sh 22` - deploy specific build to staging

Logs are located in `/storage/production-logs/` and `/storage/staging-logs/`.