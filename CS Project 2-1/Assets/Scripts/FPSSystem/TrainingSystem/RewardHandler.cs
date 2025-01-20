using System;
using UnityEngine;
using System.IO;


namespace FPSSystem.TrainingSystem
{
    [Serializable]
    public class RewardHandler : MonoBehaviour
    {   


        [SerializeField]
        private Reward currentRewardSetup; // The setup where you can adjust rewards with sliders in the editor

        [SerializeField]
        [Tooltip("Save the current reward setup made in the editor to a file after a training run is started. Also disables loading rewards from static file in StreamingAssets.")]
        private Boolean saveRewardSetupToFile = false;

        [SerializeField]
        private String customFilename = "defaultRewardSetupName";

        public Reward CurrentReward => currentRewardSetup;

        [SerializeField]
        [Tooltip("Pre-made reward setup .json to load for next training run")]
        private TextAsset setupFile;


        private String streamingAssetsDirectory;
        private String assetsDirectory;
        private String rewardsStorageDirectory = "/Rewards/";
        private String rewardsStorageFileName = "rewards";
        private String rewardsWriteDirectory = "/SavedRewards/";


        public void SaveRewardSetup(Reward data, string filePath)
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);

            File.WriteAllText(filePath, json);

            Debug.Log($"Rewards saved to {filePath}");
        }

        public RewardHandler()
        {
            streamingAssetsDirectory = Application.streamingAssetsPath;
            assetsDirectory = Application.dataPath;
        }

        public void ReadRewardSetup(){
            string storedRewardPath = streamingAssetsDirectory + rewardsStorageDirectory + rewardsStorageFileName + ".json";

            if (File.Exists(storedRewardPath))
            {
                this.currentRewardSetup = JsonUtility.FromJson<Reward>(File.ReadAllText(storedRewardPath));
                Debug.Log("Read reward setup from " + storedRewardPath);
            } else {
                Debug.Log("Reward setup file \"reward.json\" not found in " + streamingAssetsDirectory + rewardsStorageDirectory + ". Loading defaults.");
            }
            
        }

        public void Awake(){
            // Read reward setup from file dropped into setupFile field in the editor if
            if(setupFile != null && !saveRewardSetupToFile){
                currentRewardSetup = JsonUtility.FromJson<Reward>(setupFile.text);
                saveRewardSetupToFile = false;
                return;
            } 
            // Read reward setup from static file in streaming assets if not choosing to save reward setup to file (which means )
            else if(!saveRewardSetupToFile)
            {
                ReadRewardSetup();
                saveRewardSetupToFile = false;
            }

            // If rewards have not been read from anything
            if(saveRewardSetupToFile)
            {
                SaveRewardSetup(currentRewardSetup, assetsDirectory + rewardsWriteDirectory + customFilename + ".json");
            }
        }


    }

}