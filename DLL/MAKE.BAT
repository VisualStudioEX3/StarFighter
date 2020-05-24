REM ----------------------------------------------------------------------------
REM Main BAT command to compile each DLL project.
REM (C) VisualStudioEX3, José Miguel Sánchez Fernández - 2020
REM DIV Games Studio 2 (C) Hammer Technologies - 1998, 1999
REM Watcom C++ 10.6 (C) Watcom International - 1993, 1996
REM ----------------------------------------------------------------------------
REM
REM Sintax:
REM MAKE.BAT %project_name%

echo off
cls
cd %1

echo:
echo Compiling %1.DLL...
call MAKE.BAT

echo:
echo Copy %1.DLL to parent folder...
copy %1.DLL .. /y

echo:
echo Clean project...
del /f *.dll
del /f *.obj
del /f *.err

cd ..
echo off