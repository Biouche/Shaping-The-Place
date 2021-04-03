using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;




public class ConnectionHandler : MonoBehaviour
{


    public InputField pseudo;
    public InputField password;
    public Text error;
    public const string path = "./Assets/Data/Users";

    public void Login()
    {
        User user = DataService.CheckUserInfo(pseudo.text, password.text);
        if (user != null)
        {
            Scenes.SetConnectedUser(user);
            Scenes.Load(Scenes.LoadProject);
        }
        else
            error.text = "Invalid credentials";
    }
}
