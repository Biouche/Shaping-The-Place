using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPageHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoginPage()
    {
        Scenes.Load(Scenes.Login);
    }

    public void SigninPage()
    {
        Scenes.Load(Scenes.CreateUser);
    }
}
