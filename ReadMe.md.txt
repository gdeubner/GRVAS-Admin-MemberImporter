### Member Importer

This is an AWS Lambda function, triggered using EventBridge to run nightly. It imports member data from a csv, parses it, and then uploads it to a new dynamoDB table for use by other services and lambdas.

To (re)deploy this lambda to aws, run the following from your terminal, opened to the folder containing the serverless.yml folder:
`npm run build`
`serverless deploy --stage dev`