@echo off

REM Specify the Unity executable path
set UNITY_PATH=C:\Program Files\Unity\Hub\Editor\2022.3.30f1\Editor\Unity.exe

REM Check if the Unity executable exists
if not exist "%UNITY_PATH%" (
    echo Error: Unity executable not found at "%UNITY_PATH%".
    echo Please tweak the path in the batch file to match your Unity installation.
    exit /b 1
)

REM Save current directory path to a variable
setlocal
set SCRIPT_DIR=%~dp0

call conda init
call conda activate mlagents
echo Successfully launched virtual environment.

cd /d "%SCRIPT_DIR%"
echo Changed working directory back to project directory

REM Check if exec folder exists and has executable
if not exist "%SCRIPT_DIR%CS Project 2-1\exec\UnityEnvironment.exe" (
    echo Executable not found. Building project...
    "%UNITY_PATH%" -quit -batchmode -executeMethod BuildScript.BuildGame
)

call mlagents-learn --run-id ppo "config\ppo\FPS.yaml" --env="%SCRIPT_DIR%CS Project 2-1\exec\UnityEnvironment.exe" --torch-device cuda
PAUSE
