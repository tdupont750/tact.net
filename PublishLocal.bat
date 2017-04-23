dotnet clean
dotnet restore
dotnet build -c release
for /r %%a in (*.nupkg) do xcopy "%%a" d:\code\packages\ /i /y