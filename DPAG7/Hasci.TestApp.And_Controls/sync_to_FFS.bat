REM sync_to_FFS.bat
REM version 4.1: added BootSettings.xml
REM version 4.0: added IVADCEDrivers.cab.pkg and ITCScan.DLL
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
pmkdir "\SmartSystems"

pput -f .\_S2l\SR11071900_MSC_Cx70WM65_ALL.CAB "\Flash File Store\SSPB\SRs"

pput -f .\_S2l\ITCscan.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\CUsbKeysCS.dll "\Flash File Store\UserAutoInstall"
REM pput -f .\_S2l\DPAG_TestApp.exe "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Hasci.TestApp.exe "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\IntermecTestApp.exe "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\SigCapDP2.exe "\Flash File Store\UserAutoInstall"

pput -f .\_S2l\KeymapImport.exe "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Cx70_keymaps.xml "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\ck70_largeAlpha.xml "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\ck70_num.xml "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\cn70_alpha.xml "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\cn70_num.xml "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\cn70e_alpha.xml "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\cn70e_num.xml "\Flash File Store\UserAutoInstall"


REM pput -f .\_S2l\DPAG_TestApp.lnk "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Hasci.TestApp.lnk "\Flash File Store\UserAutoInstall"
REM pput -f .\_S2l\IntermecTestApp.lnk "\Flash File Store\UserAutoInstall"
REM pput -f .\_S2l\SigCapDP2.lnk "\Flash File Store\UserAutoInstall"

REM ########### remote copy ################
pput -f .\_S2l\Hasci.TestApp.DeviceControlContracts.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Hasci.TestApp.IntermecBarcodeScanControls.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Hasci.TestApp.IntermecImagerControls2.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Hasci.TestApp.IntermecPhotoControls2.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Hasci.TestApp.IntermecSignatureControls.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Hasci.TestApp.IntermecUtilityControls.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Intermec.DataCollection.CF3.5.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Intermec.Device.CF3.5.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Intermec.DeviceManagement.SmartSystem.ITCSSApi.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Intermec.Multimedia.Camera.CF35.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Intermec.Windows.Forms.InkCapture.CF35.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\ITCImager.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\ITCINK.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\ITCSSApi.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\OpenNETCF.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\OpenNETCF.Drawing.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Pocket.ComponentModel.Initialization.dll "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\Pocket.System.ComponentModel.Composition.dll "\Flash File Store\UserAutoInstall"

REM ############# CONFIG files
pput -f .\_S2l\_sstransferagent.xml "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\barcodetypes.xml "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\itcRebootDevice.exe "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\WiFiRadioOFF.xml "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\WWAN_OFF.xml "\Flash File Store\UserAutoInstall"
pput -f .\_S2l\BootSettings.xml "\Flash File Store\UserAutoInstall"

REM ############# Installers ##############
REM pput -f .\_S2l\IDL_HGO.cab "\Flash File Store\Cab"
pput -f .\_S2l\IDL.cab.pkg "\Flash File Store\Cab"
pput -f .\_S2l\IVADCEDrivers.cab.pkg "\Flash File Store\Cab"
pput -f .\_S2l\02_CNxDShow_CIL1.98.CAB "\Flash File Store\Cab"
pput -f .\_S2l\03_KBDTools.CAB "\Flash File Store\Cab"
pput -f .\_S2l\05_itc50.dll_Field_Trial.cab "\Flash File Store\Cab"
pput -f .\_S2l\ITC_KeybdClick.cab "\Flash File Store\Cab"

PAUSE !!!!!!!!!!!!! CleanBOOT now? !!!!! Ctrl-C to STOP !!!!!!!!!!!!!!
pput -f .\_S2l\CleanBootCmd.exe "\Flash File Store"
prun "\Flash file store\CleanbootCmd.exe" -CleanBoot
