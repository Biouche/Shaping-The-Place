using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CreateUser : MonoBehaviour
{

    public InputField lastName;
    public InputField firstName;
    public InputField City;
    public InputField Email;
    public InputField Password;
    public InputField Pseudo;
    public Text error;

    public void CreateUserSQLite()
    {
        string message = DataService.CreateUser(Pseudo.text, firstName.text, lastName.text, Email.text, Password.text, City.text);

        try
        {
            User newUser = DataService.GetUserById( int.Parse(message));
            Scenes.SetConnectedUser(newUser);
            Scenes.Load(Scenes.LandingPage);
        }
        catch (FormatException)
        {
            error.text = message;
        }




    }

    public void CreateUserFile()
    {
        string infoUser;
        string path = "./Assets/Data/Users/" + Pseudo.text + ".txt";
        if (File.Exists(path))
            error.text = "An error occured, please change your pseudo";
        else
        {
            infoUser = lastName.text + "\r\n" + firstName.text + "\r\n" + City.text + "\r\n" + Email.text + "\r\n" + Pseudo.text + "\r\n" + Password.text;
            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine(infoUser);
            sw.Close();
            Scenes.Load(Scenes.LandingPage);
        }
    }
}
