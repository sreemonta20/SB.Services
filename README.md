## Introduction
[Download Link](https://github.com/sreemonta20/HCS.Services) <br>
This is a Backend security web api service, which is built on .NET 6 (Main Security API service and Email service). It can be used for whole security management of any microservice. It is built on such a way that it could be integrated in future for microservices. API gayeway (Ocelot) will be introduced soon. Entity framework core code first approach has been used here.

# Reference links

- [Google](https://www.google.com/)
- [Own Knowledge base](#)

If you're new to .NET you'll want to check out the tutorial, but if you're
already a seasoned developer considering building your own .NET app with GitLab,
this should all look very familiar.

## What's contained in this project

It contains 
1. the core security feature for user management such as registering and updating user, 
list of all user, get a specific user, delete a user. 
2. These are the operation can be done by administrator after login. 
3. Token management is taken care of.
4. After 3 failed attempts, user can be locked out for 1 min. Also user can modify such settings from appsettings.json. 
5. Email configuration is well configured. Google email is a populr domain for testing whether email has been pushed or not after 3 consecutive failed attempt for login. Due to this , email settings is initialized for smtp.gmail.com. User can change into their domain and test email sending events.


## How to run this backend service along with database

First of all please download this project from github repository. Keep the project in a suitable folder.
1. Install VS 2022 along with .NET version 6 (cross platform). And install SQL server 2016 and management studio 2018 or any upgraded version.
2. Open package manager console. select HCS.Security project and type "dotnet restore" to restore all the packages on which the porject is built on.
3. Modify the environment settings (appsettngs.json) for running the backend service and database on your own environment.
4. Then type "Add-Migration SeedingData" on the same console.
5. After that type  "update-database" on the same console.
6. Now you can check the whether the database has been created on database along with the migration.
7. You can install the postman to testing the api service methods according to the explaination mentioned above. postman collection is packed up in [Postman collection & DBscript file](https://github.com/sreemonta20/HCS-Service-Postman-collection)
