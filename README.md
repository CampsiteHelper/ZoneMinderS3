# ZoneMinderS3

dotnet core console application to upload zoneminder alarm images to Amazon S3.

Borrowed heavily from https://github.com/briantroy/Zoneminder-Alert-Image-Upload-to-Amazon-S3

Installation:

1) Install dotnet core 2.0:

- https://docs.microsoft.com/en-us/dotnet/core/linux-prerequisites?tabs=netcore2x

2) Run the createTable.sql file against your zoneminder database

3) Provision a mysql user that can read zoneminder tables from mysql.  

4) Create an s3 bucket and provision access for a user (you'll need an access key id and secret key)

5) Modify the following items in appsettings.json

Key | Setting
------------ | -------------
MySqlConnection | Set to a valid connection string from your mysql instance
ImgBasePath | The path you have set in zoneminder for where images go
S3Region | The region name where your s3 bucket is.
AWS_SECRET_ACCESS_KEY | Your secret AWS key - or alternatively and safer.. set this ENV variable
AWS_ACCESS_KEY_ID | You AWS access key - or alternatively and safer.. set this ENV variable
|| The next five are optional if you want to send an email with images to yourself through SES (or any smtp server..)
AWS_SES_USERNAME | SMTP Server username
AWS_SES_PWD | SMTP Server password
AWS_SES_SMTP | SMTP Server host name
FROM_ADDDRESS | email from address
TO_ADDDRESS | email to address

6) Run it

    dotnet restore
    dotnet run & 

7) Daemonize it so it runs at start up - See the internets for how to daemonize dotnet core on your distro.

