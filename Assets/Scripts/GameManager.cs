using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // Singleton
    private static int m_referenceCount = 0;
    public static GameManager Instance;
 
    // Events
    public delegate void OnScoreUpdate(int score);
    public static event OnScoreUpdate ScoreUpdate;
    public PlayerController player;

    public Text timerUI;

    public GameObject startUI;
    public GameObject endUI;
    public Text endText;

    public float gameTime;
    private float currentTime;
    private bool isPaused;
    private bool hasEnded;

    // PIZZA AND SCORE
    private int score;
    public int Score
    {
        get
        {
            return score;
        }
        set
        {
            ScoreUpdate(value);
            score = value;
        }
    }
    public int pizzasDelivered;

    void Awake()
    {
        m_referenceCount++;
        if (m_referenceCount > 1)
        {
            DestroyImmediate(this.gameObject);
            return;
        }
 
        GameManager.Instance = this;
    }
 
    void OnDestroy()
    {
        m_referenceCount--;
        if (m_referenceCount == 0)
        {
            GameManager.Instance = null;
        }
 
    }

    void Start()
    {
        score = 0;
        pizzasDelivered = 0;
        currentTime = gameTime;
        hasEnded = false;
        startUI.SetActive(true);
        Pause();
    }

    void Update()
    {
        timerUI.text = string.Format("{0}s", (int) currentTime);
        if (isPaused) return;
        if (!hasEnded)
            currentTime -= Time.deltaTime;
        if( currentTime <= 0f)
        {
            EndGame();
        }
    }

    public void ReloadLevel()
    {   
        SceneManager.LoadScene("JLevel");
    }

    public void ToggleStartUI()
    {
        if (hasEnded) return;
        if (startUI.activeSelf)
        {
            UnPause();
            startUI.SetActive(false);
        }
        else
        {
            Pause();
            startUI.SetActive(true);
        }
    }

    public void EndGame()
    {
        hasEnded = true;
        endUI.SetActive(true);
        endText.text = string.Format("Great work!\n You delivered {0} pizzas and you got a score of {1}. ", pizzasDelivered, score);
    }

    private void Pause()
    {
        isPaused = true;
        player.Pause();
    }

    private void UnPause()
    {
        isPaused = false;
        player.UnPause();
    }

    public void AddScore(int value)
    {
        score += value;
        ScoreUpdate(score);
    }
}
