$docFolder="..\dotnetWebService\wwwroot\instructions\";
hugo
cp public/* $docFolder -Force 
rm -r public/*