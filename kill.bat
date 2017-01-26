@echo off

FOR /F "tokens=3" %%A IN ('sc queryex Imbick.Assistant.Service ^| findstr PID') DO (SET pid=%%A)
 IF %pid% NEQ "0" (
  taskkill /F /PID %pid%
 )