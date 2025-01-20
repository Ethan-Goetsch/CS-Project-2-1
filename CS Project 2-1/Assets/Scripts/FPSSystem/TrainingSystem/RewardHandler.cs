using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.IO;
using UnityEngine.AI;
using System.Diagnostics.Eventing.Reader;
using System.Collections;

namespace FPSSystem.TrainingSystem
{
    [Serializable]
    public class RewardHandler : MonoBehaviour
    {
        private String rewardSetupDirectory = "/Rewards/";

        private String gameDataPath;

        private String finalRewardPath;
        public String FinalRewardPath() => finalRewardPath;

        [SerializeField]
        private Reward currentRewardSetup; // The setup where you can adjust rewards with sliders in the editor

        [SerializeField]
        [Tooltip("Save the current reward setup made in the editor to a file after a training run is started")]
        private Boolean saveRewardSetupToFile = true;

        [SerializeField]
        private String rewardFilename = "Test";

        public Reward CurrentReward => currentRewardSetup;

        [SerializeField]
        [Tooltip("Pre-made reward setup .json to load for next training run")]
        private TextAsset setupFile;


        public void SaveRewardSetup(Reward data, string filePath)
        {
            string json = JsonUtility.ToJson(data, prettyPrint: true);

            File.WriteAllText(filePath, json);

            Debug.Log($"Data saved to {filePath}");
        }

        public RewardHandler()
        {
            gameDataPath = Application.streamingAssetsPath;
            finalRewardPath = gameDataPath + rewardSetupDirectory;
            Debug.Log("Saving rewards to : " + finalRewardPath);
        }

        public ArrayList availableFiles;

        public ArrayList GetAvailable(){

            ArrayList paths = new ArrayList();

            foreach(string filename in Directory.GetFiles(gameDataPath + rewardSetupDirectory))
            {
                if(!filename.Contains(".meta")){
                    paths.Add(filename);
                }

            }

            this.availableFiles = paths;
            return paths;
        }

        public void ReadRewardSetup(String path){
            this.currentRewardSetup = JsonUtility.FromJson<Reward>(File.ReadAllText(path));
            Debug.Log("Read reward setup from " + path);
        }

        public void Awake(){
            if(setupFile != null){
                currentRewardSetup = JsonUtility.FromJson<Reward>(setupFile.text);
            }
            else if(saveRewardSetupToFile)
            {
                SaveRewardSetup(currentRewardSetup, finalRewardPath + rewardFilename + ".json");
            }
        }


    }

}