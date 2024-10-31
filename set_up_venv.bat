call conda init
call conda create -n mlagents python=3.10.12 && conda activate mlagents
call pip3 install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121
call pip3 install grpcio
call python -m pip install ./ml-agents-envs
call python -m pip install ./ml-agents
echo Successfully set up and launched virtual environment.
cmd /k