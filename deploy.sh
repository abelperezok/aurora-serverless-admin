#!/bin/bash

serverless deploy \
--aws-profile serverless-web \
--opt-vpc-id vpc-b9fe75de \
--opt-subnet-ids subnet-97d258de,subnet-6e9d1f09,subnet-0fbf7154 \
--opt-db-pwd Aurora2018 \
--opt-db-user master \
--opt-db-name generator

STACK_NAME=aurora-serverless-admin-dev

BUCKET_NAME=`aws cloudformation describe-stacks --stack-name $STACK_NAME \
--query Stacks[0].Outputs \
| jq -r '.[] | select(.OutputKey == "S3BucketName").OutputValue'`

WEBSITE_URL=`aws cloudformation describe-stacks --stack-name $STACK_NAME \
--query Stacks[0].Outputs \
| jq -r '.[] | select(.OutputKey == "WebsiteURL").OutputValue'`

aws s3 sync ./frontend s3://$BUCKET_NAME --delete --acl=public-read

echo $WEBSITE_URL