﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDController : MonoBehaviour
{
    public PlayerController player;
    public Text speed;
    private bool initialized = true;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null || speed == null)
        {
            Debug.LogWarning("HUDController is missing references. Check the editor.");
            initialized = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;

        speed.text = string.Format("Speed: {0}", (int)player.speed);
    }
}