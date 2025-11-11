[Setup]
AppName=ClinicPro
AppVersion=1.0
DefaultDirName={pf}\ClinicPro
DefaultGroupName=ClinicPro
OutputBaseFilename=ClinicPro_Setup
Compression=lzma
SolidCompression=yes

[Files]
Source: "ClinicProApp\bin\Release\net6.0-windows\publish\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs

[Icons]
Name: "{group}\ClinicPro"; Filename: "{app}\ClinicPro.exe"
Name: "{userdesktop}\ClinicPro"; Filename: "{app}\ClinicPro.exe"

[Run]
Filename: "{app}\ClinicPro.exe"; Description: "Run ClinicPro"; Flags: nowait postinstall skipifsilent
