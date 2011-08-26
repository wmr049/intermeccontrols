REM sync_to_FFS.bat
REM version 2.5: added ITC_KeybdClick.cab
REM version 2.4: added keymap xmls
REM version 2.3: added SR11071900_MSC_Cx70WM65_ALL.CAB
REM version 2.2: replaced Hasci.TestApp.IntermecPhotoControls.dll by Hasci.TestApp.IntermecPhotoControls2.dll
REM version 2.1: added KeyboardImport stuff
REM              removed obsolete lnk files
REM version 2.0: splitted file for sync_to_S2L and remote copy
REM sync_to_S2L.bat and sync_to_FFS.bat
REM ========================================
REM ########### remote copy ################
pause Continue with REMOTE COPY? or Ctrl-C to stop batch now

pmkdir "\Flash File Store\UserAutoInstall"
pmkdir "\Flash File Store\Cab"
pmkdir "\Flash File Store\SSPB"
pmkdir "\Flash File Store\SSPB\SRs"

pput -f .\_S2l\IDL.cab.pkg "\Flash File Store\Cab"

prun "\Windows\updatebin.exe" /TopMost "\Flash File Store\Cab\IDL.cab.pkg"
