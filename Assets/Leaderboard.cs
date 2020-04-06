using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;


public class Leaderboard : MonoBehaviour
{
    private string SAVE_FOLDER;
    private const string SAVE_EXTENSION = "txt";
    public Text userName;

	[System.Serializable]
    public struct playerScore{
        public string name;
        public int value;
    }

	public Text leaderboarddisplay;
	public InputField nameEntry;
	public Button submission;
	public GameObject shutdown;

	public List<playerScore> fullleaderboard = new List<playerScore>();
	private void Awake()
	{
		SAVE_FOLDER = Application.dataPath + "/Leaderboard/";
	}

	private void OnEnable()
	{
		LoadAndDisplay();
	}

	public void SubmitHighScore()
    {
		string newName = nameEntry.text;
		fullleaderboard.Add(new playerScore() { name = newName, value = GameManager.Instance.Score });
		UnParseAndSave();
		LoadAndDisplay();
		submission.enabled = false;
		shutdown.SetActive(false);
		this.gameObject.SetActive(true);

	}


    void Start() {
        // Test if Save Folder exists
        if (!Directory.Exists(SAVE_FOLDER)) {
            // Create Save Folder
            Directory.CreateDirectory(SAVE_FOLDER);
        }
    }

	public void UnParseAndSave()
	{
		string leaderboardtext = "";

		for (int i = 0; i < fullleaderboard.Count; i++)
		{
			if (i != fullleaderboard.Count - 1)
				leaderboardtext += JsonUtility.ToJson(fullleaderboard[i]) + "\n";
			else
				leaderboardtext += JsonUtility.ToJson(fullleaderboard[i]);
		}

		Save(leaderboardtext);
	}

    public void Save(string saveString) {
        File.WriteAllText(SAVE_FOLDER + "leaderboard." + SAVE_EXTENSION, saveString);
    }

	public List<playerScore> LoadAndParse()
	{
		string loadedString = Load();
		List<playerScore> leaderboard = new List<playerScore>();
		if (! string.IsNullOrEmpty(loadedString))
		{
			string[] split = loadedString.Split("\n".ToCharArray());
			foreach(string strng in split)
			{
				leaderboard.Add(JsonUtility.FromJson<playerScore>(strng));
			}
		}

		leaderboard = SortBoard(leaderboard);
		return leaderboard;
	}

	List<playerScore> SortBoard(List<playerScore> leaderboard)
	{
		return leaderboard.OrderByDescending(x => x.value).ToList();
	}

    public string Load() {
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


	public void LoadAndDisplay()
	{
		fullleaderboard = LoadAndParse();

		string todisplay = "";

		for(int i = 0; i < fullleaderboard.Count; i++)
		{
			todisplay += fullleaderboard[i].name + "\t\t\t" + fullleaderboard[i].value;
		}

		this.leaderboarddisplay.text = todisplay;
	}
}
