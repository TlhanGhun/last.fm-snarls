!include "MUI2.nsh"
!include "checkDotNet3.nsh"

!define MIN_FRA_MAJOR "3"
!define MIN_FRA_MINOR "5"
!define MIN_FRA_BUILD "*"


; The name of the installer
Name "last.fm snarls"

; The file to write
OutFile "Setup-LastFmSnarls.exe"





; The default installation directory
InstallDir "$PROGRAMFILES\Tlhan Ghun\LastFmSnarls"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\TlhanGhun\LastFmSnarls" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin


 


;--------------------------------

  !define MUI_ABORTWARNING



!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "tlhanGhun.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "tlhanGhunWelcome.bmp"
!define MUI_WELCOMEPAGE_TITLE "last.fm snarls"
!define MUI_WELCOMEPAGE_TEXT "last.fm snarls displays the currently played track in your last.fm account as well as the previous one using the general notification system Snarl."
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "Tlhan Ghun\last.fm snarls"
!define MUI_ICON "..\..\LastFmSnarls.ico"
!define MUI_UNICON "uninstall.ico"


Var StartMenuFolder
; Pages

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE "License.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY

  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\TlhanGhun\LastFmSnarls" 
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder

  !insertmacro MUI_PAGE_INSTFILES
  !define MUI_FINISHPAGE_RUN "LastFmSnarls.exe"
  !insertmacro MUI_PAGE_FINISH




  !insertmacro MUI_UNPAGE_WELCOME
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  !insertmacro MUI_UNPAGE_FINISH





;--------------------------------




!insertmacro MUI_LANGUAGE "English"

; LoadLanguageFile "${NSISDIR}\Contrib\Language files\English.nlf"
;--------------------------------
;Version Information

  VIProductVersion "1.0.0.0"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "last.fm snarls"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" "Tlhan Ghun"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "© Tlhan Ghun GPL v.3"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "Shows notifications using Snarl about the played track in last.fm"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "1.0"







Function un.UninstallDirs
    Exch $R0 ;input string
    Exch
    Exch $R1 ;maximum number of dirs to check for
    Push $R2
    Push $R3
    Push $R4
    Push $R5
       IfFileExists "$R0\*.*" 0 +2
       RMDir "$R0"
     StrCpy $R5 0
    top:
     StrCpy $R2 0
     StrLen $R4 $R0
    loop:
     IntOp $R2 $R2 + 1
      StrCpy $R3 $R0 1 -$R2
     StrCmp $R2 $R4 exit
     StrCmp $R3 "\" 0 loop
      StrCpy $R0 $R0 -$R2
       IfFileExists "$R0\*.*" 0 +2
       RMDir "$R0"
     IntOp $R5 $R5 + 1
     StrCmp $R5 $R1 exit top
    exit:
    Pop $R5
    Pop $R4
    Pop $R3
    Pop $R2
    Pop $R1
    Pop $R0
FunctionEnd









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
  File "LastFmSnarls.ico"
  File "LICENSE.txt"
  File "Documentation.ico"
  
  
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

!insertmacro MUI_STARTMENU_WRITE_BEGIN Application

  CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\last.fm snarls.lnk" "$INSTDIR\LastFmSnarls.exe" "" "$INSTDIR\LastFmSnarls.exe" 0
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\Documentation.lnk" "$INSTDIR\Documentation.URL" "" $INSTDIR\Documentation.ico" 0
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  
!insertmacro MUI_STARTMENU_WRITE_END

  
SectionEnd


;--------------------------------

; Uninstaller

Section "Uninstall"

  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\LastFmSnarls"
  DeleteRegKey HKLM "Software\TlhanGhun\LastFmSnarls"
  ; Remove files and uninstaller
  Delete $INSTDIR\*.*

  ; Remove shortcuts, if any
  !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
    
  Delete "$SMPROGRAMS\$StartMenuFolder\\*.*"
  


  DeleteRegKey HKCU "Software\TlhanGhun\LastFmSnarls"


  ; Remove directories used
   ; RMDir "$SMPROGRAMS\$StartMenuFolder"
Push 10 #maximum amount of directories to remove
  Push "$SMPROGRAMS\$StartMenuFolder" #input string
    Call un.UninstallDirs

   
  ; RMDir "$INSTDIR"
  
  Push 10 #maximum amount of directories to remove
  Push $INSTDIR #input string
    Call un.UninstallDirs


SectionEnd
