rm -Recurse -Path release
mkdir release
mkdir release\net462\AlphabetUpdate.Client
mkdir release\net462\AlphabetUpdate.Common
mkdir release\net462\AuthYouClient
mkdir release\netstandard2.1\AlphabetUpdate.Client
mkdir release\netstandard2.1\AlphabetUpdate.Common
mkdir release\netstandard2.1\AuthYouClient

Copy-Item .\AlphabetUpdate.Client\bin\Release\net462\* .\release\net462\AlphabetUpdate.Client\
Copy-Item .\AlphabetUpdate.Common\bin\Release\net462\* .\release\net462\AlphabetUpdate.Common\
Copy-Item .\AuthYouClient\bin\Release\net462\* .\release\net462\AuthYouClient\

Copy-Item .\AlphabetUpdate.Client\bin\Release\netstandard2.1\* .\release\netstandard2.1\AlphabetUpdate.Client\
Copy-Item .\AlphabetUpdate.Common\bin\Release\netstandard2.1\* .\release\netstandard2.1\AlphabetUpdate.Common\
Copy-Item .\AuthYouClient\bin\release\netstandard2.1\* .\release\netstandard2.1\AuthYouClient\

pause