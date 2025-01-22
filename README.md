# CS Project 2-1: **Shooter Environment for Unity ML-Agents**

This repository contains a Unity project that implements a shooter environment designed for training agents using [Unity ML-Agents](https://github.com/Unity-Technologies/ml-agents). Agents navigate around obstacles, shoot at an opponent, reload, interact with pickups, and learn competitive behaviors through reinforcement learning. Our custom content for this environment are in "/CS Project 2-1/" directory.
### Collaborators:
- Stefano Bertini Coppero
- Alexandros Christoforou
- Alexandru Cioc
- Selim Elkaffas
- Ethan Goetsch
- Joris Mäntele
- Gvidas Žilinskas

## Installation
You can find pre-compiled Unity builds in the [releases section](https://github.com/Ethan-Goetsch/CS-Project-2-1/releases). Extract the archive for your platform to where you want to install the application.

## Running
Run the executable "Launch".

## Training
If you wish to train a model for the shooter environment, you can do so with either the pre-compiled builds we provide or clone the repository and modify settings and train from the Unity editor.
#### Prerequisites
- Clone the repository: clone the repository using your preferred method (Required)
- Unity Hub: Install **Unity Hub** from [Unity's official website](https://unity.com/download). (If you wish to use the Unity editor)
- Unity Editor: Install the 2022.3.30f1 LTS editor version from [Unity's editor archive](https://unity.com/releases/editor/archive)(If you wish to use the Unity editor)
- ML Agents: Follow the instructions for setting up a [conda](https://anaconda.org/anaconda/conda) environment and installing ML agents python packages from files included with this repository at the ML Agents' [installation guide](https://unity-technologies.github.io/ml-agents/Installation/).

#### Training (Using provided build)
1. From the application files directory, navigate to "UnityEnvironment_Data/StreamingAssets/Rewards" ("Project21-Linux_Data/StreamingAssets/Rewards" on Linux build) and provide a reward setup you wish to use in the "rewards.json" file. The value bounds are:
    - DamagedReward: float, [-1.0, 1.0] - Reward for taking damage
    - DamageReward: float, [-1.0, 1.0] - Reward for damaging the other agent
    - HealedReward: float, [-1.0, 1.0] - Reward for being healed by picking up a health pickup
    - ShootReward: float, [-1.0, 1.0] - Reward for shooting
    - BulletMissReward: float, [-1.0, 1.0] - Reward for missing a shot
    - GoodReloadReward: float, [-1.0, 1.0] - Reward for reloading after shooting more than one bullet
    - BadReloadReward: float, [-1.0, 1.0] - Reward for reloading after shooting only one bullet
    - AmmoRestoredAward: float, [-1.0, 1.0] - Reward for picking up an ammo pickup
    - FacingReward: float, [-1.0, 1.0] - Reward for facing the opponent within a margin of error specified by "RewardAngle"
    - RewardAngle: float, [0, 30.0]  - margin of error within which to issue "FacingReward" rewards.
2. Activate mlagents environment with conda
4. Run `mlagents-learn`, passing in any environment variables ([View "Training ML Agents" for more information.](https://unity-technologies.github.io/ml-agents/Training-ML-Agents/) Confirm mlagents is listening in and waiting for a Unity process.
5. Launch the unity environment executable.
6. Confirm training is being performed by checking the "Is training" status indicator in the GUI is green, which means the Unity and ML Agents python script communicators are communicating on the local port.

#### Training (Using the unity editor)
1. Add the cloned repository as a unity project in **Unity Hub**
2. Open the project. You should see the "FPS" environment is open.
3. Select the "RewardHandler" object and view it with the Inspector. In the inspector, you will be able to adjust agent rewards.
4. Run `mlagents-learn`, passing in any environment variables ([View "Training ML Agents" for more information.](https://unity-technologies.github.io/ml-agents/Training-ML-Agents/) Confirm mlagents is listening in and waiting for a Unity process.
5. Click "play" in the unity editor.
6. Confirm training is being performed by checking the "Is training" status indicator in the GUI is green, which means the Unity and ML Agents python script communicators are communicating on the local port.
