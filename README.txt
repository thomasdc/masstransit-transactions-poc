Start localstack and postgres containers (in root folder)
> docker-compose up

Create the database:
> create database blogging;

Perform db migrations (in Api folder):
> dotnet ef database update
