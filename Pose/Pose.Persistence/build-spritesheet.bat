@echo off
rem this batchfile builds the C# code for persisting Pose documents. It generates Document.cs from Document.proto, in the same folder as the batchfile is started.
rem This is only necessary when you have changed the .proto

rem Ensure the path to protoc.exe is correct on your machine. (see the Google.Protobuf.Tools dependency in Solution Explorer for the location of the nuget package)

%UserProfile%\.nuget\packages\google.protobuf.tools\3.13.0\tools\windows_x64\protoc.exe -I=. --csharp_out=. spritesheet.proto

echo done...
pause