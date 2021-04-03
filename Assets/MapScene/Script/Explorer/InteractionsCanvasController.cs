using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class InteractionsCanvasController : MonoBehaviour
{
    public GameObject RightPanel;
    public GameObject LoadingCanvas;
    public static InteractionsCanvasController instance;

    // Use this for initialization
    void Start()
    {
        instance = this;
        RightPanel.SetActive(false);
    }

    public void OnClickSave()
    {
        StartCoroutine(DoStuff());
    }

    public IEnumerator DoStuff()
    {
        LoadingCanvas.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        MapLoader.SaveMap();
        LoadingCanvas.SetActive(false);
    }
}

