$backupFolder="backup_existing_configs" ;
$supportFolder="installationSupportItems" ;
mkdir $backupFolder ;
cd ..\QdasTraceabilityWebApp ;
cp ServerConfig.json ..\$supportFolder\$backupFolder\ -Force ;
cp QdasConfig.toml ..\$supportFolder\$backupFolder\  -Force ;