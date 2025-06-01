# CursorLockerTrayApp

Prerequired:
- .NET 8

Start on local:
```sh
dotnet run
```

Publish to EXE:
```sh
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=false
```

Publish into single file:
```sh
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:Trim=true
```

If you need to stop lock the cursor, press `ESC`.