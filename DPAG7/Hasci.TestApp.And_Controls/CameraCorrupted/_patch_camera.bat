@echo off
CLS
echo ---------------------------------------------------------------
echo +                      Camera Patch                           +
echo +                                                             +
echo + Bitte beenden Sie Hasci.TestApp (geben Sie b99 im Haupt-    +
echo + menü ein), BEVOR Sie den Patch starten.                     +
echo +                                                             +
echo +           weiter mit einer beliebigen Taste                 +
echo +                Abbrechen mit Strg+C                         +
echo ---------------------------------------------------------------
@pause

pkill Hasci.Testapp.exe

REM ############## Camera Fix ###########
pput -f .\A3AcquireCam.dll "\Flash File Store\UserAutoInstall"
pput -f .\_sstransferagent.xml "\Flash File Store\UserAutoInstall"

pput -f .\A3AcquireCam.dll "\Windows"

preboot
@echo on
