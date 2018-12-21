# livechat-api
Sample API for live chat application with ASP.NET Core SignalR


## EF database comands
###Add migration:
```
dotnet ef migrations add MigrationName --context LiveChatDbContext -p LiveChat.Data/LiveChat.Data.csproj -s LiveChat.API/LiveChat.API.csproj -o Migrations
```

###Update database:
```
dotnet ef database update --context LiveChatDbContext -p LiveChat.Data/LiveChat.Data.csproj -s LiveChat.API/LiveChat.API.csproj
```
