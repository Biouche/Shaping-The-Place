﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenKeyboard()
    {
        //Open windows virtual keyboard
        WindowsKeyboard keyboard = new WindowsKeyboard();
        if (!WindowsKeyboard.TouchKeyboardOpened)
            keyboard.ShowTouchKeyboard();
        else
            keyboard.HideTouchKeyboard();
    }
}
