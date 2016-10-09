dotnet restore
dotnet build ./src/Host
dotnet test ./test/RigoFunc.IdentityServer.UnitTest
dotnet test ./test/RigoFunc.IdentityServer.IntergrationTests