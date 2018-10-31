cd /d "C:\Users\user\Documents\Visual Studio 2017\Projects\browser1\browser1" &msbuild "OnitorBrowser.csproj" /t:sdvViewer /p:configuration="Debug" /p:platform=x86
exit %errorlevel% 