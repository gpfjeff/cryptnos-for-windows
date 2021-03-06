[Setup]
InternalCompressLevel=ultra
OutputDir=.\bin
OutputBaseFilename=Cryptnos_1.3.4_Setup
VersionInfoVersion=1.3.4.0
VersionInfoCompany=GPF Comics
VersionInfoDescription=This program will install Cryptnos on your computer.  Cryptnos is a cryptographic passphrase generator using the Microsoft .NET 2.0 Framework.
VersionInfoTextVersion=Cryptnos Setup 1.3.4
VersionInfoCopyright=Copyright 2015, Jeffrey T. Darlington.
AppCopyright=Copyright 2015, Jeffrey T. Darlington.
AppName=Cryptnos
AppVerName=Cryptnos 1.3.4
LicenseFile=.\gpl.rtf
PrivilegesRequired=poweruser
MinVersion=0,5.0.2195sp3
DefaultDirName={pf}\Cryptnos
DefaultGroupName=Cryptnos
AppID=GPFComicsCryptnos
UninstallDisplayIcon={app}\Cryptnos.exe
Compression=lzma/ultra
ChangesEnvironment=true
AppPublisher=GPF Comics
AppPublisherURL=http://www.gpf-comics.com/
AppSupportURL=http://www.cryptnos.com/
AppUpdatesURL=http://www.cryptnos.com/
AppVersion=Cryptnos 1.3.4
UninstallDisplayName=Cryptnos 1.3.4
SetupIconFile=..\Cryptnos\Lock.ico
[Files]
Source: ..\Cryptnos\bin\Release\Cryptnos.exe; DestDir: {app}
Source: ..\BouncyCastle.Crypto.dll; DestDir: {app}
Source: ..\cryptnos_export1.xsd; DestDir: {app}; Flags: ignoreversion
Source: ..\GPFUpdateChecker.dll; DestDir: {app}
Source: ..\gpf_update_checker1.xsd; DestDir: {app}; Flags: ignoreversion
Source: help.html; DestDir: {app}; Flags: ignoreversion
Source: gpl.html; DestDir: {app}; Flags: ignoreversion
Source: ..\zxing.dll; DestDir: {app}; 
[Icons]
Name: {group}\Cryptnos; Filename: {app}\Cryptnos.exe; WorkingDir: {userdocs}; IconFilename: {app}\Cryptnos.exe; IconIndex: 0; Comment: Cryptnos allows you to generate strong passphrases using cryptographic hashes
Name: {group}\Online Help; Filename: {app}\help.html; WorkingDir: {app}; Comment: Display help information for Cryptnos in your default Web browser
Name: {group}\Uninstall Cryptnos; Filename: {uninstallexe}; WorkingDir: {app}; Comment: Uninstall Cryptnos
Name: {commondesktop}\Cryptnos; Filename: {app}\Cryptnos.exe; WorkingDir: {userdocs}; IconFilename: {app}\Cryptnos.exe; IconIndex: 0; Comment: Cryptnos allows you to generate strong passphrases using cryptographic hashes
Name: {userappdata}\Microsoft\Internet Explorer\Quick Launch\Cryptnos; Filename: {app}\Cryptnos.exe; WorkingDir: {userdocs}; IconFilename: {app}\Cryptnos.exe; IconIndex: 0; Comment: Cryptnos allows you to generate strong passphrases using cryptographic hashes
[Code]
// InitializeSetup:  Check to see if .NET 2.0 is installed and abort if it isn't.
function InitializeSetup(): Boolean;
var
   DotNetRegKey: String;
   DotNetDlURL:  String;
   ErrorCode:    Integer;
begin
   // Set up our constants, abstracted here to make changing them
   // later easier.  The first is the .NET 2.0 registry key.
   // Actually, it's the setup program's regkey, but a Google
   // search said this was the best place to look.  .NET setup will
   // not install if it finds this key, so if it's good enough for
   // Microsoft, it's good enough for us.
   DotNetRegKey := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727';
   // The URL of where to download .NET:
   DotNetDlURL := 'http://msdn2.microsoft.com/en-us/netframework/aa731542.aspx';
   // Check the registry to see if the .NET registry key exists.
   // If not, then we go to work:
   if not RegKeyExists(HKLM, DotNetRegKey) then begin
      // Ask the user if they want to download .NET:
      if MsgBox('The Microsoft .NET Framework version 2.0 or higher is required to run this application, but I couldn''t find it installed on your system.  Would you like to download it now?', mbConfirmation, MB_YESNO) = IDYES then begin
         // Open the download URL in the default browser:
         ShellExec('open', DotNetDlURL, '', '', SW_SHOW, ewNoWait, ErrorCode);
      end
      // If they decided not to download .NET now, tell them they
      // can always get it from Windows Update:
      else begin
         MsgBox('OK, but you can also install the framework through Windows Update.  This installer will now exit.', mbInformation, MB_OK);
      end;
      // In all cases above, we want to stop the installation here:
      Result := False;
   end
   // If .NET was found, everything is rosy.  Proceed with the
   // installation:
   else begin
      //MsgBox('Found .NET Framework 2.0 or higher', mbInformation, MB_OK);
      Result := True;
   end;
end;
// DeinitializeUninstall:  Offer to remove our registry keys upon uninstall if the user
// doesn't want to keep them.
procedure DeinitializeUninstall();
var
	CryptnosRegKey: String;
	GPFComicsRegKey: String;
begin
	// Define both the GPF Comics key and the Cryptnos key.  We'll definitely delete the
	// Cryptnos key if present and also the GPF Comics key if no other subkeys exists.
	GPFComicsRegKey := 'SOFTWARE\GPF Comics';
	CryptnosRegKey := GPFComicsRegKey + '\Cryptnos';
	// If the Cryptnos key exists...
	if RegKeyExists(HKCU, CryptnosRegKey) then begin
		// Confirm with the user that we'll delete their settings:
		if MsgBox('Would you like to remove your saved preferences from the registry?  If you plan to reinstall Cryptnos, you should probably say no.', mbConfirmation, MB_YESNO) = IDYES then begin
			// They said OK, so try to delete it:
			if (RegDeleteKeyIncludingSubkeys(HKCU, CryptnosRegKey)) then begin
				// If that worked, also delete the GPF Comics key if it's empty:
				RegDeleteKeyIfEmpty(HKCU, GPFComicsRegKey);
			end
			// The delete didn't work.  We should probably warn the user that we tried and
			// failed.  Note that we don't really care to inform them if (a) the keys don't
			// exist (why bother?) and (b) if they said no to removing them.
			else begin
				MsgBox('Your preferences could not be deleted for some reason.  Sorry...', mbInformation, MB_OK);
			end;
		end;
	end;
end;
