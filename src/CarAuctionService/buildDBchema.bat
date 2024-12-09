rmdir /S /Q "Data/Migrations"

dotnet ef migrations add "InitilaCreate" -o Data/Migrations
