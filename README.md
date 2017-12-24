# ZoneMinderS3

dotnet core console application to upload zoneminder alarm images to Amazon S3.

Borrowed heavily from https://github.com/briantroy/Zoneminder-Alert-Image-Upload-to-Amazon-S3

Installation:

1) Install dotnet core 2.0:

- https://docs.microsoft.com/en-us/dotnet/core/linux-prerequisites?tabs=netcore2x

2) Run the createTable.sql file against your zoneminder database

3) Provision a user that can read zoneminder tables from mysql.

4) Create an s3 bucket and provision access for a user (you'll need an access key id and secret key)

4) Modify the following items in appsettings.json

Key | Setting
------------ | -------------
MySqlConnection | Set to a valid connection string from your mysql instance
ImgBasePath | The path you have set in zoneminder for where images go
S3Region | The region name where your s3 bucket is.
AWS_SECRET_ACCESS_KEY | This is optional if you've set this same ENV variable
AWS_ACCESS_KEY_ID | This is optional if you've set this same ENV variable
