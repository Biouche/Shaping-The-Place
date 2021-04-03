using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyboardInputController : MonoBehaviour, ISelectHandler, IDeselectHandler
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnSelect(BaseEventData data)
    {
        WindowsKeyboard keyboard = new WindowsKeyboard();
        keyboard.ShowTouchKeyboard();
    }
    public void OnDeselect(BaseEventData data)
    {
        WindowsKeyboard keyboard = new WindowsKeyboard();
        keyboard.HideTouchKeyboard();
    }
}
