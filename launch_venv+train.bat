call conda init
call conda activate mlagents
echo Successfully launched virtual environment.
call mlagents-learn --run-id ppo "D:\unity\proj\config\ppo\FPS.yaml" --env="D:\unity\proj\CS Project 2-1\exec\UnityEnvironment.exe" --torch-device cuda
PAUSE