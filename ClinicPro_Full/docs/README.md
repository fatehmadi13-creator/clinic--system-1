ClinicPro - Quick Start README

1. Open ClinicProApp.sln or ClinicProApp.csproj in Visual Studio 2022.
2. Restore NuGet packages (System.Data.SQLite.Core, BCrypt.Net-Next, QuestPDF).
3. Ensure schema.sql file's property 'Copy to Output Directory' is set to 'Copy always' in Visual Studio.
4. Build (Release) and Publish -> Folder.
5. Use Inno Setup script in Installer/clinicpro_inno.iss to create installer (adjust Source path).
6. Use LicenseGenerator to produce license keys: LicenseGenerator.exe CLIENTNAME YYYYMMDD
