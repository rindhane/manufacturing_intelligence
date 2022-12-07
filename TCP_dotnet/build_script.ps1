$buildFolder="publish";
$appFolder="scannerReader";
rm -r $appFolder ;
mkdir $appFolder ;
dotnet publish -c Release -r win-x64 --self-contained true -o $buildFolder ;
#dotnet publish -c Release -r linux-x64 --self-contained true -o $buildFolder; #not working
mv $buildFolder\* $appFolder ;
cp scannerConfig.toml $appFolder;
Compress-Archive -Path $appFolder ,
                        changeConfig.cmd ,
                        StartUpScript.cmd `
                -DestinationPath $appFolder".zip";
rm -r $appFolder\* ;