!include MUI2.nsh

!system "md .\dist"

; Installer Opts
Name "Stepmania Server"
OutFile ".\dist\stepmaniaserver_installer_win-x64.exe"
Icon "Icon.ico"
RequestExecutionLevel user
Unicode True
InstallDir "C:\Server\StepmaniaServer"
SetCompressor /SOLID lzma

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\StepmaniaServer" "Install_Dir"

; Set some MUI strings
!define MUI_ICON ".\Icon.ico"
!define MUI_PAGE_HEADER_TEXT "Stepmania Server"

!define MUI_WELCOMEPAGE_TITLE "Stepmania Server Installer"
!define MUI_LICENSEPAGE_TEXT_TOP "Stepmania Server License"

; Installer Pages
!insertmacro MUI_PAGE_LICENSE ".\LICENSE"
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

; Uninstaller Pages
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

; Language
!insertmacro MUI_LANGUAGE "English"

; TODO: Check for DOTNET_ROOT environment variable to make sure application can run
; TODO: Add installer metadata

Section "Stepmania Server"
    SectionIn RO
  
    ; Set output path to the installation directory.
    SetOutPath $INSTDIR
  
    ; Put file there
    File /r ".\build\win-x64\*.*"
  
    ; Write the installation path into the registry
    WriteRegStr HKLM SOFTWARE\StepmaniaServer "Install_Dir" "$INSTDIR"
  
    ; Write the uninstall keys for Windows
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\StepmaniaServer" "DisplayName" "Stepmania Server"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\StepmaniaServer" "UninstallString" '"$INSTDIR\uninstall.exe"'
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\StepmaniaServer" "NoModify" 1
    WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\StepmaniaServer" "NoRepair" 1
    WriteUninstaller "$INSTDIR\uninstall.exe"
SectionEnd


Section "Start Menu Shortcuts"
    CreateDirectory "$SMPROGRAMS\StepmaniaServer"
    CreateShortcut "$SMPROGRAMS\StepmaniaServer\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
    CreateShortcut "$SMPROGRAMS\StepmaniaServer\StepmaniaServer.lnk" "$INSTDIR\stepmania-server.exe" "" "$INSTDIR\stepmania-server.exe" 0
SectionEnd


Section "Uninstall"
    ; Remove registry keys
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\StepmaniaServer"
    DeleteRegKey HKLM SOFTWARE\StepmaniaServer

    ; Remove files and uninstaller
    Delete $INSTDIR\stepmania-server.exe
    Delete $INSTDIR\uninstall.exe
    RMDir /r $INSTDIR

    ; Remove shortcuts, if any
    Delete "$SMPROGRAMS\StepmaniaServer\*.*"

    ; Remove directories used
    RMDir "$SMPROGRAMS\StepmaniaServer"
    RMDir "$INSTDIR"
SectionEnd
