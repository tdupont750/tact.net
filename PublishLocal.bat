dotnet clean
dotnet restore
dotnet build -c release
for /r %%a in (*.symbols.nupkg) do xcopy "%%a" d:\code\packages\ /i /y