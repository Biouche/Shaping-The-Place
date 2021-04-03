using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoPanelController : MonoBehaviour {

    public GameObject OpenTutoButton;

    public void OnClickCloseEvent()
    {
        this.gameObject.SetActive(false);
        this.OpenTutoButton.SetActive(true);
    }

    public void OnClickOpenEvent()
    {
        this.gameObject.SetActive(true);
        this.OpenTutoButton.SetActive(false);
    }
}
