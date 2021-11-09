if ($args.Length -gt 0) {
    $ver = $args[0]
}
else {
    $ver = Read-Host 'version'
}

rm -Recurse -Path release
mkdir release
mkdir release\net462\AlphabetUpdate.Client
mkdir release\net462\AlphabetUpdate.Common
mkdir release\net462\AuthYouClient
mkdir release\netstandard2.1\AlphabetUpdate.Client
mkdir release\netstandard2.1\AlphabetUpdate.Common
mkdir release\netstandard2.1\AuthYouClient

Copy-Item .\src\AlphabetUpdate.Client\bin\Release\AlphabetUpdate.Client.$ver.nupkg .\release
Copy-Item .\src\AlphabetUpdate.Common\bin\Release\AlphabetUpdate.Common.$ver.nupkg .\release
Copy-Item .\src\AuthYouClient\bin\Release\AuthYouclient.$ver.nupkg .\release

nuget add .\release\AlphabetUpdate.Client.$ver.nupkg -Source C:\p\nuget\
nuget add .\release\AlphabetUpdate.Common.$ver.nupkg -Source C:\p\nuget\
nuget add .\release\AuthYouClient.$ver.nupkg -Source C:\p\nuget\

Copy-Item .\src\AlphabetUpdate.Client\bin\Release\net462\* .\release\net462\AlphabetUpdate.Client\
Copy-Item .\src\AlphabetUpdate.Common\bin\Release\net462\* .\release\net462\AlphabetUpdate.Common\
Copy-Item .\src\AuthYouClient\bin\Release\net462\* .\release\net462\AuthYouClient\

Copy-Item .\src\AlphabetUpdate.Client\bin\Release\netstandard2.1\* .\release\netstandard2.1\AlphabetUpdate.Client\
Copy-Item .\src\AlphabetUpdate.Common\bin\Release\netstandard2.1\* .\release\netstandard2.1\AlphabetUpdate.Common\
Copy-Item .\src\AuthYouClient\bin\release\netstandard2.1\* .\release\netstandard2.1\AuthYouClient\

pause