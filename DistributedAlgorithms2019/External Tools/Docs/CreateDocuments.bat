
cd %1%
REM cd "..\docs\html"
REM "************************************************************************"
echo %CD%
REM "************************************************************************"
REM del /q /s *.*
REM for /d %%x in ("%CD%\*") do rmdir /s /q "%%x"
cd "External Tools\Docs"
doxygen Doxyfile
echo finished
cd "..\..\..\docs\html"
REM "************************************************************************"
echo %CD%
REM "************************************************************************"
start "chrome" "index.html" --profile-directory="Profile 3"
REM start chrome file:///%CD%\index.html
