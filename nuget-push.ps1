dotnet restore
dotnet build .\src\RigoFunc.IdentityServer

dotnet test .\test\RigoFunc.IdentityServer.IntergrationTests
dotnet test .\test\RigoFunc.IdentityServer.UnitTest

$project = Get-Content .\src\RigoFunc.IdentityServer\project.json | ConvertFrom-Json
$version = $project.version.Trim("-*")
nuget push .\src\RigoFunc.IdentityServer\bin\Debug\RigoFunc.IdentityServer.$version.nupkg -source nuget -apikey $env:NUGET_API_KEY