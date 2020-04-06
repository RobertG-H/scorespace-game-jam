using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class Leaderboard : MonoBehaviour
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Leaderboard/";
    private const string SAVE_EXTENSION = "txt";
    public Text userName;

    public struct playerScore{
        public string name;
        public int value;
    }

    private string prevLB;

    public void SubmitHighScore()
    {
        prevLB = Load();
        if (prevLB != null) {   
            string json = JsonUtility.ToJson(newScore);
        }
        else
        {
            playerScore newScore = new playerScore();
            newScore.name = userName.text;
            newScore.value = GameManager.Instance.Score;
            string json = JsonUtility.ToJson(newScore);
            Save(json);
        }

    }


    void Start() {
        // Test if Save Folder exists
        if (!Directory.Exists(SAVE_FOLDER)) {
            // Create Save Folder
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }
    public static void Save(string saveString) {
        File.WriteAllText(SAVE_FOLDER + "leaderboard." + SAVE_EXTENSION, saveString);
    }

    public static string Load() {
        DirectoryInfo directoryInfo = new DirectoryInfo(SAVE_FOLDER);
        // Get all save files
        FileInfo[] saveFiles = directoryInfo.GetFiles("*." + SAVE_EXTENSION);
        // Cycle through all save files and identify the most recent one
        FileInfo mostRecentFile = null;
        foreach (FileInfo fileInfo in saveFiles) {
            if (mostRecentFile == null) {
                mostRecentFile = fileInfo;
            } else {
                if (fileInfo.LastWriteTime > mostRecentFile.LastWriteTime) {
                    mostRecentFile = fileInfo;
                }
            }
        }
        // If theres a save file, load it, if not return null
        if (mostRecentFile != null) {
            string saveString = File.ReadAllText(mostRecentFile.FullName);
            return saveString;
        } else {
            return null;
        }
    }
}
