REM sync_to_S2L.bat
REM	version 4.3: changed to NOT autoinstall SR
REM version 4.2: added keytools.dll
REM version 4.1: added BootSettings.xml
REM version 4.0: added IVADCEDrivers.cab.pkg and ITCScan.DLL
REM version 2.5: added mkdir to create dirs
REM version 2.4: added ITC_KeybdClick.cab
REM version 2.3: added separate keymap xmls as one does not work
REM version 2.2: replaced Hasci.TestApp.IntermecPhotoControls.dll by Hasci.TestApp.IntermecPhotoControls2.dll
REM version 2.1: added keyboard stuff
REM version 2.0: splitted file for sync2_S2L and remote copy
REM sync_S2L.bat and sync_to_FFS.bat
REM version 1.9: started with DPAG4
REM version 1.8: changed name of PhotoControl
REM version 1.7
REM added pmkdir
REM added config and cab files
REM ++++++++++++++++++++++++++

mkdir ".\setup\Flash File Store\UserAutoInstall"
mkdir ".\setup\Flash File Store\Cab"
mkdir ".\setup\Flash File Store\SSPB"
mkdir ".\setup\Flash File Store\SSPB\SRs"

copy .\_S2l\SR11071900_MSC_Cx70WM65_ALL.CAB ".\setup\Flash File Store"

REM ########## _sstransferagent ###########
copy .\_S2l\_sstransferagent.xml ".\setup\Flash File Store\UserAutoInstall"

REM ########### Config Files ###########
copy .\_S2l\barcodetypes.xml ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\WiFiRadioOFF.xml ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\WWAN_OFF.xml ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\BootSettings.xml ".\setup\Flash File Store\UserAutoInstall"

REM ########## Keyboard Stuff #########
copy .\_S2l\ck70_largeAlpha.xml ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\ck70_num.xml ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\cn70_alpha.xml ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\cn70_num.xml ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\cn70e_alpha.xml ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\cn70e_num.xml ".\setup\Flash File Store\UserAutoInstall"
REM copy .\_S2l\Cx70_keymaps.xml ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\KeyMapImport.exe ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\keytools.dll ".\setup\Flash File Store\UserAutoInstall"

REM ########### CAB files ##############
copy .\_S2l\02_CNxDShow_CIL1.98.CAB ".\setup\Flash File Store\Cab"
copy .\_S2l\03_KBDTools.CAB ".\setup\Flash File Store\Cab"
copy .\_S2l\05_itc50.dll_Field_Trial.cab ".\setup\Flash File Store\Cab"
copy .\_S2l\IDL.cab.pkg ".\setup\Flash File Store\Cab"
copy .\_S2l\IVADCEDrivers.cab.pkg ".\setup\Flash File Store\Cab"
copy .\_S2l\ITC_KeybdClick.cab ".\setup\Flash File Store\Cab"

REM ########## EXECUTABLES #############
copy .\_S2l\CUsbKeysCS.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\DPAG_TestApp.exe ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Hasci.TestApp.exe ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\SigCapDP2.exe ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\IntermecTestApp.exe ".\setup\Flash File Store\UserAutoInstall"

REM ########## RUNTIMES #############
copy .\_S2l\itcscan.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Hasci.TestApp.DeviceControlContracts.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Hasci.TestApp.IntermecBarcodeScanControls.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Hasci.TestApp.IntermecImagerControls2.dll ".\setup\Flash File Store\UserAutoInstall"
REM copy .\_S2l\Hasci.TestApp.IntermecPhotoControls.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Hasci.TestApp.IntermecPhotoControls2.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Hasci.TestApp.IntermecSignatureControls2.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Hasci.TestApp.IntermecUtilityControls.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Intermec.DataCollection.CF3.5.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Intermec.Device.CF3.5.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Intermec.DeviceManagement.SmartSystem.ITCSSApi.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Intermec.Multimedia.Camera.CF35.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Intermec.Windows.Forms.InkCapture.CF35.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\ITCImager.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\ITCINK.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\itcRebootDevice.exe ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\ITCSSApi.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\OpenNETCF.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\OpenNETCF.Drawing.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Pocket.ComponentModel.Initialization.dll ".\setup\Flash File Store\UserAutoInstall"
copy .\_S2l\Pocket.System.ComponentModel.Composition.dll ".\setup\Flash File Store\UserAutoInstall"
