using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentController : MonoBehaviour {
    public static CommentController instance;

    public void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
        WindowsKeyboard key = new WindowsKeyboard();
        key.ShowOnScreenKeyboard();
        //key.RepositionOnScreenKeyboard(rect);
    }
    
    public void SetVisible(bool visible)
    {
        this.gameObject.SetActive(visible);
    }
}
