using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Singleton
    private static int m_referenceCount = 0;
    public static GameManager Instance;
 
    // Events
    public delegate void OnScoreUpdate(int score);
    public static event OnScoreUpdate ScoreUpdate;

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

    void Start()
    {
        score = 0;
    }

    void Update()
    {
        
    }

    public void AddScore(int value)
    {
        score += value;
        ScoreUpdate(score);
    }
}
