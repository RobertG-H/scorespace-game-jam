using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{

    // Singleton
    private static int m_referenceCount = 0;
    public static GameManager Instance;
 
    // Events
    public delegate void OnScoreUpdate(int score);
    public static event OnScoreUpdate ScoreUpdate;
    public delegate void OnPauseUpdate(bool isPaused);
    public static event OnPauseUpdate pauseUpdate;

    // Public Refs
    public AudioClip pauseMusic;
    public AudioClip gameplayMusic;
    public float gameTime;
    public Text timerUI;
    public GameObject startUI;
    public GameObject endUI;
    public Text endText;
  
    [SerializeField]
    public SFXManager sFXManager;


    // Private vars
    private float currentTime;
    private AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();

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
        if( currentTime <= 0f && !hasEnded)
        {
            EndGame();
        }
    }

    public void ReloadLevel()
    {   
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void ToggleStartUI(InputAction.CallbackContext context)
    {
        if(!context.started) return;
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
        sFXManager.playSound("time_up");

        endText.text = string.Format("PANtastic Work!\n You delivered {0} pizzas and you got a score of {1}. ", pizzasDelivered, score);
    }

    private void Pause()
    {
        audioSource.clip = pauseMusic;
        audioSource.Play();
        isPaused = true;
        pauseUpdate(isPaused);
    }

    private void UnPause()
    {
        audioSource.clip = gameplayMusic;
        audioSource.Play();
        isPaused = false;
        sFXManager.playSound("start");
        pauseUpdate(isPaused);
    }

    public void AddScore(int value)
    {
        if (hasEnded) return;
        score += value;
        ScoreUpdate(score);
    }

    public void AddPizzaCount(int value)
    {
        if (hasEnded) return;
        pizzasDelivered += value;
    }
}
