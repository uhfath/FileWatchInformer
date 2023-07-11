@echo off
sc create FileWatchInformer binpath= "%~dp0FileWatchInformer.exe %~1" start= auto displayname= "Сервис отслеживания файлов и информирования пользователей"
sc failure FileWatchInformer actions= restart/60000/restart/60000/""/60000 reset= 86400
pause
