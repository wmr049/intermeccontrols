REM version 1.0: copy and install IVADCEDrivers.cab.pkg using RAPI
REM ========================================
REM ########### remote copy ################
pause Continue with REMOTE COPY? or Ctrl-C to stop batch now

pmkdir "\Flash File Store\UserAutoInstall"
pmkdir "\Flash File Store\Cab"
pmkdir "\Flash File Store\SSPB"
pmkdir "\Flash File Store\SSPB\SRs"

pput -f .\_S2l\IVADCEDrivers.cab.pkg "\Flash File Store\Cab"

prun "\Windows\updatebin.exe" /TopMost "\Flash File Store\Cab\IVADCEDrivers.cab.pkg"
