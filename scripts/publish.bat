@ECHO OFF

cd ..
cd WebApi
CMD /C dotnet publish --output ../dist

cd ..
cd WebClient
CMD /C npm run build
xcopy /s /y "dist" "..\dist\wwwroot" 

cls
echo Web Application has been succesfully published in /dist folder...
@pause