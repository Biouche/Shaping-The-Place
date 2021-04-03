using UnityEngine;
using System.Collections;

public class GoBackHandler : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoBack()
    {
        Scenes.Load(Scenes.GetLastScene());
    }
}
