@echo off
c:\windows\microsoft.net\framework\v3.5\msbuild.exe /p:Configuration=Release
rmdir /s /q dist
mkdir dist
mkdir dist\log
xcopy /s ConnectsterAuthentication\*.* dist\ConnectsterAuthentication\
xcopy /s ConnectsterService\bin\release\*.* dist\ConnectsterService\
