; example2.nsi
;
; This script is based on example1.nsi, but it remember the directory, 
; has uninstall support and (optionally) installs start menu shortcuts.
;
; It will install example2.nsi into a directory that the user selects,

;--------------------------------

!include "checkDotNet3.nsh"

!define MIN_FRA_MAJOR "3"
!define MIN_FRA_MINOR "5"
!define MIN_FRA_BUILD "*"


; The name of the installer
Name "LastFmSnarls"

; The file to write
OutFile "Setup-LastFmSnarls.exe"


LoadLanguageFile "${NSISDIR}\Contrib\Language files\English.nlf"
;--------------------------------
;Version Information

  VIProductVersion "1.0.0.0"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "last.fm snarls"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "Tlhan Ghun"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "© Tlhan Ghun GPL v.3"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "Shows notifications using Snarl about the played track in last.fm"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "1.0"



; The default installation directory
InstallDir "$PROGRAMFILES\Tlhan Ghun\LastFmSnarls"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\TlhanGhun\LastFmSnarls" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin



;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------



; The stuff to install
Section "last.fm snarls"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  !insertmacro AbortIfBadFramework

  ; Put file there
  File "Changes.txt"
  File "CREDITS.txt"
  File "Documentation.URL"
  File "LastFm.ico"
  File "LastFmLibNet.dll"
  File "LastFmLibNet.pdb"
  File "LastFmLibNet.xml"
  File "LastFmSnarls.exe"
  File "LastFmSnarls.pdb"
  File "LICENSE.txt"
  
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\TlhanGhun\LastFmSnarls "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LastFmSnarls" "DisplayName" "last.fm snarls"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LastFmSnarls" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LastFmSnarls" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LastFmSnarls" "NoRepair" 1
  WriteUninstaller "uninstall.exe"



  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\\Tlhan Ghun"
  CreateDirectory "$SMPROGRAMS\\Tlhan Ghun\last.fm snarls"
  CreateShortCut "$SMPROGRAMS\\Tlhan Ghun\\last.fm snarls\\last.fm snarls.lnk" "$INSTDIR\LastFmSnarls.exe" "" "$INSTDIR\LastFmSnarls.exe" 0
  CreateShortCut "$SMPROGRAMS\\Tlhan Ghun\\last.fm snarls\\Documentation.lnk" "$INSTDIR\Documentation.URL" "" "$INSTDIR\Documentation.URL" 0
  CreateShortCut "$SMPROGRAMS\\Tlhan Ghun\\last.fm snarls\\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  
  
SectionEnd



;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LastFmSnarls"
  DeleteRegKey HKLM "HKLM SOFTWARE\TlhanGhun\LastFmSnarls"

  ; Remove files and uninstaller
  Delete $INSTDIR\*.*

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Tlhan Ghun\last.fm snarls\*.*"

  ; Remove directories used
   RMDir "$SMPROGRAMS\Tlhan Ghun\last.fm snarls\"
   
   RMDir "$INSTDIR"


SectionEnd
